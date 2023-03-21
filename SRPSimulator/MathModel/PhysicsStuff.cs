using System;

namespace SRPSimulator.MathModel
{
    class PhysicsStuff
    {
        // Constants
        //--------------
        //

        public static double MILLI  = 0.001;
        public static double KILO   = 1000;
        public static double GIGA   = 1000000;

        public static double ATM_TO_PASCAL = 101325;
        public static double PASCAL_TO_ATM = 9.86923e-6;

        public static double g      = 9.8;
        
        // Values transformations
        //------------------------
        //

        static public double DegreeToRadians(double Degrees)
        {
            return Math.PI * Degrees / 180;
        }

        static public double RadiansToDegree(double Radians)
        {
            return 180 * Radians / Math.PI;
        }
    }
}
