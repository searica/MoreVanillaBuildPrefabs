### Version 0.3.4
- Improved naming for custom pieces in hammer build table.
    - Format of custom piece names is now consistent with vanilla name formatting.
	- Some spelling inconsistencies from the game's internal ID's have been corrected.
- Automatically add hover text if missing for custom pieces (depending on the piece it still may not display).
- Ptched and enabled more prefabs by default.
- Disabled a prefab that explodes into a giant boulder (Thanks Cass!)
- Tweaked build requirements and costs for some prefabs.
- Patched placement of several pieces.
    - Improved placement of dvergr poles and wood pieces.
	- Fixed issue with some black marble pieces moving after placement due to discrepency between colliders and rigid bodies.
- Changed how piece Icons are generated to hopefully fix the lighting issue with some icons.

### Version 0.3.3
- Fix color artifacts in custom piece icons (Thanks again for your help Margmas).
- Fix bug that I accidentally re-introduced where world-generated CreatorShop pieces could be deconstructed.
- Added SearsCatalog as a Thunderstore dependency.
- More internal refactoring and clean-up to get ready for possibly adding some new features.

### Version 0.3.2
- Update to Jotunn 2.14.4
- Changed priority of patch for adding prefabs to fix partial incomparability with WackyDB.
- Internal refactoring to clean up code and make managing methods easier.
- Enabled some more pieces by default.
- Added EffectsList patch from PotteryBarn to fix null exceptions when using custom Armor Stands.

### Version 0.3.1
- Added NullException checks to fix compatibility issues with CreatureLevelAndLootControl.
- Changed mod to search for prefabs every time a game session is joined (has minimal impact on load time, < 50 ms on average) to prevent null prefab errors.
- Added error handling to catch incorrect build requirement ID's and throw a warning to the log.
- Thanks to Cass again for letting me know about the compatibility issue and testing out the fixes.

### Version 0.3.0
- Implemented built-in cfg file watcher to ensure changes made to cfg file are not erased.
- Fixed crashing issue with some prefabs and re-enabled them by default.
- Changed when custom pieces are added to wait until after receiving data from ServerSync (Thanks to Cass for reporting the issue and to Wackymole for helping figure out which method to patch).
- Changed method of adding custom pieces due to Null Exception error caused by adding pieces with Jotunn after ZNet.Start(), will probably switch back after Jotunn updates.

### Version 0.2.2
- Added null check to EnsureNoDuplicateZNetView(), should resolve issues caused when rejoining servers (Thanks to Cass on the Odinplus for reporting the bug).
- Mod now saves the cfg file on logout, should hopefully preserve changes made to it before reading from it when rejoining a server.

### Version 0.2.1
- Fixed clipping and placement for several prefabs.
- Adjusted snap points on a few prefabs.
- Disabled CargoCrate prefab due to failing to instantiate upon placement.
- Code clean up.

### Version 0.2.0
- Reduced load time from ~30 seconds to ~0.5 seconds (Thanks to onnan for reporting the issue and to Margmas on the OdinPlus discord for the tip on reducing config file load times.)
- Switched to using ZNetScene for patch to trigger removal of custom pieces on logout.
- Internal code refactoring and clean up.

### Versions 0.1.X
<details>
<summary>details</summary>

### Version 0.1.4
- Updated for patch 0.217.22

### Version 0.1.3
- Updated for Jotunn 2.14.2
- Removed three prefabs that caused a crash when re-logging. (This should fix compatibility issues with the Multiverse mod)
- Improved load times when re-logging.
- Changed method of adding custom build pieces to respect server configuration when changing between servers without restarting the game.
- Added configuration option to restrict placement of CreatorShop pieces to Admins.

<details>
<summary>Click to see specific prefabs that were removed (contains spoilers).</summary>

- blackmarble_tile_wall_1x1
- blackmarble_tile_wall_2x2
- blackmarble_tile_wall_2x4

</details>

### Version 0.1.2
- Fixed ILRepacker not merging ServerSync assembly when creating Release version of Thunderstore mod package.

### Version 0.1.1
- Fixed bug caused by incorrect file version in AssemblyInfo.cs
- Thanks to BLUBBSON on Github for letting me know about the bug.

### Version 0.1.0
**Big updates**
- ServerSync has now been implemented.
- Added a setting to allow admins to deconstruct CreatorShop pieces built by other players.
- Add a configuration option for each prefab that enables a generic collision patch to allow users to possibly fix placing prefabs that have not been custom patched yet.
- Improved configuration file to provide configuration descriptions and a list of acceptable values for each configuration option.
	- Crafting station names are now descriptive instead of based on the item_id in-game.

**Minor Changes**
- Tweaked resource requirements for better balance.
- Enabled more build pieces by default after tweaking the resource requirements to prevent them unlocking several biomes before they would normally be encountered by players.
- Fixed Github link in Thunderstore manifest (had to remake it and had copied the wrong template manifest).
- Improved README formatting and fixed spelling/grammar in various places.

**You need to regenerate your configuration file again.** While I didn't touch the name scheme of the config file the additional configuration options and changes to naming for crafting station configuration mean you need to regenerate the config file.

### Version 0.0.3
- World-generated pieces now drop only their default resource drops while player-built pieces drop only the resources used to build them.
- Readme updated and cleaned up (that's what I get for writing it at 1am last time).
- Configuration file naming scheme changed due to automating the process. **You need to regenerate your configuration file and copy over any customizations you made.** Sorry for the inconvenience, future updates will not touch the configuration file naming scheme again.

### Version 0.0.2
- Updated readme and added links to source code.

### Version 0.0.1
- Initial release.

</details>
