# MoreVanillaBuildPrefabs
MoreVanillaBuildPrefabs is a Valheim mod to make all vanilla prefabs buildable with the hammer (survival way) while allowing you to configure the requirements to build them.

### Acknowledgements
This mod was inspired by MoreVanillaBuilds by Galathil and PotteryBarn by ComfyMods and the core functionality of the code is based on those two mods.

## Key Feature
Because all the added build pieces are pre-existing vanilla prefabs, any pieces you build with this mod will persist in your world even if you uninstall the mod. This means that pieces you build on a server will also be visible for players without the mod and any builds using the pieces from this mod will load for players without the mod.

## Instructions
If you are using a mod manager for Thunderstore simply install the mod from there.

If you are not using a mod manager then, you need a modded instance of Valheim (BepInEx) and Jötunn plugin installed.

1. Download the last MoreVanillaBuilds.dll available in the releases section.
2. Place the MoreVanillaBuilds.dll into your BepInEx\plugins folder
3. You need launch the game first (and enter in a world) to generate the configuration files. The plugin search for prefabs in the game loading screen.
4. (Optional) Stop the game. You found a searica.valheim.morevanillabuildprefabs.cfg in your BepInEx\config folder, open it to customize mod configuration (describe below)
5. Play the game using the default configuration generated by the mod or your customized one.
6. (Recommended) Install SearsCatalog (https://valheim.thunderstore.io/package/ComfyMods/SearsCatalog/) to extend the hammer build table and allow you to access all the pieces this mod adds even if there are too many added pieces for the vanilla build table.

## Configuration
You need to edit the configuration file with client/server off ! If you use an in-game configuration manager, you need to restart the game/server to apply configuration.

For each detected prefab in the game you can:
- Enable/Disable it from the hammer.
- Define custom recipe to build the prefab in-game.
- Set the required crafting station to build and deconstruct the prefab.
- Set whether the prefab is allowed to be built inside of dungeons.

### _Global Section Configuration:
- EnableMod. Default = true.
  - Globally enable or disable this mod (restart required).
- forceAllPrefabs. Default = false.
  - Setting to true overrides individual configuration for all prefabs and enables them all for building with the hammer.
- verboseMode. Default = false.
  - You should keep it to false. This configuration displays debug information in the console (and slows down game performances)

### Prefab Configuration Sections:
The rest of the configuration files contains [xxxxxx] sections to configure each prefab. Each section contains:

- isEnable = false
  - Change it to true to show the prefab in the hammer. Note that if forceAllPrefabs is set to true, this config is ignored.
- AllowedInDungeons = false
  - Whether the piece can be built inside of dungeons, set to true to allow building it in dungeons.
- Category = CreatorShop
  - This sets tab the prefab should appear. Vanilla categories are: Misc | Crafting | Building | Furniture. This mod adds a fifth category "CreatorShop", see section on CreatorShop pieces.
- CraftingStation =
  - Set the required crafting station. Vanilla crafting station IDs are: piece_workbench | forge | piece_stonecutter | blackforge. Leaving it empty means that it can be built and deconstructed without a crafting station.
- Requirements =
  - The requirements to build the prefab. By default, no requirements needed (like creative mod). Each requirement is separated by a semicolon (;). Each requirement contain the itemID and the quantity separated by a comma (,). You can find itemID on Valheim Wiki or on this link : https://valheim-modding.github.io/Jotunn/data/objects/item-list.html. Example : requirements = Wood,5;Stone,2, in this case you need 5 woods and 2 stones to build the prefab

### Default Prefab Configuration
A number of prefabs have default crafting requirements upon the generation of the configuration file and a subset of those are set to be enabled for building with the hammer. These configurations are based on my liking and trying to ensure that someone playing with the mod will not unlock various build pieces long before they have encountered them in the world. The default configuration also means that the mod can simply be installed and used immediately to get a sense of how it works. You are of course able to change these default configurations however you please.

### Default Enabled Pieces
<details>
  <summary>Click to see a general list of enabled pieces (contains spoilers.)</summary>

  - Most black marble pieces used in Dvergr builds.
  - All Dvergr furniture.
  - Most Dvergr wooden structures.
  - Dvergr demisters.
  - Various rocks.
  - Extra furniture and decorations.
  - Turf roofs.
  - Statues.
  - Wood ledge.

  See the PrefabConfig.cs file in the source code for the full default configuration or install the mode and check the generated configuration file.
</details>

## Implementation Notes
The prefabs enabled by this mod were not necessarily intended to be built by players, so they may lack some of the things required to place them or build them such as proper collision or snap points.

All of the pieces that are enabled by default have been patched to have snap points or fix collision issues and ensure they can be built with similarly to existing vanilla pieces. While many other prefabs that are not enabled by default have been patched, the process of patching them is currently entirely manual so not all prefabs have been patched. This means that if you enable prefabs other than the ones enabled by default there is no guarantee they will behave nicely.

If you are having issues with a prefab you would like to build with but it won't appear when you try to place it the issue is likely due to missing colliders that have not been patched yet. A work around for some prefabs is to install either [Snap Points Made Easy](https://valheim.thunderstore.io/package/MathiasDecrock/Snap_Points_Made_Easy/)  by MathiasDecrock or the mod [Extra Snap Points Made Easy](https://valheim.thunderstore.io/package/Searica/Extra_Snap_Points_Made_Easy/) by Searica (that's me). Both of these mods allow you to manually select the snap points on the piece you are placing and the piece you are snapping to. Manually selecting the snap points can allow you to place a prefab that otherwise does not show up due to missing colliders. Some prefabs used for making dungeons can also behave unexpectedly such as being able to open a door but not close it.

TLDR: Some prefabs other than the ones enabled by default may be buggy, please adjust your expectations accordingly.

### Deconstructing Pieces
Since this mod adds more prefabs to the hammer, that means you can deconstruct more pieces. Currently the mod is designed so that when you deconstruct world-generate pieces they drop their normal item drops plus some fraction of the additional build resources added to them by this mod (if the prefab is enabled). For pieces that have been built by the player, they will only ever drop the resources used to build them.

### CreatorShop Pieces
If a prefab has been set to the CreatorShop category in the configuration then it will behave slightly differently than pieces from other categories.

Specifically, when a piece is set to the CreatorShop category player's can only deconstruct instances of that piece that they have placed themselves. This prevents player's from deconstructing world-generated prefabs like trees while still allowing you to build and deconstruct player-placed trees. If multiple player's have this mod enabled they can still only deconstruct CreatorShop pieces that they have placed themselves.

## Known Issues
Placing armor on the Male Armor Stand and Female Armor Stand prefabs have clipping issues where not all of the armor is displayed. I have not been able to fix this as of yet.

## Compatibility
This is a non-exhaustive list.

### Incompatible Mods
Likely incompatible with other mods that add Vanilla prefabs to the build hammer unless you disable the prefabs from this mod that overlap with the other one since conflicting build requirements can cause unexpected behavior.
- MoreVanillaBuilds (by Galathil)
- PotteryBarn (by ComfyMods)

### Compatible Mods
- AdventureBackpacks (by Vapok)
- Aegir (by blbrdv)
- AAACrafting (by Azumatt)
- AzuCraftyBoxes (by Azumatt)
- AzuMapDetails (by Azumatt)
- AzuAreaRepair (by Azumatt)
- AzuClock (by Azumatt)
- AzuExtendedPlayerInventory (by Azumatt)
- AzuSkillTweaks (by Azumatt)
- Backpacks (by Smoothbrain)
- Better Beehives (by MaxFoxGaming)
- BetterArchery (by ishid4)
- BetterCarts (by TastyChickenLegs)
- BetterPickupNotifications (by Pfhoenix)
- BetterUI Reforged (by the defside)
- BowsBeforeHoes (by Azumatt)
- BuildRestrictionTweaks (any version that is a reupload of Aedenthorn)
- ComforTweaks (by Smoothbrain)
- ComfySigns (by ComfyMods)
- CraftingFilter (by Aedenthorn)
- DeathPinRemoval (by Azumatt)
- DeezMistyBalls (by Azumatt)
- DodgeShortcut Reupload (by NetherCrowCSOLYOO)
- EmoteWheel (by virtuaCode)
- EulersRuler (by ComfyMods)
- Evasion (by Smoothbrain)
- Extra Snap Points Made Easy (by Searica)
- FascinatingCarryWeight (by kangretto)
- FastTeleport (by GemHunter1)
- FastTools (by Crystal)
- Gizmo (by ComfyMods)
- HeyListen (by ComfyMods)
- ImFRIENDLY DAMMIT (by Azumatt)
- Instantly Destroy Boats and Carts (by GoldenRevolver)
- InstantRuneText (by Azumatt)
- MapTeleport (by Numenos)
- Mead Base Icon Fix (by Sulyvana)
- MultiUserChest (by MSchmoecker)
- NoSmokeStayList (by TastyChickenLegs)
- OdinsFoodBarrels (by OdinPlus)
- PassivePowers (by Smoothbrain)
- Pathfinder (by Crystal)
- Perfect Placement (by Azumatt)
- PlanBuild (by MathiasDecrock)
- PlantEasily (by Advize)
- PlantEverything (by Advize)
- ProfitablePieces (by Azumatt)
- QoLPins (by Tekla)
- Queue Me Maybe (by Azumatt)
- Quick Stack Store Soft Trash Restock (by Goldenrevolver)
- QuickConnect (by bdew)
- QuietyPortals (by Neobotics)
- Ranching (by Smoothbrain)
- RepairAll (by LoadedGun)
- Sailing (by Smoothbrain)
- SearsCatalog (by ComfyMods)
- Server devcommands (by JereKuusela)
- ShieldMeBruh (by Vapok)
- SilenceTameWolfCub (by GetOffMyLawn)
- Snap Points Made Easy (by MathiasDecrock)
- SNEAKer (by black7ar)
- Sorted Menus Cooking Crafting and Skills Menu (by Goldenrevolver)
- SpeedyPaths (by Nextek)
- StackedBars (by Azumatt)
- StaminRegenerationFromFood (by Smoothbrain)
- SuperConfigurablePickupRadius (by TastyChickenLegs)
- TargetPortal (by Smoothbrain)
- ToggleMovementMod (by GetOffMyLawn)
- Veinmine (by WiseHorrer)
- Venture Location Reset (by VentureValheim)
- VikingsDoSwim (by blacks7ar)
- WieldEquipmentWhileSwimming (by blacks7ar)
- Probably even more, it's pretty compatible.

### Source Code
Github: https://github.com/searica/MoreVanillaBuildPrefabs
