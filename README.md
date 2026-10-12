# DEMO CHEATS
A collection of cheats for the Car Mechanic Simulator 2026 Demo.

## REQUIREMENTS
* [MelonLoader 0.7.3](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.3)

## UPDATE - 1.3.0
* Added the ability to set the Inventory to unlimited (actually 10,000).
* **The UI will show a limit for the Warehouses, but you can move more items into them.**
* Added TransferAll mod (CMS 2021) functionality to CMS 2026. See FEATURES below.
* Added persistence (saved after game exit) to the following settings: Game Speed, Walk Speed, Run Speed, Fast Mount mode and Unlimited Inventory.
* Updated the instructions to use MelonLoader 0.7.3 (slight QoL update).
* Fixed bug: Some of the key bindings were still called while searching in the store. This no longer happens.

## FEATURES
* (TransferAll) Move all items between the Player Inventory and a Warehouse with one key press (K)
* (TransferAll) Adjust the minimum condition of parts to move between 0 and 100% (-/=)
* (TransferAll) Set a maximum number of parts to move before a warning message is displayed
* Set the Inventory to unlimited for the Player and Warehouses (Warehouse UI won't show unlimited)
* Move all items from the Shopping List to Shopping Cart with one key
* Add a configured amount of money and XP to the player
* Add skill points to the player
* Max out the skills of the player (this is permanent without saving)
* Toggle fast mounting of parts
* Change the Game Speed, Player Walk Speed and Player Run Speed
* Paint the car and wheels an approved factory color by looping through the available ones
* Repair the body parts to 100% when looking at a car
* Spawn the 5 available Demo cars (with or w/o random condition)
* **Maximum 10 cars allowed in the Demo**
* Refresh the Used Parts Market immediately
* Toggle showing only the DNB Censor car parts in the Used Parts Market
* Toggle the Demo World Limits to view more of the map
* Delete a car

## DEFAULT KEYS
* 'ACCEPT': Long press while the Shopping List is open to move all items to the Shopping Cart
* *^This is the key already assigned to ACCEPT in the game settings*
* R: Opens the Demo Cheats menu
* None: Cycle through the factory colors and paint the car
* None: Repair the body parts to 100%
* K: (TransferAll) Move all items from Inventory/Warehouse to opposite tab in Inventory screens.
* -: (TransferAll) Lower the part condition to move by 10%. This value is saved in MinPartCondition value on exit.
* =: (TransferAll) Raise the part condition to move by 10%. This value is saved in MinPartCondition value on exit.

## DEFAULT SETTINGS (DemoCheats.cfg)
* OpenCheatMenu: R
* PaintCar: None
* RepairBody: None
* MoneyAmount: 1000000
* XPAmount: 1000
* GameSpeedRate: 0.25
* RandomPartConditionOnSpawn: true (false = 100%)
* GameSpeed: 1.0
* WalkSpeed: 3.0
* RunSpeed: 5.0
* FastMount: false
* UnlimitedInventory: false
* TransferAllItems: K (TransferAll)
* SetPartConditionLower: - (TransferAll)
* SetPartConditionHigher: = (TransferAll)
* MinPartCondition: 0 (TransferAll)
* MinNumOfItemsWarning: 201 (TransferAll)
- [Keycode Values](https://docs.unity3d.com/ScriptReference/KeyCode.html)

## INSTALLATION (Same as all other MelonLoader mods)
* Install [MelonLoader 0.7.3](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.3) ([MelonLoader.Installer.exe](https://github.com/LavaGang/MelonLoader/releases/download/v0.7.3/MelonLoader.Installer.exe) recommended)
* Download .zip from [Releases](https://github.com/mannly01/DemoCheats/releases/latest) page
* Unzip the DemoCheats.dll file to Car Mechanic Simulator 2026 Demo\Mods folder
* **The DemoCheats.cfg file is optional but will allow you to edit the default settings**
- Default Folder: \SteamLibrary\steamapps\common\Car Mechanic Simulator 2026 Demo\Mods\

## KNOWN ISSUES
* View [Issues Section](https://github.com/mannly01/DemoCheats/issues)

## CREDITS
Thanks to the following developers:
* [iBlaster](https://www.nexusmods.com/profile/iBlaster/mods)
* [TK3R](https://www.nexusmods.com/profile/tk3r/mods)
* [MeitziQ](https://www.nexusmods.com/profile/MeitziQ/mods)