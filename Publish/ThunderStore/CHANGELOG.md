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
