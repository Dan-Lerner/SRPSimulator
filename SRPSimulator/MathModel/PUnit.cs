using System;
using System.ComponentModel;
using SRPConfig;

namespace SRPSimulator.MathModel
{
    // Point for represent unit assembly's coordinates
    class Position
	{
		public Position()
        { }

		public Position(double x, double y)
        {
			this.x = x;
			this.y = y;
		}

		public double x = 0;
		public double y = 0;
	}

    public class PUnitConfigEmbedded : PUnitConfig
    {
        // Constants
        //------------
        //

        // Text for size labels 
        public const String labelSizeA = "Size A";
        public const String labelSizeC = "Size C";
        public const String labelSizeG = "Size G";
        public const String labelSizeH = "Size H";
        public const String labelSizeI = "Size I";
        public const String labelSizeK = "Size K";
        public const String labelSizeP = "Size P";
        public const String labelSizeR = "Size R";

		// Events
		//------------
		//

        public delegate void SizeChanged();
        public event SizeChanged SizeChangedEvent;

		// Config's parameters
		//--------------------
		//


		private int sizeR;
		[DisplayName(labelSizeR), Description("Crank rotation radius. Must be specified in mm"), Category("Unit sizes")]
		[Browsable(true)]
		public override int SizeR
		{ get => sizeR; set { sizeR = value; NotifyInitInvoke(); SizeChangedEvent?.Invoke(); } }

        private int sizeA;
        [DisplayName(labelSizeA), Description("Beam working arm length. Must be specified in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeA
        { get => sizeA; set { sizeA = value; NotifyInitInvoke(); SizeChangedEvent?.Invoke(); } }

        private int sizeC;
        [DisplayName(labelSizeC), Description("Length between beam axis and crank-beam joint. Must be specified in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeC
        { get => sizeC; set { sizeC = value; NotifyInitInvoke(); SizeChangedEvent?.Invoke(); } }

        private int sizeI;
        [DisplayName(labelSizeI), Description("in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeI
        { get => sizeI; set { sizeI = value; NotifyInitInvoke(); SizeChangedEvent?.Invoke(); } }

        private int sizeG;
        [DisplayName(labelSizeG), Description("in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeG
        { get => sizeG; set { sizeG = value; NotifyInitInvoke(); SizeChangedEvent?.Invoke(); } }

        private int sizeH;
        [DisplayName(labelSizeH), Description("in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeH
        { get => sizeH; set { sizeH = value; NotifyInitInvoke(); SizeChangedEvent?.Invoke(); } }

        private int sizeP;
        [DisplayName(labelSizeP), Description("Connecting rod length. Must be specified in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int SizeP
        { get => sizeP; set { sizeP = value; NotifyInitInvoke(); SizeChangedEvent?.Invoke(); } }

        private int sizeK;
        [DisplayName(labelSizeK), Description("Length between beam axis and crank axis. Must be specified in mm"), Category("Unit sizes")]
        [Browsable(false)]
        public override int SizeK
        { get => sizeK; set { sizeK = value; NotifyInitInvoke(); } }

        private int counterweight;
        [DisplayName("Counterweight"), Description("Counterweight in kg"), Category("Unit sizes")]
        [Browsable(true)]
        public override int Counterweight
        { get => counterweight; set { counterweight = value; NotifyInitInvoke(); } }

        // Additional sizes
        //-----------------

        private int crankWidth;
        [DisplayName("Crank width"), Description("in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int CrankWidth
        { get => crankWidth; set { crankWidth = value; SizeChangedEvent?.Invoke(); } }

        private int beamWidth;
        [DisplayName("Beam width"), Description("in mm"), Category("Unit sizes")]
        [Browsable(true)]
        public override int BeamWidth
        { get => beamWidth; set { beamWidth = value; SizeChangedEvent?.Invoke(); } }
    }

	// Onground Pumping Unit
    class PUnit : Configurable
	{
		// Constants
		//----------
		//

		// States of Pumping Unit after one step shifting 
		protected const ushort statusDDPPassed = 0x0001;	// DDP just passed
		protected const ushort statusUDPPassed = 0x0002;	// UDP just passed
		protected const ushort statusCW = 0x0004;			// Crank rotates clockwise (rigt side view)
		protected const ushort statusCCW = 0x0008;			// Crank rotates counter clockwise (rigt side view)
		protected const ushort statusUS = 0x0010;			// Upstroke
		protected const ushort statusDS = 0x0020;			// Downstroke

		protected const ushort statusERROR = 0xFFFF;   // Error occured

        // Associated objects
        //-------------------
		//

        // Config
        public override ConfigIdentity Config
		{
			get => config;
			set {
				config = value;
				config.NotifyInit += Init;
				(config as PUnitConfigEmbedded).SizeChangedEvent += SizesChanged;
				Init();
            }
		}

        // Drive object
        private Drive drive;
        public Drive Drive
        { get => drive; set { drive = value; drive.Unit = this; } }

        // Sucker rod pump object
        private SRP srp = null;
        public SRP Srp
        {
            get => srp;
            set
            {
                srp = value;

                if (srp is not null)
                {
                    srp.RodL = rodL;
                }
            }
        }

        // Output values
        //----------------------------
        //

        // DDP and UDP angles

        // Crank angle at DDP relative to the dial
        private double fiDDP = 0;
        public double DDP
		{ get => fiDDP; }

		// Crank angle at UDP relative to the dial
		private double fiUDP = 0;
        public double UDP
		{ get => fiUDP; }

        // Coordinates of joints

        // Coordinate of "crank-connecting rod" joint
        private Position crankPos = new(); 
        public Position CrankPos
		{ get => crankPos; }

        // Coordinate of beam end
        private Position beamPos = new();  
        public Position BeamPos
		{ get => beamPos; }

        // Coordinate of beam axis
        private Position beamAxis = new(); 
        public Position BeamAxis
		{ get => beamAxis; }

        // Coordinate of "beam-connecting rod" joint
        private Position crPos = new();    
        public Position CRPos
		{ get => crPos; }

		// Current coordinate of rod, 0 = DDP
		private double rodX = 0;
        public double RodX
		{ get => rodX; }

		// Other output values

        // Current velocity of rod
        private double rodV = 0;
        public double RodV
		{ get => rodV; }

		// Current acceleration of rod
		private double rodA = 0;
        public double RodA
		{ get => rodA; }

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

		// Active power on crank shaft
		private double n = 0;
        public double N
		{ get => n; }

		// Status after last quant of moving
		private ushort status = 0;
        public ushort Status
		{ get => status; }

        // Current stroke status (must be converted to events!)

        public bool DDPPassed
        { get => (status & statusDDPPassed) != 0; }

        public bool UDPPassed
        { get => (status & statusUDPPassed) != 0; }

        public bool RotationCW
        { get => (status & statusCW) != 0; }

        public bool RotationCCW
        { get => (status & statusCCW) != 0; }

        // Events
        //-------------------
		//

        public event PUnitConfigEmbedded.SizeChanged NotifySizesChanged;

        // Constructors
        //-------------------
		//

        public PUnit(Drive drive, SRP srp)
		{
			Srp = srp;
			Drive = drive;

			Config = new PUnitConfigEmbedded();

			Init();
		}

        // Methods
        //-------------------
		//

        internal override bool Init()
		{
			PUnitConfigEmbedded configInit = config as PUnitConfigEmbedded;

            // Scaling of config parameters
            sizeR = configInit.SizeR * PhysicsStuff.MILLI;
			sizeA = configInit.SizeA * PhysicsStuff.MILLI;
			sizeC = configInit.SizeC * PhysicsStuff.MILLI;
			sizeI = configInit.SizeI * PhysicsStuff.MILLI;
			sizeG = configInit.SizeG * PhysicsStuff.MILLI;
			sizeH = configInit.SizeH * PhysicsStuff.MILLI;
			sizeP = configInit.SizeP * PhysicsStuff.MILLI;
			sizeK = configInit.SizeK * PhysicsStuff.MILLI;
			counterweight = configInit.Counterweight;
            crankWidth = configInit.CrankWidth * PhysicsStuff.MILLI;
	        beamWidth = configInit.BeamWidth * PhysicsStuff.MILLI;

            configInit.Valid = false;
            
			// Coordinates of beam axis
            beamAxis.x = sizeI;
			beamAxis.y = sizeH - sizeG;

			sizeK = Math.Sqrt(beamAxis.x * beamAxis.x + beamAxis.y * beamAxis.y);

			// Angle of sizeK-line
			fi1 = 90.0 - PhysicsStuff.RadiansToDegree(Math.Acos(beamAxis.x / sizeK));

			alpha = 0;
			double fiMem = fi;

            // Angles at UDP
            if (!CalcFiUDP(ref fiUDP))
			{
				return false;
			}

			if (SetFi(fiUDP) == statusERROR)
			{
				return false;
			}

			alphaUDP = alpha;

			// Lenght of the rod stroke 
			rodL = rodX;

			if (srp is not null)
			{
				srp.RodL = rodL;
			}

			// Angles at DDP
			if (!CalcFiDDP(ref fiDDP))
			{
				return false;
			}

			if (SetFi(fiDDP) == statusERROR)
			{
				return false;
			}

            alphaDDP = alpha;
            
			SetFi(fiMem);

 //			drive.Fi = fiDDP;
            drive.Fi = fi;
            fiLast = fi;

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
			fiStep2 = 0;
			fiStep1 = 0;

			fiLast = fi;

			rodXTime2 = 0;
			rodXTime1 = 0;
			rodXTime = 0;

			rodV = 0;
			rodA = 0;

			n = 0;

			status = 0;

			return statusERROR != SetFi(fi, 0);
		}

		// This method is called for the simulation process
        // Time and angle of cranck must be set at each step
        public ushort SetFi(double fi, long time = 0)
		{
			fiStep2 = fiStep1;
			fiStep1 = this.fi;
			this.fi = fi;

			CalcAngles();

			// Coordinate of rod
			rodXStep2 = rodXStep1;
			rodXStep1 = rodX;

			rodX = FiToX(fi);

            crAngle = PhysicsStuff.RadiansToDegree(Math.Atan((crPos.x - crankPos.x) / (crPos.y - crankPos.y)));

            // "Pull" the rod
            srp?.SetX(rodX, time);

			// Status
			status = GetStatus();

			if (time > 0)
			{
				rodXTime2 = rodXTime1;
				rodXTime1 = rodXTime;
				rodXTime = time * PhysicsStuff.MILLI;

                // Angular speed
//                crankW = (fi != fiStep2 && rodXTime != rodXTime1 && rodXTime1 != rodXTime2) ?
//                   (fiStep2 - fiStep1) / (rodXTime2 - rodXTime1) + (rodXTime2 - rodXTime1) / (rodXTime2 - rodXTime) *
//                  ((fistep2 - fistep1) / (rodxtime2 - rodxtime1) - (fistep1 - fi) / (rodxtime1 - rodxtime)) : 0;

                crankW = (Math.Abs(fi - fiLast) < 180 ? Math.Abs(fi - fiLast) : 360.0 - Math.Abs(fi - fiLast)) / (rodXTime - rodXTime1);

                crankW = PhysicsStuff.DegreeToRadians(Math.Abs(crankW));

				// 1st derivative - velocity of rod (by 3 pts method)
				rodV = (rodX != rodXStep2 && rodXTime != rodXTime1 && rodXTime1 != rodXTime2) ?
					(rodXStep2 - rodXStep1) / (rodXTime2 - rodXTime1) + (rodXTime2 - rodXTime1) / (rodXTime2 - rodXTime) *
					((rodXStep2 - rodXStep1) / (rodXTime2 - rodXTime1) - (rodXStep1 - rodX) / (rodXTime1 - rodXTime)) : 0;

				// 2st derivative - acceleration of rod (by 3 pts method)
				rodA = (rodX != rodXStep2 && rodXTime != rodXTime1 && rodXTime1 != rodXTime2) ?
					((rodX - rodXStep1) / (rodXTime - rodXTime1) - (rodXStep1 - rodXStep2) / (rodXTime1 - rodXTime2)) /
					(rodXTime - rodXTime2) : 0;
			}

			fiLast = fi;

			return status;
		}

		private bool CalcAngles()
		{
			// Angles of a four-link unit and forces projections

			fi2 = (fi > fi1) ? 360f - fi + fi1 : fi1 - fi;
			double diag = Math.Sqrt(sizeR * sizeR + sizeK * sizeK - 2.0 * sizeR * sizeK * Math.Cos(PhysicsStuff.DegreeToRadians(fi2)));
			fi3 = Math.Acos((diag * diag - sizeP * sizeP - sizeC * sizeC) / (-2.0 * sizeP * sizeC));
			ksi = Math.PI / 2.0 - fi3;
			double fi41 = PhysicsStuff.RadiansToDegree(Math.Acos((sizeC * sizeC - diag * diag - sizeP * sizeP) / 
				(-2.0 * diag * sizeP)));
			double fi42 = PhysicsStuff.RadiansToDegree(Math.Acos((sizeK * sizeK - diag * diag - sizeR * sizeR) / 
				(-2.0 * diag * sizeR)));

			fi4 = fi41 + ((fi2 > 180) ? 360.0 - fi42 : fi42);
			if (fi4 >= 360)
			{
				fi4 -= 360.0;
			}

			gamma = 90.0 - (fi4 > 180 ? (360.0 - fi4) : fi4);
			betta = 90.0 - (fi > 180 ? (360.0 - fi) : fi);

			return true;
		}

		public double FiToX(double fi)
		{
			double radFi = PhysicsStuff.DegreeToRadians(fi);

			crankPos.x = sizeR * Math.Sin(radFi);
			crankPos.y = sizeR * Math.Cos(radFi);

			// Coordinate of "beam-connecting rod" joint           
			if (!Intersection(crankPos, beamAxis, sizeP, sizeC, crPos))
			{
				return -1.0;
			}

			double k = (sizeC + sizeA) / sizeC;
			beamPos.x = crPos.x + (beamAxis.x - crPos.x) * k;
			beamPos.y = crPos.y + (beamAxis.y - crPos.y) * k;

			alpha = PhysicsStuff.RadiansToDegree(Math.Atan((beamPos.y - crPos.y) / (beamPos.x - crPos.x)));

			// Coordinate of rod
			double X = (alpha - alphaDDP) * 2.0 * Math.PI * sizeA / 360.0;

			if (X < 0)
			{
				X = 0;
			}

			return X;
		}

		public double XToFi(double X, ushort status)
		{
			// Angle of the front arm of the beam
			alpha = alphaDDP + 360.0 * X / (2.0 * Math.PI * sizeA);

            // Angle of the rear arm of the beam
            double alpha1 = PhysicsStuff.DegreeToRadians(alpha + 180.0);

			crPos.x = beamAxis.x + sizeC * Math.Cos(alpha1);
			crPos.y = beamAxis.y + sizeC * Math.Sin(alpha1);

			// Coordinate of "beam-connecting rod" joint           
			Position crankAxis = new(0, 0);
			Position crankPos1 = new();
			if (!Intersection(crankAxis, crPos, sizeR, sizeP, crankPos, crankPos1))
			{
				return -1.0;
			}

			// Two variants of connecting rod coordinates
			fi5 = PhysicsStuff.RadiansToDegree(Math.Atan(crankPos.y / crankPos.x));
			fi6 = PhysicsStuff.RadiansToDegree(Math.Atan(crankPos1.y / crankPos1.x));

			fi5 = (crankPos.x < 0) ? 270.0 - fi5 : 90.0 - fi5;
			fi6 = (crankPos1.x < 0) ? 270.0 - fi6 : 90.0 - fi6;

			// Depending on the direction of rotation and phase, we choose one of variants
			if (((status & statusCW) != 0 && (status & statusUS) != 0) || 
				((status & statusCCW) != 0 && (status & statusDS) != 0))
			{
				crankPos = crankPos1;
				fi5 = fi6;
			}

			return 0;
		}

		// Transfer function from the front end of the beam to "crank-connecting rod" joint 
		public bool FToN(double F)
		{
			// Required force applied along the connecting rod
			crF = (F * sizeA / sizeC) / Math.Cos(ksi);

			// The force, applied to the crank consider as positive if concides with rotation direction

			// The gravity force of counterweight, applied to the end of crank
			cwF = counterweight * PhysicsStuff.g * Math.Cos(PhysicsStuff.DegreeToRadians(betta));
			if (((status & statusCCW) != 0 && fi < 180) || ((status & statusCW) != 0 && fi >= 180))
			{
				cwF *= -1.0;
			}

			// Static required force at the end of crank excluding force of counterweight
			crankF = crF * Math.Cos(PhysicsStuff.DegreeToRadians(gamma));
			if ((status & statusDS) != 0)
			{
				crankF *= -1.0;
			}

			// Required force at the end of crank including force of counterweight
			torqF = crankF - cwF;

			// Required force at the reductor shaft
			n = (torqF * sizeR) * crankW;

			return true;
		}

		// Transfer function from the "crank-connecting rod" joint to the front end of the beam
		public bool NToF(double N, ref double F)
		{
			torqF = (crankW != 0) ? N / (sizeR * crankW) : 0;

			cwF = counterweight * PhysicsStuff.g * Math.Cos(PhysicsStuff.DegreeToRadians(betta));
			if (((status & statusCCW) != 0 && fi < 180) || ((status & statusCW) != 0 && fi >= 180))
			{
				cwF *= -1.0;
			}

			crankF = torqF + cwF;
			if ((status & statusDS) != 0)
			{
				crankF *= -1.0;
			}

			if (Math.Abs(gamma) > 89.9999)
			{
				// Must not divide by zero
				return false;
			}

			crF = crankF / Math.Cos(PhysicsStuff.DegreeToRadians(gamma));
			F = crF * Math.Cos(ksi);
			F *= sizeC / sizeA;

			return true;
		}

		private ushort GetStatus()
		{
			// Calculations of rotation direction and of passing DDP or UDP after current step

			ushort status = 0;

			if (fi > fiLast)
			{
				if ((fi - fiLast) < 180)
				{
					status = (ushort)(statusCW | ((fi >= fiDDP && fiDDP > fiLast) ? statusDDPPassed : 0) |
						((fi >= fiUDP && fiUDP > fiLast) ? statusUDPPassed : 0));
				}
				else
				{
					status = (ushort)(statusCCW | ((fi <= fiDDP || fiDDP < fiLast) ? statusDDPPassed : 0) |
						((fi <= fiUDP || fiUDP < fiLast) ? statusUDPPassed : 0));
				}

			}
			else if (fi < fiLast)
			{
				if ((fiLast - fi) < 180)
				{
					status = (ushort)(statusCCW | ((fi <= fiDDP && fiDDP < fiLast) ? statusDDPPassed : 0) |
						((fi <= fiUDP && fiUDP < fiLast) ? statusUDPPassed : 0));
				}
				else
				{
					status = (ushort)(statusCW | ((fi >= fiDDP || fiDDP > fiLast) ? statusDDPPassed : 0) |
						((fi >= fiUDP || fiUDP > fiLast) ? statusUDPPassed : 0));
				}
			}

			// Phase detection - upstroke or downstroke
			if ((status & statusCW) != 0)
			{
				status |= (fi4 < 180) ? statusDS : statusUS;
			}
			else if ((status & statusCCW) != 0)
			{
				status |= (fi4 < 180) ? statusUS : statusDS;
			}

			return status;
		}

		private bool CalcFiDDP(ref double fi)
		{
			// Coordinate of beam-connecting rod joint at DDP
			Position crDDP = new();
			Position crankAxis = new(0, 0);

			if (!Intersection(crankAxis, beamAxis, sizeR + sizeP, sizeC, crDDP))	// It is possibe to do it by the law of cosines
			{
				return false;
			}

            // Angle of crank at DDP relative horizontal
            fi = PhysicsStuff.RadiansToDegree(Math.Atan(crDDP.y / crDDP.x));

			// Suppose DDP places in one of upper quadrants
			fi = fi > 0 ? 90.0 - fi : 270.0 - fi;

			return true;
		}

		private bool CalcFiUDP(ref double fi)
		{
			// Coordinate of crank-beam at UDP
			Position crUDP = new();
			Position crankAxis = new(0, 0);

			if (!Intersection(crankAxis, beamAxis, sizeP - sizeR, sizeC, crUDP))    // It is possibe to do it by the law of cosines
			{
				return false;
			}

            // Angle of crank at UDP relative horizontal
            fi = PhysicsStuff.RadiansToDegree(Math.Atan(crUDP.y / crUDP.x));

			// Suppose UDP places in one of upper qudrants
			fi = fi > 0 ? 270.0 - fi : 90.0 - fi;

			return true;
		}

		// Calculations of two circles intersections coordinates
		// Source: https://litunovskiy.com/gamedev/intersection_of_two_circles/
		bool Intersection(Position pos1, Position pos2, double L1, double L2, Position presult1, Position presult2 = null)
		{
			// Normalization of centers coordinates
			double RelativeX = pos2.x - pos1.x;
			double RelativeY = pos2.y - pos1.y;

			double c = (L2 * L2 - RelativeX * RelativeX - RelativeY * RelativeY - L1 * L1) / -2.0;
			double a = RelativeX * RelativeX + RelativeY * RelativeY;
			double b = -2.0 * RelativeY * c;
			double e = c * c - L1 * L1 * RelativeX * RelativeX;
			double D = b * b - 4.0 * a * e;

			if (D < 0)
			{
				// No intersections
				return false;
			}

			// When RelativeX < 0.01 calculations are breaking down and another method is applied

			presult1.y = (-1.0 * b + Math.Sqrt(D)) / (2.0 * a);
			presult1.x = (RelativeX > 0.01) ?
				pos1.x + (c - presult1.y * RelativeY) / RelativeX : -Math.Sqrt(L1 * L1 - c * c / (RelativeY * RelativeY));
			presult1.y += pos1.y;

			// !!!!!!!!!CHECK presult2!!!!!!!
			if (presult2 is not null)
			{
				presult2.y = (-1.0 * b - Math.Sqrt(D)) / (2.0 * a);
				presult2.x = (RelativeX > 0.01) ?
					pos1.x + (c - presult2.y * RelativeY) / RelativeX : Math.Sqrt(L1 * L1 - c * c / (RelativeY * RelativeY));
				presult2.y += pos1.y;
			}

			return true;
		}

        // Inner fields for calculations and states
        //-----------------------------------------
		//

        private double rodL = 0;		// Length of rod stroke

		private double crankW = 0;		// Angular velocity/frequency of crank

		// For derivative calculation

		// Coordinates
		private double rodXStep1 = 0;	// Rod position at last step
		private double rodXStep2 = 0;	// Rod position 2 steps ago

		// Times
		private double rodXTime = 0;	// Current Time
		private double rodXTime1 = 0;	// Time at last step
		private double rodXTime2 = 0;	// Time 2 steps ago

		// Rotation angle of crank
		private double fiStep1 = 0;		// Crank angle at last step
		private double fiStep2 = 0;		// Crank angle 2 steps ago

		// Angles (see Descriptions\SRPShcema)

		private double fiLast = 0;   // Crank angle at last step

		private double alphaDDP = 0; // Beam angle at DDP
		private double alphaUDP = 0; // Beam angle at UDP

		private double fi1 = 0;      // angleBeam of К line
		private double fi2 = 0;      // angleBeam between crank and К line
		private double fi3 = 0;      // angleBeam between beam and connecting rod
		private double fi4 = 0;      // angleBeam between crank and connecting rod

		private double fi5 = 0;      // angleBeam
        private double fi6 = 0;      // angleBeam

        private double betta = 0;    // angleBeam of projection of load gravity force on connecting rod
		private double ksi = 0;      // angleBeam of projection of beam force on connecting rod, in radins
		private double gamma = 0;    // angleBeam of projection of connecting rod on crank

		// Forces

		private double crF = 0;      // Connecting rod force
		private double crankF = 0;   // Projection of connecting rod force on crank
		private double cwF = 0;      // angleBeam of projection of load gravity force on crank
		private double torqF = 0;    // Force at the end of crank

        // Scaled confObject parameters
        private double sizeR;
        private double sizeA;
        private double sizeC;
        private double sizeI;
        private double sizeG;
        private double sizeH;
        private double sizeP;
        private double sizeK;
        private double counterweight;
        private double crankWidth;
        private double beamWidth;
    }
}
