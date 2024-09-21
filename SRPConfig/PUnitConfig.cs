using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SRPConfig
{
    public class PUnitConfig : ConfigIdentity, ICloneable
    {
        [DefaultValue(1200), Range(1, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeR
        { get; set; } = 3000;

        [DefaultValue(2000), Range(1, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeA
        { get; set; }

        [DefaultValue(2000), Range(1, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeC
        { get; set; }

        [DefaultValue(2000), Range(1, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeI
        { get; set; }

        [DefaultValue(750), Range(1, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeG
        { get; set; }

        [DefaultValue(3800), Range(1, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeH
        { get; set; }

        [DefaultValue(3000), Range(1, 5000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeP
        { get; set; }

        [DefaultValue(600), Range(1, 6000, ErrorMessage = defaultErrorMsg)]
        public virtual int SizeK
        { get; set; }

        [DefaultValue(2000), Range(0, 10000, ErrorMessage = defaultErrorMsg)]
        public virtual int Counterweight
        { get; set; }

        [DefaultValue(300), Range(1, 1000, ErrorMessage = defaultErrorMsg)]
        public virtual int CrankWidth
        { get; set; }

        [DefaultValue(500), Range(1, 1000, ErrorMessage = defaultErrorMsg)]
        public virtual int BeamWidth
        { get; set; }

        public virtual object Clone() => MemberwiseClone();
    }
}
