using System.Windows.Media;
using System.Windows;

namespace SRPSimulator.SRPControl
{
    // Pad pseudo object
    internal class Pad
    {
        // Constructors
        //---------------
        //

        public Pad(FrameworkElement Assembly)
        {
            this.Assembly = Assembly;

            // Transformations objects initialization

            scale = new ScaleTransform();
            this.Assembly.RenderTransform = scale;
        }

        // Pad scaling
        public void Scale(double Scale, double UnitWidth, double SizeR, double SizeG, Point Axis)
        {
            scale.ScaleX = (Scale * UnitWidth) / Assembly.ActualWidth;
            scale.ScaleY = (Scale * (SizeR - SizeG)) / Assembly.ActualHeight;

            Assembly.Margin = new Thickness(Axis.X - Scale * SizeR, 0, 0, Axis.Y - Scale * SizeR);
        }

        FrameworkElement Assembly;

        private ScaleTransform scale;
    }
}
