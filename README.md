Compares params for FromSoft games.

Uses https://github.com/JKAnderson/SoulsFormats and https://github.com/soulsmods/Paramdex
Uses updated versions of SoulsFormats and Paramdex from https://github.com/soulsmods/DSMapStudio

## Changes
### v1.4.0
* Consolidated DS1 defs to allow for cross-game comparisons.
* * DSR defs take priority due to updated pad info.
* * PTDE defs are now located in Paramdex ALT.
* 
### v1.3.0
* Updated DS2 Paramdefs and row names.
* Removed DS2S Paramdex folder and placed everything into DS2 folder.
* Added Support DS2 params from "enc_regulation.bnd.dcx".
* Fixed crash when SoulsFormats failed to read a param file.
* Error logging is now more detailed.
* Single-line logging is now off by default.
* Misc text tweaks.
### v1.2.4
* Added option to import row names from Paramdex.
* Added ParamDefs to handle Elden Ring v1.08 changes.
* Fixed output file not being opened in windows explorer when prompted.
### v1.2.3
* Added function to drag & drop files over old/new param UI elements to load them.
* Fixed multi-line logging not putting 1st change on a new line.
* When no changes have been found, put "No changes have been found" in the log.
* Output path now includes extra folder with targeted game name.
* Generate empty Paramdex ALT folders on startup.
* Added "Info" menu that mentions obscure functionality.
### v1.2.2
* Improved program stability (it should be very stable now).
* Fixed individual param comparison applying param defs twice.
* Fixed FinalDamageRateParam def.
### v1.2.1
* Fix crash when trying to compare games other than ER and DS1.
* Fix individual param comparisons when files had different names. Also updated def checks to match multi-param comparisons.
* Added option to log every field for added & removed rows.
* Added up to date Elden Ring ChrModelParam paramdef.
* Improved formatting for multi-line row logging.
* Slightly improved program stability.
### v1.2.0
* Log specifics when ParamDefs cannot be applied.
* Added ParamDefs to handle Elden Ring v1.07 changes.
* Improve param added/removed detection logic.
* Increase program stability by using ConcurrentDictionary.
* Include program info in diffs & display version number in program.
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
