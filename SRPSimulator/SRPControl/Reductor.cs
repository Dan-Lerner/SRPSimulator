using System.Windows;
using System.Windows.Media;

namespace SRPSimulator.SRPControl
{
    // Reductor pseudo object
    internal class Reductor
    {
        // Constants
        private const double AxisPositionX = 0.75;
        private const double AxisPositionY = 0.75;

        // Constructors
        //---------------
        //

        public Reductor(FrameworkElement Assembly)
        {
            this.Assembly = Assembly;

            // Transformations objects initialization

            scale = new ScaleTransform();
            this.Assembly.RenderTransform = scale;
        }

        // Reductor scaling
        public void Scale(double Scale, double Height, double SizeR, Point Axis)
        {
            scale.ScaleY = (Scale * Height) / (AxisPositionY * Assembly.ActualHeight);
            scale.ScaleX = (Scale * SizeR) / (AxisPositionX * Assembly.ActualWidth);

            Assembly.Margin = new Thickness(Axis.X - AxisPositionX * Assembly.ActualWidth * scale.ScaleX, 0, 0, Axis.Y - Scale * Height);
        }

        FrameworkElement Assembly;

        private ScaleTransform scale;
    }
}
