### Version 0.1.1
- Fixed bug caused by incorrect file version in AssemblyInfo.cs

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
