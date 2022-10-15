Compares params for FromSoft games.

Uses https://github.com/JKAnderson/SoulsFormats and https://github.com/soulsmods/Paramdex

## Changes
### v1.1.5
* Add additional ParamDefs to handle Elden Ring v1.07 changes.
* Increase program stability by using ConcurrentDictionary.
### v1.1.4
* Support multiple loading ParamDefs for cross-def comparisons
  * To use alternative ParamDefs, place them into the Paramdex ALT folder (you will need to create folders for most games).
  * If you have an alternative ParamDef that should be included with the program, let me know and I'll put it in.
* Fixed a threading crash when trying to log names.
### v1.1.3
* Catch InvalidDataException when applying ParamDefs
* Identify Params by ID instead of name
### v1.1.2
* Properly check for added/removed params
### v1.1.1
* Log entire changed byte array (in hex) instead of just the changed byte (as a decimal)
### v1.1
* Now compares byte arrays (such as with padding)

![image](https://user-images.githubusercontent.com/55667610/172688216-9231f031-6eea-44d1-9801-1e8b4c05f4e1.png)
