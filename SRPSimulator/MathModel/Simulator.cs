using System;
using System.Timers;

namespace SRPSimulator.MathModel
{
    // Intermediate class for transferring values to consumers

    public struct UnitState
    {
        public enum Status
        {
            NONE,
            UPSTROKE,
            DOWNSTROKE
        };

        public Status CurrentStatus
        { set; get; }

        public double CrankAngle
        { set; get; }

        public double BeamAngle
        { set; get; }

        public double CRAngle
        { set; get; }

        public double RodX
        { set; get; }

        public double RodF
        { set; get; }

        public double RodV
        { set; get; }

        public double N
        { set; get; }

        public long Time
        { set; get; }
    }

    // Performs simulation process throught the Time
    class Simulator
    {
        // Default simulation Time step
        private const int timeQuant = 30;

        // Events
        //-------------------
        //

        // Invoked at start of simulation
        public delegate void NotifyStart();
        public event NotifyStart InitStartEvent;

        // Invoked at end of circle (full stroke)
        public delegate void NotifyEndCircle();
        public event NotifyEndCircle EndCircleEvent;

        // Invoked after calculations on current step 
        public delegate void NotifyNext(UnitState state);
        public event NotifyNext NextStepEvent;

        // Linked objects
        //-----------------
        //

        // The main model object
        private PUnit unit;
        public PUnit Unit
        { get => unit; }

        // Constructors
        //-------------------
        //

        public Simulator(PUnit unit)
        {
            this.unit = unit;
        }

        // Methods (simulation control)
        //-------------------------------
        //

        // Stops calculations and resets state
        public void Reset()
        {
            StopTimer();

            unit.Drive.InitStart(unit.DDP);
            state.Time = 0;
            InitStartEvent();

            inProgress = false;

            state.Time = 0;
            UpdateState();
        }

        // Runs or resumes the simulation
        public bool Start()
        {
            if (inProgress)
            {
                timer.Start();
                oneCircle = false;

                return true;
            }

            Reset();

            inProgress = true;
            oneCircle = false;

            StartTimer(timeQuant);

            return true;
        }

        // Pauses the simulation
        public bool Stop()
        {
            if (inProgress)
            {
                timer.Stop();
            }

            return true;
        }

        // Starts or resumes the simulation until DDP is passed
        public bool Circle()
        {
            if (inProgress)
            {
                timer.Start();
                oneCircle = true;

                return true;
            }

            Reset();

            inProgress = true;
            oneCircle = true;

            StartTimer(timeQuant);

            return true;
        }

        // Sets current power supply's frequency (FC-drive output f)
        void SetFrequency(int freq)
        {
            frequency = freq;
        }

        // Timer Initialization
        //---------------------
        //

        private bool StartTimer(int period)
        {
            timer = new Timer(period);
            timer.Elapsed += OnTimer;
            timer.AutoReset = true;
            timer.Start();

            return true;
        }

        private bool StopTimer()
        {
            if (timer is not null)
            {
                timer.Stop();
                timer.Dispose();
            }

            return true;
        }

        // OnTimer
        // performs calculations on each Time step
        //-------------------------------------------------
        //

        private void OnTimer(Object source, ElapsedEventArgs e)
        {
            // Rotates model on next Time step with a given frequency
            unit.Drive.Rotate(50, state.Time, unit.N);

            // Converts coordinate of rod to crank angle
            unit.XToFi(unit.RodX, unit.Status);

            // Converts force on the rod to current motor power
            bool scs = unit.FToN(unit.Srp.RodF + unit.Srp.FrictionF);

            // Detects the current state
            if (unit.DDPPassed)
            {
                if (oneCircle)
                {
                    Stop();
                    EndCircleEvent();
                    oneCircle = false;
                }

                state.CurrentStatus = UnitState.Status.UPSTROKE;
            }
            else if (unit.UDPPassed)
            {
                state.CurrentStatus = UnitState.Status.DOWNSTROKE;
            }
            else
            {
                state.CurrentStatus = UnitState.Status.NONE;
            }

            state.Time += timeQuant;

            UpdateState();
        }

        // Initializes current state object and notifies consumers
        private void UpdateState()
        {
            state.CrankAngle = unit.Fi;
            state.BeamAngle = unit.Alpha;
            state.CRAngle = unit.CRAngle;
            state.RodX = unit.RodX;
            state.RodV = unit.RodV;
            state.N = unit.N;

            double F = 0;
            bool scs = unit.NToF(unit.N, ref F);
            if (scs)
            {
                // Converts from newtons to kgf
                state.RodF = F * .102f;
            }

            NextStepEvent(state);
        }

        // Inner fields for calculations and states
        //-----------------------------------------
        //

        public UnitState state;

        private Timer timer;

        private bool inProgress = false;
        private bool oneCircle = false;
        private int frequency = 50;
    }
}
