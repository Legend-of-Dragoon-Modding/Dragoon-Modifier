# Dragoon Modifier
The open-source version of the Difficulty Modifier, a RAM mod tool for Legend of Dragoon running on PSX emulators. https://legendofdragoonhardmode.wordpress.com/  
Using memory.dll for RAM reading and writing https://github.com/erfg12/memory.dll  
and CS-Script as a scripting engine https://github.com/oleg-shilo/cs-script  
Works with the NTSC, PAL, and Japan regions of the game.  
Supports ePSXe 1.6.0 -> ePSXe 2.0.5, RetroArch with Beetle PSX HW core, and PCSX2  
  
# For Users  
Activated scripts are in black text, deactivated scripts are in red. To activate a script single click the script to select it and then press the gray button on top of the script to change it's state. Some scripts require input, in this case you would double click it. You can change your emulator and region by going into "Settings" at the top menu bar. Once you see the following message "Program opened." Dragoon Modifier is running. Dragoon Modifier will only stop running when you see the following message "Fatal Error. Closing all threads." or close the program.  
  
# For Developers  
All scripts have the following four methods: Run(), Click(), Open(), and Close(). Run() is executed in the loop thread for that script. Click() is executed when the user double clicks a thread. Open() is on program open. Close() is on program exit.  
For organization purposes, if a script is only done on the field it should be in the Field folder. Anything that has multiple should go in the Other folder. IF the script is generic it should go in the main folder of Field/Battle/Hotkeys/Other. If a script is specific for a mod you are making make a sub folder in on other the folders above. Dragoon Modifier will pick up anything labeled with a type of ".cs".  
Also all scripts should start with their appropriate region codes. When switching regions scripts incompatible will be disabled.  
  
[USA]  
[JPN]  
[EUR]  
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
