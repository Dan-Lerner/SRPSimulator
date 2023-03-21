using System.Windows;
using System.Windows.Media;

namespace SRPSimulator.SRPControl
{
    // Connecting Rod pseudo object
    internal class ConnectingRod
    {
        // Constructors
        //---------------
        //

        public ConnectingRod(FrameworkElement Rod, FrameworkElement CrankJoint, FrameworkElement BeamJoint)
        {
            this.Rod = Rod;
            this.CrankJoint = CrankJoint;
            this.BeamJoint = BeamJoint;

            // Transformations objects initialization

            scaleConnectionRod = new ScaleTransform();
            rotateConnectionRod = new RotateTransform();
            transformConnectionRod = new TransformGroup();
            transformConnectionRod.Children.Add(scaleConnectionRod);
            transformConnectionRod.Children.Add(rotateConnectionRod);

            Rod.RenderTransform = transformConnectionRod;

        }

        // Connecting Rod scaling
        public void Scale(double Scale, double SizeP, double CrankWidth)
        {
            double SizeK = Scale * SizeP;

            scaleConnectionRod.ScaleX = (Scale * CrankWidth / 3) / Rod.ActualWidth;
            scaleConnectionRod.ScaleY = SizeK / Rod.ActualHeight;

            BeamJoint.Height = BeamJoint.Width = Scale * CrankWidth / 1.5;
            CrankJoint.Height = CrankJoint.Width = Scale * CrankWidth / 1.5;
        }

        // Connecting Rod positioning (moving emulation)
        public void SetPosition(double Angle, Point CrankJointPosition, Point BeamJointPosition)
        {
            rotateConnectionRod.Angle = Angle;

            BeamJoint.Margin = new Thickness(BeamJointPosition.X - BeamJoint.Height / 2, 0, 0, BeamJointPosition.Y - BeamJoint.Height / 2);
            CrankJoint.Margin = new Thickness(CrankJointPosition.X - CrankJoint.Height / 2, 0, 0, CrankJointPosition.Y - CrankJoint.Height / 2);

            Rod.Margin = new Thickness(CrankJointPosition.X - Rod.Width / 2, 0, 0, CrankJointPosition.Y);
        }

        private FrameworkElement Rod;
        private FrameworkElement CrankJoint;
        private FrameworkElement BeamJoint;

        private TransformGroup transformConnectionRod;
        private RotateTransform rotateConnectionRod;
        private ScaleTransform scaleConnectionRod;
    }
}
