using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    public class DriveConfigBrowsable : DriveConfig
    {
        private double nominalN;
        [DisplayName("Nominal motor power"), Description("In kW")]
        [Category("Elecric motor")]
        [Browsable(true)]
        public override double NominalN { 
            get => nominalN; 
            set { 
                nominalN = value; 
                NotifyInitInvoke(); 
            } 
        }

        private int polesN;
        [DisplayName("Nuber of motor poles")]
        [Category("Elecric motor")]
        [Browsable(true)]
        public override int PolesN { 
            get => polesN; 
            set { 
                polesN = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double slipIdle;
        [DisplayName("Motor rotor slipping at idle")]
        [Category("Elecric motor")]
        [Browsable(true)]
        public override double SlipIdle { 
            get => slipIdle; 
            set { 
                slipIdle = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double slipNominal;
        [DisplayName("Motor rotor slipping at nominal power")]
        [Category("Elecric motor")]
        [Browsable(true)]
        public override double SlipNominal {
            get => slipNominal; 
            set { 
                slipNominal = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double gearRatio;
        [DisplayName("Gear ratio")]
        [Category("Gear")]
        [Browsable(true)]
        public override double GearRatio { 
            get => gearRatio; 
            set { 
                gearRatio = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double smallPulleyD;
        [DisplayName("Diameter of small pullley of belting"), Description("In meters")]
        [Category("Gear")]
        [Browsable(true)]
        public override double SmallPulleyD { 
            get => smallPulleyD; 
            set { 
                smallPulleyD = value; 
                NotifyInitInvoke(); 
            } 
        }

        private double largePulleyD;
        [DisplayName("Diameter of large pullley of belting"), Description("In meters")]
        [Category("Gear")]
        [Browsable(true)]
        public override double LargePulleyD { 
            get => largePulleyD; 
            set { 
                largePulleyD = value; 
                NotifyInitInvoke(); 
            } 
        }

        private bool direction;
        [DisplayName("Direction of motor rotation")]
        [Category("Elecric motor")]
        [Browsable(true)]
        public override bool Direction { 
            get => direction; 
            set => direction = value; 
        }
    }
    
	// Electrical motor + reductor
	class Drive : Configurable
	{
        // Drived Object
        private PUnit unit = null;
        public PUnit Unit { 
            get => unit; 
            set => unit = value; 
        }

        // Rotation angle of reductor output shaft, in degrees
        private double fi = 0;
		public double Fi { 
            get => fi; 
            set => fi = value; 
        }

        public Drive(DriveConfigBrowsable config)
            : base(config)
		{
        }

		public bool SetSlipping(double slidingIdle, double slidingNominal)
		{
			slipIdle_ = slidingIdle / 100.0;
			slipNominal_ = slidingNominal / 100.0;
			return Init();
		}

		internal override bool Init()
		{
			DriveConfigBrowsable configInit = config as DriveConfigBrowsable;

			// Scaling of config parameters
            nominalN_ = configInit.NominalN * Physical.KILO;
			polesN_ = configInit.PolesN;
			slipIdle_ = configInit.SlipIdle / 100.0;
			slipNominal_ = configInit.SlipNominal / 100.0;
			gearRatio_ = configInit.GearRatio;
			smallPulleyD_ = configInit.SmallPulleyD;
			largePulleyD_ = configInit.LargePulleyD;
			direction_ = configInit.Direction;
			
			ratio_ = 1.0 / (gearRatio_ * largePulleyD_ / smallPulleyD_);

			// Linear dependence for rotor slipping emulation
			// Suppose while N==0 S=slipIdle and while N==Nном S=slipNominal
			kS_ = (slipNominal_ - slipIdle_) / nominalN_;
			bS_ = slipIdle_;

            configInit.Modified = true;
            configInit.Valid = true;
            
			return true;
		}

		// Initial angle of output reductor shaft
		public bool InitStart(double fi)
		{
			this.fi = fi;
            lastTime_ = 0;
            lastf_ = 0;
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
        public double Rotate(double f, long time)
		{
			S_ = GetSlipping(unit.N);
			
            n_ = (1 - S_) * f / polesN_;
			double lastn = (1 - S_) * lastf_ / polesN_;

			// Approximation of f
			double delta = 360.0 * ratio_ * ((n_ + lastn) / 2.0) * (time - lastTime_) / 1000.0;
            // Angle of output reductor shaft
            fi += direction_ ? delta : -delta;
			if (fi >= 360)
				fi -= 360.0;
			else if (fi < 0)
				fi += 360.0;
			lastTime_ = time;
			lastf_ = f;
			unit.Rotate(fi, time);

            return fi;
		}

    	// Simple slipping emulation
		private double GetSlipping(double N)
		{
			return kS_ * N + bS_;
		}

        private double ratio_;		// Gear ratio

		private double n_;			// Molor rotations per second
		private double S_;			// Motor rotor slipping

		private double kS_;			// Linear coeff K for slipping calc
		private double bS_;			// Linear coeff B for slipping calc

        // Scaled confObject parameters
        private double nominalN_;
        private int polesN_;
        private double slipIdle_;
        private double slipNominal_;
        private double gearRatio_;
        private double smallPulleyD_;
        private double largePulleyD_;
        private bool direction_;

		private long lastTime_;		// Previous point of Time
		private double lastf_;		// Previous supply frequency
    }
}