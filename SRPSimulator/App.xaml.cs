using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SRPSimulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal SRPServices services;

//        internal MathModel.Simulator simulator;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            services = new SRPServices();

//            simulator = new MathModel.Simulator(services.Unit);
        }

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }

        private void Application_Deactivated(object sender, EventArgs e)
        {

        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {

        }
    }
}
