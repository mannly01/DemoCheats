using Il2Cpp;
using Il2CppCMS.Core;
using Il2CppCMS.DevTools.QC;
using Il2CppCMS.Items;
using Il2CppCMS.Player.Containers;
using Il2CppCMS.UI;
using Il2CppCMS.UI.Helpers;
using Il2CppCMS.UI.Logic;
using Il2CppCMS.UI.Logic.CarInfo.Containers;
using Il2CppCMS.UI.Logic.Shop.Cart.Containers;
using Il2CppCMS.UI.Logic.Shop.List_.Containers;
using Il2CppCMS.UI.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DemoCheats
{
    public class TransferAll
    {
        /// <summary>
        /// Reference to Settings file.
        /// </summary>
        private static readonly DemoCheatsConfigFile _configFile = DemoCheats.GetConfigFile();
        /// <summary>
        /// Method to move Inventory items to a Warehouse (called from MoveInventoryOrWarehouseItems).
        /// </summary>
        /// <param name="itemStacks">The list of ItemStacks to move.</param>
        /// <param name="invItemsManager">The Player Inventory Items Manager.</param>
        /// <param name="wareItemsManager">The Current Warehouse Items Manager.</param>
        /// <returns>(itemCount, errorString) Tuple with the number of items that were moved and a potential error message.</returns>
        private static (int itemCount, string errorString) MoveInventoryItemsToWarehouse(
			Il2CppSystem.Collections.Generic.List<ItemStack> itemStacks,
            ItemContainerManager invItemsManager, ItemContainerManager wareItemsManager)
        {
#if DEBUG
            LogService.Instance.WriteToLog($"Called");
#endif
            // Setup a temporary counter to return at the end.
            int invItemCount = 0;

            // Check if items can be added to the Warehouse.
            if (!wareItemsManager.CanAddItems())
            {
#if DEBUG
                LogService.Instance.WriteToLog("Warehouse ItemsManager cannot add items");
#endif
                return (0, "Warehouse cannot add items.");
            }

            // Use the standard For loop instead of ForEach
            // so the list isn't edited during the operation.
            for (int i = 0; i < itemStacks.Count; i++)
            {
                // Get a reference to the ItemStack.
                var itemStack = itemStacks[i];
                // The Player Inventory holds the Garage items, so make sure not to move them.
                if (!itemStack.ContainsCustomizableItems)
                {
                    // This is a stack of IBaseItems.
                    // Setup a temporary list of items to remove at the end.
                    var itemsToRemove = new List<IBaseItem>();
                    // Loop through the ItemStack and move as many items as posssible.
                    for (int j = 0; j < itemStack.Items.Count; j++)
                    {
                        // Get a reference to the IBaseItem in the ItemStack.
                        var baseItem = itemStack.Items[j];
                        // Check if the condition of the part is
                        // greater than or equal to the user setting.
                        if ((baseItem.GetCondition() * 100) >= _configFile.MinPartCondition)
                        {
                            // Check that the Warehouse can add the item.
                            var itemCanBeAdded = wareItemsManager.CanAddItem(baseItem);
                            if (itemCanBeAdded)
                            {
                                var itemAdded = wareItemsManager.TryAddItem(baseItem);
                                if (itemAdded)
                                {
                                    itemsToRemove.Add(baseItem);
                                    invItemCount++;
                                }
#if DEBUG
                                else
                                {
                                    LogService.Instance.WriteToLog($"{baseItem.GetLocalizedName()} couldn't be added to the Warehouse");
                                } 
#endif
                            }
#if DEBUG
                            else
                            {
                                LogService.Instance.WriteToLog($"TEST: {baseItem.GetLocalizedName()} couldn't be added to the Warehouse");
                            } 
#endif
                        }
                    }
                    foreach (var item in itemsToRemove)
                    {
                        itemStack.RemoveItem(item);
                    }
                }
            }
            invItemsManager.RemoveEmptyStacks();

            return (invItemCount, string.Empty);
        }
        private static (int itemCount, string errorString) MoveWarehouseItemsToInventory(
			Il2CppSystem.Collections.Generic.List<ItemStack> itemStacks,
            ItemContainerManager invItemsManager, ItemContainerManager wareItemsManager)
        {
#if DEBUG
            LogService.Instance.WriteToLog($"Called");
#endif
            // Setup a temporary counter to return at the end.
            int wareItemCount = 0;

            // Check if items can be added to the Player Inventory.
            if (!invItemsManager.CanAddItems())
            {
#if DEBUG
                LogService.Instance.WriteToLog("Inventory ItemsManager cannot add items");
#endif
                return (0, "Player Inventory cannot add items.");
            }

            // Use the standard For loop instead of ForEach
            // so the list isn't edited during the operation.
            for (int i = 0; i < itemStacks.Count; i++)
            {
                // Get a reference to the ItemStack.
                var itemStack = itemStacks[i];
                // The Warehouse doesn't hold Garage items, but make sure they don't move anyway.
                if (!itemStack.ContainsCustomizableItems)
                {
                    // This is a stack of IBaseItems.
                    // Setup a temporary list of items to remove at the end.
                    var itemsToRemove = new List<IBaseItem>();
                    // Loop through the ItemStack and move as many items as posssible.
                    for (int j = 0; j < itemStack.Items.Count; j++)
                    {
                        // Get a reference to the IBaseItem in the ItemStack.
                        var baseItem = itemStack.Items[j];
                        // Check if the condition of the part is
                        // greater than or equal to the user setting.
                        if ((baseItem.GetCondition() * 100) >= _configFile.MinPartCondition)
                        {
                            // Check that the Player Inventory can add the item.
                            var itemCanBeAdded = invItemsManager.CanAddItem(baseItem);
                            if (itemCanBeAdded)
                            {
                                var itemAdded = invItemsManager.TryAddItem(baseItem);
                                if (itemAdded)
                                {
                                    itemsToRemove.Add(baseItem);
                                    wareItemCount++;
                                }
#if DEBUG
                                else
                                {
                                    LogService.Instance.WriteToLog($"{baseItem.GetLocalizedName()} couldn't be added to the Player Inventory");
                                } 
#endif
                            }
#if DEBUG
                            else
                            {
                                LogService.Instance.WriteToLog($"TEST: {baseItem.GetLocalizedName()} couldn't be added to the Player Inventory");
                            } 
#endif
                        }
                    }
                    foreach (var item in itemsToRemove)
                    {
                        itemStack.RemoveItem(item);
                    }
                }
            }
            wareItemsManager.RemoveEmptyStacks();

            return (wareItemCount, string.Empty);
        }
        /// <summary>
        /// Method to move items and groups between the user's Inventory and Garage Warehouse.
        /// </summary>
        public static void MoveInventoryOrWarehouseItems()
        {
#if DEBUG
            LogService.Instance.WriteToLog($"Called");
#endif
            var windowManager = WindowManager.Instance;
            var uiManager = UIManager.Get();

            // Check that the Warehouse Window is displayed.
            if (windowManager.activeWindows.count > 0 &&
                windowManager.IsWindowActive(WindowID.Warehouse))
            {
                // Get a reference to the Warehouse Window.
                var warehouseWindow = windowManager.GetWindowByID<WarehouseWindow>(WindowID.Warehouse);
                if (warehouseWindow != null)
                {
                    // Get a reference to the Player Inventory Panel.
                    var inventoryPanel = warehouseWindow.inventoryPanel;
                    if (inventoryPanel == null)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Inventory Panel is null");
#endif
                        return;
                    }
                    // Get a reference to the Player Inventory ItemsManager.
                    var invItemsManager = inventoryPanel.ItemsManager;
                    if (invItemsManager == null)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Inventory ItemsManager is null");
#endif
                        return;
                    }
                    // Get the InventoryCapacity of the Player Inventory.
                    var invCapacity = invItemsManager.InventoryCapacity;
                    // Get Items Count for the Player Inventory.
                    var invItemsCount = 0;
                    // Loop through the Player Inventory and count the items above the user's condition setting.
                    foreach (var itemStack in invItemsManager.Stacks)
                    {
                        // The Player Inventory holds the Garage items, so make sure not to include them.
                        if (!itemStack.ContainsCustomizableItems)
                        {
                            // Loop through each stack and get the items above the user's condition setting.
                            foreach (var item in itemStack.Items)
                            {
                                if ((item.GetCondition() * 100) >= _configFile.MinPartCondition)
                                {
                                    invItemsCount++;
                                }
                            }
                        }
                    }
                    // Get the available space in the Player Inventory.
                    var invAvailable = invCapacity - invItemsCount;

                    // Get a reference to the Warehouse Panel.
                    var warehousePanel = warehouseWindow.warehouseItemsPanel;
                    if (warehousePanel == null)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Warehouse Panel is null");
#endif
                        return;
                    }
                    // Get a reference to the Warehouse ItemsManager.
                    var wareItemsManager = warehousePanel.ItemsManager;
                    if (wareItemsManager == null)
                    {
#if DEBUG
                        LogService.Instance.WriteToLog($"Warehouse ItemsManager is null");
#endif
                        return;
                    }
                    // Get the InventoryCapacity of the Warehouse.
                    var wareCapacity = wareItemsManager.InventoryCapacity;
                    // If the user has Unlimited Inventory turned on,
                    // the UI will show a limit, but it will allow more items to be moved.
                    if (InventoryCommands.UnlimitedInventory)
                    {
                        wareCapacity = 10000;
                    }
                    // Get the Items Count for the Warehouse.
                    var wareItemsCount = 0;
                    // Loop through the Player Inventory and count the items above the user's condition setting.
                    foreach (var itemStack in wareItemsManager.Stacks)
                    {
                        // The Warehouse doesn't hold Garage items, but make sure they don't get included anyway.
                        if (!itemStack.ContainsCustomizableItems)
                        {
                            // Loop through each stack and get the items above the user's condition setting.
                            foreach (var item in itemStack.Items)
                            {
                                if ((item.GetCondition() * 100) >= _configFile.MinPartCondition)
                                {
                                    wareItemsCount++;
                                }
                            }
                        }
                    }
                    // Get the available space in the Warehouse.
                    var wareAvailable = wareCapacity - wareItemsCount;

#if DEBUG
                    // DEBUG Log for the Player Inventory.
                    LogService.Instance.WriteToLog($"Inventory Capacity: {(invCapacity < 100 ? " " : "")}{invCapacity} Current ItemsCount: {(invItemsCount < 100 ? " " : "")}{invItemsCount}\tSpace Available: {(invAvailable < 100 ? " " : "")}{invAvailable}");
                    // DEBUG Log for the Warehouse Inventory.
                    LogService.Instance.WriteToLog($"Warehouse Capacity: {(wareCapacity < 100 ? " " : "")}{wareCapacity} Current ItemsCount: {(wareItemsCount < 100 ? " " : "")}{wareItemsCount}\tSpace Available: {(wareAvailable < 100 ? " " : "")}{wareAvailable}");
#endif

                    if (inventoryPanel.IsFocused)
                    {
                        // This is the Inventory Tab.
#if DEBUG
                        LogService.Instance.WriteToLog($"Inventory Tab displayed");
#endif
                        // Check if there are items to move.
                        // If there aren't, so the user a message.
                        if (invItemsCount <= 0)
                        {
                            uiManager.ShowInfoWindow($"No items {_configFile.MinPartCondition}% and above to move.", false);
                            LogService.Instance.WriteToLog($"No items {_configFile.MinPartCondition}% and above to move.");
                        }

                        // Create an Ask Window to show the user.
                        Action<bool> confirmMove = new Action<bool>(response =>
                        {
                            if (response)
                            {
                                var itemStacks = invItemsManager.StacksCopy;
                                (int itemCount, string errorString) = MoveInventoryItemsToWarehouse(itemStacks, invItemsManager, wareItemsManager);
                                if (string.IsNullOrEmpty(errorString))
                                {
                                    LogService.Instance.WriteToLog($"{itemCount} item(s) {_configFile.MinPartCondition}% and above moved from Player Inventory to the Warehouse");
                                    uiManager.ShowPopup($"{itemCount} item(s) {_configFile.MinPartCondition}% and above moved.", PopupType.Normal);
                                }
                                else
                                {
                                    LogService.Instance.WriteToLog($"Error: {errorString}");
                                    uiManager.ShowPopup($"Error: {errorString}", PopupType.Normal);
                                }
                                warehouseWindow.RefreshWindow();
                                warehousePanel.RefreshPanel();
                            }
                            else
                            {
                                LogService.Instance.WriteToLog("User cancelled move from the Player Inventory to the Warehouse");
                            }
                        });

                        // Check if the Warehouse has enough room for the Player Inventory items.
                        if (wareAvailable >= invItemsCount)
                        {
                            if (invItemsCount >= _configFile.MinNumOfItemsWarning)
                            {
#if DEBUG
                                LogService.Instance.WriteToLog($"MinNumOfItemsWarning ({_configFile.MinNumOfItemsWarning}) threshold reached");
#endif
                                uiManager.ShowAskWindow("Move Items", $"You are moving {invItemsCount} items which is greater than the MinNumOfItemsWarning ({_configFile.MinNumOfItemsWarning}) setting.\nAre you sure?", confirmMove);
                            }
                            else
                            {
                                confirmMove.Invoke(true);
                            }
                        }
                        else
                        {
                            // The Warehouse doesn't have enough room, ask the user how to proceed.
#if DEBUG
                            LogService.Instance.WriteToLog($"Warehouse doesn't have enough capacity to move Player Inventory items");
#endif
                            uiManager.ShowAskWindow("Move Items", "The Warehouse doesn't have enough room.\nMove as many items as possible?", confirmMove);
                        }
                    }
                    else if (warehousePanel.IsFocused)
                    {
                        // This is the Warehouse Tab.
#if DEBUG
                        LogService.Instance.WriteToLog($"Warehouse Tab displayed");
#endif
                        // Check if there are items to move.
                        // If there aren't, so the user a message.
                        if (wareItemsCount <= 0)
                        {
                            uiManager.ShowInfoWindow($"No items {_configFile.MinPartCondition}% and above to move.", false);
                            LogService.Instance.WriteToLog($"No items {_configFile.MinPartCondition}% and above to move.");
                        }

                        // Create an Ask Window to show the user.
                        Action<bool> confirmMove = new Action<bool>(response =>
                        {
                            if (response)
                            {
                                var itemStacks = wareItemsManager.StacksCopy;
                                (int itemCount, string errorString) = MoveWarehouseItemsToInventory(itemStacks, invItemsManager, wareItemsManager);
                                if (string.IsNullOrEmpty(errorString))
                                {
                                    LogService.Instance.WriteToLog($"{itemCount} item(s) {_configFile.MinPartCondition}% and above moved from the Warehouse to the Player Inventory");
                                    uiManager.ShowPopup($"{itemCount} item(s) {_configFile.MinPartCondition}% and above moved.", PopupType.Normal);
                                }
                                else
                                {
                                    LogService.Instance.WriteToLog($"Error: {errorString}");
                                    uiManager.ShowPopup($"Error: {errorString}", PopupType.Normal);
                                }
                                warehouseWindow.RefreshWindow();
                                inventoryPanel.RefreshPanel();
                            }
                            else
                            {
                                LogService.Instance.WriteToLog("User cancelled move from the Warehouse to Player Inventory");
                            }
                        });

                        // Check if the Player Inventory has enough room for the Warehouse items.
                        if (invAvailable >= wareItemsCount)
                        {
                            if (wareItemsCount >= _configFile.MinNumOfItemsWarning)
                            {
#if DEBUG
                                LogService.Instance.WriteToLog($"MinNumOfItemsWarning ({_configFile.MinNumOfItemsWarning}) threshold reached");
#endif
                                uiManager.ShowAskWindow("Move Items", $"You are moving {wareItemsCount} items which is greater than the MinNumOfItemsWarning ({_configFile.MinNumOfItemsWarning}) setting.\nAre you sure?", confirmMove);
                            }
                            else
                            {
                                confirmMove.Invoke(true);
                            }
                        }
                        else
                        {
                            // The Player Inventory doesn't have enough room, ask the user how to proceed.
#if DEBUG
                            LogService.Instance.WriteToLog($"Player Inventory doesn't have enough capacity to move Warehouse items");
#endif
                            uiManager.ShowAskWindow("Move Items", "Your Inventory doesn't have enough room.\nMove as many items as possible?", confirmMove);
                        }
                    }
                }
            }
            else
            {
                // The Warehouse Window is not displayed,
                // so show the user a message.
                uiManager.ShowPopup("Please open a Warehouse first.", PopupType.Normal);
                LogService.Instance.WriteToLog("Warehouse window is not open");
            }
        }
        public static System.Collections.IEnumerator MoveShoppingListToCart()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null)
                yield return null;

            if (windowManager.activeWindows.count > 0)
            {
                if (windowManager.IsWindowActive(WindowID.Shop))
                {
                    var shopWindow = windowManager.GetWindowByID<ShopWindow>(WindowID.Shop);
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
                                            if (partsPage.ShopType == ShopType.LicensePlates)
                                            {
                                                ShopListLicensePlate slLP = slItem.TryCast<ShopListLicensePlate>();
                                                if (slLP != null)
                                                {
                                                    var licenseName = ItemHelper.ParseLicensePlateName(slLP.Name);
                                                    int price = Helper.GetPrice(
                                                        InventoryCommands.Method_Internal_Static_IBaseItem_LicensePlate_0(
                                                            Singleton<GameInventory>.Instance.licensePlatesProvider.GetRandomLicensePlate()));
                                                    
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
                                            else if (partsPage.ShopType == ShopType.Wheels)
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
#if DEBUG
                else
                {
                    var activeWindows = windowManager.activeWindows;
                    LogService.Instance.WriteToLog($"Active Window: {activeWindows.head.Value}");
                } 
#endif
            }

            yield return null;
        }

        public static void MoveAllItemsToShoppingCart()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null)
                return;
            if (windowManager.activeWindows.count > 0)
            {
                if (windowManager.IsWindowActive(WindowID.CarInfo))
                {
                    var carInfoWindow = windowManager.GetWindowByID<CarInfoWindow>(WindowID.CarInfo);
                    if (carInfoWindow != null)
                    {
                        var currentCarInfoPage = carInfoWindow.currentTab;
#if DEBUG
                        LogService.Instance.WriteToLog($"Current CarInfo Page: {currentCarInfoPage}");
#endif

                        var partsTab = carInfoWindow.partsTab;
                        if (partsTab.isActiveAndEnabled)
                        {
                            var partsCategory = partsTab.currentCategory;
                            var partsSorting = partsTab.currentSortType;
                            var partsManager = partsTab.pageManager;
#if DEBUG
                            LogService.Instance.WriteToLog($"Category: {partsCategory}, Sorting: {partsSorting}, ItemCount: {partsManager.GetItemsCount()}");
#endif
                            for (uint i = 0; i < partsManager.pagesCount; i++)
                            {
                                if (i > 0)
                                {
                                    partsManager.SetPage(i);
                                }
                                var partItemObjects = partsManager.ItemObjects;
#if DEBUG
                                LogService.Instance.WriteToLog($"Page {partsManager.currentPage}, Page Item Count: {partsManager.ItemObjects.Count}"); 
#endif
                                if (partItemObjects != null)
                                {
                                    if (partsManager.CurrentPageIsLast())
                                    {
                                        for (int j = 0; j < partsManager.currentPageItemsCount; j++)
                                        {
                                            var partItemObject = partItemObjects[j];
                                            if (partItemObject.CanAddToShopList())
                                            {
                                                partItemObject.AddToShopList();
                                            }
#if DEBUG
                                            else
                                            {
                                                LogService.Instance.WriteToLog($"{partItemObject.name} couldn't be added to shopping list");
                                            }
#endif
                                        }
                                    }
                                    else
                                    {
                                        foreach (var partItemObject in partItemObjects)
                                        {
                                            if (partItemObject.CanAddToShopList())
                                            {
                                                partItemObject.AddToShopList();
                                            }
#if DEBUG
                                            else
                                            {
                                                LogService.Instance.WriteToLog($"{partItemObject.name} couldn't be added to shopping list");
                                            }
#endif
                                        }
                                    } 
                                }
#if DEBUG
                                else
                                {
                                    LogService.Instance.WriteToLog("ItemObject array is null");
                                }
#endif
                            }
                            partsManager.SetPage(0);
                        }
                    }
                }
            }
        }
        public static System.Collections.IEnumerator MarkAllCarParts()
        {
            var windowManager = WindowManager.Instance;
            if (windowManager == null)
                yield return null;
            if (windowManager.activeWindows.count > 0)
            {
                if (windowManager.IsWindowActive(WindowID.CarInfo))
                {
                    var carInfoWindow = windowManager.GetWindowByID<CarInfoWindow>(WindowID.CarInfo);
                    if (carInfoWindow != null)
                    {
                        var currentCarInfoPage = carInfoWindow.currentTab;
#if DEBUG
                        LogService.Instance.WriteToLog($"Current CarInfo Page: {currentCarInfoPage}");
#endif

                        var partsTab = carInfoWindow.partsTab;
                        if (partsTab.isActiveAndEnabled)
                        {
                            var partsCategory = partsTab.currentCategory;
                            var partsSorting = partsTab.currentSortType;
                            var partsManager = partsTab.pageManager;
#if DEBUG
                            LogService.Instance.WriteToLog($"Category: {partsCategory}, Sorting: {partsSorting}, ItemCount: {partsManager.GetItemsCount()}, Page Item Count: {partsManager.CurrentPageItemsCount}");
#endif

                            var partItems = partsManager.Items.Cast<Il2CppSystem.Collections.Generic.List<CarInfoPart>>();
                            if (partItems != null)
                            {
                                SoundManager.Get().PlaySFX("MarkPart");
                                var uiVolume = SoundManager.Get().uiSoundsAudioSource.volume;
                                LogService.Instance.WriteToLog($"UI Volume Before: {SoundManager.Get().uiSoundsAudioSource.volume}");
                                SoundManager.Get().uiSoundsAudioSource.volume = 0;
                                LogService.Instance.WriteToLog($"UI Volume After: {SoundManager.Get().uiSoundsAudioSource.volume}");
                                foreach (var partItem in partItems)
                                {
                                    if (partItem.CanBeMarked && !partItem.IsMarked)
                                    {
                                        partItem.MarkAction.Invoke(true);
                                    }
#if DEBUG
                                    else
                                    {
                                        LogService.Instance.WriteToLog($"{partItem.Name} couldn't be marked");
                                    }
#endif
                                }
                                partsManager.RefreshParts(partsCategory, partsSorting, null);
                                yield return new WaitForSeconds(1.0f);
                                yield return new WaitForFixedUpdate();
                                yield return new WaitForEndOfFrame();
                                SoundManager.Get().uiSoundsAudioSource.volume = uiVolume;
                            }
#if DEBUG
                            else
                            {
                                LogService.Instance.WriteToLog("Cast to list failed");
                            } 
#endif
                        }
                    }
                }
            }

            yield return null;
        }
    }
}
