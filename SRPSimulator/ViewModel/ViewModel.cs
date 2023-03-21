using SRPSimulator.MathModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SRPSimulator.ViewModel
{
    internal class ViewModel : INotifyPropertyChanged
    {
        // Constructors
        //---------------
        //

        public ViewModel()
        {
            if (!DesignModeStatus.IsInDesignMode)
            {
                simulator = SRPServices.GetService(typeof(Simulator)) as Simulator;

                simulator.InitStartEvent += Clear;
                simulator.NextStepEvent += NextStep;
                simulator.EndCircleEvent += EndCircle;

                mainWindow = (System.Windows.Application.Current as App).MainWindow as MainWindow;

                notifyNext = new Simulator.NotifyNext(mainWindow.SetState);
                notifyEndCircle = new Simulator.NotifyEndCircle(EnableProperties);
            }
        }

        // Commands
        //----------------
        //

        private Command commandStart;
        public Command CommandStart
        {
            get
            {
                return commandStart ?? (commandStart = new Command(
                    obj => simulator.Start()));
            }
        }

        private Command commandStop;
        public Command CommandStop
        {
            get
            {
                return commandStop ?? (commandStop = new Command(
                    obj => simulator.Stop()));
            }
        }

        private Command commandCycle;
        public Command CommandCycle
        {
            get
            {
                return commandCycle ?? (commandCycle = new Command(
                    obj => simulator.Circle()));
            }
        }

        private Command commandClear;
        public Command CommandClear
        {
            get
            {
                return commandClear ?? (commandClear = new Command(
                    obj => simulator.Reset()));
            }
        }

        // Simulator's events dispatching
        //-------------------------------
        //

        private void Clear()
        {
            mainWindow.Clear();
                
        }

        private void NextStep(MathModel.UnitState state)
        {
            mainWindow.Dispatcher.Invoke(notifyNext, state);
        }

        private void EndCircle()
        {
            mainWindow.Dispatcher.Invoke(notifyEndCircle);
        }

        private void EnableProperties()
        {
//            mainWindow.ModelProperties.Enabled = true;
        }

        // Implementations of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        MainWindow mainWindow;

        private MathModel.Simulator simulator;

        Simulator.NotifyNext notifyNext;
        Simulator.NotifyEndCircle notifyEndCircle;
    }
}
