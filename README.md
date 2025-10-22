# Dragoon Modifier for Emulators is Discounted
Dragoon Modifier has moved over to the Severed Chains PC Port. Any bugs or issues with the current Emulator version will not be fixed.
If you are using Dragoon Modifier for Emulators please try to keep to the final stable release of v3.5.5 as it has the less amount of bugs.
https://legendofdragoon.org/guides/setup-severed-chains/
https://github.com/Legend-of-Dragoon-Modding/Severed-Chains
https://github.com/Legend-of-Dragoon-Modding/sc-dragoon-modifier

# Dragoon Modifier
The open-source version of the Difficulty Modifier, a RAM mod tool for Legend of Dragoon running on PSX emulators. https://legendofdragoonhardmode.wordpress.com/  
Using memory.dll for RAM reading and writing https://github.com/erfg12/memory.dll  
and CS-Script as a scripting engine https://github.com/oleg-shilo/cs-script  
Supports ePSXe 1.6.0 -> ePSXe 2.0.5, RetroArch with Beetle PSX HW core, and PCSX2  
  
# For Users  
In the settings tab activated scripts are in black text, deactivated scripts are in red. To activate a script single click the script to select it and then press the grey button on top of the script to change it's state. Some scripts require input, in this case you would double click it. The current script loadout can be saved or loaded by clicking Menu > Save/Load. You can change your emulator and region by going into "Settings" at the top menu bar. Once you see the following message "Program opened." Dragoon Modifier is running. Dragoon Modifier will only stop running when you see the following message "Fatal Error. Closing all threads." or close the program.  
  
# For Developers  
All scripts have the following four methods: Run(), Click(), Open(), and Close(). Run() is executed in the loop thread for that thread. Click() is executed when the user double clicks on the script. Open() is on program open. Close() is on program exit.  
For organization purposes, if a script is only done on the field/battle it should be in the Field/Battle folder. All hotkeys should go into the Hotkey folder. Anything generic or for multi use in field/battle it should go in the Other folder. If a script is specific for a mod you are making make a sub folder in on other the folders above. Dragoon Modifier will pick up anything labeled with a type of ".cs" within the Scripts folder.  
  
Also all scripts should start with their appropriate region codes. When switching regions scripts incompatible will be disabled.  
  
[NTA] - NTSC - North America / Russian Fan Translation  
[JPN] - NTSC - Japan  
[GER] - PAL - German  
[FRN] - PAL - French  
[ITL] - PAL - Italian  
[SPN] - PAL - Spanish  
[ENG] - PAL - UK English  
[ALL]  

# Credits  
Dragoon Modifier uses the following projects for functionality.  

Memory.dll - Read/Write/Scan memory functions  
https://github.com/erfg12/memory.dll  
Source modified from commit 180 to scan for all address types for RetroArch functionality.  
Fork of the source code (it ain't much) used in this project is here.  
https://github.com/Zychronix/memory.dll  
  
CS-Script - Scripting engine used for modularity.  
https://github.com/oleg-shilo/cs-script  
  
XAML Radial Progress Bar - Reader Mode's radial progress bars.  
https://github.com/panthernet/XamlRadialProgressBar  
Source modified from commit 19 to not draw the full circle all the time.  
  
Extended WPF Toolkit™ by Xceed - Reader Mode's colour picker  
https://github.com/xceedsoftware/wpftoolkit  
  
MahApps.Metro - UI  
https://github.com/MahApps/MahApps.Metro  
  
Lorc - VoodooDod - Battle Stats Tab's icons.  
https://www.reddit.com/r/IndieGaming/comments/ifmie/i_made_700_rpg_icons_free_for_use_for_your_game/  
https://opengameart.org/content/rpg-icon-font  
  
Special Mentions  
Spyrokid - https://www.youtube.com/user/spyrokid77666 - For helping me find the BGM sequence  
Xifanie/NoOneeee - http://www.romhacking.net/forum/index.php?topic=23231 - For extended item inventory code for the game  
Illeprih - https://github.com/Illeprih - For nuking just about half of the original Visual Basic code of the original project. 
