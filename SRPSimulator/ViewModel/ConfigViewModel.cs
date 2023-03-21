using SRPConfig;
using SRPSimulator.MathModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace SRPSimulator.ViewModel
{
    public class ConfigObjectProperties : ConfigIdentity
    {
        public Configurable confObject;
        public bool ShowImage;
        public bool Expand;
    }

    internal class ConfigViewModel : INotifyPropertyChanged
    {
        // Constants
        //---------------
        //
        
        private const double timeServerPing = 5000;

        private const string cofigSavePath = @"/config/";

        private const string serverConnected = "Connected";
        private const string serverDisconnected = "Disconnected";
        
        readonly MainWindow mainWindow;

        // Constructors
        //---------------
        //

        public ConfigViewModel()
        {
            if (!DesignModeStatus.IsInDesignMode) // design time errors avoiding
            {
                mainWindow = (System.Windows.Application.Current as App).MainWindow as MainWindow;

                PUnit unit = SRPServices.GetService(typeof(PUnit)) as PUnit;

                ConfigObjects = new List<ConfigObjectProperties>
                {
                    new ConfigObjectProperties
                    { Id = 0, Name = "PUnit", confObject = unit, ShowImage = true, Expand = false },
                    new ConfigObjectProperties
                    { Id = 1, Name = "Drive", confObject = unit.Drive, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 2, Name = "SRP", confObject = unit.Srp, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 3, Name = "Fluid", confObject = unit.Srp.fluid, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 4, Name = "Tubing", confObject = unit.Srp.tubing, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 5, Name = "Rod", confObject = unit.Srp.rod, ShowImage = false, Expand = true }
                };

                // Trying to load saved values
                LoadConfigs();

                // Starts server connection pinging
                timerPing = new Timer(timeServerPing);
                timerPing.Elapsed += OnPing;
                timerPing.AutoReset = true;
                timerPing.Start();
                Ping();
            }
        }

        // Properties
        //----------------
        //

        // List of all objects with configuration (Tabs)
        private List<ConfigObjectProperties> configObjects;
        public List<ConfigObjectProperties> ConfigObjects
        {
            get => configObjects;
            set { configObjects = value; OnPropertyChanged("ConfigObjects"); }
        }

        // Index of currently selected configuration Object (current tab)
        private int selectedObjectIndex;
        public int SelectedObjectIndex
        {
            get => selectedObjectIndex;
            set => _ = SetObjectIndexAsync(value);
        }

        // List of configs (combo)
        private List<ConfigIdentity> configsList = new();
        public List<ConfigIdentity> ConfigsList
        {
            get => configsList;
            set { configsList = value; OnPropertyChanged("ConfigsList"); }
        }

        // Currently selected config (combo curent selection)
        private int selectedConfig;
        public int SelectedConfig
        {
            get => selectedConfig;
            set => _ = ChangeConfigAsync(value);
        }

        // Reference to currently selected configuration object (Property greed)
        private ConfigIdentity selectedObject;
        public ConfigIdentity SelectedObject
        {
            get => selectedObject;
            set { selectedObject = value; OnPropertyChanged("SelectedObject"); }
        }

        // Name of current configuration
        private string configName;
        public string ConfigName
        {
            get => configName;
            set
            {
                configName = value;
                if (SelectedObject is not null)
                {
                    SelectedObject.Name = configName;
                }
                OnPropertyChanged("ConfigName");
            }
        }

        // Enabling/disabing of configuration control panel
        private bool isEnabled = true;
        public bool IsEnabled
        {
            get => isEnabled;
            set { isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        // Server connection status flag
        private bool connected;
        public bool Connected
        {
            get => connected;
            set
            {
                if (connected != value)
                {
                    connected = value;

                    InitServerConnection(connected);
                }
            }
        }

        // Tip for current connection status
        private string serverStatus = serverDisconnected;
        public string ServerStatus
        {
            get => serverStatus;
            set { serverStatus = value; OnPropertyChanged("ServerStatus"); }
        }

        // For simlifying of access to current config
        private ConfigIdentity Config
        {
            get => ConfigObjects[selectedObjectIndex].confObject.Config;
            set => ConfigObjects[selectedObjectIndex].confObject.Config = value;
        }

        // Commands
        //----------------
        //

        private Command commandReset;
        public Command CommandReset
        {
            get
            {
                return commandReset ??= new Command(obj => ResetToDefault());
            }
        }

        private Command commandSaveState;
        public Command CommandSaveState
        {
            get
            {
                return commandSaveState ??= new Command(obj => SaveState());
            }
        }

        private Command commandDeleteConfig;
        public Command CommandDeleteConfig
        {
            get
            {
                return commandDeleteConfig ??= new Command(
                    obj => _ = DeleteConfigAsync(),
                    obj => CanDeleteConfig());
            }
        }

        private Command commandSaveConfig;
        public Command CommandSaveConfig
        {
            get
            {
                return commandSaveConfig ??= new Command(
                    obj => SaveConfig(obj as string),
                    obj => CanSaveConfig());
            }
        }

        // Asyncs
        //----------------
        //

        // Performs changing of configurable object
        private async Task<bool> SetObjectIndexAsync(int Index)
        {
            bool result = false;

            if (Connected)
            {
                result = await LoadConfigItemsAsync(ConfigObjects[Index].confObject.Config);
            }

            SelectedObject = ConfigObjects[Index].confObject.Config;

            selectedObjectIndex = Index;
            OnPropertyChanged("SelectedObjectIndex");

            if (ConfigsList.Count == 0)
            {
                SetDefaultConfig();
            }

            SetCurrentConfigTitle();

            return result;
        }

        // Performs changing of configuration for selected object
        private async Task<bool> ChangeConfigAsync(int Item)
        {
            bool result = false;

            if (configsList is not null && Item < configsList.Count)
            {
                result = await LoadConfigAsync(SelectedObject, configsList[Item].Id);

                if (result)
                {
                    selectedConfig = Item;
                    OnPropertyChanged("SelectedConfig");
                }
            }

            return result;
        }

        // Adds new config into DB
        private async Task<object> AddConfigAsync(IConfigIdentity ConfigObject)
        {
            IsEnabled = false;

            SRPHttpClient client = new();
            var config = await client.Add(ConfigObject);

            CheckRequestResult(client);

            IsEnabled = true;

            return config;
        }

        // Updates config in DB
        private async Task<object> UpdateConfigAsync(IConfigIdentity ConfigObject, int Id)
        {
            IsEnabled = false;

            SRPHttpClient client = new();
            var config = await client.Update(ConfigObject, Id);

            CheckRequestResult(client);

            IsEnabled = true;

            return config;
        }

        // Loads list of configs identities for current object
        private async Task<bool> LoadConfigItemsAsync(object ConfigObject)
        {
            IsEnabled = false;

            SRPHttpClient client = new();
            var configs = await client.Index(ConfigObject);

            CheckRequestResult(client);

            IsEnabled = true;

            if (configs is null)
            {
                return false;
            }

            List<ConfigIdentity> items = new();
            items.AddRange(configs.OrderBy(x => x.Name));
            ConfigsList = items;

            return true;
        }

        // Loads selected config from DB
        private async Task<bool> LoadConfigAsync(object ConfigObject, int Id)
        {
            IsEnabled = false;

            SRPHttpClient client = new();
            var config = await client.Get(ConfigObject, Id);

            CheckRequestResult(client);

            if (config is not null)
            {
                Config = config as ConfigIdentity;
                SelectedObject = Config;
                configName = Config.Name;
                SelectedObject.Modified = false;
                SaveState();
            }

            IsEnabled = true;

            return true;
        }

        // Deletes configuration from DB
        private async Task<bool> DeleteConfigAsync()
        {
            IsEnabled = false;

            SRPHttpClient client = new();
            ConfigIdentity config = await client.Delete(SelectedObject, ConfigsList[selectedConfig].Id)
                as ConfigIdentity;

            CheckRequestResult(client);

            if (config is not null)
            {
                if (configsList.Remove(new ConfigIdentity { Id = config.Id, Name = config.Name }))
                {
                    if (configsList.Count == 0)
                    {
                        SetDefaultConfig();
                    }

                    UpdateConfigList();
                }
            }

            IsEnabled = true;

            return true;
        }

        // Commands processing
        //-------------------------------
        //

        // Saves state of currently selected config
        public bool SaveState()
        {
            return Config.Save();
        }

        // Loads all saved configs states
        public void LoadConfigs()
        {
            foreach (var config in ConfigObjects)
            {
                config.confObject.Config.Load(cofigSavePath);
            }
        }

        // Accessibility for command execution
        private bool CanDeleteConfig()
        {
            if (DesignModeStatus.IsInDesignMode)
            {
                return true;
            }

            return Config.Id > 0;
        }

        // Accessibility for command execution
        private bool CanSaveConfig()
        {
            return SelectedObject is null ? false : SelectedObject.Modified;
        }

        // Writing of config into DB
        private bool SaveConfig(string Name)
        {
            ConfigIdentity result = null;

            ConfigIdentity item = ConfigsList.FirstOrDefault(x => x.Name == Name);

            if (item != null)
            {
                // Update
                if (item.Id == ConfigsList[selectedConfig].Id)
                {
                    result = Task.Run(
                        async () => await UpdateConfigAsync(Config, ConfigsList[selectedConfig].Id))
                        .Result
                        as ConfigIdentity;
                }
            }
            else
            {
                // New
                Config.InitForAdd(Name);

                result = Task.Run(
                    async () => await AddConfigAsync(Config))
                    .Result
                    as ConfigIdentity;

                if (result is not null)
                {
                    ConfigsList.Add(new ConfigIdentity { Id = result.Id, Name = result.Name });
                    UpdateConfigList(result.Id);
                }
            }

            if (result is not null)
            {
                SelectedObject.Modified = false;
            }

            return result is not null;
        }

        // Trick to update views via binding
        private void UpdateConfigList(int SelectedID = -1)
        {
            List<ConfigIdentity> list = new();
            list.AddRange(ConfigsList.OrderBy(x => x.Name));
            ConfigsList = list;

            SelectedConfig = SelectedID != -1 ? 
                ConfigsList.TakeWhile(x => x.Id != SelectedID).Count() : 
                0;
        }

        // Sets current config
        private void SetDefaultConfig()
        {
            SelectedObject = Config;
            ConfigName = SelectedObject.Name;
        }

        // Resets config to default values
        private void ResetToDefault()
        {
            SelectedObject.ResetToDefault();
            ConfigName = SelectedObject.Name;
            SelectedObject = null;
            SelectedObject = Config;
            SelectedObject.Modified = false;
        }

        // A little trick to select current config, cause binding doesnt work with the same value
        private void SetCurrentConfigTitle()
        {
            // Saving the Name and Modified flag
            bool Modified = SelectedObject.Modified;
            string Name = SelectedObject.Name;

            // Changing the name causes changing selection in the combo
            ConfigName = "";
            ConfigName = Name;

            SelectedObject.Modified = Modified;
        }

        // Connection testing
        //-------------------------------
        //

        // Timer's handle
        private void OnPing(Object source, ElapsedEventArgs e)
        {
            mainWindow.Dispatcher.Invoke(Ping);
        }

        private async void Ping()
        {
            SRPHttpClient client = new();
            await client.Ping();
            CheckRequestResult(client);
        }

        private void InitServerConnection(bool connected)
        {
            if (connected)
            {
                _ = InitConnect();
            }
            else
            {
                InitDisconnect();
            }
        }

        private async Task<bool> InitConnect()
        {
            bool result = await LoadConfigItemsAsync(Config);

            if (result)
            {
                SetCurrentConfigTitle();

                return true;
            }

            return false;
        }

        private void InitDisconnect()
        {
            List<ConfigIdentity> items = new();
            ConfigsList = items;
        }

        private bool CheckRequestResult(SRPHttpClient client)
        {
            if (!client.Result)
            {
                ServerStatus = client.ErrorMessage;
                return Connected = false;  
            }

            ServerStatus = serverConnected;
            return Connected = true;
        }

        // Implementations of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private readonly Timer timerPing;
    }

}

