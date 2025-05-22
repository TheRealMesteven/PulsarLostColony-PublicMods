Mod Name: QuickChat
Mod Author: Badryuiner, Mest
Mod Version: 0.2.2
Game Version: 1.2.07
PML Version: 0.12.3.31

Description:
Detects keypresses when not in chat and runs bindable command / phrase.

Due to the latest PML update, keybindings can be created by mod authors.
This allows you, the users to also do this. It is useful for Joystick / VR controls
as you can get them to emulate whichever key you want to bind a command to.

Install Instructions:
-Have the Pulsar-Mono beta selected.
-Have PulsarModLoader Installed.
-Drag and drop QuickChat.dll into PULSARLostColony\Mods
-Drag and drop QuickChat.Json into PULSARLostColony\

Usage:
> Use 'QuiChat_KeyCodes.txt' to find the key name of the keybind you want.
> Edit 'QuickChat.json' with a notepad editor to include the keybind 
and command / phrase. FOLLOWING JSON FORMATTING!

Example:
[
  {
    "key": "Equals",
    "msg": "/help"
  },
  {
    "key": "Minus",
    "msg": "Sometimes I dream about cheese"
  }
]

> Reload the game or use '/qcr' to reload the keybindings.

Commands:
/qcr - Reloads the keybindings.