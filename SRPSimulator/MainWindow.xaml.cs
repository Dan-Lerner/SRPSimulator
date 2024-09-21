using System.Windows;
using SRPSimulator.MathModel;

namespace SRPSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double Angle { get; set; }  

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            dmg.Clear();
            dmg.BeginUpstroke();

            dmg1.Clear();
            dmg1.BeginUpstroke();

            dmg2.Clear();
            dmg2.BeginUpstroke();
        }

        public void SetState(MathModel.SRPState state)
        {
            labelTime.Content = string.Format("{0:f} c", state.Time * Physical.MILLI);
            labelRodX.Content = string.Format("{0:f3} m", state.RodX);
            labelRodF.Content = string.Format("{0:f1} N", state.RodF);
            labelRodV.Content = string.Format("{0:f3} m/c", state.RodV); ;
            labelDriveN.Content = string.Format("{0:f1} W", state.N);
            labelFreq.Content = string.Format("{0:f1} Hz", state.Freq);

            mycontrol.StateChanged();

            switch (state.CurrentStatus)
            {
                case SRPState.Stage.Upstroke:
                    dmg.BeginUpstroke();
                    dmg1.BeginUpstroke();
                    dmg2.BeginUpstroke();
                    break;
                case SRPState.Stage.Downstroke:
                    dmg.BeginDownstroke();
                    dmg1.BeginDownstroke();
                    dmg2.BeginDownstroke();
                    break;
            }

            dmg.AddData(state.RodX, state.RodF);
            dmg1.AddData(state.RodX, state.N);
            dmg2.AddData(state.Time / 1000f, state.N);
        }

        private void mycontrol_Loaded(object sender, RoutedEventArgs e)
        {
            Simulator simulator = SRPServices.GetService(typeof(Simulator)) as Simulator;
            mycontrol.Unit = simulator.Unit;
        }

        private void MainWnd_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void ModelProperties1_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
