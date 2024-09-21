using System;
using System.ComponentModel;

namespace SRPSimulator.MathModel
{
    public class SRPConfigBrowsable : SRPConfig.SRPConfig
    {
        private bool anchor;
        [DisplayName("Anchor presence")]
		[Category("Unit sizes")]
        [Browsable(true)]
        public override bool Anchor { 
			get => anchor; 
			set { 
				anchor = value; NotifyInitInvoke(); 
			} 
		}

        private double plungerD;
        [DisplayName("Plunger diameter"), Description("In mm")]
		[Category("SRP sizes")]
        [Browsable(true)]
        public override double PlungerD { 
			get => plungerD; 
			set { 
				plungerD = value; 
				NotifyInitInvoke(); 
			} 
		}

        private double filling;
        [DisplayName("Pump filling"), Description("")]
		[Category("Well conditions")]
		[Browsable(true)]
		public override double Filling { 
			get => filling; 
			set { 
				filling = value; 
				NotifyInitInvoke(); 
			} 
		}

        private double zatrubP;
        [DisplayName("Annulus pressure"), Description("Well annulus gas pressure in atm")]
		[Category("Well conditions")]
        [Browsable(true)]
        public override double ZatrubP { 
			get => zatrubP; set { 
				zatrubP = value; NotifyInitInvoke(); 
			} 
		}

        private double pipeP;
        [DisplayName("Pipe pressure"), Description("Onsurface pipeline pressure in atm")]
		[Category("Well conditions")]
        [Browsable(true)]
		public override double PipeP { 
			get => pipeP; 
			set { 
				pipeP = value; 
				NotifyInitInvoke(); 
			} 
		}

        private double fluidH;
        [DisplayName("Fluid level"), Description("Fluid level relative to intake in m")]
		[Category("Well conditions")]
        [Browsable(true)]
        public override double FluidH { 
			get => fluidH; 
			set { 
				fluidH = value; 
				NotifyInitInvoke(); 
			} 
		}

        private double frictionPlunger;
        [DisplayName("Plunger friction force"), Description("In Newtons")]
		[Category("Well conditions")]
        [Browsable(true)]
        public override double FrictionPlunger { 
			get => frictionPlunger; 
			set { 
				frictionPlunger = value; 
				NotifyInitInvoke(); 
			} 
		}

		private double frictionSeal;
        [DisplayName("Seal friction force"), Description("In Newtons")]
		[Category("Well conditions")]
        [Browsable(true)]
        public override double FrictionSeal { 
			get => frictionSeal; 
			set { 
				frictionSeal = value; 
				NotifyInitInvoke(); 
			} 
		}

		public double upperValveLeakage;
        [DisplayName("Upper valve leakage"), Description("Dimensionless")]
        [Category("Deterioration emulation")]
        [Browsable(true)]
        public override double UpperValveLeakage { 
			get => upperValveLeakage; 
			set { 
				upperValveLeakage = value; 
				NotifyInitInvoke(); 
			} 
		}

		public double intakeValveLeakage;
        [DisplayName("Intake valve leakage"), Description("Dimensionless")]
        [Category("Deterioration emulation")]
        [Browsable(true)]
        public override double IntakeValveLeakage {
			get => intakeValveLeakage; 
			set { 
				intakeValveLeakage = value; 
				NotifyInitInvoke(); 
			} 
		}
    }

    internal class Valve(double startClosingV, double endClosingV)
    {
        public bool IsLeak() { return endClosingV > 0; }

        public double GetSealing(double V)
        {
            if (V < 0 || !IsLeak() || V > endClosingV)
                return 1;
            if (V < startClosingV)
                return 0;
            return (V - startClosingV) / (endClosingV - startClosingV);
        }
    }

    class SRP : Configurable
    {
        private Fluid fluid;
        public Fluid Fluid { 
			get => fluid; 
			set => fluid = value; 
		}

        private Rod rod;
        public Rod Rod {
			get => rod;
			set => rod = value;
		}

        private Tubing tubing;
        public Tubing Tubing {
			get => tubing;
			set => tubing = value;
		}

        // Length of rod stroke, is initializing from Unit obj
        public double rodL;
		public double RodL
		{ get => rodL; set { rodL = value; /*Init();*/ } }

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

        // Current velocity of the rod
        private double rodV = 0;
        public double RodV
        { get => rodV; }

        private double plungerV = 0;
        public double PlungerV
        { get => plungerV; }

        // Current acceleration of the rod
        private double rodA = 0;
        public double RodA
        { get => rodA; }

        public SRP(SRPConfigBrowsable config, Fluid fluid, Rod rod, Tubing tubing)
			: base(null)
        {
			Fluid = fluid;
			Rod = rod;
			Tubing = tubing;
			Config = config;
			rodVCalculator_ = new();
            plungerVCalculator = new();
        }

        internal override bool Init()
        {
            SRPConfigBrowsable configInit = config as SRPConfigBrowsable;
            TubingConfigBrowsable tubingConfig = tubing.Config as TubingConfigBrowsable;

            if (fluid is null || tubing is null || rod is null)
            {
                configInit.Valid = false;
                return false;
            }

            // Scaling of config parameters
            anchor_ = configInit.Anchor;
            plungerD_ = configInit.PlungerD * Physical.MILLI;
			filling_ = Math.Clamp(configInit.Filling, 0, 1);
            zatrubP_ = configInit.ZatrubP / Physical.PASCAL_TO_ATM;
            pipeP_ = configInit.PipeP / Physical.PASCAL_TO_ATM;
            fluidH_ = configInit.FluidH;
            frictionPlunger_ = configInit.FrictionPlunger;
            frictionSeal_ = configInit.FrictionSeal;
            upperValve_ = new(0, configInit.UpperValveLeakage);
            intakeValve_ = new(0, configInit.IntakeValveLeakage);

            // Intake pressure
            fluidP_ = fluid.FluidDensity * Physical.g * fluidH_;

            // Plunger area and effective plunger area (whithout rod cross sectional area)
            plungerS_ = Math.PI * plungerD_ * plungerD_ / 4.0;
            double plungerSEffective = plungerS_ - rod.Sections[^1].S;

			double plungerUpsideP = pipeP_ + fluid.FluidDensity * Physical.g * tubingConfig.Length;

            // Pressures on the plunger's top and bottom
            double plungerFTop = plungerUpsideP * plungerSEffective;
            double plungerFBottomDS = plungerUpsideP * plungerS_;
            double plungerFBottomUS = (zatrubP_ + fluidP_) * plungerS_;

            // Rod weight in fluid (force at the upper end in steady downstroke)
            double depth = 0;
            rodFDS_ = rod.Weight;
            for (short ii = 0; ii < rod.Sections.Count; ii++) {
                depth += rod.Sections[ii].Config.Length;
                rodFDS_ += (pipeP_ + fluid.FluidDensity * Physical.g * depth) * rod.interS[ii];
            }

            // Force at the upper rod end in steady upstroke
            rodFUS_ = rodFDS_ + plungerFTop - plungerFBottomUS + frictionPlunger_;

            // + Force of pressure on lower end of the rod
            rodFDS_ += plungerFTop - plungerFBottomDS - frictionPlunger_;

            // Forces, applyed on lower end of tubing
            tubingFDS_ = plungerUpsideP * tubing.InnerS + frictionPlunger_;
            tubingFUS_ = (zatrubP_ + fluidP_) * tubing.InnerS - frictionPlunger_;

            // Force of tubing stretch, (i.e. difference between forces at up and downstroke)
            tubingMaxTension_ = anchor_ ? 0 : (tubingFDS_ - tubingFUS_) * tubing.TensionK;

            // Force of rod stretch, (i.e. difference between forces at US and DS)
            // (or, equivalently, the difference between forces on the bottom of the plunger at DS and US)
            rodMaxTension_ = (rodFUS_ - rodFDS_) * rod.TensionK;

            // Total maximum stretch
            maxTension_ = tubingMaxTension_ + rodMaxTension_;

            // Coefficient of hydrotransformation between the plunger and the end of the tubing
            double hydroTransK = tubing.InnerS / plungerS_;

            // Reduced stretch coefficient of tubing and rod
            tensionK_ = rod.TensionK + (anchor_ ? 0 : hydroTransK * tubing.TensionK);

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
        }

        public double test;

        // This method performs the step by step simulation of pump operation
        public double SetX(double X, long time)
		{
            rodV = rodVCalculator_.NextStep(X, time);
            plungerV = plungerVCalculator.NextStep(plungerX, time);

            var tension = X - plungerX;
			rodF = rodFDS_ + tension / tensionK_;

            var upperValveSealing = upperValve_.GetSealing(rodV);
            var tensionUS = upperValveSealing * maxTension_;

            test = plungerV;

            if (X < rodXLast_) {
				if (filling_ < 1.0 && plungerX > filling_ * (rodL - maxTension_)) {
					// Gas compression in progress
					frictionF = -frictionSeal_;
					if (rodF <= rodFUS_ - 2.0 * frictionPlunger_) {
						double frictionTension = 2.0 * frictionPlunger_ * tensionK_;
						plungerX = X - maxTension_ + frictionTension;
					}
				}
				else {
                    var intakeValveSealing = intakeValve_.GetSealing(-rodV);
                    var tensionDS = tensionUS - intakeValveSealing * tensionUS;
                    frictionF = -frictionSeal_;
                    if (tension <= tensionDS)
						plungerX = X - tensionDS;
                    // else rod compression in progress
				}
			}
			else if (X > rodXLast_) {
				// Seal friction force is always added while rod in motion
				// Friction of plunger depends on rod stretch force.
				// Until it less than rodFUS plunger is stopped
				frictionF = frictionSeal_;
                if (tension >= tensionUS)
                    plungerX = X - tensionUS;
            }
			else {
//				status = rodStopped;
			}

			rodXLast_ = X;

            return rodF;
		}

        // Inner calculations and states

		private long time_;					// Set Time
		private double fluidP_;				// Intake fluid pressure
		private double plungerS_;			// Square of plunger
		private double rodFDS_;				// Rod weight in fluid (at downstroke), N
		private double rodFUS_;				// Rod + fluid weight (at upstroke), N
		private double tubingMaxTension_;	// Max theoretical tubing stretch
		private double rodMaxTension_;      // Max theoretical rod stretch
		private double maxTension_;         // Max theoretical tubing and rod stretch
		private double tensionK_;			// Reduced stretch coefficient of tubing and rod

        double tubingFDS_;
        double tubingFUS_;
        
        private double rodXLast_;			// Rod position at last step

        // Scaled confObject parameters
        private bool anchor_;
        private double plungerD_;
        private double filling_;
        private double zatrubP_;
        private double pipeP_;
        private double fluidH_;
        private double frictionPlunger_;
        private double frictionSeal_;

		Valve upperValve_;
		Valve intakeValve_;
		DerivativeCalculator rodVCalculator_;
		DerivativeCalculator plungerVCalculator;
    }
}