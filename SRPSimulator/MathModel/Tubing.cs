using System;
using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    public class TubingConfigBrowsable : TubingConfig
    {
        public double moduleJung;
        [DisplayName("Jung module"), Description("Jung module for tubing matheril in Pa")]
        [Category("Tubing matherial")]
        [Browsable(true)]
        public override double ModuleJung { 
            get => moduleJung; 
            set { 
                moduleJung = value; 
                NotifyInitInvoke(); 
            } 
        }

		public double density;
        [DisplayName("Density"), Description("Density of tubing matheril in kg/m3")]
        [Category("Tubing matherial")]
        [Browsable(true)]
        public override double Density { 
            get => density; 
            set { 
                density = value; 
                NotifyInitInvoke(); 
            } 
        }

		public double length;
        [DisplayName("Length"), Description("Length of whole tubing in m")]
        [Category("Tubing geometry")]
        [Browsable(true)]
        public override double Length { 
            get => length; 
            set { 
                length = value; 
                NotifyInitInvoke(); 
            } 
        }

		public double innerD;
        [DisplayName("Inner diameter"), Description("Inner tubing diameter in mm")]
        [Category("Tubing geometry")]
        [Browsable(true)]
        public override double InnerD { 
            get => innerD; 
            set { 
                innerD = value; 
                NotifyInitInvoke(); 
            } 
        }

		public double outerD;
        [DisplayName("Outer diameter"), Description("Outer tubing diameter in mm")]
        [Category("Tubing geometry")]
        [Browsable(true)]
        public override double OuterD { 
            get => outerD; 
            set { 
                outerD = value; 
                NotifyInitInvoke(); 
            } 
        }
    }

    class Tubing : Configurable
	{
		// Inner volume, m3
		private double v;
		public double V
		{ get => v; }

		// Tension coeff X=FL/(SE) -> tensionK = L/(SE) -> X=F*tensionK;
		private double tensionK;
		public double TensionK
		{ get => tensionK; }

		// Inner cross-sectional area
		private double innerS;
		public double InnerS
		{ get => innerS; }

		// Whole cross-sectional area
		private double outerS;
		public double OuterS
		{ get => outerS; }

        public Tubing(TubingConfigBrowsable config) 
            : base(config)
        {
        }

        internal override bool Init()
        {
            TubingConfigBrowsable configInit = config as TubingConfigBrowsable;

            // Scaling of the parameters
            moduleJung_ = configInit.ModuleJung;
			density_ = configInit.Density;
			length_ = configInit.Length;
			innerD_ = configInit.InnerD * Physical.MILLI;
			outerD_ = configInit.OuterD * Physical.MILLI;
			innerS = Math.PI * innerD_ * innerD_ / 4.0;
			outerS = Math.PI * outerD_ * outerD_ / 4.0;
			S_ = outerS - innerS;
			v = length_ * Math.PI * innerD_ * innerD_ / 4.0;
			tensionK = length_ / (S_ * moduleJung_);
            configInit.Modified = true;
            configInit.Valid = true;
            return true;
		}

		// Inner calculations and states

		private double S_;			// cross-sectional area, m2

        // Scaled confObject parameters
        private double moduleJung_;
        private double density_;
        private double length_;
        private double innerD_;
        private double outerD_;
    };
}
