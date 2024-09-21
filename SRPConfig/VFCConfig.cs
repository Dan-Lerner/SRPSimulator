using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace SRPConfig
{
    public class VFCConfig : ConfigIdentity, ICloneable
    {
        public const double speedAcceleration = 10; // Hz/s
        public const double speedDeceleration = 20; // Hz/s
        public const double defFrequency = 50; // Hz

        [DefaultValue(speedAcceleration), Range(0, 500, ErrorMessage = defaultErrorMsg)]
        public virtual double Acceleration
        { get; set; }

        [DefaultValue(speedDeceleration), Range(0, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual double Deceleration
        { get; set; }

        [DefaultValue(defFrequency), Range(0, 500, ErrorMessage = defaultErrorMsg)]
        public virtual double Frequency
        { get; set; }

        [DefaultValue(false)]
        public virtual bool ActivateProgram
        { get; set; }

        // Count of points
        [DefaultValue(0), Range(1, 20, ErrorMessage = defaultErrorMsg)]
        [NotMapped, XmlIgnore]
        public virtual int Count
        { get; set; }

        // Set of points
        [XmlIgnore]
        public virtual List<FreqPointConfig> FreqPoints
        { get; set; } = new();

        public object Clone() => MemberwiseClone();
    }

    public class FreqPointConfig : ConfigIdentity, ICloneable
    {
        public const double defFrequency = 50;
        public const long defTime = 0;

        public virtual int FreqPointConfigId
        { get; set; }

        [DefaultValue(defTime), Range(0, 1E+11, ErrorMessage = defaultErrorMsg)]
        public virtual long Time
        { get; set; }

        [DefaultValue(defFrequency), Range(0, 500, ErrorMessage = defaultErrorMsg)]
        public virtual double Frequency
        { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
