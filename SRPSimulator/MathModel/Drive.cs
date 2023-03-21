using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    public class DriveConfigEmbedded : DriveConfig
    {
        // Config's parameters
        //--------------------
        //

        private double nominalN;
        [DisplayName("Nominal motor power"), Description("In kW"), Category("Elecric motor")]
        [Browsable(true)]
        public override double NominalN
        { get => nominalN; set { nominalN = value; NotifyInitInvoke(); } }

        private int polesN;
        [DisplayName("Nuber of motor poles"), Category("Elecric motor")]
        [Browsable(true)]
        public override int PolesN
        { get => polesN; set { polesN = value; NotifyInitInvoke(); } }

        private double slipIdle;
        [DisplayName("Motor rotor slipping at idle"), Category("Elecric motor")]
        [Browsable(true)]
        public override double SlipIdle
        { get => slipIdle; set { slipIdle = value; NotifyInitInvoke(); } }

        private double slipNominal;
        [DisplayName("Motor rotor slipping at nominal power"), Category("Elecric motor")]
        [Browsable(true)]
        public override double SlipNominal
        { get => slipNominal; set { slipNominal = value; NotifyInitInvoke(); } }

        private double gearRatio;
        [DisplayName("Gear ratio"), Category("Gear")]
        [Browsable(true)]
        public override double GearRatio
        { get => gearRatio; set { gearRatio = value; NotifyInitInvoke(); } }

        private double smallPulleyD;
        [DisplayName("Diameter of small pullley of belting"), Description("In meters"), Category("Gear")]
        [Browsable(true)]
        public override double SmallPulleyD
        { get => smallPulleyD; set { smallPulleyD = value; NotifyInitInvoke(); } }

        private double largePulleyD;
        [DisplayName("Diameter of large pullley of belting"), Description("In meters"), Category("Gear")]
        [Browsable(true)]
        public override double LargePulleyD
        { get => largePulleyD; set { largePulleyD = value; NotifyInitInvoke(); } }

        private bool direction;
        [DisplayName("Direction of motor rotation"), Category("Elecric motor")]
        [Browsable(true)]
        public override bool Direction
        { get => direction; set { direction = value; } }
    }
    
	// Electrical motor + reductor
	class Drive : Configurable
	{
        // Drived Object
        private PUnit unit = null;
        public PUnit Unit
        { get => unit; set { unit = value; } }

        // Rotation angle of reductor output shaft, in degrees
        private double fi = 0;
		public double Fi
		{ get => fi; set { fi = value; } }

        // Constructors
        //-------------------

        public Drive()
		{
			Config = new DriveConfigEmbedded();

			Init();
        }

        // Initializtion
        //--------------

		public bool SetSliding(double slidingIdle, double slidingNominal)
		{
			this.slipIdle = slidingIdle / 100.0;
			this.slipNominal = slidingNominal / 100.0;

			return Init();
		}

		internal override bool Init()
		{
			DriveConfigEmbedded configInit = config as DriveConfigEmbedded;

			// Scaling of config parameters
            nominalN = configInit.NominalN * PhysicsStuff.KILO;
			polesN = configInit.PolesN;
			slipIdle = configInit.SlipIdle / 100.0;
			slipNominal = configInit.SlipNominal / 100.0;
			gearRatio = configInit.GearRatio;
			smallPulleyD = configInit.SmallPulleyD;
			largePulleyD = configInit.LargePulleyD;
			direction = configInit.Direction;
			
			ratio = 1.0 / (gearRatio * largePulleyD / smallPulleyD);

//			nGear = 0;

			// Linear dependence for rotor slipping emulation
			// Suppose while N==0 S=slipIdle and while N==Nном S=slipNominal
			kS = (slipNominal - slipIdle) / nominalN;
			bS = slipIdle;

            configInit.Modified = true;
            configInit.Valid = true;
            
			return true;
		}

		// Simulation process
		//-------------------

		// Initial angle of output reductor shaft
		public bool InitStart(double fi)
		{
			this.fi = fi;

            lastTime = 0;
            lastf = 0;

            unit.InitStart(fi);

			return true;
		}


        /// <summary>
        /// This method must be called periodically to dynamical emulation of drive opeation
        /// The smaller interval between Time on last and current steps smoother the rotation
        /// </summary>
        /// <param name="f">supply frequency, Hz</param>
        /// <param name="time">current Time, ms</param>
        /// <param name="load">load on reductor shaft, Watt</param>
        /// <returns></returns>
        public double Rotate(double f, long time, double load)
		{
			// Slipping
			S = Sliding(load);

			n = (1 - S) * f / polesN;
			double lastn = (1 - S) * lastf / polesN;

			// Frequency corresponding to the actual speed
//			fReal = (S != 1f) ? n * polesN : 0;

			// Instant gear speed
//			nGear = 60.0 * ratio * (n + lastn) / 2.0;

			// Output shaft moving

			// Approximation of f
			double delta = 360.0 * ratio * ((n + lastn) / 2.0) * (time - lastTime) / 1000.0;

            // Angle of output reductor shaft
            fi += direction ? delta : -delta;

			if (fi >= 360)
			{
				fi -= 360.0;
			}
			else if (fi < 0)
			{
				fi += 360.0;
			}

			lastTime = time;
			lastf = f;

			unit.SetFi(fi, time);

			return fi;
		}

		private double Sliding(double N)
		{
			// Simple slipping emulation
			return kS * N + bS;
		}

        // Inner calculations and states
        //------------------------------

        private double ratio;		// Common gear ratio

		private double n;			// Molor rotations per second
		private double S;			// Motor rotor slipping

//		private double fReal;		// Frequency with slipping consideration
//		private double nGear;		// Reductor RPM

		private double kS;			// Linear coeff K for slipping calc
		private double bS;			// Linear coeff B for slipping calc

        // Scaled confObject parameters
        private double nominalN;
        private int polesN;
        private double slipIdle;
        private double slipNominal;
        private double gearRatio;
        private double smallPulleyD;
        private double largePulleyD;
        private bool direction;

		// Saved values and states on last step of calculation
		//----------------------------------------------------

		private long lastTime;		// Point of Time
		private double lastf;		// Supply frequency
    }
}