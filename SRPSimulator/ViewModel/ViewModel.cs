using SRPSimulator.MathModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SRPSimulator.ViewModel
{
    internal class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        Simulator.NotifyNext notifyNext;
        Simulator.NotifyEndCircle notifyEndCircle;

        public ViewModel()
        {
            if (!DesignModeStatus.IsInDesignMode) {
                simulator = SRPServices.GetService(typeof(Simulator)) as Simulator;

                simulator.InitStartEvent += Clear;
                simulator.NextStepEvent += NextStep;
                simulator.EndCircleEvent += EndCircle;

                mainWindow = (System.Windows.Application.Current as App).MainWindow as MainWindow;
                notifyNext = new Simulator.NotifyNext(mainWindow.SetState);
                notifyEndCircle = new Simulator.NotifyEndCircle(EnableProperties);
            }
                
            dataSenderUDP = new();
        }

        public void StartSimulation(object value, EventArgs e) => simulator.Start();

        public bool CanStartSimulation(object value) => true;

        public void PauseSimulation(object value, EventArgs e) => simulator.Pause();

        public bool CanPauseSimulation(object value) => true;

        public void Circle(object value, EventArgs e) => simulator.OneCircle();

        public bool CanCircle(object value) => true;

        public void StopSimulation(object value, EventArgs e) => simulator.Stop();

        public bool CanStopSimulation(object value) => true;

        private void Clear()
        {
            mainWindow.Clear();
        }

        private void NextStep(MathModel.SRPState state)
        {
            dataSenderUDP.Send(new SRPData((Int32)state.Time, (float)state.N, (float)state.Freq,
                (float)state.RodX, (float)state.RodF, state.DDP ? 0x01 : 0x00));
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private MathModel.Simulator simulator;
        private MainWindow mainWindow;
        private DataSender dataSenderUDP;

    }
}
