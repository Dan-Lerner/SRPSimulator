using System;
using System.ComponentModel;
using SRPConfig;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace SRPSimulator.MathModel
{
    [DisplayName("VFC"), Description("Variable frequency converter")]
    [Category("Drive")]
    public class VFCConfigBrowsable : VFCConfig
    {
        private double acceleration;
        [DisplayName("Acceleration"), Description("in Hz/s")]
        [Category("VFC")]
        [Browsable(true)]
        public override double Acceleration {
            get => acceleration;
            set {
                acceleration = value;
                NotifyInitInvoke();
            }
        }

        private double deceleration;
        [DisplayName("Deceleration"), Description("in Hz/s")]
        [Category("VFC")]
        [Browsable(true)]
        public override double Deceleration {
            get => deceleration;
            set {
                deceleration = value;
                NotifyInitInvoke();
            }
        }

        public double frequency;
        [DisplayName("Frequency"), Description("in Hz")]
        [Category("VFC")]
        [Browsable(true)]
        public override double Frequency { 
            get => frequency;
            set {
                frequency = value;
                NotifyInitInvoke();
            }
        }

        private bool activateProgram;
        [DisplayName("Use program")]
        [Category("VFC")]
        [Browsable(true)]
        public override bool ActivateProgram { 
            get => activateProgram; 
            set => activateProgram = value; 
        }

        [DisplayName("Count of points"), Description("Count of points")]
        [Category("VFC")]
        [JsonIgnore, XmlIgnore]
        public override int Count {
            get => points.Count;
            set {
                if (points.Count != value) {
                    while (points.Count < value) {
                        FreqConfigBrowsable point = new();
                        point.NotifyInit += InitVFC;
                        points.Add(point);
                    }
                    while (points.Count > value)
                        points.RemoveAt(points.Count - 1);
                    NotifyInitInvoke();
                }
            }
        }

        private ExpandablePropertyList<FreqConfigBrowsable> points = [];
        [DisplayName("Frequency points"), Description("Frequency change points")]
        [Category("VFC")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandablePropertyConverter<FreqConfigBrowsable>))]
        public new ExpandablePropertyList<FreqConfigBrowsable> FreqPoints {
            get => points;
            set {
                points = value;
                foreach (var section in points)
                    section.NotifyInit += InitVFC;
            }
        }

        private bool InitVFC()
        {
            NotifyInitInvoke();
            return true;
        }
    }

    [DisplayName("Frequency change point"), Description("Frequency change point")]
    [Category("VFC")]
    [TypeConverter(typeof(ExpandablePropertyConverter<FreqConfigBrowsable>))]
    public class FreqConfigBrowsable : FreqPointConfig
    {
        [Browsable(false)]
        public override string Name
        { get; set; }

        [Browsable(false)]
        public override int FreqPointConfigId
        { get; set; }

        private long time;
        [DisplayName("Time"), Description("Time in s")]
        [Category("VFC")]
        [Browsable(true)]
        public override long Time {
            get => time;
            set {
                time = value;
                NotifyInitInvoke();
            }
        }

        private double frequency;
        [DisplayName("Frequency"), Description("Frequency in Hz")]
        [Category("VFC")]
        [Browsable(true)]
        public override double Frequency {
            get => frequency;
            set {
                frequency = value;
                NotifyInitInvoke();
            }
        }
    }

    class FreqPoint
    {
        public FreqPoint(long time, double f)
        {
            this.time = time;
            this.f = f;
        }
        public long time;
        public double f;
    }

    [DisplayName("VFC"), Description("VFC")]
    [Category("Drive")]
    public static class DoubleExtensions
    {
        public static bool EqualTo(this double value1, double value2, double epsilon)
        {
            return Math.Abs(value1 - value2) < epsilon;
        }
    }

    class VFC : Configurable
    {
        const double precision = 0.0001;

        public VFC(VFCConfigBrowsable config)
            : base(config)
        {
            (Config as VFCConfigBrowsable).Count = 4;
            proramInProgress_ = false;
        }

        private Drive drive;
        public Drive Drive {
            get => drive;
            set => drive = value;
        }

        public double Frequency { 
            get => frequency_;
            set {
                frequencySet_ = value;
            }
        }

        internal override bool Init()
        {
            VFCConfigBrowsable configInit = config as VFCConfigBrowsable;

            acceleration_ = configInit.Acceleration * Physical.MILLI;
            deceleration_ = configInit.Deceleration * Physical.MILLI;

            points = configInit.FreqPoints
                .Select(p => new FreqPoint((long)(p.Time * Physical.KILO), p.Frequency))
                .OrderBy(o => o.time)
                .ToList();

            timeEnd_ = points.Count() > 0 ? points.Last().time : 0;

            Frequency = configInit.Frequency;

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
        }

        public bool IsProgramModeActive()
        {
            return (config as VFCConfigBrowsable).ActivateProgram;
        }

        public void StartProgram(long time)
        {
            timeProgramStart_ = time;
            proramInProgress_ = true;
            currentPoint_ = 0;
        }

        public void SetDefaultFrequency()
        {
            Frequency = (config as VFCConfigBrowsable).Frequency;
        }

        public void Operate(long time)
        {
            if (proramInProgress_) {
                if (time - timeProgramStart_ >= points[currentPoint_].time) {
                    Frequency = points[currentPoint_].f;
                    if (++currentPoint_ == points.Count())
                        proramInProgress_ = false;
                }
            }
            if (!frequencySet_.EqualTo(frequency_, precision)) {
                if (frequency_ < frequencySet_) {
                    frequency_ += acceleration_ * (time - timeLast_);
                    if (frequency_ > frequencySet_)
                        frequency_ = frequencySet_;
                }
                else {
                    frequency_ -= deceleration_ * (time - timeLast_);
                    if (frequency_ < frequencySet_)
                        frequency_ = frequencySet_;
                }
                if (frequencySet_.EqualTo(frequency_, precision))
                    frequency_ = frequencySet_;
            }
            drive.Rotate(frequency_, time);
            timeLast_ = time;
        }

        private bool proramInProgress_;
        private int currentPoint_;
        private double frequencySet_;
        private long timeProgramStart_;
        private long timeEnd_;
        private long timeLast_;
        private double frequency_;
        private double acceleration_;
        private double deceleration_;
        private List<FreqPoint> points;
    }
}
