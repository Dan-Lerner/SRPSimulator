using System;
using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    public class TubingConfigEmbedded : TubingConfig
    {
        // Config's parameters
        //--------------------
        //

        public double moduleJung;
        [DisplayName("Jung module"), Description("Jung module for tubing matheril in Pa"), Category("Tubing matherial")]
        [Browsable(true)]
        public override double ModuleJung
        { get => moduleJung; set { moduleJung = value; NotifyInitInvoke(); } }

		public double density;
        [DisplayName("Density"), Description("Density of tubing matheril in kg/m3"), Category("Tubing matherial")]
        [Browsable(true)]
        public override double Density
        { get => density; set { density = value; NotifyInitInvoke(); } }

		public double length;
        [DisplayName("Length"), Description("Length of whole tubing in m"), Category("Tubing geometry")]
        [Browsable(true)]
        public override double Length
        { get => length; set { length = value; NotifyInitInvoke(); } }

		public double innerD;
        [DisplayName("Inner diameter"), Description("Inner tubing diameter in mm"), Category("Tubing geometry")]
        [Browsable(true)]
        public override double InnerD
        { get => innerD; set { innerD = value; NotifyInitInvoke(); } }

		public double outerD;
        [DisplayName("Outer diameter"), Description("Outer tubing diameter in mm"), Category("Tubing geometry")]
        [Browsable(true)]
        public override double OuterD
        { get => outerD; set { outerD = value; NotifyInitInvoke(); } }
    }

    class Tubing : Configurable
	{
		// Output values
		//----------------------

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

        // Constructors
        //-------------------
        //
        
        public Tubing() : base()
        {
            Config = new TubingConfigEmbedded();

            Init();
        }

        internal override bool Init()
        {
            TubingConfigEmbedded configInit = config as TubingConfigEmbedded;

            // Scaling of config parameters
            moduleJung = configInit.ModuleJung;
			density = configInit.Density;
			length = configInit.Length;
			innerD = configInit.InnerD * PhysicsStuff.MILLI;
			outerD = configInit.OuterD * PhysicsStuff.MILLI;

			innerS = Math.PI * innerD * innerD / 4.0;
			outerS = Math.PI * outerD * outerD / 4.0;
			S = outerS - innerS;
			v = length * Math.PI * innerD * innerD / 4.0;

			tensionK = length / (S * moduleJung);

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
		}

		// Inner calculations and states
		//------------------------------

		private double S;			// cross-sectional area, m2

        // Scaled confObject parameters
        private double moduleJung;
        private double density;
        private double length;
        private double innerD;
        private double outerD;
    };
}
