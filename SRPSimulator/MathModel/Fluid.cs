using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    public class FluidConfigEmbedded : FluidConfig
    {
        // Config's parameters
        //--------------------
        //

        private double oilDensity;
		[DisplayName("Oil density"), Description("in g/sm3"), Category("Fluid parameters")]
        [Browsable(true)]
        public override double OilDensity
        { get => oilDensity; set { oilDensity = value; NotifyInitInvoke(); } }

        private double waterDensity;
        [DisplayName("Water density"), Description("in g/sm3"), Category("Fluid parameters")]
        [Browsable(true)]
        public override double WaterDensity
        { get => waterDensity; set { waterDensity = value; NotifyInitInvoke(); } }

        private double waterHoldup;
        [DisplayName("Amount of water"), Description("in %"), Category("Fluid parameters")]
        [Browsable(true)]
        public override double WaterHoldup
        { get => waterHoldup; set { waterHoldup = value; NotifyInitInvoke(); } }

        private double gasFactor;
        [DisplayName("Gas factor"), Description(""), Category("Fluid parameters")]
        [Browsable(true)]
        public override double GasFactor
        { get => gasFactor; set { gasFactor = value; NotifyInitInvoke(); } }
    }
    
	// Well fluid
    class Fluid : Configurable
	{
		// Output values
		//------------------------------

		private double fluidDensity;    // Well fluid density
		internal double FluidDensity
		{ get => fluidDensity; }

        // Constructors
        //-------------------

        public Fluid()
		{
            Config = new FluidConfigEmbedded();

            Init();
        }

		internal override bool Init()
        {
            FluidConfigEmbedded configInit = config as FluidConfigEmbedded;

            // Scaling of config parameters
            oilDensity = configInit.OilDensity * PhysicsStuff.KILO;
            waterDensity = configInit.WaterDensity * PhysicsStuff.KILO;
            waterHoldup = configInit.WaterHoldup;
            gasFactor = configInit.GasFactor;

            fluidDensity = (waterHoldup * waterDensity + (100.0 - waterHoldup) * oilDensity) / 100.0;

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
		}

        // Inner calculations and states
        //------------------------------

        // Scaled confObject parameters
        private double oilDensity;
        private double waterDensity;
        private double waterHoldup;
        private double gasFactor;
    };
}
