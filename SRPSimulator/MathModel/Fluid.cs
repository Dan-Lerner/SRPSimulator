using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    public class FluidConfigBrowsable : FluidConfig
    {
        private double oilDensity;
		[DisplayName("Oil density"), Description("in g/sm3")]
        [Category("Fluid parameters")]
        [Browsable(true)]
        public override double OilDensity { 
            get => oilDensity; 
            set { 
                oilDensity = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double waterDensity;
        [DisplayName("Water density"), Description("in g/sm3")]
        [Category("Fluid parameters")]
        [Browsable(true)]
        public override double WaterDensity { 
            get => waterDensity; 
            set { 
                waterDensity = value; 
                NotifyInitInvoke();
            } 
        }

        private double waterHoldup;
        [DisplayName("Amount of water"), Description("in %")]
        [Category("Fluid parameters")]
        [Browsable(true)]
        public override double WaterHoldup { 
            get => waterHoldup; 
            set { 
                waterHoldup = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double gasFactor;
        [DisplayName("Gas factor"), Description("")]
        [Category("Fluid parameters")]
        [Browsable(true)]
        public override double GasFactor { 
            get => gasFactor; 
            set { 
                gasFactor = value; 
                NotifyInitInvoke(); 
            } 
        }
    }
    
	// Well fluid
    class Fluid : Configurable
	{
		private double fluidDensity;    // Well fluid density
		internal double FluidDensity
		{ get => fluidDensity; }

        public Fluid(FluidConfigBrowsable config)
            : base(config)
		{
        }

		internal override bool Init()
        {
            FluidConfigBrowsable configInit = config as FluidConfigBrowsable;

            // Scaling of config parameters
            oilDensity_ = configInit.OilDensity * Physical.KILO;
            waterDensity_ = configInit.WaterDensity * Physical.KILO;
            waterHoldup_ = configInit.WaterHoldup;
            gasFactor_ = configInit.GasFactor;

            fluidDensity = (waterHoldup_ * waterDensity_ + (100.0 - waterHoldup_) * oilDensity_) / 100.0;

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
		}

        // Scaled confObject parameters
        private double oilDensity_;
        private double waterDensity_;
        private double waterHoldup_;
        private double gasFactor_;
    };
}
