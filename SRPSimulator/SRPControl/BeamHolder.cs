using System.Windows;
using System.Windows.Media;

namespace SRPSimulator.SRPControl
{
    // Beam holder pseudo object
    internal class BeamHolder
    {
        // Constructors
        //---------------
        //

        public BeamHolder(FrameworkElement Assembly, FrameworkElement BeamAxis)
        {
            this.Assembly = Assembly;
            this.BeamAxis = BeamAxis;

            // Transformations objects initialization

            scale = new ScaleTransform();
            this.Assembly.RenderTransform = scale;
        }

        // Beam holder scaling
        public void Scale(double Scale, double Height, Point Axis)
        {
            scale.ScaleX = (Scale * Height) / (Assembly.ActualHeight - BeamAxis.ActualHeight / 2);
            scale.ScaleY = scale.ScaleX;

            Assembly.Margin = new Thickness(Axis.X - Assembly.ActualWidth / 2, 0, 0, Axis.Y - Scale * Height);
        }

        // Calculates actual beam holder width
        public double GetHolderWidth(double Scale)
        {
            return Assembly.ActualWidth * scale.ScaleX / Scale;
        }

        FrameworkElement Assembly;
        FrameworkElement BeamAxis;

        private ScaleTransform scale;
    }
}
