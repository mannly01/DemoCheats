using Il2Cpp;
using Il2CppCMS.Shared;
using Il2CppCMS.UI;
using Il2CppCMS.UI.Logic;
using Il2CppCMS.UI.Windows;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemoCheats
{
    public static class BuildInfo
    {
        public const string Name = "Demo Cheats";
        public const string Description = "A collection of mods for the Car Mechanic Simulator 2026 Demo.";
        public const string Author = "mannly82";
        public const string Company = "The Mann Design";
        public const string Version = "1.0.1";
        public const string DownloadLink = "https://github.com/mannly01/DemoCheats/releases/new";
        public const string MelonGameCompany = "Red Dot Games";
        public const string MelonGameName = "Car Mechanic Simulator 2026 Demo";
    }

    /// <summary>
    /// Create a "DemoCheats.cfg" file in the Mods folder.
    /// </summary>
    public class DemoCheatsConfigFile
    {
        /// <summary>
        /// Settings Category
        /// </summary>
        private const string SettingsCatName = "DemoCheatsSettings";
        private readonly MelonPreferences_Category _settings;
        private readonly string _configFilePath;

        /// <summary>
        /// User setting for the key to add money to the player.
        /// </summary>
        public KeyCode AddMoney => _addMoney.Value;
        private readonly MelonPreferences_Entry<KeyCode> _addMoney;
        /// <summary>
        /// User setting for the amount of money to add.
        /// </summary>
        public int MoneyAmount
        {
            get => _moneyAmount.Value;
            set { _moneyAmount.Value = value; }
        }
        private readonly MelonPreferences_Entry<int> _moneyAmount;
        /// <summary>
        /// User setting for the game speed.
        /// </summary>
        public float GameSpeed
        {
            get => _gameSpeed.Value;
            set { _gameSpeed.Value = value; }
        }
        private readonly MelonPreferences_Entry<float> _gameSpeed;
        /// <summary>
        /// User setting for the key to refresh the Used Parts Market (UPM).
        /// </summary>
        public KeyCode RefreshUPM => _refreshUPM.Value;
        private readonly MelonPreferences_Entry<KeyCode> _refreshUPM;
        /// <summary>
        /// Uwer setting for the key to toggle the DNB Censor parts in the UPM.
        /// </summary>
        public KeyCode ToggleUPM => _toggleUPM.Value;
        private readonly MelonPreferences_Entry<KeyCode> _toggleUPM;
        /// <summary>
        /// User setting for the key to toggle demo world limits
        /// </summary>
        public KeyCode ToggleDemoWorldLimits => _toggleDemoWorldLimits.Value;
        private readonly MelonPreferences_Entry<KeyCode> _toggleDemoWorldLimits;

        /// <summary>
        /// Implementation of Settings properties.
        /// </summary>
        public DemoCheatsConfigFile()
        {
#if DEBUG
            LogService.Instance.WriteToLog("Called", "ConfigFile.Init");
#endif
            _settings = MelonPreferences.CreateCategory(SettingsCatName);
            _settings.SetFilePath(Path.Combine("Mods", "DemoCheats.cfg"));
            _configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", "DemoCheats.cfg");

            _addMoney = _settings.CreateEntry(nameof(AddMoney), KeyCode.F4,
                description: "Press this key to add the following amount of money to the player.");
            _moneyAmount = _settings.CreateEntry(nameof(MoneyAmount), 1000000,
                description: "The amount of money to add to the player.");
            _gameSpeed = _settings.CreateEntry(nameof(GameSpeed), 1.0f,
                description: "Sets the Game Speed of the Unity Engine (1.0-2.0, 1.5 is recommended max).");
            _refreshUPM = _settings.CreateEntry(nameof(RefreshUPM), KeyCode.F5,
                description: "Press this key to refresh the Used Parts Market immediately.");
            _toggleUPM = _settings.CreateEntry(nameof(ToggleUPM), KeyCode.F6,
                description: "Press this key to show only the DNB Censor car parts in the Used Parts Market.");
            _toggleDemoWorldLimits = _settings.CreateEntry(nameof(ToggleDemoWorldLimits), KeyCode.F7,
                description: "Press this key to toggle the demo world limits.");

            try
            {
                if (!File.Exists(_configFilePath))
                {
                    _settings.SaveToFile();
                }
            }
            catch (Exception ex)
            {
                LogService.Instance.WriteToLog($"Unable to ensure config path: {ex.Message}", "ConfigFile.Init");
            }

#if DEBUG
            // Logging for debug purposes.
            LogService.Instance.WriteToLog($"AddMoney: {AddMoney}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"MoneyAmount: {MoneyAmount}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"GameSpeed: {GameSpeed}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"RefreshUPM: {RefreshUPM}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"ToggleUPM: {ToggleUPM}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"ToggleDemoWorldLimits: {ToggleDemoWorldLimits}", "ConfigFile.Init");
#endif
        }
    }

    public class DemoCheats : MelonMod
    {
        /// <summary>
        /// Reference to Settings file.
        /// </summary>
        private DemoCheatsConfigFile _configFile;

        /// <summary>
        /// Global reference to the current scene.
        /// </summary>
        private string _currentScene = string.Empty;

        /// <summary>
        /// Variable to store whether the user want to see just DNB Censor parts.
        /// </summary>
        private bool _onlyDNBParts = false;

        /// <summary>
        /// References to the demo world limit game objects.
        /// </summary>
        private bool _demoLimitsActive = true;
        private GameObject _demoWallsGO = null;
        private GameObject _garage_Exterior_Demo_ColliderGO = null;
        private GameObject _garage_Exterior_Demo_Wall_Blocked_1GO = null;
        private GameObject _demoVehiclesDetectorGO = null;

        public override void OnInitializeMelon()
        {
            // Tell the user a log file was created.
            MelonLogger.Msg("Creating Log File...");
            LogService.Instance.Initialize("DemoCheats");
            // Tell the user that we're loading the Settings.
            MelonLogger.Msg("Loading Settings...");
            _configFile = new DemoCheatsConfigFile();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (buildIndex == -1)
            {
                return;
            }
            // Save a reference to the current scene.
            _currentScene = sceneName?.ToLower() ?? string.Empty;
            if (_currentScene.Equals("christmas") ||
                _currentScene.Equals("easter") ||
                _currentScene.Equals("halloween"))
            {
                LogService.Instance.WriteToLog($"{sceneName} custom scene is active");
                _currentScene = "garage";
            }
#if DEBUG
            if (_currentScene.Equals("garage") ||
                _currentScene.Equals("barn") ||
                _currentScene.Equals("junkyard"))
            {
                LogService.Instance.WriteToLog($"SceneName: {_currentScene}");
            }
#endif
            if (_currentScene.Equals("garage"))
            {
                var targets = new List<string>() { "DemoWalls", "Garage_Exterior_Demo_Collider", "Garage_Exterior_Demo_Wall_Blocked_1", "DemoVehiclesDetector" };
                foreach (var name in targets)
                {
                    GameObject target = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == name);
                    if (target != null)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Found GameObject {GetGameObjectPath(target)}");
#endif
                        switch (name)
                        {
                            case "DemoWalls":
                                _demoWallsGO = target;
                                break;
                            case "Garage_Exterior_Demo_Collider":
                                _garage_Exterior_Demo_ColliderGO = target;
                                break;
                            case "Garage_Exterior_Demo_Wall_Blocked_1":
                                _garage_Exterior_Demo_Wall_Blocked_1GO = target;
                                break;
                            case "DemoVehiclesDetector":
                                _demoVehiclesDetectorGO = target;
                                break;
                        }
                    }
#if DEBUG
                    else
                    {
                        LogService.Instance.WriteToLog($"Could not find GameObject with name {name}");
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Adjusts the game speed based on the value in the config file.
        /// </summary>
        public void AdjustGameSpeed()
        {
            if (_configFile == null)
            {
                LogService.Instance.WriteToLog("Config file missing - cannot adjust game speed");
                return;
            }

            // Clamp to a sensible range to avoid breaking physics badly.
            float clamped = Mathf.Clamp(_configFile.GameSpeed, 0.1f, 2.0f);
            Time.timeScale = clamped;
            Singleton<UIManager>.Instance.ShowPopup($"Game Speed set to {clamped}.", PopupType.Normal);
        }

        /// <summary>
        /// Refreshes the Used Parts Market parts list and updates the parts page.
        /// </summary>
        private void RefreshUPM()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null) return;

            if (windowManager.activeWindows.count > 0 &&
                windowManager.IsWindowActive(WindowID.Shop))
            {
                var shopWindow = windowManager.GetWindowByID<ShopWindow25>(WindowID.Shop);
                if (shopWindow != null)
                {
                    //LogService.Instance.WriteToLog("Shop Window Active");
                    //var currentShopType = shopWindow.currentShopType;
                    //LogService.Instance.WriteToLog($"Current ShopType: {currentShopType}");
                    //if (currentShopType == ShopType25.UsedParts)
                    //{
                    //    LogService.Instance.WriteToLog($"Used Parts Page Active");
                    //}
                    var usedPartsManager = shopWindow.usedPartsShopManager;
                    if (usedPartsManager != null)
                    {
                        usedPartsManager.GenerateParts();
                    }
                    var partsPage = shopWindow.partsPage;
                    if (partsPage != null && partsPage.ShopType == ShopType25.UsedParts)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Used Parts Page Active");
#endif
                        partsPage.UpdateItems();
                        LogService.Instance.WriteToLog("Refreshed Used Parts Market");
                    }
                }
            }
        }

        /// <summary>
        /// Toggles off any car parts that aren't the DNB Censor and then
        /// refreshes the Used Parts Market parts list and updates the parts page.
        /// </summary>
        private void ToggleUPM()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null) return;

            if (windowManager.activeWindows.count > 0 &&
                windowManager.IsWindowActive(WindowID.Shop))
            {
                var shopWindow = windowManager.GetWindowByID<ShopWindow25>(WindowID.Shop);
                if (shopWindow != null)
                {
                    var usedPartsManager = shopWindow.usedPartsShopManager;
                    if (usedPartsManager != null)
                    {
                        if (_onlyDNBParts)
                        {
                            usedPartsManager.InitializeItemsPool();
                        }
                        else
                        {
                            List<Il2CppContainers.UsedPartsShopItem> itemsToKeep = new List<Il2CppContainers.UsedPartsShopItem>();
                            var items = usedPartsManager.itemsPool;
                            if (items != null)
                            {
                                for (int i = 0; i < items.Count; i++)
                                {
                                    var item = items[i];
                                    if (item != null && item.ItemId != null && item.ItemId.ToLower().Contains("dnb"))
                                    {
                                        itemsToKeep.Add(item);
                                    }
                                }

#if DEBUG
                                //else
                                //{
                                //    LogService.Instance.WriteToLog(item.ItemId);
                                //}
                                // The following is the output from the above line.
                                //var parts = new List<string>() { "car_dnbcensor-bumper_front2",
                                //                                "car_dnbcensor-mirror_right",
                                //                                "car_dnbcensor-mirror_left",
                                //                                "car_dnbcensor-sideskirt_right2",
                                //                                "car_dnbcensor-bumper_rear",
                                //                                "car_dnbcensor-door_front_left",
                                //                                "car_dnbcensor-sideskirt_left",
                                //                                "car_dnbcensor-door_front_right",
                                //                                "car_dnbcensor-bed_cover_openable",
                                //                                "car_dnbcensor-fender_front_right",
                                //                                "car_dnbcensor-fender_front_left",
                                //                                "car_dnbcensor-bumper_rear2",
                                //                                "car_dnbcensor-sideskirt_right",
                                //                                "car_dnbcensor-trunk",
                                //                                "car_dnbcensor-sideskirt_left2",
                                //                                "car_dnbcensor-hood",
                                //                                "car_dnbcensor-bumper_front",
                                //                                "car_dnbcensor-hood2" };
#endif

                                // Replace items pool contents with the filtered list
                                items.Clear();
                                foreach (var keep in itemsToKeep)
                                {
                                    items.Add(keep);
                                }
                            }
                        }
                        usedPartsManager.GenerateParts();
                        _onlyDNBParts = !_onlyDNBParts;
                        LogService.Instance.WriteToLog("Toggled Used Parts Market");
                    }
                    var partsPage = shopWindow.partsPage;
                    if (partsPage != null && partsPage.ShopType == ShopType25.UsedParts)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Used Parts Page Active");
#endif
                        partsPage.UpdateItems();
                        LogService.Instance.WriteToLog("Refreshed Used Parts Market");
                    }
                }
            }
        }

        private void ToggleDemoWorldLimits()
        {
            _demoLimitsActive = !_demoLimitsActive;
            if (_demoWallsGO != null)
            {
                _demoWallsGO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"DemoWalls: {(_demoWallsGO.activeSelf ? "Enabled" : "Disabled")}");
            }
            if (_garage_Exterior_Demo_ColliderGO != null)
            {
                _garage_Exterior_Demo_ColliderGO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"Garage_Exterior_Demo_Collider: {(_garage_Exterior_Demo_ColliderGO.activeSelf ? "Enabled" : "Disabled")}");
            }
            if (_garage_Exterior_Demo_Wall_Blocked_1GO != null)
            {
                _garage_Exterior_Demo_Wall_Blocked_1GO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"Garage_Exterior_Demo_Wall_Blocked_1: {(_garage_Exterior_Demo_Wall_Blocked_1GO.activeSelf ? "Enabled" : "Disabled")}");
            }
            if (_demoVehiclesDetectorGO != null)
            {
                _demoVehiclesDetectorGO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"DemoVehiclesDetector: {(_demoVehiclesDetectorGO.activeSelf ? "Enabled" : "Disabled")}");
            }
            LogService.Instance.WriteToLog($"Demo World Limits: {(_demoLimitsActive ? "Enabled" : "Disabled")}");
            Singleton<UIManager>.Instance.ShowPopup($"Demo World Limits {(_demoLimitsActive ? "Enabled" : "Disabled")}", PopupType.Normal);
        }

        public override void OnUpdate()
        {
#if DEBUG
            if (Input.GetKeyDown(KeyCode.J))
            {
                // This is a test key that should not be shipped with a release.
                //RemoveDemoWalls();
                //AdjustGameSpeed();
                CheckIfInputIsFocused();
            }
#endif
            if (_configFile == null) return;

            if (Input.GetKeyDown(_configFile.AddMoney))
            {
                // Check if a window is open and show the user a message to close it first.
                if (Singleton<WindowManager>.Instance?.activeWindows.count > 0)
                {
                    Singleton<UIManager>.Instance.ShowPopup($"Please close any open windows first.", PopupType.Normal);
                }
                else
                {
                    // Get a reference to the Shared Game Data Manager.
                    var sharedGameDataManager = SharedGameDataManager.Instance;
#if DEBUG
                    LogService.Instance.WriteToLog($"Original Money: {sharedGameDataManager?.money}");
#endif
                    // Adds the money amount in the config file to the player.
                    sharedGameDataManager?.AddMoneyRpc(_configFile.MoneyAmount);
#if DEBUG
                    LogService.Instance.WriteToLog($"New Money: {sharedGameDataManager?.money}");
                    //LogService.Instance.WriteToLog($"Inv Size Multi: {sharedGameDataManager.inventorySizeMultiplier}");
                    //sharedGameDataManager.inventorySizeMultiplier = 5;
                    //LogService.Instance.WriteToLog($"New Inv Size Multi: {sharedGameDataManager.inventorySizeMultiplier}");
                    //LogService.Instance.WriteToLog($"Xp Multi: {sharedGameDataManager.xpMultiplier}");
                    //sharedGameDataManager.xpMultiplier = 5;
                    //LogService.Instance.WriteToLog($"New Xp Multi: {sharedGameDataManager.xpMultiplier}");
#endif
                }
            }

            if (Input.GetKeyDown(_configFile.RefreshUPM))
            {
                if (_currentScene.Equals("garage"))
                {
                    RefreshUPM();
                }
            }

            if (Input.GetKeyDown(_configFile.ToggleUPM))
            {
                if (_currentScene.Equals("garage"))
                {
                    ToggleUPM();
                }
            }

            if (Input.GetKeyDown(_configFile.ToggleDemoWorldLimits))
            {
                if (_currentScene.Equals("garage"))
                {
                    ToggleDemoWorldLimits();
                }
            }
        }

        /// <summary>
        /// Gets the path to the GameOject.
        /// </summary>
        /// <param name="obj">The GameObject that will have it's path returned.</param>
        /// <returns>(string) The path to the passed GameObject.</returns>
        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        /// <summary>
        /// If the user is using the Search box,
        /// the mod should do nothing.
        /// </summary>
        /// <returns>(bool) True if the Search/Input Field is being used.</returns>
        private bool CheckIfInputIsFocused()
        {
            try
            {
                var evt = EventSystem.current;
                if (evt != null)
                {
                    var activeGO = evt.currentSelectedGameObject;
                    if (activeGO != null)
                    {
                        // Check for legacy InputField
                        if (activeGO.GetComponent<InputField>()?.isFocused == true)
                            return true;

                        // If TMP is used, it often exposes TMP_InputField type; do a safer type-name check
                        var tmpComp = activeGO.GetComponent("TMP_InputField");
                        if (tmpComp != null)
                        {
                            // assume TMP input is focused when selected
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Keep silent - on some builds some APIs might be unavailable; default to not focused.
            }
            return false;
        }
    }

    /// <summary>
    /// A service to allow for debug logging.
    /// </summary>
    public class LogService
    {
        // Thread-safe lazy singleton.
        private static readonly Lazy<LogService> _lazyInstance = new Lazy<LogService>(() => new LogService());
        public static LogService Instance => _lazyInstance.Value;

        // The path to the log file.
        private string _logFilePath = string.Empty;
        // A list of strings to write to the log file.
        private readonly List<string> _logs = new List<string>();
        // A sync lock for thread-safety.
        private readonly object _sync = new object();

        // A public reference to the log count.
        // This will be used when the mod closes to write any pending logs.
        public int LogCount
        {
            get { lock (_sync) { return _logs.Count; } }
        }

        public void Initialize(string fileName)
        {
            try
            {
                // Create a DateTime string to log.
                string logDate = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
                // Create the log file path string.
                _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", $"{fileName}.log");
                // Ensure directory exists
                var dir = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Create the log file and write some initial information to it.
                File.WriteAllLines(_logFilePath, new List<string> { $"{BuildInfo.Name} - {BuildInfo.Version}", $"CMS 2026 Demo - {Il2Cpp.GameSettings.BuildVersion}", $"Log Created: {logDate}" });
            }
            catch (Exception ex)
            {
                // In case logging initialization fails, keep using in-memory buffer.
                _logFilePath = string.Empty;
#if DEBUG
                MelonLogger.Msg($"LogService.Initialize failed: {ex.Message}");
#endif
            }
        }

        /// <summary>
        /// Method to write a string to the log file.
        /// </summary>
        /// <param name="message">A string with the message to log.</param>
        /// <param name="callerName">The method that the log is being created.</param>
        public void WriteToLog(string message, [CallerMemberName] string callerName = "")
        {
            var logString = $"{DateTime.Now:HH:mm:ss}\t{callerName}\t{message}.";

#if DEBUG
            MelonLogger.Msg(message);
#endif

            lock (_sync)
            {
                _logs.Add(logString);

                if (string.IsNullOrWhiteSpace(_logFilePath))
                {
                    // Not initialized yet; retain in buffer.
                    return;
                }

                try
                {
                    if (File.Exists(_logFilePath))
                    {
                        File.AppendAllLines(_logFilePath, _logs);
                        _logs.Clear();
                    }
                    else
                    {
                        // recreate file if missing
                        File.WriteAllLines(_logFilePath, new[] { $"{BuildInfo.Name} - {BuildInfo.Version}", $"CMS 2026 Demo - {Il2Cpp.GameSettings.BuildVersion}", $"Log Created: {DateTime.Now:MM-dd-yyyy HH:mm:ss}" });
                        File.AppendAllLines(_logFilePath, _logs);
                        _logs.Clear();
                    }
                }
                catch (Exception ex)
                {
                    // If it fails, keep entries in _logs for next attempt
                    _logs.Add($"{DateTime.Now:HH:mm:ss}\tLogService.WriteToLog: Unable to write to log: {ex.Message}");
                }
            }
        }
    }
}
