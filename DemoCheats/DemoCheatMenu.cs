using Il2Cpp;
using Il2CppCMS.Core;
using Il2CppCMS.UI;
using MelonLoader;
using System.Linq;
using UnityEngine;

namespace DemoCheats
{
    public class DemoCheatMenu
    {
        public static System.Collections.Generic.List<object> commandList;
        public static bool cheatMenuShown = false;
        public static bool showHelp = false;
        private static bool isInitialized = false;

        private static string nl = System.Environment.NewLine;
        private static int drawDoubleSizeFor4K = 1; // for 4K display, this is 2

        public static System.Collections.IEnumerator ToggleConsoleFirstTimeDisplay()
        {
            yield return new WaitForEndOfFrame();
            cheatMenuShown = !cheatMenuShown;
            yield break;
        }

        public static void ToggleCheatMenu()
        {
            if (!cheatMenuShown)
            {
                cheatMenuShown = true;
                if (!isInitialized)
                {
                    cheatMenuShown = false;
                    ShowCheatMenu();
                    MelonCoroutines.Start(ToggleConsoleFirstTimeDisplay());
                }
                if (GameMode.Get().currentMode != gameMode.UI)
                {
                    GameMode.Get().SetCurrentMode(gameMode.UI);
                }
            }
            else
            {
                cheatMenuShown = false;
                if (GameMode.Get().currentMode == gameMode.UI &&
                    Singleton<WindowManager>.Instance.activeWindows.Count > 0)
                {
                    GameMode.Get().SetCurrentMode(gameMode.UI);
                }
                else
                {
                    GameMode.Get().SetCurrentMode(gameMode.Garage);
                }
            }
        }

        public static void ShowCheatMenu()
        {
            drawDoubleSizeFor4K = 1;
            if (GameSettings.VideoSettingsData.ScreenResolutionX >= 2560)
            {
                drawDoubleSizeFor4K = 2;
            }

            commandList = new System.Collections.Generic.List<object>();
            commandList.Add(new DebugCommand("togglemenu", "Hide this Cheat Menu." + "You can also press ESC key.", "X", () => { }));
            commandList.Add(new DebugCommand("addmoney", "Add Money to Player.", "Money+", () =>
            {
                Helpers.AddMoney();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("addxp", "Add XP to the Player", "XP+", () =>
            {
                Helpers.AddXP();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("gamespeeddown", "Turn down the Game Speed.", "Game-", () =>
            {
                Helpers.AdjustGameSpeedUp(false);
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("gamespeedup", "Turn up the Game Speed.", "Game+", () =>
            {
                Helpers.AdjustGameSpeedUp(true);
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("addskillpoint", "Add a Skill Point to the Player.", "Skill Pt+", () =>
            {
                Helpers.AddSkillPoints();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("maxskills", "Max out the Player skills.", "Max Skills", () =>
            {
                Helpers.MaxSkills();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("walkspeeddown", "Turn down the Walk Speed", "Walk-", () =>
            {
                Helpers.AdjustWalkSpeedUp(false);
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("walkspeedup", "Turn up the Walk Speed", "Walk+", () =>
            {
                Helpers.AdjustWalkSpeedUp(true);
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("togglefastmount", "Toggle ON/OFF Fast Mounting of parts", "Toggle Fast Mount", () =>
            {
                Helpers.ToggleFastMount();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("runspeeddown", "Turn down the Run Speed", "Run-", () =>
            {
                Helpers.AdjustRunSpeedUp(false);
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("runspeedup", "Turn up the Run Speed", "Run+", () =>
            {
                Helpers.AdjustRunSpeedUp(true);
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("paintcar", "Cycle through the factory paint colors.", "Cycle Car Paint", () =>
            {
                Helpers.CyclePaintColors();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("repaircar", "Repair the car's body parts.", "Repair Body Parts", () =>
            {
                Helpers.RepairCar();
            }));
            commandList.Add(new DebugCommand("cyclenewcar", "Cycle through the available cars to spawn.", "Cycle Spawn Cars", () =>
            {
                Helpers.CycleSpawnCar();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("spawncar", "Spawn the selected car.", "Spawn Car", () =>
            {
                MelonCoroutines.Start(Helpers.SpawnCar());
            }));
            // Had to put these on the bottom because clicking them also click the shopping list behind them.
            commandList.Add(new DebugCommand("refreshupm", "Refresh the Used Parts Market.", "Refresh", () =>
            {
                Helpers.RefreshUPM();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("toggleupm", "Toggle non-DNB Censor parts in the UPM.", "Toggle DNB Parts", () =>
            {
                Helpers.ToggleUPM();
            }, new DebugCommandAdditionalData { doNotCloseMenu = true }));
            commandList.Add(new DebugCommand("toggledemolimits", "Toggle ON/OFF the demo world limits.", "Toggle Demo Limits", () =>
            {
                Helpers.ToggleDemoWorldLimits();
            }));
            commandList.Add(new DebugCommand("deletecar", "Deletes the car in view.", "Delete Car", () =>
            {
                Helpers.DeleteCar();
            }));

            isInitialized = true;
        }

        public static void DrawCheatMenu()
        {
            if (!cheatMenuShown)
            {
                return;
            }
            
            if (GameSettings.ConsoleMode)
            {
                string message = string.Join(nl,
                    "<color=#1BBAF6>Please note:",
                    "Cheat menu works with mouse only.",
                    "Please select 'Legacy' controller mode.",
                    "</color>");
                UIManager.Get().ShowInfoWindow(message, false);
                ToggleCheatMenu();
                return;
            }

            int menuWidth = 380 * drawDoubleSizeFor4K;
            int menuHeight = 450 * drawDoubleSizeFor4K;
            int menuX = Screen.width - (menuWidth + 30);
            int menuY = Screen.height - (menuHeight + 30);
            int menuPadding = 15 * drawDoubleSizeFor4K;

            GUIStyle labelStyle, sliderStyle, sliderStyleThumb, toggleStyle, textStyle, buttoniStyle, toolTipStyle, boxStyle;
            labelStyle = GUI.skin.label;
            labelStyle.fontSize = 14 * drawDoubleSizeFor4K;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            sliderStyle = GUI.skin.horizontalSlider;
            sliderStyle.fixedHeight = 12 * drawDoubleSizeFor4K;      //default 12
            sliderStyleThumb = GUI.skin.horizontalSliderThumb;
            sliderStyleThumb.fixedWidth = 12 * drawDoubleSizeFor4K;
            sliderStyleThumb.fixedHeight = 12 * drawDoubleSizeFor4K;

            toggleStyle = GUI.skin.toggle;
            toggleStyle.fontSize = 11 * drawDoubleSizeFor4K;

            textStyle = GUI.skin.textField;
            textStyle.fontSize = 14 * drawDoubleSizeFor4K;

            buttoniStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13 * drawDoubleSizeFor4K,
                fontStyle = FontStyle.Bold
            };
            int buttonWidth = 160 * drawDoubleSizeFor4K;
            int buttonHeight = 30 * drawDoubleSizeFor4K;

            toolTipStyle = GUI.skin.label;
            toolTipStyle.fontSize = 14 * drawDoubleSizeFor4K;
            toolTipStyle.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(menuX + menuPadding, menuY - 50, menuWidth, 40), GUI.tooltip, toolTipStyle);

            string warning = string.Join(nl,
                "WARNING!  These are CHEATS!",
                "Please save your game before using them.");
            boxStyle = GUI.skin.box;
            boxStyle.fontSize = 13 * drawDoubleSizeFor4K;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;
            boxStyle.padding = new RectOffset(menuPadding, menuPadding, menuPadding, menuPadding);
            // The menu box with the warning at the top-left.
            GUI.Box(new Rect(menuX, menuY, menuWidth, menuHeight), warning, boxStyle);

            // Close button in the top-right.
            DebugCommandBase closeCommand = commandList.Cast<DebugCommandBase>().First(c => c.CommandFormat.Equals("X"));
            if (closeCommand != null)
            {
                string description = closeCommand.CommandDescription;
                if (GUI.Button(new Rect((menuX + menuWidth) - (menuPadding * 3), menuY + menuPadding, buttonHeight, buttonHeight), new GUIContent(closeCommand.CommandFormat, description), buttoniStyle))
                {
                    if (closeCommand as DebugCommand != null)
                    {
                        if (closeCommand.AdditionalData.doNotCloseMenu == false)
                        {
                            ToggleCheatMenu();
                        }
                        (closeCommand as DebugCommand).Invoke();
                    }
                }
            }

            // Values for the 4 columns of 75px buttons.
            // Use columns 1 & 3 for 160px double buttons.
            int column1 = menuPadding;
            int column2 = menuPadding + (85 * drawDoubleSizeFor4K);
            int column3 = (menuWidth / 2) + menuPadding;
            int column4 = (menuWidth / 2) + (100 * drawDoubleSizeFor4K);
            int rowY = menuY + (menuPadding * 3);

            // Row of labels
            GUI.Label(new Rect(menuX + column1, menuY + (menuPadding * 3), buttonWidth, buttonHeight), "Player Cheats", labelStyle);
            GUI.Label(new Rect(menuX + column3, menuY + (menuPadding * 3), buttonWidth, buttonHeight), "Speed Cheats", labelStyle);
            rowY = rowY + buttonHeight;

            // First Row of Buttons
            for (int i = 1; i < 5; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                if (command != null)
                {
                    string description = command.CommandDescription;
                    int buttonX = 0;
                    buttonWidth = 75 * drawDoubleSizeFor4K;
                    switch (i)
                    {
                        case 1:
                            buttonX = menuX + column1;
                            break;
                        case 2:
                            buttonX = menuX + column2;
                            break;
                        case 3:
                            buttonX = menuX + column3;
                            break;
                        case 4:
                            buttonX = menuX + column4;
                            break;
                        default:
                            break;
                    }
                    if (GUI.Button(new Rect(buttonX, rowY, buttonWidth, buttonHeight),
                                   new GUIContent(command.CommandFormat, description), buttoniStyle))
                    {
                        if (command as DebugCommand != null)
                        {
                            if (command.AdditionalData.doNotCloseMenu == false)
                            {
                                ToggleCheatMenu();
                            }
                            (command as DebugCommand).Invoke();
                            return;
                        }
                    }
                }
            }
            rowY = rowY + (int)(buttonHeight * 1.5f);

            // Second Row of Buttons.
            for (int i = 5; i < 9; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                if (command != null)
                {
                    string description = command.CommandDescription;
                    int buttonX = 0;
                    buttonWidth = 75 * drawDoubleSizeFor4K;
                    switch (i)
                    {
                        case 5: // Blank for now
                            buttonX = menuX + column1;
                            break;
                        case 6:
                            buttonX = menuX + column2;
                            break;
                        case 7:
                            buttonX = menuX + column3;
                            break;
                        case 8:
                            buttonX = menuX + column4;
                            break;
                        default:
                            break;
                    }
                    if (GUI.Button(new Rect(buttonX, rowY, buttonWidth, buttonHeight),
                                   new GUIContent(command.CommandFormat, description), buttoniStyle))
                    {
                        if (command as DebugCommand != null)
                        {
                            if (command.AdditionalData.doNotCloseMenu == false)
                            {
                                ToggleCheatMenu();
                            }
                            (command as DebugCommand).Invoke();
                            return;
                        }
                    }
                }
            }
            rowY = rowY + (int)(buttonHeight * 1.5f);

            // Third Row of Buttons
            for (int i = 9; i < 12; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                if (command != null)
                {
                    string description = command.CommandDescription;
                    int buttonX = 0;
                    buttonWidth = 75 * drawDoubleSizeFor4K;
                    switch (i)
                    {
                        case 9: // Blank for now
                            buttonX = menuX + column1;
                            buttonWidth = 160 * drawDoubleSizeFor4K;
                            break;
                        case 10:
                            buttonX = menuX + column3;
                            break;
                        case 11:
                            buttonX = menuX + column4;
                            break;
                        default:
                            break;
                    }
                    if (GUI.Button(new Rect(buttonX, rowY, buttonWidth, buttonHeight),
                                   new GUIContent(command.CommandFormat, description), buttoniStyle))
                    {
                        if (command as DebugCommand != null)
                        {
                            if (command.AdditionalData.doNotCloseMenu == false)
                            {
                                ToggleCheatMenu();
                            }
                            (command as DebugCommand).Invoke();
                            return;
                        }
                    }
                }
            }
            rowY = rowY + buttonHeight;

            // Row label
            GUI.Label(new Rect(menuX, rowY, menuWidth, buttonHeight), "Car Repair", labelStyle);
            rowY = rowY + buttonHeight;

            // Fourth Row of buttons
            for (int i = 12; i < 14; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                if (command != null)
                {
                    string description = command.CommandDescription;
                    int buttonX = 0;
                    buttonWidth = 160 * drawDoubleSizeFor4K;
                    buttonX = menuX + (i %2 == 0 ? column3 : column1);
                    if (GUI.Button(new Rect(buttonX, rowY, buttonWidth, buttonHeight),
                                   new GUIContent(command.CommandFormat, description), buttoniStyle))
                    {
                        if (command as DebugCommand != null)
                        {
                            if (command.AdditionalData.doNotCloseMenu == false)
                            {
                                ToggleCheatMenu();
                            }
                            (command as DebugCommand).Invoke();
                            return;
                        }
                    }
                }
            }
            rowY = rowY + buttonHeight;

            // Row label
            GUI.Label(new Rect(menuX, rowY, menuWidth, buttonHeight), "Car Spawning", labelStyle);
            rowY = rowY + buttonHeight;

            // Fifth Row of buttons
            for (int i = 14; i < 16; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                if (command != null)
                {
                    string description = command.CommandDescription;
                    int buttonX = 0;
                    buttonWidth = 160 * drawDoubleSizeFor4K;
                    buttonX = menuX + (i % 2 == 0 ? column3 : column1);
                    if (GUI.Button(new Rect(buttonX, rowY, buttonWidth, buttonHeight),
                                   new GUIContent(command.CommandFormat, description), buttoniStyle))
                    {
                        if (command as DebugCommand != null)
                        {
                            if (command.AdditionalData.doNotCloseMenu == false)
                            {
                                ToggleCheatMenu();
                            }
                            (command as DebugCommand).Invoke();
                            return;
                        }
                    }
                }
            }
            rowY = rowY + buttonHeight;

            // Row label
            GUI.Label(new Rect(menuX, rowY, menuWidth, buttonHeight), "Used Parts Market", labelStyle);
            rowY = rowY + buttonHeight;

            // Sixth Row of buttons
            for (int i = 16; i < 18; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                if (command != null)
                {
                    string description = command.CommandDescription;
                    int buttonX = 0;
                    buttonWidth = 160 * drawDoubleSizeFor4K;
                    buttonX = menuX + (i % 2 == 0 ? column3 : column1);
                    if (GUI.Button(new Rect(buttonX, rowY, buttonWidth, buttonHeight),
                                   new GUIContent(command.CommandFormat, description), buttoniStyle))
                    {
                        if (command as DebugCommand != null)
                        {
                            if (command.AdditionalData.doNotCloseMenu == false)
                            {
                                ToggleCheatMenu();
                            }
                            (command as DebugCommand).Invoke();
                            return;
                        }
                    }
                }
            }
            rowY = rowY + buttonHeight;

            // Row label
            GUI.Label(new Rect(menuX, rowY, menuWidth, buttonHeight), "Miscellaneous", labelStyle);
            rowY = rowY + buttonHeight;

            // Seventh Row of buttons
            for (int i = 18; i < 20; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                if (command != null)
                {
                    string description = command.CommandDescription;
                    int buttonX = 0;
                    buttonWidth = 160 * drawDoubleSizeFor4K;
                    buttonX = menuX + (i % 2 == 0 ? column3 : column1);
                    if (GUI.Button(new Rect(buttonX, rowY, buttonWidth, buttonHeight),
                                   new GUIContent(command.CommandFormat, description), buttoniStyle))
                    {
                        if (command as DebugCommand != null)
                        {
                            if (command.AdditionalData.doNotCloseMenu == false)
                            {
                                ToggleCheatMenu();
                            }
                            (command as DebugCommand).Invoke();
                            return;
                        }
                    }
                }
            }

            Event eventti = Event.current;
        }
    }

    public class DebugCommandBase
    {
        private string _CommandId;
        private string _CommandDescription;
        private string _CommandFormat;
        private DebugCommandAdditionalData _additionalData;

        public string CommandID { get { return _CommandId; } }
        public string CommandDescription { get { return _CommandDescription; } }
        public string CommandFormat { get { return _CommandFormat; } }
        public DebugCommandAdditionalData AdditionalData { get { return _additionalData; } }

        public override string ToString()
        {
            return _CommandId;
        }

        public DebugCommandBase(string id, string desciption, string format, DebugCommandAdditionalData additionalData = null)
        {
            _CommandId = id;
            _CommandDescription = desciption;
            _CommandFormat = format;
            if (additionalData == null)
                _additionalData = new DebugCommandAdditionalData();
            else
                _additionalData = additionalData;
        }
    }
    public class DebugCommandAdditionalData //Minun lisäykset
    {
        public bool doNotCloseMenu = false;
        public bool isToggle = false;
        public bool ToggleValue = false;
        public bool isSlide = false;
        public float SlideValue = 0;
        public bool isText = false;
        public string TextValue = "";
    }
    public class DebugCommand : DebugCommandBase
    {
        private System.Action command;
        public DebugCommand(string id, string description, string format, System.Action command, DebugCommandAdditionalData additionalData = null) : base(id, description, format, additionalData)
        {
            this.command = command;
        }
        public void Invoke()
        {
            command.Invoke();
        }
    }
    public class DebugCommand<T1> : DebugCommandBase
    {
        private System.Action<T1> command;
        public DebugCommand(string id, string description, string format, System.Action<T1> command, DebugCommandAdditionalData additionalData = null) : base(id, description, format, additionalData)
        {
            this.command = command;
        }
        //public DebugCommand(string id, string descrption, string format, System.Action<T1> Command) : base(id, descrption, format) {
        public void Invoke(T1 value)
        {
            command.Invoke(value);
        }
    }
}
