using System;
using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    public class PUnitConfigBrowsable : PUnitConfig
    {
        public const String labelSizeA = "Size A";
        public const String labelSizeC = "Size C";
        public const String labelSizeG = "Size G";
        public const String labelSizeH = "Size H";
        public const String labelSizeI = "Size I";
        public const String labelSizeK = "Size K";
        public const String labelSizeP = "Size P";
        public const String labelSizeR = "Size R";

        public delegate void SizeChanged();
        public event SizeChanged SizeChangedEvent;

		private int sizeR;
		[DisplayName(labelSizeR), Description("Crank rotation radius. Must be specified in mm")] 
		[Category("Unit sizes")]
		[Browsable(true)]
		public override int SizeR { 
			get => sizeR; 
			set { 
				sizeR = value; 
				NotifyInitInvoke(); 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int sizeA;
        [DisplayName(labelSizeA), Description("Beam working arm length. Must be specified in mm")] 
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeA { 
			get => sizeA; 
			set { 
				sizeA = value; 
				NotifyInitInvoke(); 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int sizeC;
        [DisplayName(labelSizeC), Description("Length between beam axis and crank-beam joint. Must be specified in mm")] 
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeC { 
			get => sizeC; 
			set { 
				sizeC = value; 
				NotifyInitInvoke(); 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int sizeI;
        [DisplayName(labelSizeI), Description("in mm")]
		[ Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeI { 
			get => sizeI; 
			set { 
				sizeI = value; 
				NotifyInitInvoke(); 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int sizeG;
        [DisplayName(labelSizeG), Description("in mm")]
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeG { 
			get => sizeG; 
			set { 
				sizeG = value; 
				NotifyInitInvoke(); 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int sizeH;
        [DisplayName(labelSizeH), Description("in mm")]
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeH { 
			get => sizeH; 
			set { 
				sizeH = value; 
				NotifyInitInvoke(); 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int sizeP;
        [DisplayName(labelSizeP), Description("Connecting rod length. Must be specified in mm")]
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeP { 
			get => sizeP; 
			set { 
				sizeP = value; 
				NotifyInitInvoke(); 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int sizeK;
        [DisplayName(labelSizeK), Description("Length between beam axis and crank axis. Must be specified in mm")]
		[Category("Unit sizes")]
        [Browsable(false)]
        public override int SizeK { 
			get => sizeK; 
			set { 
				sizeK = value; 
				NotifyInitInvoke(); 
			} 
		}

        private int counterweight;
        [DisplayName("Counterweight"), Description("Counterweight in kg")]
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int Counterweight { 
			get => counterweight; 
			set { 
				counterweight = value; 
				NotifyInitInvoke(); 
			} 
		}

        // Additional sizes

        private int crankWidth;
        [DisplayName("Crank width"), Description("in mm")]
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int CrankWidth { 
			get => crankWidth; 
			set { 
				crankWidth = value; 
				SizeChangedEvent?.Invoke(); 
			} 
		}

        private int beamWidth;
        [DisplayName("Beam width"), Description("in mm")]
		[Category("Unit sizes")]
        [Browsable(true)]
        public override int BeamWidth { 
			get => beamWidth; 
			set { 
				beamWidth = value; 
				SizeChangedEvent?.Invoke(); 
			} 
		}
    }

	// Onground Pumping Unit
    class PUnit : Configurable
	{
		// States of Pumping Unit after one step shifting 
		protected const ushort statusDDPPassed = 0x0001;	// DDP just passed
		protected const ushort statusUDPPassed = 0x0002;	// UDP just passed
		protected const ushort statusCW = 0x0004;			// Crank rotates clockwise (rigt side view)
		protected const ushort statusCCW = 0x0008;			// Crank rotates counter clockwise (rigt side view)
		protected const ushort statusUS = 0x0010;			// Upstroke
		protected const ushort statusDS = 0x0020;			// Downstroke

		protected const ushort statusERROR = 0xFFFF;		// Error occured

        public override ConfigIdentity Config {
			get => config;
			set {
				config = value;
				if (config != null) {
					config.NotifyInit += Init;
					(config as PUnitConfigBrowsable).SizeChangedEvent += SizesChanged;
					Init();
				}
            }
		}

        private Drive drive;
        public Drive Drive { 
			get => drive; 
			set { 
				drive = value;
				drive.Unit = this; 
			} 
		}

        private VFC vfc;
        public VFC Vfc { 
			get => vfc; 
			set { 
				vfc = value;
				vfc.Drive = drive;
			} 
		}

        private SRP srp = null;
        public SRP Srp {
            get => srp;
            set {
                srp = value;
                if (srp is not null)
                    srp.RodL = rodL_;
            }
        }

        // Crank angle at DDP relative to the dial
        private double fiDDP = 0;
        public double DDP
		{ get => fiDDP; }

		// Crank angle at UDP relative to the dial
		private double fiUDP = 0;
        public double UDP
		{ get => fiUDP; }

        // Coordinate of the "crank-connecting rod" joint
        private Position crankPos = new(); 
        public Position CrankPos
		{ get => crankPos; }

        // Coordinate of the beam end
        private Position beamPos = new();  
        public Position BeamPos
		{ get => beamPos; }

        // Coordinate of the beam axis
        private Position beamAxis = new(); 
        public Position BeamAxis
		{ get => beamAxis; }

        // Coordinate of the "beam-connecting rod" joint
        private Position crPos = new();    
        public Position CRPos
		{ get => crPos; }

		// Current coordinate of the rod, 0 = DDP
		private double rodX = 0;
        public double RodX
		{ get => rodX; }

		// Crank angle (0 - DDP)
		private double fi = 0;
        public double Fi
		{ get => fi; }

		// Beam angle relative to the horizontal
		private double alpha = 0;
        public double Alpha
		{ get => alpha; }

		// Connection rod angle relative to the horizontal
		private double crAngle = 0;
        public double CRAngle
        { get => crAngle; }

		// Active power on the crank shaft
		private double n = 0;
        public double N
		{ get => n; }

		// Status after the last quant of moving
		private ushort status = 0;
        public ushort Status
		{ get => status; }

        // Current stroke status

        public bool DDPPassed
        { get => (status & statusDDPPassed) != 0; }

        public bool UDPPassed
        { get => (status & statusUDPPassed) != 0; }

        public bool RotationCW
        { get => (status & statusCW) != 0; }

        public bool RotationCCW
        { get => (status & statusCCW) != 0; }

        public event PUnitConfigBrowsable.SizeChanged NotifySizesChanged;

        public PUnit(PUnitConfigBrowsable config, VFC vfc, Drive drive, SRP srp)
			: base(null)
		{
			Srp = srp;
			Drive = drive;
			Vfc = vfc;
			Config = config;
		}

        internal override bool Init()
		{
			PUnitConfigBrowsable configInit = config as PUnitConfigBrowsable;

            // Scaling of config parameters
            sizeR_ = configInit.SizeR * Physical.MILLI;
			sizeA_ = configInit.SizeA * Physical.MILLI;
			sizeC_ = configInit.SizeC * Physical.MILLI;
			sizeI_ = configInit.SizeI * Physical.MILLI;
			sizeG_ = configInit.SizeG * Physical.MILLI;
			sizeH_ = configInit.SizeH * Physical.MILLI;
			sizeP_ = configInit.SizeP * Physical.MILLI;
			sizeK_ = configInit.SizeK * Physical.MILLI;
			counterweight_ = configInit.Counterweight;
            crankWidth_ = configInit.CrankWidth * Physical.MILLI;
	        beamWidth_ = configInit.BeamWidth * Physical.MILLI;

            configInit.Valid = false;
            
			// Coordinates of beam axis
            beamAxis.x = sizeI_;
			beamAxis.y = sizeH_ - sizeG_;

			sizeK_ = Math.Sqrt(beamAxis.x * beamAxis.x + beamAxis.y * beamAxis.y);

			// Angle of sizeK-line
			fi1_ = 90.0 - Physical.RadiansToDegree(Math.Acos(beamAxis.x / sizeK_));

			alpha = 0;
			double fiMem = fi;

            // Angles at UDP
            if (!CalcFiUDP(ref fiUDP))
				return false;
			if (SetFi(fiUDP) == statusERROR)
				return false;
			alphaUDP_ = alpha;

			// Lenght of the rod stroke 
			rodL_ = rodX;
			if (srp is not null)
				srp.RodL = rodL_;

			// Angles at DDP
			if (!CalcFiDDP(ref fiDDP))
				return false;
			if (SetFi(fiDDP) == statusERROR)
				return false;
            alphaDDP_ = alpha;
			SetFi(fiMem);
			//drive.Fi = fiDDP;
			drive.Fi = fi;
            fiLast_ = fi;

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
		}

		private void SizesChanged()
		{
			NotifySizesChanged?.Invoke();
			config.Modified = true;
        }

        public bool InitStart(double fi)
		{
			fiStep2_ = 0;
			fiStep1_ = 0;
			fiLast_ = fi;
			rodXTime2_ = 0;
			rodXTime1_ = 0;
			rodXTime_ = 0;
			n = 0;
			status = 0;
			return statusERROR != SetFi(fi, 0);
		}

		// This method is called for the simulation process
		// Time and angle of the cranck must be set at each step
		public void Rotate(double fi, long time = 0)
		{
			SetFi(fi, time);
            FToN(Srp.RodF + Srp.FrictionF);
        }

        public ushort SetFi(double fi, long time = 0)
		{
			fiStep2_ = fiStep1_;
			fiStep1_ = this.fi;
			this.fi = fi;

			CalcAngles();

			// Coordinate of rod
			rodXStep2_ = rodXStep1_;
			rodXStep1_ = rodX;

			rodX = FiToX(fi);

            crAngle = Physical.RadiansToDegree(Math.Atan((crPos.x - crankPos.x) / (crPos.y - crankPos.y)));

            // "Pull" the rod
            srp?.SetX(rodX, time);

			// Status
			status = GetStatus();

			if (time > 0) {
				rodXTime2_ = rodXTime1_;
				rodXTime1_ = rodXTime_;
				rodXTime_ = time * Physical.MILLI;

				// Angular speed
				crankW_ = (Math.Abs(fi - fiLast_) < 180 ? 
					Math.Abs(fi - fiLast_) : 
					360.0 - Math.Abs(fi - fiLast_)) / (rodXTime_ - rodXTime1_);

				//crankW = (fi != fiStep2 && rodXTime != rodXTime1 && rodXTime1 != rodXTime2) ?
				//   (fiStep2 - fiStep1) / (rodXTime2 - rodXTime1) + (rodXTime2 - rodXTime1) / (rodXTime2 - rodXTime) *
				//  ((fistep2 - fistep1) / (rodxtime2 - rodxtime1) - (fistep1 - fi) / (rodxtime1 - rodxtime)) : 0;

                crankW_ = Physical.DegreeToRadians(Math.Abs(crankW_));
			}

			fiLast_ = fi;

			return status;
		}

		private bool CalcAngles()
		{
			// Angles of a four-link unit and forces projections
			fi2_ = (fi > fi1_) ? 360f - fi + fi1_ : fi1_ - fi;
			double diag = Math.Sqrt(sizeR_ * sizeR_ + sizeK_ * sizeK_ - 2.0 * sizeR_ * sizeK_ * 
				Math.Cos(Physical.DegreeToRadians(fi2_)));
			fi3_ = Math.Acos((diag * diag - sizeP_ * sizeP_ - sizeC_ * sizeC_) / (-2.0 * sizeP_ * sizeC_));
			ksi_ = Math.PI / 2.0 - fi3_;
			double fi41 = Physical.RadiansToDegree(Math.Acos((sizeC_ * sizeC_ - diag * diag - sizeP_ * sizeP_) / 
				(-2.0 * diag * sizeP_)));
			double fi42 = Physical.RadiansToDegree(Math.Acos((sizeK_ * sizeK_ - diag * diag - sizeR_ * sizeR_) / 
				(-2.0 * diag * sizeR_)));
			fi4_ = fi41 + ((fi2_ > 180) ? 360.0 - fi42 : fi42);
			if (fi4_ >= 360)
				fi4_ -= 360.0;
			gamma_ = 90.0 - (fi4_ > 180 ? (360.0 - fi4_) : fi4_);
			betta_ = 90.0 - (fi > 180 ? (360.0 - fi) : fi);
			return true;
		}

		public double FiToX(double fi)
		{
			double radFi = Physical.DegreeToRadians(fi);
			crankPos.x = sizeR_ * Math.Sin(radFi);
			crankPos.y = sizeR_ * Math.Cos(radFi);

			// Coordinate of "beam-connecting rod" joint           
			if (!Physical.Intersection(crankPos, beamAxis, sizeP_, sizeC_, crPos))
				return -1.0;

			double k = (sizeC_ + sizeA_) / sizeC_;
			beamPos.x = crPos.x + (beamAxis.x - crPos.x) * k;
			beamPos.y = crPos.y + (beamAxis.y - crPos.y) * k;

			alpha = Physical.RadiansToDegree(Math.Atan((beamPos.y - crPos.y) / (beamPos.x - crPos.x)));

			// Coordinate of rod
			double X = (alpha - alphaDDP_) * 2.0 * Math.PI * sizeA_ / 360.0;

			if (X < 0)
				X = 0;

			return X;
		}

		public double XToFi(double X, ushort status)
		{
			// Angle of the front arm of the beam
			alpha = alphaDDP_ + 360.0 * X / (2.0 * Math.PI * sizeA_);

            // Angle of the rear arm of the beam
            double alpha1 = Physical.DegreeToRadians(alpha + 180.0);

			crPos.x = beamAxis.x + sizeC_ * Math.Cos(alpha1);
			crPos.y = beamAxis.y + sizeC_ * Math.Sin(alpha1);

			// Coordinate of "beam-connecting rod" joint           
			Position crankAxis = new(0, 0);
			Position crankPos1 = new();
			if (!Physical.Intersection(crankAxis, crPos, sizeR_, sizeP_, crankPos, crankPos1))
				return -1.0;

			// Two variants of connecting rod coordinates
			fi5_ = Physical.RadiansToDegree(Math.Atan(crankPos.y / crankPos.x));
			fi6_ = Physical.RadiansToDegree(Math.Atan(crankPos1.y / crankPos1.x));

			fi5_ = (crankPos.x < 0) ? 270.0 - fi5_ : 90.0 - fi5_;
			fi6_ = (crankPos1.x < 0) ? 270.0 - fi6_ : 90.0 - fi6_;

			// Depending on the direction of rotation and phase, we choose one of variants
			if (((status & statusCW) != 0 && (status & statusUS) != 0) || 
				((status & statusCCW) != 0 && (status & statusDS) != 0))
			{
				crankPos = crankPos1;
				fi5_ = fi6_;
			}

			return 0;
		}

		// Transfer function from the front end of the beam to "crank-connecting rod" joint 
		public bool FToN(double F)
		{
			// Required force applied along the connecting rod
			crF_ = (F * sizeA_ / sizeC_) / Math.Cos(ksi_);

			// The force, applied to the crank consider as positive if concides with rotation direction

			// The gravity force of counterweight, applied to the end of crank
			cwF_ = counterweight_ * Physical.g * Math.Cos(Physical.DegreeToRadians(betta_));
			if (((status & statusCCW) != 0 && fi < 180) || ((status & statusCW) != 0 && fi >= 180))
				cwF_ *= -1.0;

			// Static required force at the end of crank excluding force of counterweight
			crankF_ = crF_ * Math.Cos(Physical.DegreeToRadians(gamma_));
			if ((status & statusDS) != 0)
				crankF_ *= -1.0;

			// Required force at the end of crank including force of counterweight
			torqF_ = crankF_ - cwF_;

			// Required force at the reductor shaft
			n = (torqF_ * sizeR_) * crankW_;

			return true;
		}

		// Transfer function from the "crank-connecting rod" joint to the front end of the beam
		public bool NToF(double N, ref double F)
		{
			torqF_ = (crankW_ != 0) ? N / (sizeR_ * crankW_) : 0;

			cwF_ = counterweight_ * Physical.g * Math.Cos(Physical.DegreeToRadians(betta_));
			if (((status & statusCCW) != 0 && fi < 180) || ((status & statusCW) != 0 && fi >= 180))
				cwF_ *= -1.0;

			crankF_ = torqF_ + cwF_;
			if ((status & statusDS) != 0)
				crankF_ *= -1.0;

			if (Math.Abs(gamma_) > 89.9999)
				// Must not divide by zero
				return false;

			crF_ = crankF_ / Math.Cos(Physical.DegreeToRadians(gamma_));
			F = crF_ * Math.Cos(ksi_);
			F *= sizeC_ / sizeA_;

			return true;
		}

		private ushort GetStatus()
		{
			// Calculations of rotation direction and of passing DDP or UDP after current step

			ushort status = 0;

			if (fi > fiLast_)
				if ((fi - fiLast_) < 180)
					status = (ushort)(statusCW | ((fi >= fiDDP && fiDDP > fiLast_) ? statusDDPPassed : 0) |
						((fi >= fiUDP && fiUDP > fiLast_) ? statusUDPPassed : 0));
				else
					status = (ushort)(statusCCW | ((fi <= fiDDP || fiDDP < fiLast_) ? statusDDPPassed : 0) |
						((fi <= fiUDP || fiUDP < fiLast_) ? statusUDPPassed : 0));
			else if (fi < fiLast_)
				if ((fiLast_ - fi) < 180)
					status = (ushort)(statusCCW | ((fi <= fiDDP && fiDDP < fiLast_) ? statusDDPPassed : 0) |
						((fi <= fiUDP && fiUDP < fiLast_) ? statusUDPPassed : 0));
				else
					status = (ushort)(statusCW | ((fi >= fiDDP || fiDDP > fiLast_) ? statusDDPPassed : 0) |
						((fi >= fiUDP || fiUDP > fiLast_) ? statusUDPPassed : 0));

			// Phase detection - upstroke or downstroke
			if ((status & statusCW) != 0)
				status |= (fi4_ < 180) ? statusDS : statusUS;
			else if ((status & statusCCW) != 0)
				status |= (fi4_ < 180) ? statusUS : statusDS;

			return status;
		}

		private bool CalcFiDDP(ref double fi)
		{
			// Coordinate of beam-connecting rod joint at DDP
			Position crDDP = new();
			Position crankAxis = new(0, 0);

			if (!Physical.Intersection(crankAxis, beamAxis, sizeR_ + sizeP_, sizeC_, crDDP))
				return false;

            // Angle of crank at DDP relative horizontal
            fi = Physical.RadiansToDegree(Math.Atan(crDDP.y / crDDP.x));

			// Suppose DDP places in one of upper quadrants
			fi = fi > 0 ? 90.0 - fi : 270.0 - fi;

			return true;
		}

		private bool CalcFiUDP(ref double fi)
		{
			// Coordinate of crank-beam at UDP
			Position crUDP = new();
			Position crankAxis = new(0, 0);

			if (!Physical.Intersection(crankAxis, beamAxis, sizeP_ - sizeR_, sizeC_, crUDP))
				return false;

            // Angle of crank at UDP relative horizontal
            fi = Physical.RadiansToDegree(Math.Atan(crUDP.y / crUDP.x));

			// Suppose UDP places in one of upper qudrants
			fi = fi > 0 ? 270.0 - fi : 90.0 - fi;

			return true;
		}

        private double rodL_ = 0;		// Length of rod stroke
		private double crankW_ = 0;		// Angular velocity/frequency of crank

		// For derivative calculation

		// Coordinates
		private double rodXStep1_ = 0;	// Rod position at last step
		private double rodXStep2_ = 0;	// Rod position 2 steps ago

		// Times
		private double rodXTime_ = 0;	// Current Time
		private double rodXTime1_ = 0;	// Time at last step
		private double rodXTime2_ = 0;	// Time 2 steps ago

		// Rotation angle of crank
		private double fiStep1_ = 0;	// Crank angle at last step
		private double fiStep2_ = 0;	// Crank angle 2 steps ago

		// Angles (see Descriptions\SRPShcema)
		private double alphaDDP_ = 0; // Beam angle at DDP
		private double alphaUDP_ = 0; // Beam angle at UDP
		private double fi1_ = 0;      // angleBeam of К line
		private double fi2_ = 0;      // angleBeam between crank and К line
		private double fi3_ = 0;      // angleBeam between beam and connecting rod
		private double fi4_ = 0;      // angleBeam between crank and connecting rod
		private double fi5_ = 0;      // angleBeam
        private double fi6_ = 0;      // angleBeam
        private double betta_ = 0;    // angleBeam of projection of load gravity force on connecting rod
		private double ksi_ = 0;      // angleBeam of projection of beam force on connecting rod, in radins
		private double gamma_ = 0;    // angleBeam of projection of connecting rod on crank

		private double fiLast_ = 0;   // Crank angle at last step

		// Forces
		private double crF_ = 0;      // Connecting rod force
		private double crankF_ = 0;   // Projection of connecting rod force on crank
		private double cwF_ = 0;      // angleBeam of projection of load gravity force on crank
		private double torqF_ = 0;    // Force at the end of crank

        // Scaled confObject parameters
        private double sizeR_;
        private double sizeA_;
        private double sizeC_;
        private double sizeI_;
        private double sizeG_;
        private double sizeH_;
        private double sizeP_;
        private double sizeK_;
        private double counterweight_;
        private double crankWidth_;
        private double beamWidth_;
    }
}
