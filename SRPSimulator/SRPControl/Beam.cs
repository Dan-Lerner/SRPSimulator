using System.Windows;
using System.Windows.Media;

namespace SRPSimulator.SRPControl
{
    // Beam pseudo object 
    internal class Beam
    {
        // Constants
        private const double InitialBeamEndRadius = 150;    // Applied in pt
        private const double BeamJointOffset = 200;         // Applied in mm

        // Beam angle (setting this angle emulates Beam moving)
        public double Angle
        { set => rotate.Angle = value; }

        // Constructors
        //---------------
        //

        public Beam(FrameworkElement Assembly, FrameworkElement Arm, FrameworkElement Ending)
        {
            this.Assembly = Assembly;
            this.Arm = Arm;
            this.Ending = Ending;

            // Transformations objects initialization

            scaleBeamArm = new ScaleTransform();
            scaleBeamEnd = new ScaleTransform();
    
            rotate = new RotateTransform();
            scale = new ScaleTransform();
            transform = new TransformGroup();
            transform.Children.Add(rotate);
            transform.Children.Add(scale);

            this.Arm.RenderTransform = scaleBeamArm;
            this.Ending.RenderTransform = scaleBeamEnd;
            this.Assembly.RenderTransform = transform;
        }

        // Beam scaling
        public void Scale(double Scale, double SizeA, double SizeC, Point Axis)
        {
            double ScaleA = (Scale * SizeA) / InitialBeamEndRadius;

            // Size A without Ending
            double SizeAReduced = ScaleA * Ending.ActualWidth / Scale;

            // Size C with additional length for rear joint
            double SizeCExtended = SizeC + BeamJointOffset;

            double BeamArmK = SizeCExtended / (double)(SizeA + SizeCExtended);

            this.Axis = Axis;

            scaleBeamArm.ScaleX = Scale * (SizeA - SizeAReduced + SizeCExtended) / Arm.ActualWidth;
            scaleBeamArm.ScaleY = ScaleA;

            Assembly.RenderTransformOrigin = new Point(BeamArmK, 0.5);
            Assembly.Width = Scale * (SizeA + SizeCExtended);

            scaleBeamEnd.ScaleX = scaleBeamEnd.ScaleY = ScaleA;

            Assembly.Margin = new Thickness(Axis.X - Scale * SizeCExtended, 0, 0, Axis.Y - Assembly.ActualHeight / 2);
        }

        private FrameworkElement Assembly;
        private FrameworkElement Arm;
        private FrameworkElement Ending;

        private Point Axis;

        private ScaleTransform scaleBeamArm;

        private ScaleTransform scaleBeamEnd;

        private TransformGroup transform;
        private RotateTransform rotate;
        private ScaleTransform scale;
    }
}
