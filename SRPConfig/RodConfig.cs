using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace SRPConfig
{
    public class RodConfig : ConfigIdentity, ICloneable
    {
        // Count of sections
        [DefaultValue(0), Range(1, 10, ErrorMessage = defaultErrorMsg)]
        [NotMapped, XmlIgnore]
        public virtual int Count
        { get; set; }

        // Set of sections
        [XmlIgnore]
        public virtual List<RodSectionConfig> RodSectionConfigs
        { get; set; } = new();

        public object Clone() => MemberwiseClone();
    }

    public class RodSectionConfig : ConfigIdentity, ICloneable
    {
        public const double moduleJungDef = 2.12E+11;   // Jung module for section matherial, Pa
        public const double densityDef = 7850.0;        // Density of section matherial, kg/m3

        public virtual int RodConfigId
        { get; set; }

        [DefaultValue(moduleJungDef), Range(1E+11, 1E+12, ErrorMessage = defaultErrorMsg)]
        public virtual double ModuleJung
        { get; set; }

        [DefaultValue(densityDef), Range(1000.0, 20000.0, ErrorMessage = defaultErrorMsg)]
        public virtual double Density
        { get; set; }

        [DefaultValue(100), Range(1, 10000, ErrorMessage = defaultErrorMsg)]
        public virtual double Length
        { get; set; }

        [DefaultValue(22), Range(1, 300, ErrorMessage = defaultErrorMsg)]
        public virtual double D
        { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
