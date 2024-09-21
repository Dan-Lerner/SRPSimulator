using SRPConfig;
using SRPSimulator.MathModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SRPSimulator.ViewModel
{
    public class ConfigObjectProperties : ConfigIdentity
    {
        public Configurable confObject;
        public bool ShowImage;
        public bool Expand;
    }

    internal class ConfigViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private const double timeServerPing = 5000;
        private const string cofigSavePath = @"/config/";
        private const string serverConnected = "Connected";
        private const string serverDisconnected = "Disconnected";
        
        readonly MainWindow mainWindow;

        public ConfigViewModel()
        {
            if (!DesignModeStatus.IsInDesignMode) // design time errors avoiding
            {
                mainWindow = (System.Windows.Application.Current as App).MainWindow as MainWindow;

                PUnit unit = SRPServices.GetService(typeof(PUnit)) as PUnit;

                ObjectsList = new List<ConfigObjectProperties> {
                    new ConfigObjectProperties
                    { Id = 0, Name = "PUnit", confObject = unit, ShowImage = true, Expand = false },
                    new ConfigObjectProperties
                    { Id = 6, Name = "VFC", confObject = unit.Vfc, ShowImage = false, Expand = true },
                    new ConfigObjectProperties
                    { Id = 1, Name = "Drive", confObject = unit.Drive, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 2, Name = "SRP", confObject = unit.Srp, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 3, Name = "Fluid", confObject = unit.Srp.Fluid, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 4, Name = "Tubing", confObject = unit.Srp.Tubing, ShowImage = false, Expand = false },
                    new ConfigObjectProperties
                    { Id = 5, Name = "Rod", confObject = unit.Srp.Rod, ShowImage = false, Expand = true }
                };

                LoadConfigsFromXML();
                _ = Ping();
            }
        }

        // List of all objects with configuration (Tabs)
        private List<ConfigObjectProperties> objectsList;
        public List<ConfigObjectProperties> ObjectsList {
            get => objectsList;
            set { 
                objectsList = value; 
                OnPropertyChanged("ObjectsList"); 
            }
        }

        // Index of currently selected configuration Object (current tab)
        private int activeObjectIndex;
        public int ActiveObjectIndex {
            get => activeObjectIndex;
            set {
                activeObjectIndex = value;
                _ = SetActiveObjectAsync(value);
            }
        }

        // List of configs (combo)
        private List<ConfigIdentity> configsList = new();
        public List<ConfigIdentity> ConfigsList {
            get => configsList;
            set { 
                configsList = value; 
                OnPropertyChanged("ConfigsList"); 
            }
        }

        // Currently selected config (combo curent selection)
        private int selectedConfig;
        public int SelectedConfig {
            get => selectedConfig;
            set => _ = ChangeConfigAsync(value);
        }

        // Reference to currently selected configuration object (Property greed)
        private ConfigIdentity activeObject;
        public ConfigIdentity ActiveObject {
            get => activeObject;
            set { 
                activeObject = value; 
                OnPropertyChanged("ActiveObject"); 
            }
        }

        // Name of current configuration
        private string configName;
        public string ConfigName {
            get => configName;
            set {
                configName = value;
                if (ActiveObject != null)
                    ActiveObject.Name = configName;
                OnPropertyChanged("ConfigName");
            }
        }

        // Enabling/disabing of configuration control panel
        private bool enableOperations = true;
        public bool EnableOperations {
            get => enableOperations;
            set { 
                enableOperations = value; 
                OnPropertyChanged("EnableOperations");
            }
        }

        // Server connection status flag
        private bool connected;
        public bool Connected {
            get => connected;
            set {
                connected = value;
                OnPropertyChanged("Connected");
            }
        }

        // Tip for current connection status
        private string serverStatus = serverDisconnected;
        public string ServerStatus {
            get => serverStatus;
            set { 
                serverStatus = value; 
                OnPropertyChanged("ServerStatus"); 
            }
        }

        // Simlifying of access to current config
        private ConfigIdentity Config {
            get => ObjectsList[activeObjectIndex].confObject.Config;
            set => ObjectsList[activeObjectIndex].confObject.Config = value;
        }

        // Loads all saved configs states
        public void LoadConfigsFromXML()
        {
            foreach (var config in ObjectsList)
                config.confObject.Config.Load(cofigSavePath);
        }

        public bool CanDeleteConfig(object value)
        {
            //if (DesignModeStatus.IsInDesignMode)
            //    return true;
            return Config.Id > 0;
        }

        public void DeleteConfig(object value, EventArgs e)
        {
            _ = DeleteConfigAsync();
        }

        public bool CanResetConfig(object value) => true;

        public void ResetConfig(object value, EventArgs e)
        {
            Config.ResetToDefault();
            ConfigName = Config.Name;
            OnPropertyChanged("ActiveObject");
            ActiveObject = null;
            ActiveObject = Config;
            Config.Modified = false;
        }

        public bool CanSaveState(object value) => true;

        public void SaveState(object value, EventArgs e) => SaveObjectState();
        
        public void SaveObjectState() => Config.Save();

        public void SaveConfig(object value, EventArgs e)
        {
            _ = SaveChangesAsync(value as string);
        }

        public bool CanSaveConfig(object value = null)
        {
            return Config == null ? false : Config.Modified;
        }

        // Loads list of configs identities for current object
        private async Task SetActiveObjectAsync(int index)
        {
            var newSelection = ObjectsList[index].confObject.Config;
            if (Connected)
                await LoadConfigListAsync(newSelection);
            ActiveObject = newSelection;
            OnPropertyChanged("SelectedObjectIndex");
            if (ConfigsList.Count == 0)
                SetDefaultConfig();
            SyncronizeConfigTitle();
        }

        private async Task<bool> LoadConfigListAsync(object obj)
        {
            EnableOperations = false;
            var configs = await httpClient_.Index(obj);
            CheckRequestStatus();
            if (configs == null)
                return false;
            ConfigsList = configs.OrderBy(x => x.Name).ToList();
            EnableOperations = true;
            return true;
        }

        // Performs changing of configuration for selected object
        private async Task ChangeConfigAsync(int configIndex)
        {
            if (!await LoadConfigAsync(Config, configsList[configIndex].Id))
                return;
            selectedConfig = configIndex;
            OnPropertyChanged("SelectedConfig");
        }

        // Loads selected config from DB
        private async Task<bool> LoadConfigAsync(object objectToConfigure, int configId)
        {
            EnableOperations = false;
            var config = await httpClient_.Get(objectToConfigure, configId);
            CheckRequestStatus();
            if (config != null)
            {
                Config = config as ConfigIdentity;
                ActiveObject = Config;
                configName = Config.Name;
                Config.Modified = false;
                SaveObjectState();
                EnableOperations = true;
                return true;
            }
            EnableOperations = true;
            return true;
        }

        // Adds new config into DB
        private async Task<object> AddConfigAsync(IConfigIdentity ConfigObject)
        {
            EnableOperations = false;
            var config = await httpClient_.Add(ConfigObject);
            CheckRequestStatus();
            EnableOperations = true;
            return config;
        }

        // Updates config in DB
        private async Task<object> UpdateConfigAsync(IConfigIdentity ConfigObject, int Id)
        {
            EnableOperations = false;
            var config = await httpClient_.Update(ConfigObject, Id);
            CheckRequestStatus();
            EnableOperations = true;
            return config;
        }

        public async Task<object> SaveChangesAsync(string Name)
        {
            ConfigIdentity result = null;
            var item = ConfigsList.FirstOrDefault(x => x.Name == Name);
            if (item != null) {
                // Updates only the active config
                if (item.Id == ConfigsList[selectedConfig].Id)
                    result = await UpdateConfigAsync(Config, ConfigsList[selectedConfig].Id)
                        as ConfigIdentity;
            }
            else {
                // New
                Config.InitForAdd(Name);
                result = await AddConfigAsync(Config) as ConfigIdentity;
                if (result != null) {
                    ConfigsList.Add(result);
                    UpdateConfigList(result.Id);
                }
            }
            if (result != null)
                Config.Modified = false;
            return result;
        }

        private async Task<bool> DeleteConfigAsync()
        {
            EnableOperations = false;
            ConfigIdentity config = await httpClient_.Delete(Config, ConfigsList[selectedConfig].Id)
                as ConfigIdentity;
            CheckRequestStatus();
            if (config == null) {
                if (configsList.Remove(config)) {
                    if (configsList.Count == 0)
                        SetDefaultConfig();
                    UpdateConfigList();
                }
            }
            EnableOperations = true;
            return config != null;
        }

        private void UpdateConfigList(int SelectedID = -1)
        {
            ConfigsList = ConfigsList.OrderBy(x => x.Name).ToList();
            SelectedConfig = SelectedID != -1 ? 
                ConfigsList.TakeWhile(x => x.Id != SelectedID).Count() : 
                0;
        }

        // Sets current config
        private void SetDefaultConfig()
        {
            ConfigName = Config.Name;
        }

        private void SyncronizeConfigTitle()
        {
            var Modified = Config.Modified;
            var Name = Config.Name;
            // Changing the name causes changing selection in the combo
            ConfigName = "";
            ConfigName = Name;
            Config.Modified = Modified;
        }

        private async Task Ping()
        {
            await httpClient_.Ping();
            CheckRequestStatus();
        }

        private void CheckRequestStatus()
        {
            if (connected != httpClient_.Success) {
                Connected = httpClient_.Success;
                if (connected) {
                    _ = InitServerConnect();
                    ServerStatus = serverConnected;
                }
                else {
                    ConfigsList = new();
                    ServerStatus = httpClient_.ErrorMessage;
                }
            }
        }

        private async Task InitServerConnect()
        {
            if (await LoadConfigListAsync(Config))
                SyncronizeConfigTitle();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        SRPHttpClient httpClient_ = new();
    }

}