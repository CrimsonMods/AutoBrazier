# AutoBrazier

## Install (Manual)
- Install BepInEx
- Install Bloodstone
- Extract AutoBrazier.dll into (VRising server folder)/BepInEx/plugins (or BepInEx/BloodstonePlugins)
- [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/) recommended for in-game hosted games

# How to use
- Braziers will automatically turn on and off with day/night for online players/clans.

# Configuration
```
## Settings file was created by plugin AutoBrazier 0.1.0
## Plugin GUID: AutoBrazier

[Server]

## Turn braziers on when day starts, and off during the night starts, for online players/clans only.
# Setting type: Boolean
# Default value: true
autoToggleEnabled = true
```

# Changelog
0.1.0 - 5/18/24 : 
- Updated [QuickBrazier](https://github.com/harminded/QuickBrazier) by harminded to work with V Rising 1.0
