using Il2Cpp;
using Il2CppCMS.Core;
using Il2CppCMS.DevTools;
using Il2CppCMS.UI;
using Il2CppCMS.UI.Description;
using Il2CppCMS.UI.Helpers;
using Il2CppCMS.UI.Logic;
using Il2CppCMS.UI.Logic.Shop.Cart.Containers;
using Il2CppCMS.UI.Logic.Shop.List_.Containers;
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
        public const string Name = "Demo Cheats";
        public const string Description = "A collection of cheats for the Car Mechanic Simulator 2026 Demo.";
        public const string Author = "mannly82";
        public const string Company = "The Mann Design";
        public const string Version = "1.3.0";
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
                description: "Press this key to open the cheat menu.");
            _paintCar = _settings.CreateEntry(nameof(PaintCar), KeyCode.None,
                description: "Set this to a key to rotate through the factory colors and paint the car you are looking at");
            _repairBody = _settings.CreateEntry(nameof(RepairBody), KeyCode.None,
                description: "Set this to a key to repair the body parts to 100%");
            _moneyAmount = _settings.CreateEntry(nameof(MoneyAmount), 1000000,
                description: "The amount of money to add to the player.");
            _xpAmount = _settings.CreateEntry(nameof(XPAmount), 1000,
                description: "The amount of XP to add to the player.");
            _gameSpeedRate = _settings.CreateEntry(nameof(GameSpeedRate), 0.25f,
                description: "Sets the the rate at which the game speed will be adjusted when using the Game Speed-/+ cheats.");
            _randomPartConditionOnSpawn = _settings.CreateEntry(nameof(RandomPartConditionOnSpawn), true,
                description: "Sets whether cars will spawn with random condition, false = 100% condition.");

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
            LogService.Instance.WriteToLog($"Paint Car: {PaintCar}");
            LogService.Instance.WriteToLog($"Repair Body: {RepairBody}");
            LogService.Instance.WriteToLog($"MoneyAmount: {MoneyAmount}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"XPAmount: {XPAmount}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"GameSpeedRate: {GameSpeedRate}", "ConfigFile.Init");
            LogService.Instance.WriteToLog($"RandomPartConditionOnSpawn: {RandomPartConditionOnSpawn}", "ConfigFile.Init");
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

        private Dictionary<string, string> _partPrices = new Dictionary<string, string>();

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
                        LogService.Instance.WriteToLog($"Could not find GameObject with name {name}");
                    }
#endif
                }
                GetBoundKeys();
            }
        }

        private System.Collections.IEnumerator MoveShoppingListToCart()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null)
                yield return null;

            if (windowManager.activeWindows.count > 0)
            {
                if (windowManager.IsWindowActive(WindowID.Shop))
                {
                    var shopWindow = windowManager.GetWindowByID<ShopWindow25>(WindowID.Shop);
                    if (shopWindow != null)
                    {
                        var partsPage = shopWindow.partsPage;
                        if (partsPage != null)
                        {
                            var shopListManager = Singleton<GameManager>.Instance.shopListManager;
                            var slItems = shopListManager.listItems;
                            var shopCartManager = Singleton<GameManager>.Instance.ShopCartManager;
                            var scItems = shopCartManager.cartItems;

                            if (partsPage.isActiveAndEnabled)
                            {
                                if (slItems?.Count > 0)
                                {
                                    Il2CppSystem.Collections.Generic.List<IShopListItem> tempList = new Il2CppSystem.Collections.Generic.List<IShopListItem>();
                                    foreach (var slItem in slItems)
                                    {
                                        if (slItem.ShopType == partsPage.ShopType)
                                        {
                                            //LogService.Instance.WriteToLog($"Part ID: {slItem.ItemID}");
                                            //LogService.Instance.WriteToLog($"Part Name: {GameInventory.Instance.GetLocalizedName(slItem.ItemID)}");
                                            var gameItem = GameInventory.Instance.GetItemProperty(slItem.ItemID);
                                            if (partsPage.ShopType == ShopType25.LicensePlates)
                                            {
                                                ShopListLicensePlate slLP = slItem.TryCast<ShopListLicensePlate>();
                                                if (slLP != null)
                                                {
                                                    var licenseName = ItemHelper.ParseLicensePlateName(slLP.Name);
                                                    int price = Helper.GetPrice(TestScript.Method_Internal_Static_IBaseItem_LicensePlate_0(
                                                    Singleton<GameInventory>.Instance.LicensePlatesProvider.GetRandomLicensePlate()));
#if DEBUG
                                                    LogService.Instance.WriteToLog($"Parsed Part Name: {licenseName}");
                                                    LogService.Instance.WriteToLog($"License Plate Price: ${price}");
#endif
                                                    ShopCartLicensePlate scLP = new ShopCartLicensePlate()
                                                    {
                                                        Amount = slLP.Amount,
                                                        ItemID = licenseName,
                                                        Price = (uint)price,
                                                        ShopType = slLP.ShopType
                                                    };
                                                    shopCartManager.AddToCart(new IShopCartItem(scLP.Pointer));
                                                }
                                            }
                                            else if (partsPage.ShopType == ShopType25.Wheels)
                                            {
                                                ShopListRim slR = slItem.TryCast<ShopListRim>();
                                                if (slR != null)
                                                {
                                                    ShopCartRim scR = new ShopCartRim()
                                                    {
                                                        Amount = slR.Amount,
                                                        ET = slR.ET,
                                                        ItemID = slR.ItemID,
                                                        Price = gameItem.Price,
                                                        Size = slR.Size,
                                                        ShopType = slR.ShopType
                                                    };
                                                    shopCartManager.AddToCart(new IShopCartItem(scR.Pointer));
                                                }
                                                else
                                                {
                                                    ShopListTire slT = slItem.TryCast<ShopListTire>();
                                                    if (slT != null)
                                                    {
                                                        ShopCartTire scT = new ShopCartTire()
                                                        {
                                                            Amount = slT.Amount,
                                                            ItemID = slT.ItemID,
                                                            Price = gameItem.Price,
                                                            Profile = slT.Profile,
                                                            Size = slT.Size,
                                                            ShopType = slT.ShopType,
                                                            Width = slT.Width
                                                        };
                                                        shopCartManager.AddToCart(new IShopCartItem(scT.Pointer));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ShopCartItem scItem = new ShopCartItem()
                                                {
                                                    Amount = slItem.Amount,
                                                    ItemID = slItem.ItemID,
                                                    Price = gameItem.Price,
                                                    ShopType = slItem.ShopType
                                                };
                                                shopCartManager.AddToCart(new IShopCartItem(scItem.Pointer));
                                            }

                                            tempList.Add(slItem);
                                        }
                                    }
                                    foreach (var tempItem in tempList)
                                    {
                                        partsPage.shopList.RemoveListItem(tempItem);
                                    }
                                    partsPage.UpdateItems();
                                    partsPage.ShopCart.UpdateItems(partsPage.ShopType);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var activeWindows = windowManager.activeWindows;
#if DEBUG
                    LogService.Instance.WriteToLog($"Active Window: {activeWindows.head.Value}");
#endif
                }
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

            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/manual/ActionBindings.html#working-with-bindings
            if (_enterHoldAction != null)
            {
                if (_enterHoldAction.WasPerformedThisFrame())
                {
#if DEBUG
                    LogService.Instance.WriteToLog($"{_enterHoldAction.GetBindingDisplayString().Split('|')[0].Trim()} was performed");
#endif

                    // Move all items in the Shopping List to the Cart.
                    MelonCoroutines.Start(MoveShoppingListToCart());
                }
            }
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
