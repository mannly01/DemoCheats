# DEMO CHEATS
A collection of cheats for the Car Mechanic Simulator 2027 Demo.

## REQUIREMENTS
* [MelonLoader 0.7.3](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.3)

## UPDATE - 2.1.3
* Fixed Bug: The window and light tint were not persisting after the game exited.
* Fixed Bug: The Max Skills cheat wasn't working if the user upgraded a skill manually first.
* Fixed Bug: RepairBody key was constantly being checked in OnUpdate.
* Added multiple comments to source files and adjusted log file output.
* **Some functions are only available to the Host in multiplayer.**

## FEATURES
* Add a configured amount of money and XP to the player.
* Add skill points to the player.
* Max out the skills of the player. **(this now requires saving)**
* Toggle fast mounting of parts.
* Set the Inventory to unlimited for the Player and Warehouses (Warehouse UI will show 200 limit still). **(HOST SETS FOR BOTH)**
* Change the Game Speed.
* Change the Player Walk and Run Speed. **(HOST SETS FOR BOTH)**
* Paint the car and wheels an approved factory color by looping through the available ones. **(HOST ONLY)**
* Tint the windows and lights with the Cycle Paint button. **(HOST ONLY)**
* Repair the body parts to 100% when looking at a car. **(HOST ONLY)**
* Spawn the 7 available Demo cars (with or w/o random condition). **(HOST ONLY)**
* **Maximum 10 cars allowed in the Demo**
* Refresh the Used Parts Market immediately.
* Toggle showing only the Unavailable body parts for Spawn car in the Used Parts Market.
* Toggle the Demo World Limits to view more of the map .**(BOTH HOST & CLIENT MUST TOGGLE)**
* Delete a car. **(HOST ONLY)**

## TRANSFERALL FEATURES
* Move all items between the Player Inventory and a Warehouse with one key press. **(K)**
* Adjust the minimum condition of parts to move between 0 and 100%. **(-/=)**
* Set a maximum number of parts to move before a warning message is displayed.
* Move all items from the Shopping List to Shopping Cart with one key press. **(K)**
* Add all car parts to the Shopping List from the Parts page of the Car with one key press. **(K)**
* Mark all car parts from the Parts page of the Car with one key hold. **(HOLD K)**

## DEFAULT KEYS
* R: Opens the Demo Cheats menu.
* None: Cycle through the factory colors and paint the car. **(HOST ONLY)**
* None: Repair the body parts to 100%. **(HOST ONLY)**
* K: (Inventory/Warehouse) Move all items from Inventory/Warehouse to opposite tab in Inventory screens.
* K: (Shopping List) While the Shopping List is open in a Shop moves all items to the Shopping Cart.
* K: (Car Parts) Add all the car parts to the Shopping List while on the Car Info page.
* K: (Car Parts) Long press to mark all car parts while on the Car Info page.
* -: Lower the part condition to move by 10%. This value is saved in MinPartCondition value on exit.
* =: Raise the part condition to move by 10%. This value is saved in MinPartCondition value on exit.

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
* TransferAllItems: K
* SetPartConditionLower: -
* SetPartConditionHigher: =
* MinPartCondition: 0
* MinNumOfItemsWarning: 201
- [Keycode Values](https://docs.unity3d.com/ScriptReference/KeyCode.html)

## INSTALLATION (Same as all other MelonLoader mods)
* Install [MelonLoader 0.7.3](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.3) ([MelonLoader.Installer.exe](https://github.com/LavaGang/MelonLoader/releases/download/v0.7.3/MelonLoader.Installer.exe) recommended)
* Download .zip from [Releases](https://github.com/mannly01/DemoCheats/releases/latest) page
* Unzip the DemoCheats.dll file to Car Mechanic Simulator 2027 Demo\Mods folder
* **The DemoCheats.cfg file is optional but will allow you to edit the default settings**
- Default Folder: \SteamLibrary\steamapps\common\Car Mechanic Simulator 2027 Demo\Mods\

## KNOWN ISSUES
* View [Issues Section](https://github.com/mannly01/DemoCheats/issues)

## CREDITS
Thanks to the following developers:
* [iBlaster](https://www.nexusmods.com/profile/iBlaster/mods)
* [TK3R](https://www.nexusmods.com/profile/tk3r/mods)
* [MeitziQ](https://www.nexusmods.com/profile/MeitziQ/mods)