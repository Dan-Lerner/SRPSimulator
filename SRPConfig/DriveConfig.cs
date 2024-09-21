using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SRPConfig
{
    public class DriveConfig : ConfigIdentity, ICloneable
    {
        [DefaultValue(15), Range(0.1, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double NominalN
        { get; set; }

		[DefaultValue(4), Range(2, 32, ErrorMessage = defaultErrorMsg)]
        public virtual int PolesN
        { get; set; }

		[DefaultValue(0), Range(0, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double SlipIdle
        { get; set; }

		[DefaultValue(1), Range(0, 100, ErrorMessage = defaultErrorMsg)]
        public virtual double SlipNominal
        { get; set; }

        [DefaultValue(37.18), Range(1, 1000, ErrorMessage = defaultErrorMsg)]
        public virtual double GearRatio
        { get; set; }

		[DefaultValue(125), Range(1, 1000, ErrorMessage = defaultErrorMsg)]
        public virtual double SmallPulleyD
        { get; set; }

		[DefaultValue(710), Range(1, 1000, ErrorMessage = defaultErrorMsg)]
        public virtual double LargePulleyD
        { get; set; }

		[DefaultValue(false)]
        public virtual bool Direction
        { get; set; }

        public virtual object Clone() => MemberwiseClone();
    }
}
