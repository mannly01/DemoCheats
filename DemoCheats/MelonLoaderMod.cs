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

namespace DemoCheats
{
    public static class BuildInfo
    {
        public const string Name = "Demo Cheats";
        public const string Description = "A collection of mods for the Car Mechanic Simulator 2026 Demo.";
        public const string Author = "mannly82";
        public const string Company = "The Mann Design";
        public const string Version = "1.0.0";
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
            _settings.SetFilePath("Mods/DemoCheats.cfg");
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

            if (!File.Exists($"{Directory.GetCurrentDirectory()}\\Mods\\DemoCheats.cfg"))
            {
                _settings.SaveToFile();
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
            _currentScene = sceneName.ToLower();
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
                        LogService.Instance.WriteToLog($"Found GameObject{GetGameObjectPath(target)}");
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
            Time.timeScale = _configFile.GameSpeed;
            Singleton<UIManager>.Instance.ShowPopup($"Game Speed set to {_configFile.GameSpeed}.", PopupType.Normal);
        }

        /// <summary>
        /// Refreshes the Used Parts Market parts list and updates the parts page.
        /// </summary>
        private void RefreshUPM()
        {
            var windowManager = WindowManager.Instance;
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
                    if (partsPage != null)
                    {
                        if (partsPage.ShopType == ShopType25.UsedParts)
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
        }

        /// <summary>
        /// Toggles off any car parts that aren't the DNB Censor and then
        /// refreshes the Used Parts Market parts list and updates the parts page.
        /// </summary>
        private void ToggleUPM()
        {
            var windowManager = WindowManager.Instance;
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
                            List<Il2CppContainers.UsedPartsShopItem> toDelete = new List<Il2CppContainers.UsedPartsShopItem>();
                            var items = usedPartsManager.itemsPool;
                            for (int i = 0; i < items.Count; i++)
                            {
                                var item = items[i];
                                if (!item.ItemId.ToLower().Contains("dnb"))
                                {
                                    toDelete.Add(item);
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
                            }
                            foreach (var item in toDelete)
                            {
                                items.Remove(item);
                            }
                        }
                        usedPartsManager.GenerateParts();
                        _onlyDNBParts = !_onlyDNBParts;
                        LogService.Instance.WriteToLog("Toggled Used Parts Market");
                    }
                    var partsPage = shopWindow.partsPage;
                    if (partsPage != null)
                    {
                        if (partsPage.ShopType == ShopType25.UsedParts)
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
            if (Input.GetKeyDown(_configFile.AddMoney))
            {
                // Check if a window is open and show the user a message to close it first.
                if (Singleton<WindowManager>.Instance.activeWindows.count > 0)
                {
                    Singleton<UIManager>.Instance.ShowPopup($"Please close any open windows first.", PopupType.Normal);
                }
                else
                {
                    // Get a reference to the Shared Game Data Manager.
                    var sharedGameDataManager = SharedGameDataManager.Instance;
#if DEBUG
                    LogService.Instance.WriteToLog($"Original Money: {sharedGameDataManager.money}");
#endif
                    // Adds the money amount in the config file to the player.
                    sharedGameDataManager.AddMoneyRpc(_configFile.MoneyAmount);
#if DEBUG
                    LogService.Instance.WriteToLog($"New Money: {sharedGameDataManager.money}");
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
            //            var inputFields = GameObject.FindObjectsOfType<InputField>();
            //            foreach (var inputField in inputFields)
            //            {
            //                if (inputField != null)
            //                {
            //                    if (inputField.isFocused)
            //                    {
            //#if DEBUG
            //                        MelonLogger.Msg("Input Field Focused");
            //#endif
            //                        return true;
            //                    }
            //                }
            //            }
            //var activeGO = EventSystem.current.currentSelectedGameObject;
            //if (activeGO != null)
            //{
            //    var inputField = activeGO.GetComponent<InputField>();
            //    if (inputField != null)
            //    {
            //        MelonLogger.Msg("GO InputField Active");
            //        return true;
            //    }
            //}
            //var activeInput = EventSystem.current.currentInputModule;
            //if (activeInput != null)
            //{
            //    var inputField = activeInput.GetComponent<InputField>();
            //    if (inputField != null)
            //    {
            //        MelonLogger.Msg("IM InputField Active");
            //        return true;
            //    }
            //}
            //var gameManager = GameManager.Instance;
            //var inputManager = gameManager.InputManager;
            //if (inputManager == null)
            //{
            //    if (inputManager.IsUICommonEnabled())
            //    {
            //        MelonLogger.Msg("UI Common Input Enabled");
            //        var uiCommonActions = inputManager.GetUICommon();
            //        if (uiCommonActions != null) {
            //            MelonLogger.Msg("UI Common Actions Enabled");
            //        }
            //    }
            //}

            return false;
        }
    }

    /// <summary>
    /// A service to allow for debug logging.
    /// </summary>
    public class LogService
    {
        // A static reference to the service.
        private static LogService _instance;
        public static LogService Instance => _instance ?? (_instance = new LogService());

        // The path to the log file.
        private string _logFilePath = string.Empty;
        // A list of strings to write to the log file.
        private readonly List<string> _logs = new List<string>();
        // A public reference to the log count.
        // This will be used when the mod closes to write any pending logs.
        public int LogCount => _logs.Count;

        public void Initialize(string fileName)
        {
            // Create a DateTime string to log.
            string logDate = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            // Create the log file path string.
            _logFilePath = $"{Directory.GetCurrentDirectory()}\\Mods\\{fileName}.log";
            // Create the log file and write some initial information to it.
            // Example:
            // {fileName} - 1.0.0
            // CMS 2026 Demo - 1.0.1
            // Log Created: 01-01-2024 00:00:01
            File.WriteAllLines(_logFilePath, new List<string> { $"{BuildInfo.Name} - {BuildInfo.Version}", $"CMS 2026 Demo - {Il2Cpp.GameSettings.BuildVersion}", $"Log Created: {logDate}" });
        }

        /// <summary>
        /// Method to write a string to the log file.
        /// </summary>
        /// <param name="message">A string with the message to log.</param>
        /// <param name="callerName">The method that the log is being created.</param>
        public void WriteToLog(string message, [CallerMemberName] string callerName = "")
        {
            // Create the log string with DateTime, Calling Method and Message string.
            var logString = $"{DateTime.Now:HH:mm:ss}\t{callerName}\t{message}.";
            // Add the string to the list of messsage.
            // This is done in case the file cannot be written to (it is not async).
            _logs.Add(logString);
            // Check that the log string is not empty.
            if (!string.IsNullOrWhiteSpace(_logFilePath))
            {
                // Check that the log file exists.
                if (File.Exists(_logFilePath))
                {
                    // Try to append the log strings to the log file
                    // and then clear the log string list.
                    try
                    {
                        File.AppendAllLines(_logFilePath, _logs);
                        _logs.Clear();
                    }
                    catch (Exception)
                    {
                        // The strings could not be written to the file.
                        // This is usually caused by a lock on the file
                        // (if it is currently being written to).
                        // Add them to the list and they will be written next time.
                        _logs.Add($"{DateTime.Now:HH:mm:ss}\tLogService.WriteToLog: Unable to write to log.");
                        _logs.Add(logString);
                    }
                }
                else
                {
                    // The log file was not found.
                    // This should not happen.
                    _logs.Add($"{DateTime.Now:HH:mm:ss}\tLogService.WriteToLog: Log file not found.");
                    _logs.Add(logString);
                }
            }
            else
            {
                // The log file path was empty.
                // This should not happen.
                _logs.Add($"{DateTime.Now:HH:mm:ss}\tLogService.WriteToLog: Log file not initialized.");
                _logs.Add(logString);
            }
#if DEBUG
            MelonLogger.Msg(message);
#endif
        }
    }
}
