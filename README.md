# DCS Configuration Manager

### [Download](https://github.com/TheFitzZZ/DCS-Config-Manager/raw/master/DCS-ConfigMgmt/Installer/DCSCM-Setup.zip) current version 2017 R2
**SmartScreen will still tell you it's not signed ... even though it is :-( - Will try to fix this asap**

### What's this?
The DCS Configuration Manager was created to make the tough DCS life a little easier and help you with those pesky configuration files! The goal was to make you interact with the app itself as little as possible, as noone likes more overhead before you can go fly your sortie! Use it once, set everything up - done!


![DCSCM](https://github.com/TheFitzZZ/DCS-Config-Manager/blob/master/DCM.gif)


### Features:
- **Shared, single-instance control mappings!**
Links your control settings of all your DCS instances to just one. No more import/export when you changed your mappings and switch from 1.5 to 2.0! (or to beta, or any direction really).
- **VR configuration management!**
Don't own a GeForce 1280ti yet? Need to turn down you GFX to play VR every time and switch it back later? Now you can change between VR and non-VR configurations with just one click in the app. Don't want to start the app? Let it create shortcuts for you, so you can directly launch the right version with the right configuration!

- **Updater!** Starting the app will also check for updates, so you don't miss out on new features. Doesn't happen when you use the app created shortcuts, though. DCS startup times are long enough...

![DCSCMIcons](https://github.com/TheFitzZZ/DCS-Config-Manager/blob/master/icons.PNG)

### Things to note:
- Windows 10 SmartScreen will not trust it in the early weeks of release (until I know how to stop it from ignoring the certificate when downloaded from the internet)
- If you're really paranoid (and I won't blame you for that), grab the source, check the code and compile it yourself.

### Changelog:
02.11.2017 - RC7 -> R1
- Warning message added for settings tab
- Reset overhaul to include the "clone"
- Fixed first run symlink detection (just visual problems)
- Added prevention against starting two instances (e.g. app + shortcut clone)
- Renamed "link controls" to "shared control" and overhauled instruction text for more clarity
- Will now detect malformed options.lua files and quit before I put your settings into a blender. Nice of me, eh?

01.11.2017 - RC6
- Various fixes for crashes and inconsistencies
- Binaries are now signed - such cert, very trust, much awesome - but when you download the zip SmartScreen still yells at you... this is very much not awesome! (Will try to fix next release)
- First start won't switch configs anymore ... crisis averted!

30.10.2017 - RC5
- Added settings reset button ... a little hidden - you don't need that anyway!
- Added checks so a re-install doesn't hurt anymore (hopefully)

28.10.2017 - RC4
- Added line tab separator
- Disabled text wrap for directory textbox and lowered font size for more visibility
- Anti-Aliasing enabled for images
- Now automatically prepares configs for VR/non-vr
- Added global configuration / temp application directory for shortcuts



Please note: this project is neither related nor endorsed by Eagle Dynamics or The Fighter Collection. It's just a fan project for the community. If you as a rights holder have any issues with used assets, please contact me and I'll resolve this asap.
