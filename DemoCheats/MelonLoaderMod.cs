using Il2Cpp;
using Il2CppCMS.Core;
using Il2CppCMS.DevTools.QC;
using Il2CppCMS.Player.Controller;
using Il2CppCMS.Scenes.Loader;
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
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DemoCheats
{
    public static class BuildInfo
    {
#if DEBUG
        public const string Name = "Demo Cheats (DEBUG)";
#else
        public const string Name = "Demo Cheats";
#endif
        public const string Description = "A collection of cheats for the Car Mechanic Simulator 2026 Demo.";
        public const string Author = "mannly82";
        public const string Company = "The Mann Design";
#if DEBUG
        public const string Version = "1.3.99";
#else
        public const string Version = "1.3.0";
#endif
        public const string DownloadLink = "https://www.nexusmods.com/carmechanicsimulator2026/mods/6";
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
        /// User setting for the key to open the cheat menu.
        /// </summary>
        public KeyCode OpenCheatMenu => _openCheatMenu.Value;
        private readonly MelonPreferences_Entry<KeyCode> _openCheatMenu;
        /// <summary>
        /// User setting for the key to paint the car that the user is looking at.
        /// </summary>
        public KeyCode PaintCar => _paintCar.Value;
        private readonly MelonPreferences_Entry<KeyCode> _paintCar;
        /// <summary>
        /// User setting for the key to repair the body parts to 100%.
        /// </summary>
        public KeyCode RepairBody => _repairBody.Value;
        private readonly MelonPreferences_Entry<KeyCode> _repairBody;
        /// <summary>
        /// User setting for the amount of money to add to the player.
        /// </summary>
        public int MoneyAmount
        {
            get => _moneyAmount.Value;
            set { _moneyAmount.Value = value; }
        }
        private readonly MelonPreferences_Entry<int> _moneyAmount;
        /// <summary>
        /// User setting for the amount of XP to add to the player.
        /// </summary>
        public int XPAmount
        {
            get => _xpAmount.Value;
            set { _xpAmount.Value = value; }
        }
        private readonly MelonPreferences_Entry<int> _xpAmount;
        /// <summary>
        /// User setting for game speed adjustment rate.
        /// </summary>
        public float GameSpeedRate
        {
            get => _gameSpeedRate.Value;
            set { _gameSpeedRate.Value = value; }
        }
        private readonly MelonPreferences_Entry<float> _gameSpeedRate;
        /// <summary>
        /// User setting for the spawning of cars with random part condition.
        /// </summary>
        public bool RandomPartConditionOnSpawn
        {
            get => _randomPartConditionOnSpawn.Value;
            set { _randomPartConditionOnSpawn.Value = value; }
        }
        private readonly MelonPreferences_Entry<bool> _randomPartConditionOnSpawn;

        /// <summary>
        /// Setting to save Game Speed between sessions.
        /// </summary>
        public float GameSpeed
        {
            get => _gameSpeed.Value;
            set { _gameSpeed.Value = value; }
        }
        private readonly MelonPreferences_Entry<float> _gameSpeed;
        /// <summary>
        /// Setting to save Player Walk Speed between sessions.
        /// </summary>
        public float WalkSpeed
        {
            get => _walkSpeed.Value;
            set { _walkSpeed.Value = value; }
        }
        private readonly MelonPreferences_Entry<float> _walkSpeed;
        /// <summary>
        /// Setting to save Player Run Speed between sessions.
        /// </summary>
        public float RunSpeed
        {
            get => _runSpeed.Value;
            set { _runSpeed.Value = value; }
        }
        private readonly MelonPreferences_Entry<float> _runSpeed;
        /// <summary>
        /// Setting to save Fast Mount between sessions.
        /// </summary>
        public bool FastMount
        {
            get => _fastMount.Value;
            set { _fastMount.Value = value; }
        }
        private readonly MelonPreferences_Entry<bool> _fastMount;
        /// <summary>
        /// Setting to save Unlimited Inventory between sessions.
        /// </summary>
        public bool UnlimitedInventory
        {
            get => _unlimitedInventory.Value;
            set { _unlimitedInventory.Value = value; }
        }
        private readonly MelonPreferences_Entry<bool> _unlimitedInventory;

        // The following settings are for the TransferAll additions.
        #region TransferAll
        /// <summary>
        /// User setting for the key to transfer the Inventory/Warehouse/Junk items.
        /// </summary>
        public KeyCode TransferAllItems => _transferAllItems.Value;
        public readonly MelonPreferences_Entry<KeyCode> _transferAllItems;
        /// <summary>
        /// User setting for the key to set the condition of parts percentage lower by 10%.
        /// </summary>
        public KeyCode SetPartConditionLower => _setPartConditionLower.Value;
        public readonly MelonPreferences_Entry<KeyCode> _setPartConditionLower;
        /// <summary>
        /// Method to change the MinPartCondition setting lower by 10%.
        /// </summary>
        public void SetPartConditionLowerBy10()
        {
            MinPartCondition -= 10;
            if (MinPartCondition < 0)
            {
                MinPartCondition = 0;
            }
#if DEBUG
            LogService.Instance.WriteToLog($"Called");
#endif
        }
        /// <summary>
        /// User setting for the key to set the condition of parts percentage higher by 10%.
        /// </summary>
        public KeyCode SetPartConditionHigher => _setPartConditionHigher.Value;
        public readonly MelonPreferences_Entry<KeyCode> _setPartConditionHigher;
        /// <summary>
        /// Method to change the MinPartCondition setting higher by 10%.
        /// </summary>
        public void SetPartConditionHigherBy10()
        {
            MinPartCondition += 10;
            if (MinPartCondition > 100)
            {
                MinPartCondition = 100;
            }
#if DEBUG
            LogService.Instance.WriteToLog($"Called");
#endif
        }
        /// <summary>
        /// User setting for the minimum part condition to transfer.
        /// </summary>
        public int MinPartCondition
        {
            get => _minPartCondition.Value;
            set { _minPartCondition.Value = value; }
        }
        public readonly MelonPreferences_Entry<int> _minPartCondition;
        /// <summary>
        /// User setting for the minimum number of items to show a warning message before transfer.
        /// </summary>
        public int MinNumOfItemsWarning => _minNumOfItemsWarning.Value;
        public readonly MelonPreferences_Entry<int> _minNumOfItemsWarning;
        #endregion

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

            _openCheatMenu = _settings.CreateEntry(nameof(OpenCheatMenu), KeyCode.R,
                description: "https://docs.unity3d.com/ScriptReference/KeyCode.html for KeyCodes.\nPress this key to open the cheat menu. Default: R");
            _paintCar = _settings.CreateEntry(nameof(PaintCar), KeyCode.None,
                description: "Set this to a key to rotate through the factory colors and paint the car you are looking at. Default: None");
            _repairBody = _settings.CreateEntry(nameof(RepairBody), KeyCode.None,
                description: "Set this to a key to repair the body parts to 100%. Default: None");
            _moneyAmount = _settings.CreateEntry(nameof(MoneyAmount), 1000000,
                description: "The amount of money to add to the player. Default: 1000000");
            _xpAmount = _settings.CreateEntry(nameof(XPAmount), 1000,
                description: "The amount of XP to add to the player. Default: 1000");
            _gameSpeedRate = _settings.CreateEntry(nameof(GameSpeedRate), 0.25f,
                description: "Sets the the rate at which the game speed will be adjusted when using the Game Speed-/+ cheats. Default: 0.25f");
            _randomPartConditionOnSpawn = _settings.CreateEntry(nameof(RandomPartConditionOnSpawn), true,
                description: "Sets whether cars will spawn with random condition, false = 100% condition. Default: true");

            _gameSpeed = _settings.CreateEntry(nameof(GameSpeed), 1.0f,
                description: "\nThe following settings are to store game-wide adjustments.\nThey are saved upon exit of the game.\nThis setting changes (and stores) the Game Speed. Default: 1.0");
            _walkSpeed = _settings.CreateEntry(nameof(WalkSpeed), 3.0f,
                description: "This setting changes (and stores) the Player's Max Walk Speed. Default: 3.0");
            _runSpeed = _settings.CreateEntry(nameof(RunSpeed), 5.0f,
                description: "This setting changes (and stores) the Player's Max Run Speed. Default: 5.0");
            _fastMount = _settings.CreateEntry(nameof(FastMount), false,
                description: "This setting changes (and stores) whether Fast Mount Mode is on/off. Default: false");
            _unlimitedInventory = _settings.CreateEntry(nameof(UnlimitedInventory), false,
                description: "This setting changes (and stores) whether the Inventory is unlimited (actually 10,000). Default: false\nThe Warehouse Inventory will incorrectly display an item maximum.");

            // The following settings are for the TransferAll additions.
            #region TransferAll
            _transferAllItems = _settings.CreateEntry(nameof(TransferAllItems), KeyCode.K,
                description: "\nThe following settings are for the TransferAll mod additions.\nPress this key to transfer all current items.");
            _setPartConditionLower = _settings.CreateEntry(nameof(SetPartConditionLower), KeyCode.Minus,
                description: "Press this key to set the condition percentage lower by 10%. Default: -\nThis value is saved to MinPartCondition value below on exit.");
            _setPartConditionHigher = _settings.CreateEntry(nameof(SetPartConditionHigher), KeyCode.Equals,
                description: "Press this key to set the condition percentage higher by 10%. Default: =\nThis value is saved to MinPartCondition value below on exit.");
            _minPartCondition = _settings.CreateEntry(nameof(MinPartCondition), 0,
                description: "This value will transfer parts with a condition equal to and above this number. Default: 0\nSET TO 0 TO TRANSFER EVERYTHING.");
            _minNumOfItemsWarning = _settings.CreateEntry(nameof(MinNumOfItemsWarning), 201,
                description: "This will display a warning if the items to move are above this number. Default: 201\nThis prevents large moves by accident.");
            #endregion

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
            LogService.Instance.WriteToLog($"AddMoney: {OpenCheatMenu}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"Paint Car: {PaintCar}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"Repair Body: {RepairBody}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"MoneyAmount: {MoneyAmount}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"XPAmount: {XPAmount}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"GameSpeedRate: {GameSpeedRate}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"RandomPartConditionOnSpawn: {RandomPartConditionOnSpawn}", "ConfigFile.Init");

            LogService.Instance.WriteToLog($"GameSpeed: {GameSpeed}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"WalkSpeed: {WalkSpeed}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"RunSpeed: {RunSpeed}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"FastMount: {FastMount}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"Unlimited Inventory: {UnlimitedInventory}", "ConfigFile.Init");

            // The following settings are for the TransferAll additions.
            #region TransferAll
            LogService.Instance.WriteToLog($"TransferAllItems: {TransferAllItems}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"SetPartConditionLower: {SetPartConditionLower}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"SetPartConditionHigher: {SetPartConditionHigher}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"MinPartCondition: {MinPartCondition}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"MinNumOfItemsWarning: {MinNumOfItemsWarning}", "ConfigFile.Init");
            #endregion
#endif
        }
    }

    public class DemoCheats : MelonMod
    {
        /// <summary>
        /// Reference to Settings file.
        /// </summary>
        private static DemoCheatsConfigFile _configFile;
        public static DemoCheatsConfigFile GetConfigFile() { return _configFile; }

        /// <summary>
        /// Global reference to the current scene.
        /// </summary>
        private static string _currentScene = string.Empty;
        public static string GetCurrentScene() { return _currentScene; }

        private InputAction _enterAction;
        private InputAction _enterHoldAction;
        private InputAction _exitAction;

        private readonly Dictionary<string, string> _partPrices = new Dictionary<string, string>();

        public override void OnInitializeMelon()
        {
            // Tell the user a log file was created.
            MelonLogger.Msg("Creating Log File...");
            LogService.Instance.Initialize("DemoCheats");
            // Tell the user that we're loading the Settings.
            MelonLogger.Msg("Loading Settings...");
            _configFile = new DemoCheatsConfigFile();
        }

        private void GetBoundKeys()
        {
            var asset = Resources.FindObjectsOfTypeAll<InputActionAsset>()[0];
            _enterAction = asset.FindAction("UI Common/Accept", false);
            _enterHoldAction = asset.FindAction("UI Common/Accept Hold", false);
            _exitAction = asset.FindAction("UI Common/Cancel", false);
#if DEBUG
            LogService.Instance.WriteToLog($"Enter Action: {_enterAction.GetBindingDisplayString()}");
            LogService.Instance.WriteToLog($"Enter Hold Action: {_enterHoldAction.GetBindingDisplayString()}");
            LogService.Instance.WriteToLog($"Exit Action: {_exitAction.GetBindingDisplayString()}");
#endif
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
                                Helpers.DemoWallsGO = target;
                                break;
                            case "Garage_Exterior_Demo_Collider":
                                Helpers.Garage_Exterior_Demo_ColliderGO = target;
                                break;
                            case "Garage_Exterior_Demo_Wall_Blocked_1":
                                Helpers.Garage_Exterior_Demo_Wall_Blocked_1GO = target;
                                break;
                            case "DemoVehiclesDetector":
                                Helpers.DemoVehiclesDetectorGO = target;
                                break;
                        }
                    }
#if DEBUG
                    else
                    {
                        LogService.Instance.WriteToLog($"Could not find GameObject with name: {name}");
                    }
#endif
                }
                GetBoundKeys();

                System.Collections.IEnumerator WaitForSceneLoad()
                {
                    yield return new WaitForSeconds(1f);
                    while (SceneLoader.Instance.loadingScene)
                        yield return new WaitForSeconds(1);

                    GameObject playerControllerGO =
                        Resources.FindObjectsOfTypeAll<GameObject>()
                        .FirstOrDefault(go => go.name == "Player Controller (1)");
                    if (playerControllerGO != null)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"GameSpeed Before: {Time.timeScale}"); 
#endif
                        if (Time.timeScale != _configFile.GameSpeed)
                        {
                            Time.timeScale = _configFile.GameSpeed;
#if DEBUG
                            LogService.Instance.WriteToLog($"GameSpeed After: {Time.timeScale}"); 
#endif
                        }
                        PlayerMovement playerMovement = playerControllerGO.GetComponent<PlayerMovement>();
#if DEBUG
                        LogService.Instance.WriteToLog($"WalkSpeed Before: {playerMovement.settings.MaxWalkingSpeed}"); 
#endif
                        if (playerMovement.settings.MaxWalkingSpeed != _configFile.WalkSpeed)
                        {
                            playerMovement.settings.MaxWalkingSpeed = _configFile.WalkSpeed;
#if DEBUG
                            LogService.Instance.WriteToLog($"WalkSpeed After: {playerMovement.settings.MaxWalkingSpeed}"); 
#endif
                        }
#if DEBUG
                        LogService.Instance.WriteToLog($"RunSpeed Before: {playerMovement.settings.MaxRunningSpeed}"); 
#endif
                        if (playerMovement.settings.MaxRunningSpeed != _configFile.RunSpeed)
                        {
                            playerMovement.settings.MaxRunningSpeed = _configFile.RunSpeed;
#if DEBUG
                            LogService.Instance.WriteToLog($"RunSpeed After: {playerMovement.settings.MaxRunningSpeed}"); 
#endif
                        }
#if DEBUG
                        LogService.Instance.WriteToLog($"FastMount Before: {DevSettings.FastMountMode}"); 
#endif
                        if (DevSettings.FastMountMode != _configFile.FastMount)
                        {
                            DevSettings.FastMountMode = _configFile.FastMount;
#if DEBUG
                            LogService.Instance.WriteToLog($"FastMount After: {DevSettings.FastMountMode}"); 
#endif
                        }
#if DEBUG
                        LogService.Instance.WriteToLog($"Unlimited Inventory Before: {InventoryCommands.UnlimitedInventory}"); 
#endif
                        if (InventoryCommands.UnlimitedInventory != _configFile.UnlimitedInventory)
                        {
                            InventoryCommands.UnlimitedInventory = _configFile.UnlimitedInventory;
#if DEBUG
                            LogService.Instance.WriteToLog($"Unlimited Inventory After: {InventoryCommands.UnlimitedInventory}");
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                        LogService.Instance.WriteToLog("Player Controller null");
#endif
                    }

                    yield return null;
                }
                MelonCoroutines.Start(WaitForSceneLoad());
            }
        }
        public override void OnDeinitializeMelon()
        {
#if DEBUG
            LogService.Instance.WriteToLog($"Called");
#endif
            // Verify all the log strings have been written.
            if (LogService.Instance.LogCount > 0)
            {
                LogService.Instance.WriteToLog("Mod Deinitializing with pending log strings");
            }
        }

        public override void OnUpdate()
        {
#if DEBUG
            if (_enterAction != null)
            {
                if (_enterAction.WasReleasedThisFrame())
                {
                    LogService.Instance.WriteToLog($"{_enterAction.GetBindingDisplayString().Split('|')[0].Trim()} was released");
                }
            }
            if (_exitAction != null)
            {
                if (_exitAction.WasReleasedThisFrame())
                {
                    LogService.Instance.WriteToLog($"{_exitAction.GetBindingDisplayString().Split('|')[0].Trim()} was released");
                } 
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (!CheckIfInputIsFocused())
                {
                    //var gameManager = GameManager.Instance;
                    //if (gameManager == null)
                    //    return;
                    //var inputManager = gameManager.inputManager;
                    //if (inputManager != null)
                    //{
                    //    // This is only enabled when the user is driving a car.
                    //    // Might come in handy later.
                    //    // GameMode.Get().currentMode == gameMode.CarDrive is another way to tell.
                    //    //LogService.Instance.WriteToLog($"IsCarEnabled: {inputManager.IsCarEnabled()}");
                    //}
                    //LogService.Instance.WriteToLog($"Game Mode: {GameMode.Get().currentMode}");
                    //// Get the GameObject being looked at.
                    //GameObject goInView = GameScript.Get().GetIOMouseOverGO();
                    //// Get the car being looked at.
                    //CarLoader carInView = GameScript.Get().GetIOMouseOverCarLoader2();
                    //// Get the part being looked at.
                    //CarPart carPart = GameScript.Get().GetIOMouseOverCarLoader();
                    //// If the user is looking at a car and a part, paint the car.
                    //// The game stores the last car looked at sometimes.
                    //// If the car is not null but the part is, the user isn't looking at a car.
                    //if (carInView != null && carPart != null)
                    //{
                    //    LogService.Instance.WriteToLog($"GameObject: {goInView.name}");
                    //    LogService.Instance.WriteToLog($"Car Loader: {carInView.name}");
                    //    LogService.Instance.WriteToLog($"Car Part: {carPart.name}");
                    //}
                    //LogService.Instance.WriteToLog($"Active Windows: {WindowManager.Instance.activeWindows.count}");

                    // If the user upgrades a skill, the UnlockAllSkills() cheat doesn't work.
                    //var skillUpgrades = Resources.FindObjectsOfTypeAll<SkillUpgradeItem>();
                    //LogService.Instance.WriteToLog($"Skill Count: {skillUpgrades.Count}");
                    //AdditionalRequirement additionalRequirement;
                    //foreach (var skillUpgrade in skillUpgrades)
                    //{
                    //    switch (skillUpgrade.AssignedSkillID)
                    //    {
                    //        case "fast_mount":
                    //            LogService.Instance.WriteToLog("-- Fast Mount --");
                    //            LogService.Instance.WriteToLog($"Fast Mount can unlock: {skillUpgrade.canUnlock}");
                    //            additionalRequirement = skillUpgrade.additionalRequirement;
                    //            LogService.Instance.WriteToLog($"Additional Requirement: {additionalRequirement.requirementText.text}");
                    //            LogService.Instance.WriteToLog($"Requirement Fullfilled: {additionalRequirement.isFulfilled}");
                    //            LogService.Instance.WriteToLog($"Current Value: {additionalRequirement.currentValue}");
                    //            LogService.Instance.WriteToLog($"Required Value: {additionalRequirement.requiredValue}");
                    //            LogService.Instance.WriteToLog("-- Fast Mount --");
                    //            break;
                    //        case "fast_partremove":
                    //            LogService.Instance.WriteToLog("-- Fast Part Remove --");
                    //            LogService.Instance.WriteToLog($"Fast Part Remove can unlock: {skillUpgrade.canUnlock}");
                    //            additionalRequirement = skillUpgrade.additionalRequirement;
                    //            LogService.Instance.WriteToLog($"Additional Requirement: {additionalRequirement.requirementText.text}");
                    //            LogService.Instance.WriteToLog($"Requirement Fullfilled: {additionalRequirement.isFulfilled}");
                    //            LogService.Instance.WriteToLog($"Current Value: {additionalRequirement.currentValue}");
                    //            LogService.Instance.WriteToLog($"Required Value: {additionalRequirement.requiredValue}");
                    //            LogService.Instance.WriteToLog("-- Fast Part Remove --");
                    //            break;
                    //        case "inventory_capacity":
                    //            LogService.Instance.WriteToLog("-- Inventory Capacity --");
                    //            LogService.Instance.WriteToLog($"Inventory Capacity can unlock: {skillUpgrade.canUnlock}");
                    //            additionalRequirement = skillUpgrade.additionalRequirement;
                    //            LogService.Instance.WriteToLog($"Additional Requirement: {additionalRequirement.requirementText.text}");
                    //            LogService.Instance.WriteToLog($"Requirement Fullfilled: {additionalRequirement.isFulfilled}");
                    //            LogService.Instance.WriteToLog($"Current Value: {additionalRequirement.currentValue}");
                    //            LogService.Instance.WriteToLog($"Required Value: {additionalRequirement.requiredValue}");
                    //            LogService.Instance.WriteToLog("-- Inventory Capacity --");
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}

                    // This adds coffee to the player.
                    // They can buy the coffee machine to do this too.
                    //PlayerCommands.ActivatePowerUp();

                    // This prints the active controls being shown at the bottom of the Shop window.
                    //var windowManager = WindowManager.Instance;
                    //if (windowManager == null)
                    //    return;

                    //if (windowManager.activeWindows.count > 0)
                    //{
                    //    if (windowManager.IsWindowActive(WindowID.Shop))
                    //    {
                    //        var shopWindow = windowManager.GetWindowByID<ShopWindow25>(WindowID.Shop);
                    //        if (shopWindow != null)
                    //        {
                    //            var descriptionManager = GameObject.FindFirstObjectByType<UIDescriptionManager>();
                    //            if (descriptionManager != null)
                    //            {
                    //                var shop = descriptionManager.GetComponentInChildren<UIDescription>();
                    //                if (shop != null)
                    //                {
                    //                    foreach (var description in shop.descriptions)
                    //                    {
                    //                        if (description.isActive)
                    //                        {
                    //                            LogService.Instance.WriteToLog($"Action: {description.ActionName}: {description.InputAction.GetBindingDisplayString()}");
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
#endif
            if (_configFile == null) return;

            if (Input.GetKeyDown(_configFile.OpenCheatMenu))
            {
                if (_currentScene.Equals("garage"))
                {
                    if (GameMode.Get().currentMode == gameMode.CarDrive)
                    {
                        UIManager.Get().ShowPopup("Please exit the vehicle first.", PopupType.Normal);
                        LogService.Instance.WriteToLog("Player is inside a vehicle.");
                        return;
                    }
                    var windowManager = WindowManager.Instance;
                    if (windowManager == null)
                        return;

                    if (windowManager.activeWindows.count > 0)
                    {
                        if (windowManager.IsWindowActive(WindowID.Shop))
                        {
                            var shopWindow = windowManager.GetWindowByID<ShopWindow25>(WindowID.Shop);
                            if (shopWindow != null)
                            {
                                var partsPage = shopWindow.partsPage;
                                if (partsPage != null && partsPage.ShopType == ShopType25.UsedParts)
                                {
                                    if (!CheckIfInputIsFocused())
                                    {
                                        DemoCheatMenu.ToggleCheatMenu();
                                    }
                                }
                            }
                        }
#if DEBUG
                        else
                        {
                            var activeWindows = windowManager.activeWindows;
                            LogService.Instance.WriteToLog($"Active Window: {activeWindows.head.Value}");
                        }
#endif
                    }
                    else
                    {
                        DemoCheatMenu.ToggleCheatMenu();
                    } 
                }
            }

            if (Input.GetKeyDown(_configFile.PaintCar) &&
                _configFile.PaintCar != KeyCode.None)
            {
                if (_currentScene.Equals("garage"))
                {
                    // Check that the user isn't typing in an InputField.
                    if (!CheckIfInputIsFocused())
                    {
                        Helpers.CyclePaintColors();
                    }
                }
            }

            if (Input.GetKey(_configFile.RepairBody) &&
                _configFile.RepairBody != KeyCode.None)
            {
                if (_currentScene.Equals("garage"))
                {
                    // Check that the user isn't typing in an InputField.
                    if (!CheckIfInputIsFocused())
                    {
                        Helpers.RepairCar();
                    }
                }
            }

            // The following settings are for the TransferAll additions.
            #region TransferAll
            // This key will set the MinPartCondition value lower by 10%
            // and then show the user the current value.
            if (Input.GetKeyUp(_configFile.SetPartConditionLower))
            {
                if (!CheckIfInputIsFocused())
                {
                    _configFile.SetPartConditionLowerBy10();
                    // The standard Popup doesn't show over the UI,
                    // so show an Info Window if a UIWindow is displayed.
                    if (WindowManager.Instance.activeWindows.count > 0)
                    {
                        UIManager.Get().ShowInfoWindow($"Transfer Part Condition: {_configFile.MinPartCondition}%", false);
                    }
                    else
                    {
                        UIManager.Get().ShowPopup($"Transfer Part Condition: {_configFile.MinPartCondition}%", PopupType.Normal);
                    }
                    LogService.Instance.WriteToLog($"Transfer Part Condition: {_configFile.MinPartCondition}%");
                }
#if DEBUG

                else
                {
                    LogService.Instance.WriteToLog("Search box has focus");
                }
#endif
            }
            // This key will set the MinPartCondition value higher by 10%
            // and then show the user the current value.
            if (Input.GetKeyUp(_configFile.SetPartConditionHigher))
            {
                if (!CheckIfInputIsFocused())
                {
                    _configFile.SetPartConditionHigherBy10();
                    // The standard Popup doesn't show over the UI,
                    // so show an Info Window if a UIWindow is displayed.
                    if (WindowManager.Instance.activeWindows.count > 0)
                    {
                        UIManager.Get().ShowInfoWindow($"Transfer Part Condition: {_configFile.MinPartCondition}%", false);
                    }
                    else
                    {
                        UIManager.Get().ShowPopup($"Transfer Part Condition: {_configFile.MinPartCondition}%", PopupType.Normal);
                    }
                    LogService.Instance.WriteToLog($"Transfer Part Condition: {_configFile.MinPartCondition}%");
                }
#if DEBUG
                else
                {
                    LogService.Instance.WriteToLog("Search box has focus");
                }
#endif
            }

            // Check if the user pressed the TransferAllItems Key in Settings.
            if (Input.GetKeyUp(_configFile.TransferAllItems))
            {
#if DEBUG
                LogService.Instance.WriteToLog($"TransferAllItems ({_configFile.TransferAllItems}) key pressed");
#endif
                // Check if the user is currently using the Search box.
                if (!CheckIfInputIsFocused())
                {
                    // Check that the user is in the Garage.
                    if (_currentScene.Equals("garage"))
                    {
                        // Do work on the Inventory or Warehouse.
                        TransferAll.MoveInventoryOrWarehouseItems();
                    }
                }
#if DEBUG
                else
                {
                    LogService.Instance.WriteToLog("Search box has focus");
                }
#endif
            }
            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/manual/ActionBindings.html#working-with-bindings
            if (_enterHoldAction != null)
            {
                if (_enterHoldAction.WasPerformedThisFrame())
                {
#if DEBUG
                    LogService.Instance.WriteToLog($"{_enterHoldAction.GetBindingDisplayString().Split('|')[0].Trim()} was performed");
#endif

                    // Move all items in the Shopping List to the Cart.
                    MelonCoroutines.Start(TransferAll.MoveShoppingListToCart());
                }
            }
            #endregion
        }

        public override void OnGUI()
        {
            DemoCheatMenu.DrawCheatMenu();
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
            var evt = EventSystem.current;
            if (evt != null)
            {
                var activeGO = evt.currentSelectedGameObject;
                if (activeGO != null)
                {
                    // Check for legacy InputField
                    if (activeGO.GetComponent<InputField>() != null)
                    {
                        if (activeGO.GetComponent<InputField>()?.isFocused == true)
                        {
                            return true;
                        }
                    }

                    // If TMP is used, it often exposes TMP_InputField type; do a safer type-name check
                    var tmpComp = activeGO.GetComponent("TMP_InputField");
                    if (tmpComp != null)
                    {
                        // assume TMP input is focused when selected
                        return true;
                    }
                }
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
                LogService.Instance.WriteToLog($"LogService.Initialize failed: {ex.Message}");
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
            if (!message.Contains("Called"))
            {
                MelonLogger.Msg(message);
            }
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
                        File.WriteAllLines(_logFilePath, new[] { $"{BuildInfo.Name} - {BuildInfo.Version}", $"CMS 2026 Demo - {GameSettings.BuildVersion}", $"Log Created: {DateTime.Now:MM-dd-yyyy HH:mm:ss}" });
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
