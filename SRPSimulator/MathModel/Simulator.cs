using System;
using System.Timers;
using static System.Windows.Forms.AxHost;

namespace SRPSimulator.MathModel
{
    // Data transferring to the consumers
    public struct SRPState
    {
        public enum Stage
        {
            None,
            Upstroke,
            Downstroke
        };

        public Stage CurrentStatus { set; get; }
        public double CrankAngle { set; get; }
        public double BeamAngle { set; get; }
        public double CRAngle { set; get; }
        public double RodX { set; get; }
        public double RodF { set; get; }
        public double RodV { set; get; }
        public double N { set; get; }
        public double Freq { set; get; }
        public long Time { set; get; }
        public bool DDP { set; get; }
    }

    // Performs the simulation process throught the Time
    class Simulator
    {
        // Default simulation Time step
        private const int timeQuant = 30;   // ms

        // Invoked at start of simulation
        public delegate void NotifyStart();
        public event NotifyStart InitStartEvent;

        // Invoked at end of circle (full stroke)
        public delegate void NotifyEndCircle();
        public event NotifyEndCircle EndCircleEvent;

        // Invoked after calculations on current step 
        public delegate void NotifyNext(SRPState state);
        public event NotifyNext NextStepEvent;

        private PUnit unit;
        public PUnit Unit { 
            get => unit; 
        }

        public Simulator(PUnit unit)
        {
            this.unit = unit;
            unit.InitStart(unit.DDP);
            SetShaftNoise(true, 300);
        }

        public void SetShaftNoise(bool noise, double amplitude)
        {
            noise_ = noise;
            noise_amplidude_ = amplitude;
        }

        public void Start()
        {
            paused_ = false;

            if (inProgress_) {
                timer_.Start();
                return;
            }

            state.Time = 0;
            unit.Drive.InitStart(unit.DDP);
            InitStartEvent();
            inProgress_ = true;
            StartTimer(timeQuant);
        }

        // Stops calculations and resets state
        public void Stop()
        {
            StopTimer();
            inProgress_ = false;
            oneCircle_ = false;
            paused_ = false;
        }

        // Pauses the simulation
        public void Pause()
        {
            if (!inProgress_)
                return;

            if (paused_)
                Start();
            else {
                timer_.Stop();
                paused_ = true;
            }
        }

        // Starts or resumes the simulation until DDP is passed
        public void OneCircle()
        {
            oneCircle_ = true;
            if (!inProgress_ || paused_)
                Start();
        }

        private void EndCircle()
        {
            Pause();
            EndCircleEvent();
            oneCircle_ = false;
        }

        private void StartTimer(int period)
        {
            timer_ = new Timer(period);
            timer_.Elapsed += OnTimer;
            timer_.AutoReset = true;
            timer_.Start();
        }

        private void StopTimer()
        {
            if (timer_ != null) {
                timer_.Stop();
                timer_.Dispose();
            }
        }

        // Performs calculations on each Time step
        private void OnTimer(Object source, ElapsedEventArgs e)
        {
            unit.Vfc.Operate(state.Time);

            // Detects the current state
            if (unit.DDPPassed) {
                if (oneCircle_) 
                    EndCircle();
                state.CurrentStatus = SRPState.Stage.Upstroke;

                if (unit.Vfc.IsProgramModeActive())
                    unit.Vfc.StartProgram(state.Time);
                else
                    unit.Vfc.SetDefaultFrequency();
            }
            else if (unit.UDPPassed)
                state.CurrentStatus = SRPState.Stage.Downstroke;
            else
                state.CurrentStatus = SRPState.Stage.None;

            state.Time += timeQuant;
            UpdateState(unit.DDPPassed);
        }

        // Initializes current state object and notifies consumers
        private void UpdateState(bool ddp = false)
        {
            state.CrankAngle = unit.Fi;
            state.BeamAngle = unit.Alpha;
            state.CRAngle = unit.CRAngle;
            state.RodX = unit.RodX;
            //state.RodV = unit.Srp.RodV;
            state.RodV = unit.Srp.test;
            state.N = unit.N;
            if (noise_) {
                rand_ ??= new Random();
                state.N += noise_amplidude_ * (1.0 - 2.0 * rand_.NextDouble());
            }
            state.RodF = unit.Srp.RodF + unit.Srp.FrictionF;
            // Back conversion test WMG->DMG
            //bool scs = unit.NToF(unit.N, ref state.RodF);
            //if (scs)
            // Converts from newtons to kgf
            state.RodF *= .102f;
            state.Freq = unit.Vfc.Frequency;
            state.DDP = ddp;
            NextStepEvent(state);
        }

        public SRPState state;

        // Inner fields for calculations and states

        private bool inProgress_ = false;
        private bool paused_ = false;
        private bool oneCircle_ = false;
        private bool noise_ = false;

        private double noise_amplidude_;
        
        private Random rand_;
        private Timer timer_;
    }
}
