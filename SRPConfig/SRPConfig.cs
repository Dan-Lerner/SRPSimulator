using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SRPConfig
{
    public class SRPConfig : ConfigIdentity, ICloneable
    {
        [DefaultValue(false)]
        public virtual bool Anchor
        { get; set; }

        [DefaultValue(44.5), Range(1, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double PlungerD
        { get; set; }

        [DefaultValue(1), Range(0, 1, ErrorMessage = defaultErrorMsg)]
        public virtual double Filling
        { get; set; }

        [DefaultValue(0), Range(-100, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double ZatrubP
        { get; set; }

        [DefaultValue(23), Range(-100, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double PipeP
        { get; set; }

        [DefaultValue(700), Range(0, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual double FluidH
        { get; set; }

        [DefaultValue(1000), Range(0, 10000, ErrorMessage = defaultErrorMsg)]
        public virtual double FrictionPlunger
        { get; set; }

        [DefaultValue(3000), Range(0, 10000, ErrorMessage = defaultErrorMsg)]
        public virtual double FrictionSeal
        { get; set; }

        [DefaultValue(0), Range(0, 10000, ErrorMessage = defaultErrorMsg)]
        public virtual double UpperValveLeakage
        { get; set; }

        [DefaultValue(0), Range(0, 10000, ErrorMessage = defaultErrorMsg)]
        public virtual double IntakeValveLeakage
        { get; set; }

        public virtual object Clone() => MemberwiseClone();
    }
}
