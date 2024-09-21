using System.Windows.Media;
using System.Windows;

namespace SRPSimulator.SRPControl
{
    // Well pseudo object
    internal class Well
    {
        // Constants
        private const double WellHeight = 500;  // Applied in mm
        private const double WellWidth = 200;   // Applied in mm

        public Well(FrameworkElement Assembly)
        {
            this.Assembly = Assembly;

            // Transformations objects initialization

            scale = new ScaleTransform();
            this.Assembly.RenderTransform = scale;
        }

        // Well Rod scaling
        public void Scale(double Scale, double SizeAI, double LandLevel, Point Axis)
        {
            scale.ScaleX = (Scale * WellWidth) / Assembly.ActualWidth;
            scale.ScaleY = (Scale * WellHeight) / Assembly.ActualHeight;

            Assembly.Margin = new Thickness(Axis.X + Scale * SizeAI - Assembly.ActualWidth / 2, 0, 0, Axis.Y - Scale * LandLevel);
        }

        FrameworkElement Assembly;

        private ScaleTransform scale;
    }
}
