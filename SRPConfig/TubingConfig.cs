using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SRPConfig
{
    public class TubingConfig : ConfigIdentity, ICloneable
    {
        public const double moduleJungDef = 2.1E+11;    // Jung module for tubing matherial, Pa
        public const double densityDef = 7850.0;        // Density of tubing matherial, kg/m3

        [DefaultValue(moduleJungDef), Range(1E+11, 1E+12, ErrorMessage = defaultErrorMsg)]
        public virtual double ModuleJung
        { get; set; }

        [DefaultValue(densityDef), Range(1000.0, 20000.0, ErrorMessage = defaultErrorMsg)]
        public virtual double Density
        { get; set; }

        [DefaultValue(1517), Range(1, 10000, ErrorMessage = defaultErrorMsg)]
        public virtual double Length
        { get; set; }

        [DefaultValue(49.3), Range(1, 300, ErrorMessage = defaultErrorMsg)]
        public virtual double InnerD
        { get; set; }

        [DefaultValue(60.3), Range(1, 300, ErrorMessage = defaultErrorMsg)]
        public virtual double OuterD
        { get; set; }

        public virtual object Clone() => MemberwiseClone();
    }
}
