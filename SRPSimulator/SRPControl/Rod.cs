using System.Windows.Media;
using System.Windows;

namespace SRPSimulator.SRPControl
{
    // Rod pseudo object
    internal class Rod
    {
        // Constants just for visualization
        // Ерун not used in calculations
        private const double WellHeight = 500;      // Applied in mm
        private const double WellWidth = 200;       // Applied in mm

        private const double NMTPosition = 100;     // Applied in mm

        private const double RodDiameter = 60;      // Applied in mm
        private const double HawserDiameter = 25;   // Applied in mm

        // Constructors

        public Rod(FrameworkElement Assembly, FrameworkElement UpperRod, FrameworkElement PolishedRod, 
            FrameworkElement Traverse, FrameworkElement Hawser, FrameworkElement Well)
        {
            this.Assembly = Assembly;
            this.PolishedRod = PolishedRod;
            this.Traverse = Traverse;
            this.Hawser = Hawser;
            this.Well = Well;
            this.UpperRod = UpperRod;

            // Transformations objects initialization

            scale = new ScaleTransform();
            scaleWell = new ScaleTransform();

            this.Assembly.RenderTransform = scale;
            this.Well.RenderTransform = scaleWell;
        }

        // Polished Rod scaling
        public void Scale(double Scale, double SizeA, double SizeI, double LandLevel, Point AxisCrank, Point AxisBeam)
        {
            PolishedRod.Width = Scale * RodDiameter;
            Hawser.Width = Scale * HawserDiameter;

            Assembly.Height = AxisBeam.Y - AxisCrank.Y + Scale * LandLevel;
            Hawser.Height = Assembly.Height;

            Assembly.Margin = new Thickness(AxisBeam.X + Scale * (SizeA - HawserDiameter / 2) - Assembly.ActualWidth / 2, 0, 0, AxisCrank.Y - Scale * LandLevel);
            
            // Well scale

            scaleWell.ScaleX = (Scale * WellWidth) / Well.ActualWidth;
            scaleWell.ScaleY = (Scale * WellHeight) / Well.ActualHeight;

            Well.Margin = new Thickness(AxisCrank.X + Scale * (SizeA + SizeI - HawserDiameter / 2) - Well.ActualWidth / 2, 0, 0, AxisCrank.Y - Scale * LandLevel);

            this.LandLevel = LandLevel;
            LastScale = Scale;
            NMTCoordinate = AxisCrank.Y - Scale * LandLevel + WellHeight + NMTPosition;
            Y0 = AxisCrank.Y;
        }

        // Rod moving emulation
        public void SetState(double RodX)
        {
            UpperRod.Height = LastScale * (WellHeight + NMTPosition + RodX);
        }

        FrameworkElement Assembly;
        FrameworkElement PolishedRod;
        FrameworkElement Traverse;
        FrameworkElement Hawser;
        FrameworkElement Well;
        FrameworkElement UpperRod;

        private ScaleTransform scale;
        private ScaleTransform scaleWell;

        double LandLevel;
        double NMTCoordinate;
        double LastScale;
        double Y0;
    }
}
