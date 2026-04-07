using Il2Cpp;
using Il2CppCMS.Core;
using Il2CppCMS.Core.Car;
using Il2CppCMS.Core.Car.Containers;
using Il2CppCMS.DevTools;
using Il2CppCMS.DevTools.QC;
using Il2CppCMS.Player.Controller;
using Il2CppCMS.Player.Skills;
using Il2CppCMS.Shared;
using Il2CppCMS.UI;
using Il2CppCMS.UI.Logic;
using Il2CppCMS.UI.Windows;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DemoCheats
{
    public class Helpers
    {
        /// <summary>
        /// Reference to Settings file.
        /// </summary>
        private static DemoCheatsConfigFile _configFile = DemoCheats.GetConfigFile();

        /// <summary>
        /// Global variable to maintain whether the user is painting a car and the color to paint it.
        /// </summary>
        private static bool _isPainting = false;
        private static int _currentColorIndex = -1;
        private static CarLoader _currentCar;

        private static readonly System.Collections.Generic.List<string> _spawnCarIDs = new System.Collections.Generic.List<string>()
        {
            "car_dnbcensor",
            "car_katagiritamagobp",
            "car_luxorstreamlinermk3",
            "car_mayenm5",
            "car_salemariesmk3"
        };
        private static readonly System.Collections.Generic.List<string> _spawnCarNames = new System.Collections.Generic.List<string>()
        {
            "DNB Censor",
            "Katagiri Tamago BP",
            "Luxor Streamliner Mk3",
            "Mayen M5",
            "Salem Aries MK3"
        };
        private static int _currentSpawnIndex = 0;

        /// <summary>
        /// Variable to store whether the user want to see just DNB Censor parts.
        /// </summary>
        private static bool _onlyDNBParts = false;

        /// <summary>
        /// References to the demo world limit game objects.
        /// </summary>
        private static bool _demoLimitsActive = true;
        public static GameObject DemoWallsGO = null;
        public static GameObject Garage_Exterior_Demo_ColliderGO = null;
        public static GameObject Garage_Exterior_Demo_Wall_Blocked_1GO = null;
        public static GameObject DemoVehiclesDetectorGO = null;

        /// <summary>
        /// Adds the amount of money from the config file to the player.
        /// </summary>
        public static void AddMoney()
        {
            if (_configFile == null)
            {
                LogService.Instance.WriteToLog("Config file missing");
                return;
            }
            // Get a reference to the Shared Game Data Manager.
            var sharedGameDataManager = SharedGameDataManager.Instance;
            // Add the money amount from the config file to the player.
            sharedGameDataManager.AddMoneyRpc(_configFile.MoneyAmount);
            LogService.Instance.WriteToLog($"Added ${_configFile.MoneyAmount:C}");
            UIManager.Get().ShowPopup($"Added ${_configFile.MoneyAmount:C}.", PopupType.Normal);
#if DEBUG
            // TODO: Implement Inventory Resizing
            //LogService.Instance.WriteToLog($"Inv Size Multi: {sharedGameDataManager.inventorySizeMultiplier}");
            //sharedGameDataManager.inventorySizeMultiplier = 5;
            //LogService.Instance.WriteToLog($"New Inv Size Multi: {sharedGameDataManager.inventorySizeMultiplier}");
            // TODO: Implement XP Multiplication
            //LogService.Instance.WriteToLog($"Xp Multi: {sharedGameDataManager.xpMultiplier}");
            //sharedGameDataManager.xpMultiplier = 5;
            //LogService.Instance.WriteToLog($"New Xp Multi: {sharedGameDataManager.xpMultiplier}");
#endif
        }

        /// <summary>
        /// Adds the amount of XP from the config file to the player.
        /// </summary>
        public static void AddXP()
        {
            if (_configFile == null)
            {
                LogService.Instance.WriteToLog("Config file missing");
                return;
            }
            PlayerCommands.AddPlayerExp(_configFile.XPAmount);
            LogService.Instance.WriteToLog($"Added {_configFile.XPAmount} XP");
            UIManager.Get().ShowPopup($"Added {_configFile.XPAmount} XP.", PopupType.Normal);
        }

        /// <summary>
        /// Adjusts the game speed based on the value in the config file.
        /// </summary>
        public static void AdjustGameSpeedUp(bool up)
        {
            if (_configFile == null)
            {
                LogService.Instance.WriteToLog("Config file missing");
                return;
            }
            if (up)
            {
                float newScale = Time.timeScale += _configFile.GameSpeedRate;
                if (newScale > 2.0f)
                {
                    LogService.Instance.WriteToLog("Max Game Speed reached");
                    UIManager.Get().ShowPopup("Max Game Speed reached.", PopupType.Normal);
                    return;
                }
                else
                {
                    Time.timeScale = newScale;
                    LogService.Instance.WriteToLog($"New Game Speed: {newScale}");
                    UIManager.Get().ShowPopup($"New Game Speed: {newScale}.", PopupType.Normal);
                }
            }
            else
            {
                float newScale = Time.timeScale -= _configFile.GameSpeedRate;
                if (newScale < 0.25f)
                {
                    LogService.Instance.WriteToLog("Min Game Speed reached");
                    UIManager.Get().ShowPopup("Min Game Speed reached.", PopupType.Normal);
                    return;
                }
                else
                {
                    Time.timeScale = newScale;
                    LogService.Instance.WriteToLog($"New Game Speed: {newScale}");
                    UIManager.Get().ShowPopup($"New Game Speed: {newScale}.", PopupType.Normal);
                }
            }
        }

        /// <summary>
        /// Adds skill points to the player.
        /// This can't be set in the config file.
        /// </summary>
        public static void AddSkillPoints()
        {
            PlayerSkillSystem playerSkillSystem = Singleton<GameManager>.Instance.PlayerSkillSystem;
            playerSkillSystem.AddPoints();
            LogService.Instance.WriteToLog($"Added 1 skill point");
            UIManager.Get().ShowPopup("Added 1 skill point.", PopupType.Normal);
        }

        public static void MaxSkills()
        {
            Singleton<TestScript>.Instance.UnlockAllSkills();
            LogService.Instance.WriteToLog($"Maxed Player Skills");
            UIManager.Get().ShowPopup("Maxed Player Skills.", PopupType.Normal);
        }

        /// <summary>
        /// Adjusts the Player Walk Speed Up/Down.
        /// </summary>
        /// <param name="up"></param>
        public static void AdjustWalkSpeedUp(bool up)
        {
            GameObject playerControllerGO = 
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "Player Controller (1)");
            PlayerMovement playerMovement = playerControllerGO.GetComponent<PlayerMovement>();
            if (up)
            {
                float newSpeed = playerMovement.settings.MaxWalkingSpeed += 1.0f;
                if (newSpeed > 20)
                {
                    LogService.Instance.WriteToLog("Max Walk Speed reached");
                    UIManager.Get().ShowPopup("Max Walk Speed reached.", PopupType.Normal);
                    return;
                }
                else
                {
                    playerMovement.settings.MaxWalkingSpeed = newSpeed;
                    LogService.Instance.WriteToLog($"New Walk Speed: {newSpeed}");
                    UIManager.Get().ShowPopup($"New Walk Speed: {newSpeed}.", PopupType.Normal);
                }
            }
            else
            {
                float newSpeed = playerMovement.settings.MaxWalkingSpeed -= 1.0f;
                if (newSpeed < 0.0f)
                {
                    LogService.Instance.WriteToLog("Min Walk Speed reached");
                    UIManager.Get().ShowPopup("Min Walk Speed reached.", PopupType.Normal);
                    return;
                }
                else
                {
                    playerMovement.settings.MaxWalkingSpeed = newSpeed;
                    LogService.Instance.WriteToLog($"New Walk Speed: {newSpeed}");
                    UIManager.Get().ShowPopup($"New Walk Speed: {newSpeed}.", PopupType.Normal);
                }
            }
        }

        public static void ToggleFastMount()
        {
            DevSettings.FastMountMode = !DevSettings.FastMountMode;
            LogService.Instance.WriteToLog($"Fast Mounting: {DevSettings.FastMountMode}");
            UIManager.Get().ShowPopup($"Fast Mounting: {DevSettings.FastMountMode}.", PopupType.Normal);
        }

        /// <summary>
        /// Adjusts the Player Running Speed Up/Down
        /// </summary>
        /// <param name="up"></param>
        public static void AdjustRunSpeedUp(bool up)
        {
            GameObject playerControllerGO =
                Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "Player Controller (1)");
            PlayerMovement playerMovement = playerControllerGO.GetComponent<PlayerMovement>();
            if (up)
            {
                float newSpeed = playerMovement.settings.MaxRunningSpeed += 1.0f;
                if (newSpeed > 20)
                {
                    LogService.Instance.WriteToLog("Max Run Speed reached");
                    UIManager.Get().ShowPopup("Max Run Speed reached.", PopupType.Normal);
                    return;
                }
                else
                {
                    playerMovement.settings.MaxRunningSpeed = newSpeed;
                    LogService.Instance.WriteToLog($"New Run Speed: {newSpeed}");
                    UIManager.Get().ShowPopup($"New Run Speed: {newSpeed}.", PopupType.Normal);
                }
            }
            else
            {
                float newSpeed = playerMovement.settings.MaxRunningSpeed -= 1.0f;
                if (newSpeed < 1.0f)
                {
                    LogService.Instance.WriteToLog("Min Run Speed reached");
                    UIManager.Get().ShowPopup("Min Run Speed reached.", PopupType.Normal);
                    return;
                }
                else
                {
                    playerMovement.settings.MaxRunningSpeed = newSpeed;
                    LogService.Instance.WriteToLog($"New Run Speed: {newSpeed}");
                    UIManager.Get().ShowPopup($"New Run Speed: {newSpeed}.", PopupType.Normal);
                }
            }
        }

        /// <summary>
        /// Cycles through the cars available colors.
        /// </summary>
        public static void CyclePaintColors()
        {
            if (!_isPainting)
            {
                // Get the GameObject being looked at.
                GameObject goInView = GameScript.Get().GetIOMouseOverGO();
                // Get the car being looked at.
                CarLoader carInView = GameScript.Get().GetIOMouseOverCarLoader2();
                // Get the part being looked at.
                CarPart carPart = GameScript.Get().GetIOMouseOverCarLoader();
                // If the user is looking at a car and a part, paint the car.
                // The game stores the last car looked at sometimes.
                // If the car is not null but the part is, the user isn't looking at a car.
                if (carInView != null && carPart != null)
                {
                    if (_currentCar == null)
                    {
                        _currentCar = carInView;
                        LogService.Instance.WriteToLog($"Current Car is null, now: {carInView.CarID}");
                    }
                    else if (!_currentCar.CarID.Equals(carInView.CarID))
                    {
                        // A new car is being painted.
                        _currentColorIndex = -1;
                        _currentCar = carInView;
                        LogService.Instance.WriteToLog($"New car being painted: {carInView.CarID}");
                    }

                    _isPainting = true;
#if DEBUG
                    LogService.Instance.WriteToLog("---Started Painting Car---");
                    LogService.Instance.WriteToLog($"Factory Color: {carInView.factoryColor}");
                    LogService.Instance.WriteToLog($"Car In View Current Color: {carInView.CurrentColor}");
#endif
                    // Get the allowed Factory colors for the car.
                    var allowedColors = carInView.AllowedColors;
                    if (allowedColors.Count > 0)
                    {
                        if (_currentColorIndex == -1)
                        {
                            // Set the color index to the first color;
                            _currentColorIndex = 0;
                        }
                        // Select the color from the allowed list.
                        AllowedColor currentColor = allowedColors[_currentColorIndex];
#if DEBUG
                        LogService.Instance.WriteToLog($"Current Color Index: {_currentColorIndex}");
                        LogService.Instance.WriteToLog($"Current Color: {currentColor.Color}");
#endif
                        if (goInView.name.Contains("rim") ||
                            goInView.name.Contains("tire"))
                        {
                            var wheels = carInView.Wheels;
                            wheels.FrontLeftWheelHandle.GetComponent<PartScript>().SetColor(currentColor.Color);
                            wheels.FrontRightWheelHandle.GetComponent<PartScript>().SetColor(currentColor.Color);
                            wheels.RearLeftWheelHandle.GetComponent<PartScript>().SetColor(currentColor.Color);
                            wheels.RearRightWheelHandle.GetComponent<PartScript>().SetColor(currentColor.Color);
                        }
                        //else if (carPart.name.Contains("headlight"))
                        //{
                        //    LogService.Instance.WriteToLog("Looking at headlight");
                        //}
                        //else if (carPart.name.Contains("taillight"))
                        //{
                        //    LogService.Instance.WriteToLog("Looking at taillight");
                        //}
                        //else if (carPart.name.Contains("window"))
                        //{
                        //    LogService.Instance.WriteToLog("Looking at window");
                        //}
                        else
                        {
                            // This extension replaces looping through all the parts and painting them.
                            // It also updates the car information with the new color.
                            // SO MUCH EASIER!
                            CarLoaderExtension.SetRandomCarColor(carInView, currentColor, false);
                            // QoLMod showed me this way... Didn't realize the part could be null!
                            // carInView.SetCarColor(null, currentColor.Color);
                        }

                        _currentColorIndex++;
                        if (_currentColorIndex >= allowedColors.Count)
                        {
                            _currentColorIndex = 0;
                        }
#if DEBUG
                        LogService.Instance.WriteToLog($"Factory Color: {carInView.factoryColor}");
                        LogService.Instance.WriteToLog($"Car In View Current Color: {carInView.CurrentColor}");
#endif
                    }
                    else
                    {
                        LogService.Instance.WriteToLog("Error: Vehicle has no available colors");
                        UIManager.Get().ShowPopup("Error: Vehicle has no available colors.", PopupType.Normal);
                    }
                    _isPainting = false;
#if DEBUG
                    LogService.Instance.WriteToLog("---Finished Painting Car---");
#endif
                }
                else
                {
                    // The user isn't looking at a car.
                    UIManager.Get().ShowPopup("Please look at a car.", PopupType.Normal);
                    LogService.Instance.WriteToLog("User not looking at a car");
                } 
            }
            else
            {
                // The car is being painted.
                UIManager.Get().ShowPopup("Please wait while the car is being painted.", PopupType.Normal);
                LogService.Instance.WriteToLog("A car is currently being painted");
            }
        }

        /// <summary>
        /// Repairs all the body parts (including frame and interior) to 100%.
        /// </summary>
        public static void RepairCar()
        {
            // Get the car being looked at.
            CarLoader carInView = GameScript.Get().GetIOMouseOverCarLoader2();
            // Get the part being looked at.
            CarPart carPart = GameScript.Get().GetIOMouseOverCarLoader();
            // If the user is looking at a car and a part, paint the car.
            // The game stores the last car looked at sometimes.
            // If the car is not null but the part is, the user isn't looking at a car.
            if (carInView != null && carPart != null)
            {
                // This is a developer function and may not work in future updates.
                carInView.Dev_RepairAllBody();
            }
            else
            {
                // The user isn't looking at a car.
                UIManager.Get().ShowPopup("Please look at a car.", PopupType.Normal);
                LogService.Instance.WriteToLog("User not looking at a car");
            }
        }

        /// <summary>
        /// Cycles through the 5 available cars for the user to spawn.
        /// </summary>
        public static void CycleSpawnCar()
        {
            _currentSpawnIndex++;
            if (_currentSpawnIndex >= _spawnCarIDs.Count)
            {
                _currentSpawnIndex = 0;
            }
            var currentCarName = _spawnCarNames[_currentSpawnIndex];
            // Tell the user the car that will spawn.
            UIManager.Get().ShowPopup($"Car to spawn: {currentCarName}.", PopupType.Normal);
            LogService.Instance.WriteToLog($"Car to spawn: {currentCarName}");
        }

        /// <summary>
        /// Spawn the selected car from above.
        /// </summary>
        /// <returns>(null) Used for the coroutine.</returns>
        public static System.Collections.IEnumerator SpawnCar(string spawnCarFrom = "")
        {
            if (_configFile == null)
                yield return null;
            var carLoadersGO = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "CarLoaders");
            if (carLoadersGO != null)
            {
                int nextIndex = -1;
                int spawnedCars = 0;
                var carLoaders = carLoadersGO.GetComponentsInChildren<CarLoader>();
                // Get a count of actual spawned cars to tell the user.
                // The loop after this, fills in any vehicle at any index.
                foreach (var loader in carLoaders)
                {
                    if (!string.IsNullOrWhiteSpace(loader.CarID))
                    {
                        spawnedCars++;
                    }
                }
                if (spawnedCars == 10)
                {
                    // Tell the user the max cars is 10.
                    UIManager.Get().ShowPopup($"Maximum Cars of 10 reached.", PopupType.Normal);
                    LogService.Instance.WriteToLog($"Maximum Cars of 10 reached");
                    yield return null;
                }
                // Loop through the loaders and spawn a car.
                for (int i = 0; i < carLoaders.Length; i++)
                {
                    var carLoader = carLoaders[i];
                    if (string.IsNullOrWhiteSpace(carLoader.CarID) &&
                        nextIndex == -1)
                    {
                        // Try moving the car under the road and
                        // then back after setting the part condition.

                        var carDebug = carLoader.GetComponent<CarDebug>();
                        if (carDebug != null)
                        {
                            if (_currentSpawnIndex == -1)
                            {
                                _currentSpawnIndex = 0;
                            }
                            string currentSpawnCarID = _spawnCarIDs[_currentSpawnIndex];
                            switch (spawnCarFrom)
                            {
                                // This does nothing in the demo.
                                case "auction":
                                    carDebug.AuctionCar();
                                    break;
                                // This does nothing in the demo.
                                case "junkyard":
                                    carDebug.SalvageCar();
                                    break;
                                // This doesn't check if parking spots are full and
                                // spawns cars somewhere else (I haven't looked for them).
                                case "random":
                                    carDebug.SpawnRandomCar();
                                    break;
                                default:
                                    carDebug.LoadCar(currentSpawnCarID, (_currentSpawnIndex == 3 ? 1 : 0));
                                    break;
                            }
                            // It takes time for the game to populate the car parts.
                            while (!carLoader.done)
                            {
                                yield return new WaitForSeconds(0.1f);
                            }
                            yield return new WaitForFixedUpdate();
                            yield return new WaitForEndOfFrame();
                            // The string check isn't necessary for the demo, but I'm leaving it for reference.
                            if (_configFile.RandomPartConditionOnSpawn && string.IsNullOrWhiteSpace(spawnCarFrom))
                            {
                                // Set a random condition to all the parts.
                                carDebug.SetRandomPartsCondition();
                            }
                            if (!string.IsNullOrWhiteSpace(carLoader.CarID))
                            {
                                UIManager.Get().ShowPopup($"Car {spawnedCars + 1} of 10 spawned.", PopupType.Normal);
                                nextIndex++;
                            }
                            // This was wrong.
                            // TODO: Find out how to tell if 8 parking spots are taken
                            //else
                            //{
                            //    LogService.Instance.WriteToLog($"Maximum Cars Parked in spots");
                            //}
                        }
                    }
#if DEBUG
                    LogService.Instance.WriteToLog($"Car {i + 1}: {carLoader.CarID}");
#endif
                }
            }

            yield return null;
        }

        /// <summary>
        /// Refreshes the Used Parts Market parts list and updates the parts page.
        /// </summary>
        public static void RefreshUPM()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null)
                return;

            if (windowManager.activeWindows.count > 0 &&
                windowManager.IsWindowActive(WindowID.Shop))
            {
                var shopWindow = windowManager.GetWindowByID<ShopWindow25>(WindowID.Shop);
                if (shopWindow != null)
                {
                    // Saved for reference.
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
                        if (partsPage.sidebarManager.isFocused)
                        {
                            partsPage.FocusMainWindow();
                        }
                        partsPage.UpdateItems();
                    }
                    LogService.Instance.WriteToLog("Refreshed Used Parts Market");
                }
            }
            else
            {
                UIManager.Get().ShowPopup("Open the Shop Window first.", PopupType.Normal);
            }
        }

        /// <summary>
        /// Toggles off any car parts that aren't the DNB Censor and then
        /// refreshes the Used Parts Market parts list and updates the parts page.
        /// </summary>
        public static void ToggleUPM()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null)
                return;

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
                                // Saved for reference
                                //else
                                //{
                                //    LogService.Instance.WriteToLog(item.ItemId);
                                //}
                                // The following is the output from the above line for DNB Censor.
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
                        UIManager.Get().ShowPopup($"Toggled Used Parts Market {(_onlyDNBParts ? "ON" : "OFF")}.", PopupType.Normal);
                    }
                    var partsPage = shopWindow.partsPage;
                    if (partsPage != null && partsPage.ShopType == ShopType25.UsedParts)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Used Parts Page Active");
#endif
                        if (partsPage.sidebarManager.isFocused)
                        {
                            partsPage.FocusMainWindow();
                        }
                        partsPage.UpdateItems();
                    }
                    LogService.Instance.WriteToLog("Refreshed Used Parts Market");
                }
            }
            else
            {
                UIManager.Get().ShowPopup("Open the Shop Window first.", PopupType.Normal);
            }
        }

        /// <summary>
        /// Toggles the Demo World blockers ON/OFF.
        /// </summary>
        public static void ToggleDemoWorldLimits()
        {
            _demoLimitsActive = !_demoLimitsActive;
            if (DemoWallsGO != null)
            {
                DemoWallsGO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"DemoWalls: {(DemoWallsGO.activeSelf ? "ON" : "OFF")}");
            }
            if (Garage_Exterior_Demo_ColliderGO != null)
            {
                Garage_Exterior_Demo_ColliderGO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"Garage_Exterior_Demo_Collider: {(Garage_Exterior_Demo_ColliderGO.activeSelf ? "ON" : "OFF")}");
            }
            if (Garage_Exterior_Demo_Wall_Blocked_1GO != null)
            {
                Garage_Exterior_Demo_Wall_Blocked_1GO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"Garage_Exterior_Demo_Wall_Blocked_1: {(Garage_Exterior_Demo_Wall_Blocked_1GO.activeSelf ? "ON" : "OFF")}");
            }
            if (DemoVehiclesDetectorGO != null)
            {
                DemoVehiclesDetectorGO.SetActive(_demoLimitsActive);
                LogService.Instance.WriteToLog($"DemoVehiclesDetector: {(DemoVehiclesDetectorGO.activeSelf ? "ON" : "OFF")}");
            }
            LogService.Instance.WriteToLog($"Demo World Limits: {(_demoLimitsActive ? "ON" : "OFF")}");
            UIManager.Get().ShowPopup($"Demo World Limits {(_demoLimitsActive ? "ON" : "OFF")}", PopupType.Normal);
        }

        /// <summary>
        /// Deletes the car the user is looking at.
        /// </summary>
        public static void DeleteCar()
        {
            // Get the car being looked at.
            CarLoader carInView = GameScript.Get().GetIOMouseOverCarLoader2();
            // Get the part being looked at.
            CarPart carPart = GameScript.Get().GetIOMouseOverCarLoader();
            // If the user is looking at a car and a part, paint the car.
            // The game stores the last car looked at sometimes.
            // If the car is not null but the part is, the user isn't looking at a car.
            if (carInView != null && carPart != null)
            {
                // This is a developer function and may not work in future updates.
                var carDebug = carInView.GetComponent<CarDebug>();
                carDebug.DeleteCar();
            }
            else
            {
                // The user isn't looking at a car.
                UIManager.Get().ShowPopup("Please look at a car.", PopupType.Normal);
                LogService.Instance.WriteToLog("User not looking at a car");
            }
        }
    }
}
