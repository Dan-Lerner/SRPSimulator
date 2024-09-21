using System;
using System.Windows;
using System.Windows.Controls;
using SRPSimulator.MathModel;
using SRPSimulator.SRPControl;

namespace SRPSimulator
{
    /// <summary>
    /// Interaction logic for UnitControl.xaml
    /// </summary>
    public partial class UnitControl : UserControl
    {
        // Constants
        // Minimal space allowed from unit to controls' border
        // Applied in mm
        private const double UnitMargin = 300; 

        // Punp unit object
        private PUnit unit;
        internal PUnit Unit
        { 
            get => unit; 
            set
            {
                unit = value;
                value.NotifySizesChanged += SizesChanged;
                SizesChanged();
            }
        }

        // Constructors
        //---------------
        //

        public UnitControl()
        {
            InitializeComponent();

            // Pseudo objects for visual assemblies
            beam = new Beam(Beam, BeamArm, BeamEnd);
            crank = new Crank(Crank, CrankArm, CrankAxis);
            connectionRod = new ConnectingRod(ConnectionRod, CRCrankJoint, CRBeamJoint);
            beamHolder = new BeamHolder(BeamHolder, BeamAxis);
            reductor = new Reductor(Reductor);
            pad = new Pad(Pad);
            rod = new SRPControl.Rod(Rod, UpperRodJoint, PolishedRod, Traverse, Hawser, Well);
        }

        // By means of this method Pumping Units' movement is emulated
        // It is possible to realize one angle assigment (Fi) only to set current position
        // But now it's easier to set two angles already calculated in the Model
        public void StateChanged()
        {
            if (unit is null)
            {
                return;
            }

            beam.Angle = -unit.Alpha;
            crank.Angle = unit.Fi;

            PUnitConfigBrowsable unitConfig = unit.Config as PUnitConfigBrowsable;

            // Joints positions;
            BeamConnectionRodJoint.X = BeamAxisCenter.X + Math.Cos(Physical.DegreeToRadians(-unit.Alpha)) * Scale * -unitConfig.SizeC;
            BeamConnectionRodJoint.Y = BeamAxisCenter.Y + Math.Sin(Physical.DegreeToRadians(-unit.Alpha)) * Scale * unitConfig.SizeC;

            CrankConnectionRodJoint.X = CrankAxisCenter.X + Math.Sin(Physical.DegreeToRadians(unit.Fi)) * Scale * unitConfig.SizeR;
            CrankConnectionRodJoint.Y = CrankAxisCenter.Y + Math.Cos(Physical.DegreeToRadians(unit.Fi)) * Scale * unitConfig.SizeR;

            connectionRod.SetPosition(unit.CRAngle, CrankConnectionRodJoint, BeamConnectionRodJoint);
            rod.SetState(unit.RodX * Physical.KILO);
        }

        // This method is called when Units' sizes are changed
        // Rescales and reinitializes all elements in control
        public void SizesChanged()
        {
            if (unit is null)
            {
                return;
            }

            // Scale the entire Control area

            double t = Height;

            // Gonfig object with all settings
            PUnitConfigBrowsable unitConfig = unit.Config as PUnitConfigBrowsable;

            // Max Unit sizes calculations in 2 variants
            double UnitWidth1 = unitConfig.SizeR + unitConfig.SizeI + unitConfig.SizeA + 2 * UnitMargin;
            double UnitWidth2 = unitConfig.SizeC + unitConfig.SizeA + 2 * UnitMargin;
            double UnitHeight1 = unitConfig.SizeH + unitConfig.SizeA + 2 * UnitMargin;
            double UnitHeight2 = unitConfig.SizeR + unitConfig.SizeH - unitConfig.SizeG + unitConfig.SizeA + 2 * UnitMargin;

            double Round(double value)
            { 
                return Math.Ceiling(value * Physical.MILLI) * Physical.KILO;
            }

            // It rounded to meters 
            UnitWidth1 = Round(UnitWidth1);
            UnitWidth2 = Round(UnitWidth2);
            UnitHeight1 = Round(UnitHeight1);
            UnitHeight2 = Round(UnitHeight2);

            // Scales coefficient in 4 variants
            double ScaleX1 = ActualWidth / UnitWidth1;
            double ScaleX2 = ActualWidth / UnitWidth2;
            double ScaleY1 = ActualHeight / UnitHeight1;
            double ScaleY2 = ActualHeight / UnitHeight2;

            // Choose smaller scale coefficient
            Scale = ScaleX1 < ScaleX2 ? ScaleX1 : ScaleX2;
            Scale = Scale < ScaleY1 ? Scale : ScaleY1;
            Scale = Scale < ScaleY2 ? Scale : ScaleY2;

            // Crank axis center definig (0,0 coordinate) depends on unit layout
            if (UnitWidth1 > UnitWidth2)
            {
                CrankAxisCenter.X = (ActualWidth - Scale * (unitConfig.SizeR + unitConfig.SizeI + unitConfig.SizeA + UnitMargin)) / 2;
                CrankAxisCenter.X += Scale * unitConfig.SizeR;
            }
            else
            {
                CrankAxisCenter.X = (ActualWidth - Scale * (unitConfig.SizeC + unitConfig.SizeA + UnitMargin)) / 2;
                CrankAxisCenter.X += Scale * (unitConfig.SizeC - unitConfig.SizeI);
            }

            if (UnitHeight1 > UnitHeight2)
            {
                CrankAxisCenter.Y = Scale * (unitConfig.SizeG + UnitMargin);
            }
            else
            {
                CrankAxisCenter.Y = Scale * (unitConfig.SizeR + UnitMargin);
            }

            BeamAxisCenter.X = CrankAxisCenter.X + Scale * unitConfig.SizeI;
            BeamAxisCenter.Y = CrankAxisCenter.Y + Scale * (unitConfig.SizeH - unitConfig.SizeG);

            // Scaling all assemblings
            crank.Scale(Scale, unitConfig.SizeR, unitConfig.CrankWidth, CrankAxisCenter);
            beam.Scale(Scale, unitConfig.SizeA, unitConfig.SizeC, BeamAxisCenter);
            connectionRod.Scale(Scale, unitConfig.SizeP, unitConfig.CrankWidth);
            beamHolder.Scale(Scale, unitConfig.SizeH, BeamAxisCenter);
            
            // Pad width depends on Arm size
            double ActualCrankArmSize = crank.GetArmSize(Scale);
            reductor.Scale(Scale, unitConfig.SizeG, ActualCrankArmSize, CrankAxisCenter);
            pad.Scale(Scale, ActualCrankArmSize + unitConfig.SizeI + beamHolder.GetHolderWidth(Scale) / 2,
                ActualCrankArmSize, unitConfig.SizeG, CrankAxisCenter);
            
            // Ground level calculation depending on unis layout
            double LandLevel = (UnitHeight1 > UnitHeight2) ? unitConfig.SizeG : ActualCrankArmSize;
            rod.Scale(Scale, unitConfig.SizeA, unitConfig.SizeI, LandLevel, CrankAxisCenter, BeamAxisCenter);

            // Position updating considering new sizes
            StateChanged();
        }

        // Rescaling when resizing the window
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizesChanged();
        }

        // Pseudoobjects for visualisation ant transformations
        private Beam beam;
        private Crank crank;
        private ConnectingRod connectionRod;
        private BeamHolder beamHolder;
        private Reductor reductor;
        private Pad pad;
        private SRPControl.Rod rod;

        // Current scale coefficient
        private double Scale;

        // Axes and joints coordinates
        private Point CrankAxisCenter;
        private Point BeamAxisCenter;
        private Point BeamConnectionRodJoint;
        private Point CrankConnectionRodJoint;
    }
}
