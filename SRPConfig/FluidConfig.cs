using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SRPConfig
{
    public class FluidConfig : ConfigIdentity, ICloneable
    {
        [DefaultValue(0.874), Range(1, 5, ErrorMessage = defaultErrorMsg)]
        public virtual double OilDensity
        { get; set; }

        [DefaultValue(1.145), Range(1, 5, ErrorMessage = defaultErrorMsg)]
        public virtual double WaterDensity
        { get; set; }

        [DefaultValue(73), Range(0, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double WaterHoldup
        { get; set; }

        [DefaultValue(0), Range(0, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double GasFactor
        { get; set; }

        public virtual object Clone() => MemberwiseClone();
    }
}
