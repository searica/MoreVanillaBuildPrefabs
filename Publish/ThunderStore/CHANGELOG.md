### Version 0.1.0
- Tweaked resource requirements for better balance.
- Enabled more build pieces by default after tweaking the resource requirements to prevent them unlocking several biomes before they would normally be encountered by players.
- Fixed Github link in Thunderstore manifest (had to regenerate it and had copied the wrong manifest).
- Improved configuration file to provide configuration descriptions and a list of acceptable values for each configuration option.
  - Crafting station names are now descriptive instead of based on the item_id in-game.
- Added configuration option to enable a collision placement patch on a per prefab basis. Allows users to potentially fix placement issues with prefabs I have not got around to making custom patches for yet.
- **You need to regenerate your configuration file again.** While I didn't touch the name scheme of the config file the additional configuration options and changes to naming for crafting station configuration mean you need to regenerate the config file.

### Version 0.0.3
- World-generated pieces now only their default resource drops while player-built pieces only drop the resources used to build them.
- Readme updated and cleaned up (that's what I get for writing it at 1am last time).
- Configuration file naming scheme changed due to automating the process. **You need to regenerate your configuration file and copy over any customizations you made.** Sorry for the inconvenience, future updates will not touch the configuration file naming scheme again.

### Version 0.0.2
- Updated readme and added links to source code.

### Version 0.0.1
- Initial release.
