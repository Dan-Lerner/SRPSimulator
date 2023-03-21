using SRPConfig;
using SRPSimulator.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SRPSimulator.PropertiesControl
{
    /// <summary>
    /// Adapter for Windows Forms panel
    /// </summary>
    public partial class PropertiesControl : UserControl, INotifyPropertyChanged
    {
        public PropertiesControl()
        {
            InitializeComponent();

            Properties.ParentControl = this;
        }

        // Commands
        //----------------
        //

        public static readonly DependencyProperty CommandResetProperty;
        public static readonly DependencyProperty CommandSaveStateProperty;
        public static readonly DependencyProperty CommandSaveProperty;
        public static readonly DependencyProperty CommandDeleteProperty;

        // Properties
        //------------
        //

        public static readonly DependencyProperty SelectedObjectIndexProperty;
        public static readonly DependencyProperty SelectedObjectProperty;
        public static readonly DependencyProperty ConfigObjectsProperty;
        public static readonly DependencyProperty ConfigItemsProperty;
        public static readonly DependencyProperty SelectedConfigProperty;
        public static readonly DependencyProperty ConfigNameProperty;
        public static readonly DependencyProperty ServerStatusProperty;
        public static readonly DependencyProperty SelectedParameterProperty;

        static PropertiesControl()
        {
            CommandResetProperty = DependencyProperty.Register(
                "CommandReset",
                typeof(Command),
                typeof(PropertiesControl));

            CommandSaveStateProperty = DependencyProperty.Register(
                "CommandSaveState",
                typeof(Command),
                typeof(PropertiesControl));

            CommandSaveProperty = DependencyProperty.Register(
                "CommandSave",
                typeof(Command),
                typeof(PropertiesControl));

            CommandDeleteProperty = DependencyProperty.Register(
                "CommandDelete",
                typeof(Command),
                typeof(PropertiesControl));

            SelectedObjectIndexProperty = DependencyProperty.Register(
                "SelectedObjectIndex", 
                typeof(int), 
                typeof(PropertiesControl));

            SelectedObjectProperty = DependencyProperty.Register(
                "SelectedObject", 
                typeof(object), 
                typeof(PropertiesControl),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = new PropertyChangedCallback(
                        OnSelectedObjectPropertyChanged)
                });

            ConfigObjectsProperty = DependencyProperty.Register(
                "ConfigObjects", 
                typeof(List<ConfigObjectProperties>), 
                typeof(PropertiesControl),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = new PropertyChangedCallback(
                        OnConfigObjectsPropertyChanged)
                });

            ConfigItemsProperty = DependencyProperty.Register(
                "ConfigItems", 
                typeof(List<ConfigIdentity>), 
                typeof(PropertiesControl),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = new PropertyChangedCallback(
                        OnConfigItemsPropertyChanged)
                });
        
            SelectedConfigProperty = DependencyProperty.Register(
                "SelectedConfig", 
                typeof(int), 
                typeof(PropertiesControl),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = new PropertyChangedCallback(
                        OnSelectedConfigChanged)
                });

            ConfigNameProperty = DependencyProperty.Register(
                "ConfigName", 
                typeof(string), 
                typeof(PropertiesControl),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = new PropertyChangedCallback(
                        OnConfigNamePropertyChanged)
                });

            ServerStatusProperty = DependencyProperty.Register(
                "ServerStatus", 
                typeof(string), 
                typeof(PropertiesControl),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = new PropertyChangedCallback(
                        OnServerStatusPropertyChanged)
                });

            SelectedParameterProperty = DependencyProperty.Register(
                "SelectedParameter", 
                typeof(int), 
                typeof(PropertiesControl));
        }

        // Property wrapers
        //-------------------------------
        //

        public Command CommandReset
        {
            get { return (Command)GetValue(CommandResetProperty); }
            set { SetValue(CommandResetProperty, value); }
        }

        public Command CommandSaveState
        {
            get { return (Command)GetValue(CommandSaveStateProperty); }
            set { SetValue(CommandSaveStateProperty, value); }
        }

        public Command CommandSave
        {
            get { return (Command)GetValue(CommandSaveProperty); }
            set { SetValue(CommandSaveProperty, value); }
        }

        public Command CommandDelete
        {
            get { return (Command)GetValue(CommandDeleteProperty); }
            set { SetValue(CommandDeleteProperty, value); }
        }

        public int SelectedObjectIndex
        {
            get { return (int)GetValue(SelectedObjectIndexProperty); }
            set { SetValue(SelectedObjectIndexProperty, value); }
        }

        public object SelectedObject
        {
            get { return (int)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }

        public List<ConfigObjectProperties> ConfigObjects
        {
            get { return (List<ConfigObjectProperties>)GetValue(ConfigObjectsProperty); }
            set { SetValue(ConfigObjectsProperty, value); }
        }

        public List<ConfigIdentity> ConfigItems
        {
            get { return (List<ConfigIdentity>)GetValue(ConfigItemsProperty); }
            set { SetValue(ConfigItemsProperty, value); }
        }

        public int SelectedConfig
        {
            get { return (int)GetValue(SelectedConfigProperty); }
            set { SetValue(SelectedConfigProperty, value); }
        }

        public string ConfigName
        {
            get { return (string)GetValue(ConfigNameProperty); }
            set { SetValue(ConfigNameProperty, value); }
        }

        public string ServerStatus
        {
            get { return (string)GetValue(ServerStatusProperty); }
            set { SetValue(ServerStatusProperty, value); }
        }

        public int SelectedParameter
        {
            get { return (int)GetValue(SelectedParameterProperty); }
            set { SetValue(SelectedParameterProperty, value); }
        }

        // PropertyChangedCallbacks
        //-------------------------------
        //

        private static void OnSelectedObjectPropertyChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            PropertiesControl control = d as PropertiesControl;
            control.SetObject(e.NewValue);
        }

        private static void OnConfigObjectsPropertyChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            PropertiesControl control = d as PropertiesControl;
            control.UpdateTabs();
        }

        private static void OnConfigItemsPropertyChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            PropertiesControl control = d as PropertiesControl;
            control.BindConfigItems();
        }

        private static void OnSelectedConfigChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            PropertiesControl control = d as PropertiesControl;
            control.SetSelectedConfig((int)e.NewValue);
        }

        private static void OnConfigNamePropertyChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            PropertiesControl control = d as PropertiesControl;
            control.SetConfigName(e.NewValue as string);
        }

        private static void OnServerStatusPropertyChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            PropertiesControl control = d as PropertiesControl;
        }

        // Communications to WF controls
        //-------------------------------
        //

        public void UpdateTabs()
        {
            Properties.UpdateTabs(ConfigObjects);
        }

        private void SetObject(object Object)
        {
            Properties.SetObject(Object);
        }

        private void BindConfigItems()
        {
            Properties.BindConfigsNames(ConfigItems, "Id", "Name");
        }

        private void SetSelectedConfig(int selectedConfig)
        {
            Properties.SetSelectedConfig(selectedConfig);
        }

        private void SetConfigName(string Name)
        {
            Properties.SetConfigName(Name);
        }

        // Properties for command's statuses
        //-------------------------------
        //

        bool canDelete;
        public bool CanDelete
        {
            get { return canDelete; }
            set { canDelete = value; OnPropertyChanged("CanDelete"); }
        }

        bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { canSave = value; OnPropertyChanged("CanSave"); }
        }

        // Dispatcing of command statuses
        //-------------------------------
        //

        private void propertiesControl_Loaded(object sender, RoutedEventArgs e)
        {
            CommandDelete.CanExecuteChanged += CanExecuteDelete;
            CommandSave.CanExecuteChanged += CanExecuteSave;
        }

        void CanExecuteDelete(object sender, EventArgs e)
        {
            CanDelete = CommandDelete.CanExecute("");
        }

        void CanExecuteSave(object sender, EventArgs e)
        {
            CanSave = CommandSave.CanExecute("");
        }

        // Implementations of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
