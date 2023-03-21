using System;
using System.ComponentModel;

namespace SRPSimulator.MathModel
{
    public class SRPConfigEmbedded : SRPConfig.SRPConfig
    {
        // Config's parameters
        //--------------------
        //

        private bool anchor;
        [DisplayName("Anchor presence"), Category("Unit sizes")]
        [Browsable(true)]
        public override bool Anchor
        { get => anchor; set { anchor = value; NotifyInitInvoke(); } }

        private double plungerD;
        [DisplayName("Plunger diameter"), Description("In mm"), Category("SRP sizes")]
        [Browsable(true)]
        public override double PlungerD
        { get => plungerD; set { plungerD = value; NotifyInitInvoke(); } }

        private double filling;
        [DisplayName("Pump filling"), Description(""), Category("Well conditions")]
		[Browsable(true)]
		public override double Filling
		{ get => filling; set { filling = value; NotifyInitInvoke(); } }

        private double zatrubP;
        [DisplayName("Annulus pressure"), Description("Well annulus gas pressure in atm"), Category("Well conditions")]
        [Browsable(true)]
        public override double ZatrubP
        { get => zatrubP; set { zatrubP = value; NotifyInitInvoke(); } }

        private double pipeP;
        [DisplayName("Pipe pressure"), Description("Pipeline pressure in atm"), Category("Well conditions")]
        [Browsable(true)]
		public override double PipeP
		{ get => pipeP; set { pipeP = value; NotifyInitInvoke(); } }

        private double fluidH;
        [DisplayName("Fluid level"), Description("Fluid level relative to intake in m"), Category("Well conditions")]
        [Browsable(true)]
        public override double FluidH
        { get => fluidH; set { fluidH = value; NotifyInitInvoke(); } }

        private double frictionPlunger;
        [DisplayName("Plunger friction force"), Description("In Newtons"), Category("Well conditions")]
        [Browsable(true)]
        public override double FrictionPlunger
        { get => frictionPlunger; set { frictionPlunger = value; NotifyInitInvoke(); } }

		private double frictionSeal;
        [DisplayName("Seal friction force"), Description("In Newtons"), Category("Well conditions")]
        [Browsable(true)]
        public override double FrictionSeal
        { get => frictionSeal; set { frictionSeal = value; NotifyInitInvoke(); } }
    }

    // Sucker Rod Pump
    class SRP : Configurable
    {
		// Rod motion stages
		//------------------
		//

//		private const byte rodStopped = 0;		// Stopped
//		private const byte rodDirectionUS = 1;	// Upstroke
//		private const byte rodDirectionDS = 2;  // Downstroke

		// Input values
		//-----------------
		//

		// Length of rod stroke, is initializing from Unit obj
		public double rodL;
		public double RodL
		{ get => rodL; set { rodL = value; /*Init();*/ } }

		// Output values
		//------------------
		//

		// Current force at the upper end of the rod
		private double rodF;
		public double RodF
		{ get => rodF; }

		// Current friction force
		private double frictionF;
		public double FrictionF
		{ get => frictionF; }

		// Plunger position relative to intake
		private double plungerX;
		public double PlungerX
		{ get => plungerX; }

        // Constructors
        //-------------------
        //
        
		public SRP(Fluid fluid, Rod rod, Tubing tubing)
        {
			this.fluid = fluid;
			this.rod = rod;
			this.tubing = tubing;

            Config = new SRPConfigEmbedded();

            Init();
        }

        // Methods
        //-------------------
        //
        
		// This method performs the step by step simulation of pump operation
        public double SetX(double X, long time)
		{
			rodXStep2 = rodXStep1;
			rodXStep1 = rodX;
			rodX = X;

			timeStep2 = timeStep1;
			timeStep1 = this.time;
			this.time = time;

			double tension = rodX - plungerX;

			// Coefficient of hydrotransformation between the plunger and the end of the tubing
			double hydroTransK = tubing.InnerS / plungerS;

			// Reduced stretch coefficient of tubing rod
			double tensionK = rod.TensionK + (anchor ? 0 : hydroTransK * tubing.TensionK);

			rodF = rodFDS + tension / tensionK;

			if (rodX < rodXStep1)
			{
//				status = rodDirectionDS;

				if (filling < 1.0 && plungerX > filling * (rodL - maxTension))
				{
					// Gas compression in progress

					frictionF = -frictionSeal;

					if (rodF <= rodFUS - 2.0 * frictionPlunger)
					{
						double frictionTension = 2.0 * frictionPlunger * tensionK;

						plungerX = rodX - maxTension + frictionTension;
					}
				}
				else
				{
					// Rod compression in progress

					frictionF = -frictionSeal;

					if (rodF <= rodFDS)
					{
						plungerX = rodX;
					}
				}
			}
			else if (rodX > rodXStep1)
			{
//				status = rodDirectionUS;

				// Seal friction force is always added while rod in motion
				// Friction of plunger depends on rod stretch force.
				// Until it less than rodFUS plunger is stopped
				frictionF = frictionSeal;

				if (rodF >= rodFUS)
				{
					// rodF = rodFUS;
					plungerX = rodX - maxTension;
				}
			}
			else
			{
//				status = rodStopped;
			}

			return rodF;
		}

		internal override bool Init()
		{
            SRPConfigEmbedded configInit = config as SRPConfigEmbedded;

            if (fluid is null || tubing is null || rod is null)
            {
                configInit.Valid = false;
                return false;
            }

            // Scaling of config parameters
            anchor = configInit.Anchor;
            plungerD = configInit.PlungerD * PhysicsStuff.MILLI;
            if (configInit.Filling < 0)
            {
                filling = 0;
            }
            else if (configInit.Filling > 1)
            {
                filling = 1.0;
            }
            else
            {
                filling = configInit.Filling;
            }
            zatrubP = configInit.ZatrubP / PhysicsStuff.PASCAL_TO_ATM;
			pipeP = configInit.PipeP / PhysicsStuff.PASCAL_TO_ATM;
			fluidH = configInit.FluidH;
			frictionPlunger = configInit.FrictionPlunger;
			frictionSeal = configInit.FrictionSeal;

			// Intake pressure
			fluidP = fluid.FluidDensity * PhysicsStuff.g * fluidH;

			// Plunger area and effective plunger area (whithout rod cross sectional area)
			plungerS = Math.PI * plungerD * plungerD / 4.0;
			double plungerSEffective = plungerS - rod.Sections[^1].S;
 //           double plungerSEffective = plungerS - rod.Sections[rod.Sections.Count - 1].S;

            TubingConfigEmbedded tubingConfig = tubing.Config as TubingConfigEmbedded;
            
			// Pressures on the plunger's top and bottom
            double plungerF = (pipeP + fluid.FluidDensity * PhysicsStuff.g * tubingConfig.Length) * plungerSEffective;
			double plungerFBottomDS = (pipeP + fluid.FluidDensity * PhysicsStuff.g * tubingConfig.Length) * plungerS;
			double plungerFBottomUS = (zatrubP + fluidP) * plungerS;

			// Rod weight in fluid (force at the upper end in steady downstroke)
			double depth = 0;
			rodFDS = rod.Weight;
			for (short ii = 0; ii < rod.Sections.Count; ii++)
			{
				depth += rod.Sections[ii].Config.Length;

				rodFDS += (pipeP + fluid.FluidDensity * PhysicsStuff.g * depth) * rod.interS[ii];
			}

			// Force at the upper rod end in steady upstroke
			rodFUS = rodFDS + plungerF - plungerFBottomUS + frictionPlunger;

			// + Force of pressure on lower end of the rod
			rodFDS += plungerF - plungerFBottomDS - frictionPlunger;

			// Forces, applyed on lower end of tubing
			double tubingFDS = (pipeP + fluid.FluidDensity * PhysicsStuff.g * tubingConfig.Length) * tubing.InnerS + frictionPlunger;
			double tubingFUS = (zatrubP + fluidP) * tubing.InnerS - frictionPlunger;

			// Force of tubing stretch, (i.e. difference between forces at up and downstroke)
			tubingMaxTension = anchor ? 0 : (tubingFDS - tubingFUS) * tubing.TensionK;

			// Force of rod stretch, (i.e. difference between forces at US and DS)
			// (or, equivalently, the difference between forces on the bottom of the plunger at DS and US)
			rodMaxTension = (rodFUS - rodFDS) * rod.TensionK;

			// Total maximum stretch
			maxTension = tubingMaxTension + rodMaxTension;

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
		}

		// DI objects
		//---------------------------------
		// It must be initilized before use

		internal Fluid fluid;
		internal Rod rod;
		internal Tubing tubing;

        // Inner calculations and states
        //------------------------------

        private double rodX;				// Set coordinate of rod upper end
		private long time;					// Set Time

//		private byte status;                // Motion stage

		private double fluidP;				// Intake fluid pressure

		private double plungerS;			// Square of plunger

		private double rodFDS;				// Rod weight in fluid (at downstroke), N
		private double rodFUS;				// Rod + fluid weight (at upstroke), N

		private double tubingMaxTension;	// Max theoretical tubing stretch
		private double rodMaxTension;       // Max theoretical rod stretch
		private double maxTension;          // Max theoretical tubing and rod stretch

		// For derivative calculation
		
		private double rodXStep1;			// Rod position at last step
		private double rodXStep2;           // Rod position at 2 steps ago

		private long timeStep1;             // Time at last step
		private long timeStep2;             // Time at 2 steps ago

        // Scaled confObject parameters
        private bool anchor;
        private double plungerD;
        private double filling;
        private double zatrubP;
        private double pipeP;
        private double fluidH;
        private double frictionPlunger;
        private double frictionSeal;
    }
}