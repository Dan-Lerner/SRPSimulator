using System.Windows;
using System.Windows.Media;

namespace SRPSimulator.SRPControl
{
    // Crank pseudo object
    internal class Crank
    {
        // Constants
        private const double JointEndSize = 200; // mm

        // Crank Angle (setting this angle emulates Crank moving)
        public double Angle
        { set => rotate.Angle = value;  }

        // Constructors
        //---------------
        //

        public Crank(FrameworkElement Assembly, FrameworkElement Arm, FrameworkElement Axis)
        {
            this.Assembly = Assembly;
            this.Arm = Arm;
            this.Axis = Axis;

            // Transformations objects initialization

            rotate = new RotateTransform();
            scale = new ScaleTransform();
            transform = new TransformGroup();
            transform.Children.Add(scale);
            transform.Children.Add(rotate);

            this.Assembly.RenderTransform = transform;
        }

        // Crank Rod scaling
        public void Scale(double Scale, double SizeR, double CrankWidth, Point AxisPosition)
        {
            scale.ScaleX = (Scale * CrankWidth) / Arm.ActualWidth;
            scale.ScaleY = (Scale * (SizeR + 2 * JointEndSize)) / Arm.ActualHeight;

            rotate.CenterY = -Scale * JointEndSize;

            Assembly.Margin = new Thickness(AxisPosition.X - Assembly.ActualWidth / 2, 0, 0, AxisPosition.Y - Scale * JointEndSize);

            Axis.Height = Axis.Width = Arm.Width / 1.5;
            Axis.Margin = new Thickness(AxisPosition.X - Axis.Height / 2, 0, 0, AxisPosition.Y - Axis.Height / 2);
        }

        // Calculates actual whole working arm length
        public double GetArmSize(double Scale)
        {
            return Arm.Height * scale.ScaleY / Scale - JointEndSize;
        }

        FrameworkElement Assembly;
        FrameworkElement Arm;
        FrameworkElement Axis;

        TransformGroup transform;
        RotateTransform rotate;
        ScaleTransform scale;
    }
}
