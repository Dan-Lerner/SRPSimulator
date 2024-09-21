using System;
using System.Collections.Generic;
using System.ComponentModel;
using SRPConfig;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Linq;

namespace SRPSimulator.MathModel
{
    [DisplayName("Rod"), Description("Rod")]
    [Category("SRP")]
    public class RodConfigBrowsable : RodConfig
    {
        [DisplayName("Count of sections"), Description("Count of sections")]
        [Category("Rod")]
        [JsonIgnore, XmlIgnore]
        public override int Count { 
            get => sections.Count; 
            set {
                if (sections.Count != value) {
                    while (sections.Count < value) {
                        RodSectionConfigBrowsable section = new();
                        section.NotifyInit += InitRod;
                        sections.Add(section);
                    }
                    while (sections.Count > value) 
                        sections.RemoveAt(sections.Count - 1);
                    NotifyInitInvoke();
                }
            } 
        }

        private ExpandablePropertyList<RodSectionConfigBrowsable> sections = new();
        [DisplayName("Rod sections"), Description("Rod sections")]
        [Category("Rod")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandablePropertyConverter<RodSectionConfigBrowsable>))]
        public new ExpandablePropertyList<RodSectionConfigBrowsable> RodSectionConfigs { 
            get => sections; 
            set {
                sections = value; 
                foreach (var section in sections)
                    section.NotifyInit += InitRod;
            }
        }

        private bool InitRod()
        {
            NotifyInitInvoke();
            return true;
        }
    }

    [DisplayName("Rod section"), Description("Rod section characteristics")]
    [Category("SRP")]
    [TypeConverter(typeof(ExpandablePropertyConverter<RodSectionConfigBrowsable>))]
    public class RodSectionConfigBrowsable : RodSectionConfig
    {
        [Browsable(false)]
        public override string Name
        { get; set; }

        [Browsable(false)]
        public override int RodConfigId
        { get; set; }

        private double moduleJung;
        [DisplayName("Jung module"), Description("Jung module for rod matheril in Pa")]
        [Category("Rod matherial")]
        [Browsable(true)]
        public override double ModuleJung {
            get => moduleJung; 
            set { 
                moduleJung = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double density;
        [DisplayName("Density"), Description("Density of rod matheril in kg/m3")]
        [Category("Rod matherial")]
        [Browsable(true)]
        public override double Density { 
            get => density; 
            set { 
                density = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double length;
        [DisplayName("Length"), Description("Length of rod section in m")]
        [Category("Rod section geometry")]
        [Browsable(true)]
        public override double Length { 
            get => length; 
            set { 
                length = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double d;
        [DisplayName("Rod diameter"), Description("Rod diameter in mm")]
        [Category("Rod section geometry")]
        [Browsable(true)]
        public override double D { 
            get => d; 
            set { 
                d = value; 
                NotifyInitInvoke(); 
            } 
        }
    }

    [DisplayName("Rod"), Description("Rod")]
    [Category("SRP")]
    class Rod : Configurable
    {
        private List<RodSection> sections = [];
        public List<RodSection> Sections { 
            get => sections;
            set => sections = value;
        }

        // Whole rod length, m
        private double length = 0;
        internal double Length
        { get => length; }

        // Whole rod volume, m3
        private double v = 0;
        internal double V
        { get => v; }

        // Whole rod mass, kg
        private double mass = 0;
        internal double Mass
        { get => mass; }

        // Whole rod weight, Newtons
        private double weight = 0;
        internal double Weight
        { get => weight; }

        // Reduced tension coeff for the whole rod
        // X1=FL1/(S1E)
        // X=FL1/(S1E)+FL2/(S2E)+...
        // tensionK = L1/(S1E)+L2/(S2E)+...
        // X=F*tensionK;
        private double tensionK;
        internal double TensionK
        { get => tensionK; }

        public Rod(RodConfigBrowsable config)
            : base(config)
        {
            (Config as RodConfigBrowsable).Count = 4;
        }

        internal override bool Init()
        {
            RodConfigBrowsable configInit = config as RodConfigBrowsable;

            length = 0;
            v = 0;
            mass = 0;
            weight = 0;
            tensionK = 0;

            // If the number of sections has changed then sets a new list of sections
            if (configInit.Count != sections.Count)
                sections = configInit.RodSectionConfigs.Select(S => new RodSection(S)).ToList();

            foreach (var section in sections)
                if (!section.InitSection()) {
                    configInit.Valid = false;
                    return false;
                }

            // Values initialization
			interS = new double[configInit.Count];
            for (short ii = 0; ii < sections.Count; ii++) {
                if (sections[ii].Config.Length == 0)
                    break;

                if ((ii + 1) < sections.Count && sections[ii + 1].Config.Length > 0)
                    interS[ii] = sections[ii + 1].S - sections[ii].S;

                length += sections[ii].Config.Length;
                v += sections[ii].V;
                mass += sections[ii].Mass;
                weight += sections[ii].Weight;

                tensionK += sections[ii].Config.Length / (sections[ii].S * sections[ii].Config.ModuleJung);
            }

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
        }

        public double[] interS;
    };

    public class RodSection
    {
        // Config
        private RodSectionConfigBrowsable config;
        public RodSectionConfigBrowsable Config
        { get => config; }

        // Mass of section, kg
        private double mass;
        internal double Mass
        { get => mass; }

        // Weight, Newtons
        private double weight;
        internal double Weight
        { get => weight; }

        // Volume, m3
        private double v;
        internal double V
        { get => v; }

        // Cross sectional area, m2
        private double s;
        internal double S
        { get => s; }

        public RodSection(RodSectionConfigBrowsable config)
        {
            this.config = (RodSectionConfigBrowsable)config;
            InitSection();
        }

        internal bool InitSection()
        {
            // Scaling of config parameters
            moduleJung_ = config.ModuleJung;
            density_ = config.Density;
            length_ = config.Length;
            d_ = config.D * Physical.MILLI;
            s = Math.PI * d_ * d_ / 4.0;
			v = s * length_;
			mass = density_ * v;
			weight = Physical.g * mass;
            return true;
		}

        // Inner fields for calculations and states

        // Scaled confObject parameters
        private double moduleJung_;
        private double density_;
        private double length_;
        private double d_;
    };
}
