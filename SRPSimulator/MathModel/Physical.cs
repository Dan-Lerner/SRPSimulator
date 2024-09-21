using SRPSimulator.SRPControl;
using System;

namespace SRPSimulator.MathModel
{
    // Point for represent unit assembly's coordinates
    public class Position
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

    public class Physical
    {
        // Constants

        public static double MILLI  = 0.001;
        public static double KILO   = 1000;
        public static double GIGA   = 1000000;

        public static double ATM_TO_PASCAL = 101325;
        public static double PASCAL_TO_ATM = 9.86923e-6;

        public static double g      = 9.8;
        
        // Values transformations

        static public double DegreeToRadians(double Degrees) => Math.PI * Degrees / 180;

        static public double RadiansToDegree(double Radians) => 180 * Radians / Math.PI;

        // Calculations of two circles intersections coordinates
        // Source: https://litunovskiy.com/gamedev/intersection_of_two_circles/
        static public bool Intersection(Position pos1, Position pos2, double L1, double L2,
            Position presult1, Position presult2 = null)
        {
            // Normalization of centers
            double RelativeX = pos2.x - pos1.x;
            double RelativeY = pos2.y - pos1.y;

            double c = (L2 * L2 - RelativeX * RelativeX - RelativeY * RelativeY - L1 * L1) / -2.0;
            double a = RelativeX * RelativeX + RelativeY * RelativeY;
            double b = -2.0 * RelativeY * c;
            double e = c * c - L1 * L1 * RelativeX * RelativeX;
            double D = b * b - 4.0 * a * e;

            if (D < 0)
                // No intersections
                return false;

            // When RelativeX < 0.01 calculations are breaking down and another method is applied
            presult1.y = (-1.0 * b + Math.Sqrt(D)) / (2.0 * a);
            presult1.x = (RelativeX > 0.01) ?
                pos1.x + (c - presult1.y * RelativeY) / RelativeX :
                -Math.Sqrt(L1 * L1 - c * c / (RelativeY * RelativeY));
            presult1.y += pos1.y;

            // !!!!!!!!!CHECK presult2!!!!!!!
            if (presult2 is not null)
            {
                presult2.y = (-1.0 * b - Math.Sqrt(D)) / (2.0 * a);
                presult2.x = (RelativeX > 0.01) ?
                    pos1.x + (c - presult2.y * RelativeY) / RelativeX :
                    Math.Sqrt(L1 * L1 - c * c / (RelativeY * RelativeY));
                presult2.y += pos1.y;
            }

            return true;
        }
    }

    public class DerivativeCalculator
    {
        public DerivativeCalculator()
        {
            value_ = 0;
            value1_ = 0;
            value2_ = 0;
            time_ = 0;
            time1_ = 0;
            time2_ = 0;
            derivative1_ = 0;
            derivative2_ = 0;
        }

        public double NextStep(double value, double time)
        {
            value2_ = value1_;
            value1_ = value_;
            value_ = value;

            if (time > 0) {
                time2_ = time1_;
                time1_ = time_;
                time_ = time * Physical.MILLI;

                // 1st derivative - velocity of rod (by 3 pts method)
                derivative1_ = (value_ != value2_ && time_ != time1_ && time1_ != time2_) ?
                    (value2_ - value1_) / (time2_ - time1_) + (time2_ - time1_) / 
                    (time2_ - time_) * ((value2_ - value1_) / (time2_ - time1_) - 
                    (value1_ - value_) / (time1_ - time_)) : 
                    0;

                // 2st derivative - acceleration of rod (by 3 pts method)
                derivative2_ = (value_ != value2_ && time_ != time1_ && time1_ != time2_) ?
                    ((value_ - value1_) / (time_ - time1_) - (value1_ - value2_) / 
                    (time1_ - time2_)) / (time_ - time2_) : 
                    0;
            }
            return derivative1_;
        }

        private double derivative1_;
        private double derivative2_;
        private double value_;
        private double value1_;
        private double value2_;
        private double time_ = 0;
        private double time1_ = 0;
        private double time2_ = 0;
    }
}
