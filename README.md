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
