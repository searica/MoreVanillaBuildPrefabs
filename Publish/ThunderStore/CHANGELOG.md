<div class="header">
	<h2>Versions 1.X.X</h2>
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Version</th>
			<th align="center">Notes</th>
		</tr>
		<tr>
			<td align="center">1.6.8</td>
			<td align="left">
				<ul>
					<li>Okay, the compatibility issue with PlantEverything is actually fixed now. Turned out that there was still an unintended interaction preventing the cultivator from removing things if you did not add any pieces from MVBP to the cultivator in you config.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.7</td>
			<td align="left">
				<ul>
					<li>Fixed new compatibility issue with PlantEverything that prevented you from using the cultivator to remove pickables if you enabled CanRemoveFlora in PlantEverything.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.7</td>
			<td align="left">
				<ul>
					<li>Fixed new compatibility issue with PlantEverything that prevented you from using the cultivator to remove pickables if you enabled CanRemoveFlora in PlantEverything.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.6</td>
			<td align="left">
				<ul>
					<li>Bugfix for serving tray not being able to remove the pieces it places while MVBP is installed. Serving tray now works normally and correctly handles any pieces added to it using MVBP.</li>
					<li>Fixed compatibility issue with PlantEverything that allowed you to remove any piece added by MVBP with the cultivator if you enabled CanRemoveFlora in PlantEverything.</li>
					<li>Fixed bug when removing non WearNTear pieces that prevented the zdo from being properly destroyed.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.5</td>
			<td align="left">
				<ul>
					<li>Adjust default portal placement settings so that it stops clipping into the ground when not placed on a floor.</li>
					<li>Adjusted patch priority to enforce stable interactions with snapping to custom points from ExtraSnapPointsMadeEasy.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.4</td>
			<td align="left">
				<ul>
					<li>Added a new config setting to allow player build fortress pieces to become invunerable to damage from anything with a lower tool tier than seige weapons. Does not affect actual fortress locations at all.</li>
					<li>Added snap points to almost all ashlands build pieces. Some pillar and roof pieces still do not have snap points yet.</li>
					<li>Implemented the same automatic snap point naming conventions as for Extra Snaps Made Easy, so most custom snap points added by this mod will now be named "Extra" unless I have manually named them.</li>
					<li>Tweaked various build pieces to add snap points names for the custom snap points added by this mod. Nowhere near complete, but it's a start.</li>
					<li>Adjusted placement of the Dvergr Portal piece so that it is now allowed to clip into the ground and will do so when placing. This avoids the issue of not actually being able to walk into the portal due how sharp the rim of the platform is.</li>
					<li>Caught some capitialization mistakes I had missed last update (hopefully got them all now).</li>
					<li>Added custom name for Ashlands_Altar prefab and tweaked default build resources.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.3</td>
			<td align="left">
				<ul>
					<li>Updated custom piece names to match current capitalization scheme.</li>
					<li>Added custom names for all grausten ashlands build pieces to make the purpose of each piece clearer and avoid spoilers.</li>
					<li>Added custom piece descriptions for all charred fortress build pieces to hint at which ones require seige weapons to break and which ones can be broken by anything because the base game forgot to set the value to require a higher tool level.</li>
					<li>Rebalanced default resource costs for all grausten ashlands build pieces.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.1/1.6.2</td>
			<td align="left">
				<ul>
					<li>Quick fix for removing pieces with the hoe and cultivator. The hammer can always remove anything you place but the hoe and cultivator can only remove things you place with them.</li>
					<li>Second quick fix to correct a typo that I made between testing it worked and building the release version that broke piece removal entirely.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.6.0</td>
			<td align="left">
				<ul>
					<li>Fixed bug with purple brazier collider layer and changes to the Dvergr portal not persisting without MVBP installed.</li>
					<li>Configured default resource costs and enabled more ashlands pieces by default.</li>
					<li>Increased the inventory sizes of chest unlocked in the mistlands and later.</li>
					<li>Improved automatic piece tagging for build menu filtering.</li>
					<li>Improved automatic sorting of pieces added by MVBP within the build menu.</li>
					<li>Added configuration options to set which piece table a prefab should be added to so that pieces can be added to the hammer, hoe, or cultivator.</li>
					<li>Adjusted the default piece table config settings for most nature based pieces to move them to the hoe and cultivator.</li>
					<li>Enabled advanced piece table options for the hoe and cultivator, but only when there are pieces added to it by MVBP, otherwise it retains default behaviour.</li>
					<li>Added more custom categories to improve filtering of pieces added by MVBP. Currently all pieces in custom categories that are added by MVBP are considered "CreativeMode" pieces.</li>
					<li>Some small performance improvements and tidying up of code.</li>
					<li>Updated default prefab configs to include every prefab in the game and updated python script for generating default configs from a config file. Not really useful for most mod users but nessecary for me to configure default for any new prefabs.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.5.2</td>
			<td align="left">
				<ul>
					<li>Fixed vanilla bug where placing a cape on the male/female armor stands threw a bunch of errors and could crash the game.</li>
					<li>Fixed the comfort group category of the male/female armor stands not always being changed to the "Display" group.</li>
					<li>Attempted to fix issue with trailership and SnowFirTree 2 but that will need more work.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.5.1</td>
			<td align="left">
				<ul>
					<li>Fixed seasonal pieces not being enabled until you toggled the seasonal pieces config setting.</li>
					<li>Patched player built eternal braziers to provide warmth in the same radius as normal braziers. Note: only works while MVBP is installed.</li>
					<li>Set the comfort group for eternal braziers and other sources of fire to the Fire comfort group, this only applies to player built pieces and should persist after removing MVBP.</li>
					<li>Set the fancy armor stands to be part of the display comfort group.</li>
					<li>Adjusted default config settings for portal prefab.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.5.0</td>
			<td align="left">
				<ul>
					<li>Updated for Deep North release!</li>
					<li>Removed dependency on SearsCatalog as it is no longer needed.</li>
					<li>Added automatic tagging of new pieces so they are sorted correctly in the new build menu.</li>
					<li>Added more config options for piece categories to match the current game options (note: this no longer seem to affect placement in build menu).</li>
					<li>Config file will auto update to find all possible pieces after you start up the game, be warned that I have not tested or patched any of the new pieces it will find yet!</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.4.4</td>
			<td align="left">
				<ul>
					<li>Bugfix for piece removal.</li>
					<li>update for new game version.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.4.3</td>
			<td align="left">
				<ul>
					<li>Bugfix for icons of pieces with texture patches not always updating on restart.</li>
					<li>Improved handling of if a piece should be remove-able.</li>
					<li>All pieces you build are now always remove-able by you.</li>
					<li>For any prefab that MVBP adds a Piece component to, only player built instances of it are removeable with the hammer.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.4.2</td>
			<td align="left">
				<ul>
					<li>Fix boss stones not dropping trophy.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.4.1</td>
			<td align="left">
				<ul>
					<li>Bugfix not emptying loot from placed chests.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.4.0</td>
			<td align="left">
				<ul>
					<li>Removed portal patch config setting. The Dvergr stone portal now allows allows you to teleport all items (even after removing MVBP).</li>
					<li>Added support for shudnal's configuration manager.</li>
					<li>Added snap points to Dvergr treasure chests.</li>
					<li>Limited implementation of named snap points. Will be expanded on in the future.<li>
					<li>Updated config section ordering to work with new configuration manager changes to sorting config sections.</li>
					<li>Exposed placement patch and clipping config settings for all prefabs but ones that need to be true are set to read only.</li>
					<li>Refactored a lot of code to make future updates easier and prepare for adding localization support.</li>
					<li>Removed public API for `IsPieceAddedByMVBP` as it was not needed for `LocationReset` compatibility.</li>
					<li>Added filtering to ignore explosions and AOE effects.</li>
					<li>Automatically filter out MineRock5 prefabs with missing colliders.</li>
					<li>Set default requirements to an example string rather than an empty string to avoid draw field issues with configuration maangers.</li>
					<li>Fixed everburing piece names to match new case convention and changed (Everburning) to (Eternal).</li>
					<li>Added some new pieces to be enabled by default.</li>
					<li>Updated Jotunn.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.3.5</td>
			<td align="left">
				<ul>
					<li>Modified public API for `IsPieceAddedByMVBP` to include an optional Piece parameter that can be passed to avoid duplicating `GetComponent` calls.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.3.4</td>
			<td align="left">
				<ul>
					<li>Added null prefab check in `IsPieceAddedByMVBP` directly rather than relying on the null check in `GetPrefabName` just in case a prefab is ever added that has the name set to an empty string.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.3.3</td>
			<td align="left">
				<ul>
					<li>Compatibility fix for `MonstrumDeepNorth`.</li>
					<li>Added public method to check if `MVBP` has added a `Piece`` component to a prefab. Intended to support compatibility with `LocatationResest`.</li>
					<li>Fixed possible NRE when verbosity set to `High` and using other mods that add pieces.</li>
					<li>Updated README.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.3.2</td>
			<td align="left">
				<ul>
					<li>Compatibility fix for Niflheim + Beastiary by Rusty.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.3.1</td>
			<td align="left">
				<ul>
					<li>Updated to Jotunn 2.22.0 and restored pre Bog Witch config syncing behaviour so configs are only synced if the mod is installed on the server.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.3.0</td>
			<td align="left">
				<ul>
					<li>Quick hotfix to get things working for the bog witch update (thanks for the transpiler patch Margmas).</li>
					<li>Improved error handling to avoid throwing errors when resource cost requirement is missing in config (thanks flo123333).</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.2.3</td>
			<td align="left">
				<ul>
					<li>Updated naming scheme to match new piece name capitalization.</li>
					<li>Updated dependencies.</li>
					<li>Fixed issue with Verbosity settings in config not saving correctly.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.2.2</td>
			<td align="left">
				<ul>
					<li>Added new drawer interface for customizing piece requirements in-game.</li>
					<li>Updated default configuration so everything that is not a fire/explosion is now placeable by default.</li>
					<li>Improved logic used in the automatic placement patch.</li>
					<li>Added a feature that modifies the size of chests added by this mod, and the changes will persist even after removing the mod.</li>
					<li>Added some pieces that would previously self-destruct after being placed as they are now prevented from self-destructing and this prevention persists even after removing this mod.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.2.1</td>
			<td align="left">
				<ul>
					<li>Updated default piece categories to use new Ashlands categories. You will probably need to regenerate your config file due to the category names changing in Vanilla.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.2.0</td>
			<td align="left">
				<ul>
					<li>Patched to work on Ashlands and prevent crashing the weather system.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.1.0</td>
			<td align="left">
				<ul>
					<li>Player-built doors that do not require keys can now always be opened/closed even if they cannot normally be opened/closed. This change also persists even after this mod is uninstalled.</li>
					<li>Improved compatibility with PlantEverything so that MVBP ignores vanilla pickable objects that PlantEverything modifies even if they are not buildable in PlantEverything.</li>
					<li>Added a few new pieces to this version thanks to some help from Jere to figure out how to persistently edit prefab data even after uninstalling the mod.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.8</td>
			<td align="left">
				<ul>
					<li>Fixed a placement bug for corner stairs and cleaned up the code internally.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.7</td>
			<td align="left">
				<ul>
					<li>Turns out I accidentally reintroduced an old bug when doing some optimizations. Have now re-fixed bug where some prefabs that were generated in dungeons would drop non-vanilla drops when destroyed.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.6</td>
			<td align="left">
				<ul>
					<li>Changed stoneblock_fracture prefab to keep it's MineRock component now that resource exploits have been prevented. This should help with using MVBP to set up vanilla servers.</li>
					<li>Update so that piece icons from MVBP in the Plan Hammer for PlanBuild will be correct. Still need to wait for another update to PlanBuild to fix issues with pieces not being removed from the PlanHammer when disabled in MVBP though.</li>
					<li>Updated Jotunn</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.5</td>
			<td align="left">
				<ul>
					<li>Fixed bug that broke prevention of resource exploits when building and removing pickables.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.4</td>
			<td align="left">
				<ul>
					<li>Removed special characters from cfg file. <b>YOU HAVE TO REGENERATE YOUR CONFIGURATION FILE!</b></li>
					<li>Improved placement of some pieces to remove the large offsets relative to where players pointed their hammer.</li>
					<li>Fixed bug where disabled pieces that were not in the CreatorShop or Nature categories could be deconstructed by players.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.3</td>
			<td align="left">
				<ul>
					<li>Bugfix for placement of corner stair pieces.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.2</td>
			<td align="left">
				<ul>
					<li>Minor performance optimizations.</li>
					<li>Added compatibility for CustomShips mod.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.1</td>
			<td align="left">
				<ul>
					<li>Fixed issues with enemies targeting pieces they shouldn't.</li>
					<li>Fixed rare bug that only occurred on some remote linux server hosting services.</li>
					<li>Sped up initialization and re-initialization by ~30%.</li>
					<li>Optimized code to use bool checks instead of null checks wherever possible.</li>
					<li>Redesigned in-game updates to configuration to not require expensive operations like reflection.</li>
					<li>Improved compatibility with other mods that affect the same pieces, MVBP now only overwrites the values related to the configuration settings when re-initializing.</li>
					<li>Piece components are now disabled if the prefab is disabled in the configuration settings and the prefab does not have a piece component in the Vanilla game.</li>
				</ul>
			</td>
		</tr>
		<tr>
			<td align="center">1.0.0</td>
			<td align="left">
				<ul>
					<li><b>Feature complete release!</b></li>
					<li>Added new portal and customization to allow unrestricted teleportation with it.</li>
					<li>Added new unsafe patch to allow using a new fermenter.</li>
					<li>Fixed bug where PlayerBase was not being applied correctly when the patch for it was enabled.</li>
					<li>Patched a secret door to allow closing it if it was built by a player.</li>
					<li>Added configuration options to let users customize some of the piece textures.</li>
					<li>Fixed the clipping issues on the armor stands enabled by MoreVanillaBuildPrefabs.</li>
				</ul>
			</td>
		</tr>
	</tbody>
</table>

<div class="header">
	<h2>Versions 0.X.X</h2>
</div>
<details>
	<summary>Click to expand</summary>
	<div class="header">
		<h3>Versions 0.6.X</h3>
	</div>
	<details>
		<summary>Click to expand</summary>
		<table>
			<tbody>
				<tr>
					<th align="center">Version</th>
					<th align="center">Notes</th>
				</tr>
				<tr>
					<td align="center">0.6.0</td>
					<td align="left">
						<ul>
							<li>Updated for newest patch</li>
							<li>Changed configuration file format for non-prefab sections to use toggles in-game.</li>
							<li>
								Added configuration options to enable:
								<ul>
									<li>All seasonal pieces.</li>
									<li>Patching siding door pieces placed by players and enable sliding doors by default.</li>
									<li>Patching comfort values for pieces added by this mod.</li>
									<li>Patching new torches, fires, and beds to have a PlayerBase effect.</li>
								</ul>
							</li>
							<li>Added unsafe patch section and the option of enabling new beds for sleeping.</li>
							<li>Improved resource costs and removal of pieces with a MineRock component (the smaller mineable rocks).</li>
							<li>Minor performance optimizations.</li>
						</ul>
					</td>
				</tr>
			</tbody>
		</table>
	</details>
	<div class="header">
		<h3>Versions 0.5.X</h3>
	</div>
	<details>
		<summary>Click to expand</summary>
		<table>
			<tbody>
				<tr>
					<th align="center">Version</th>
					<th align="center">Notes</th>
				</tr>
				<tr>
					<td align="center">0.5.1</td>
					<td align="left">
						<ul>
							<li>Added more null checks to handle other mods destroying prefabs on log-out.</li>
							<li>Set MVBP to ignore prefabs added by Creature Level and Loot Control.</li>
							<li>Improved compatibility with PlanBuild.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.5.0</td>
					<td align="left">
						<ul>
							<li>Built against new BepInEx and Jotunn.</li>
							<li>Improved default settings so all vanilla prefabs in game version 0.217.25 are placeable.</li>
							<li>Improved removal of custom prefabs with the hammer, vanilla layers are no longer altered under any circumstances.</li>
							<li>Updated for ExtraSnapPointsMadeEasy's new API.</li>
							<li>Improved compatibility with ExpandWorld.</li>
							<li>Fixed issue with one prefab being impossible to unlock.</li>
							<li>Overhauled how mineable prefabs work (see README). You may need to tweak your configuration file to use the prefabs ending in "_frac" or "_destruction" now.</li>
							<li>Fixed bug where CreativeMode pieces that were built by players could be removed by the creator even if they aren't enabled in the config. Now pieces can only by removed via the hammer if they are enabled in the config.</li>
							<li>Global configuration section split into Global, Admin, and Customization in preparation for adding new customization features.</li>
						</ul>
					</td>
				</tr>
			</tbody>
		</table>
	</details>
	<div class="header">
		<h3>Versions 0.4.X</h3>
	</div>
	<details>
		<summary>Click to expand</summary>
		<table>
			<tbody>
				<tr>
					<th align="center">Version</th>
					<th align="center">Notes</th>
				</tr>
				<tr>
					<td align="center">0.4.8</td>
					<td align="left">
						<ul>
							<li>Configuration changes made in-game will now persist properly after logging out.</li>
							<li>Sped up re-initialization slightly.</li>
							<li>Update README with known issues</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.7</td>
					<td align="left">
						<ul>
							<li>Hotfix to prevent infinite re-initialization loop in multi-player. Turns out that even if I test with a dedicated server, some bugs only show up with multiple clients.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.6</td>
					<td align="left">
						<ul>
							<li>Fixed issue with server config changes not persisting after log-out unless the server was restarted. Config changes for the server are now saved to the server config file when you log-out.</li>
							<li>Re-enabled prefabs that spawn a MineRock5 component as they can be useful for building islands. A warning about how they work is now automatically added to the piece description.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.5</td>
					<td align="left">
						<ul>
							<li>
								Improved deconstruction of non-WearNTear pieces.
								<ul>
									<li>Destroying player-built pieces via damaging them will always drop the resources for building the piece now.</li>
									<li>Deconstructing non-WearNTear pieces will now destroy them using the Destructible component if it is present (this means removing ice pieces makes them shatter and play the ice SFX).</li>
								</ul>
							</li>
							<li>Added config settings to control piece clipping during placement.</li>
							<li>Patched MineRock script to prevent NRE on newly placed pieces.</li>
							<li>Improved filtering to prevent detecting prefabs that create giant boulders when you remove them.</li>
							<li>Automatically adds missing removal SFX for WearNTear pieces.</li>
							<li>Hotfix for issues with invalid placement due to changing collider layers.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.4</td>
					<td align="left">
						<ul>
							<li>Removed server requirement so that console players can enjoy the mod vicariously through crossplay.</li>
							<li>Improved sorting of prefabs added by this mod (that took a while).</li>
							<li>Added automatic piece classification to hopefully allow correct sorting of pieces from other mods and future updates.</li>
							<li>Improved automatic naming of prefabs.</li>
							<li>Fixed layer issue on some pieces that prevented targeting them for removal.</li>
							<li>Removing pieces now triggers the destruction effects if they exist.</li>
							<li>Fixed possible exploit involving pickables with extra random item drops.</li>
							<li>Patched chair prefabs so you can now sit in them.</li>
							<li>Minor performance optimizations.</li>
							<li>Removed piece descriptions that were duplicates of piece names.</li>
							<li>Disabled a prefab that would disappear 10 seconds after placing it.</li>
							<li>Improved descriptions for several prefabs.</li>
							<li>Removed the snap points added to the center of all prefabs (use ExtraSnapPointsMadeEasy instead).</li>
							<li>Fixed minor compatibility issue with RRR, warning should no longer trigger and MVBP will no longer detect prefabs added by RRR.</li>
							<li>Added config option to enable placing vanilla crops so you can make pretty gardens if you want.</li>
							<li>Added colliders to allow removing large straw rug.</li>
							<li>Tweaked some default resource costs.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.3</td>
					<td align="left">
						<ul>
							<li>
								Added a new piece category "Nature".
								<ul>
									<li>Changed Admin only settings to account for new category.</li>
									<li>Tweaked default configuration to account for new category.</li>
								</ul>
							</li>
							<li>
								Changed how CreativeMode works.
								<ul>
									<li>CreativeMode now sets whether pieces from the CreatorShop and Nature categories are enabled for building.</li>
									<li>Changing the CreativeMode setting now updates while in-game.</li>
								</ul>
							</li>
							<li>Fixed the cloth behaviour on the new ship.</li>
							<li>Tweaked snap points to mimic vanilla pieces more. Can use ExtraSnapPointsMadeEasy if you want more precise snap points.</li>
							<li>Improved compatibility with ExtraSnapPointsMadeEasy to allow dynamically changing extra snap points as MoreVanillaBuildPrefabs dynamically adds/removes build pieces.</li>
							<li>Pickable pieces now drop the pickable item when deconstructed if it has not already been picked.</li>
							<li>ItemStand pieces now drop the attached item when deconstructed if an attached item exists.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.2</td>
					<td align="left">
						<ul>
							<li>Changed mod so that if a client has the mod, then the server they are connecting to must also have the mod (see README for details).</li>
							<li>Implemented a CreativeMode configuration option (see README for details).</li>
							<li>All pieces that are missing placement sound effects now have default sfx assigned based on the required crafting station. (Missing deconstruction sounds effects are not fixed though as that requires adding WearNTear or Destructable components to pieces).</li>
							<li>Fixed bug where deconstructing player-built pieces with world modifiers set to turn off build costs would cause world-generated destruction drops to occur.</li>
							<li>Player-buil5 barrels no longer drop random loot when destroyed. They still do not return the resources used to build them when destroyed (they do return build resources if deconstructed though).</li>
							<li>Patched Dvergr furniture items so they provide support and you can now place item stands on them.</li>
							<li>Patched some more prefabs and improved placement for others, as usual each update.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.1</td>
					<td align="left">
						<ul>
							<li>Minor update to fix the stuttering issue when editing the configuration via the in-game configuration manager. The mod now only updates after closing the configuration manager.</li>
							<li>Tweaked update logic to avoid re-initializing if receiving config data from server or reloading the config file doesn't actually change any config setting values.</li>
							<li>Added some everburning torches and braziers that do not require fuel to the default configuration. Currently they are configured to unlock sometime during the last biome in the current game version.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.4.0</td>
					<td align="left">
						<ul>
							<li>
								<b>Massive update</b>, I basically re-wrote the mod to allow it to dynamically respond to configuration setting changes while in-game.
							</li>
							<li>Switched to using Jotunn's server syncing features instead of ServerSync.</li>
							<li>Removed Locking Configuration setting. If you install the mod on the server it will now always sync data to clients.</li>
							<li>Change `VerboseMode` to `Verbosity`. There are now three logging levels you can select from to output more or less information. This should make debugging easier when issues are reported.</li>
							<li>Changed some Global configuration setting names to more descriptive.</li>
							<li>Fixed issue where sometimes configuration data from the server wouldn't sync correctly. The mod now always re-initializes the configuration whenever configuration data is received from the server.</li>
							<li>Changed how building and deconstructing pickable objects is handled to prevent exploits.</li>
							<li>Optimized load times for dynamic syncing. The very first time the mod loads on a clean install it takes about ~300 ms as it generates new icons. After that, when the mod initializes or responds to configuration settings changes it averages ~110-160 ms.</li>
							<li>Patched some more prefabs, including making a hidden sailing ship fully functional.</li>
							<li>Various internal tweaks to reduce the odds of compatibility issues with other mods.</li>
							<li>Possibly more stuff I forgot about, it was a pretty big re-write. The new README should still cover everything important though.</li>
						</ul>
					</td>
				</tr>
			</tbody>
		</table>
	</details>
	<div class="header">
		<h3>Versions 0.3.X</h3>
	</div>
	<details>
		<summary>Click to expand</summary>
		<table>
			<tbody>
				<tr>
					<th align="center">Version</th>
					<th align="center">Notes</th>
				</tr>
				<tr>
					<td align="center">0.3.7</td>
					<td align="left">
						<ul>
							<li>Fixed compatibility with WackyDB, (my bad, while rewriting the code to add pieces I switched from a prefix to a postfix).</li>
							<li>Switch stone chest to prefer the one with animations.</li>
							<li>Renaming of custom chests to be more descriptive.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.3.6</td>
					<td align="left">
						<ul>
							<li>Switched back to custom methods to add pieces as removing pieces added by Jotunn on log out led to unintended behaviour.</li>
							<li>Slightly reduced load times.</li>
							<li>Patched placement of treasure chests so they no longer contain random loot (world-generated treasure chests are unaffected).</li>
							<li>Removed treasure chests that were Duplicates of each other.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.3.5</td>
					<td align="left">
						<ul>
							<li>Switched back to adding pieces via Jotunn.</li>
							<li>More automatic naming improvements.</li>
							<li>Quick fix for null exception error that broke the mod last release (Somehow the option that allowed me to reference the publicized assembles got unchecked).</li>
							<li>
								Changed ModGUID to match mod name. <b>This changes the name of your cfg file. So after it regenerates copy over any changes you've made via a text editor and delete your old one.</b>
							</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.3.4</td>
					<td align="left">
						<ul>
							<li>
								Improved naming for custom pieces in hammer build table.
								<ul>
									<li>Format of custom piece names is now consistent with vanilla name formatting.</li>
									<li>Some spelling inconsistencies from the game's internal ID's have been corrected.</li>
								</ul>
							</li>
							<li>Automatically add hover text if missing for custom pieces (depending on the piece it still may not display).</li>
							<li>Patched and enabled more prefabs by default.</li>
							<li>Disabled a prefab that explodes into a giant boulder when hit with a pickaxe (Thanks Cass!)</li>
							<li>Tweaked build requirements and costs for some prefabs.</li>
							<li>
								Patched placement of several pieces.
								<ul>
									<li>Improved placement of dvergr poles and wood pieces.</li>
									<li>Fixed issue with some black marble pieces moving after placement due to discrepancy between colliders and rigid bodies.</li>
								</ul>
							</li>
							<li>Changed how piece Icons are generated to hopefully fix the lighting issue with some icons.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.3.3</td>
					<td align="left">
						<ul>
							<li>Fix color artifacts in custom piece icons (Thanks again for your help Margmas).</li>
							<li>Fix bug that I accidentally re-introduced where world-generated CreatorShop pieces could be deconstructed.</li>
							<li>Added SearsCatalog as a Thunderstore dependency.</li>
							<li>More internal refactoring and clean-up to get ready for possibly adding some new features.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.3.2</td>
					<td align="left">
						<ul>
							<li>Update to Jotunn 2.14.4</li>
							<li>Changed priority of patch for adding prefabs to fix partial incomparability with WackyDB.</li>
							<li>Internal refactoring to clean up code and make managing methods easier.</li>
							<li>Enabled some more pieces by default.</li>
							<li>
								Added EffectsList patch from PotteryBarn to fix null exceptions when using custom Armor Stands.
							</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.3.1</td>
					<td align="left">
						<ul>
							<li>Added NullException checks to fix compatibility issues with CreatureLevelAndLootControl.</li>
							<li>
								Changed mod to search for prefabs every time a game session is joined (has minimal impact on load time, < 50 ms on average) to prevent null prefab errors.
							</li>
							<li>Added error handling to catch incorrect build requirement ID's and throw a warning to the log.</li>
							<li>
								Thanks to Cass again for letting me know about the compatibility issue and testing out the fixes.
							</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.3.0</td>
					<td align="left">
						<ul>
							<li>Implemented built-in cfg file watcher to ensure changes made to cfg file are not erased.</li>
							<li>Fixed crashing issue with some prefabs and re-enabled them by default.</li>
							<li>Changed when custom pieces are added to wait until after receiving data from ServerSync (Thanks to Cass for reporting the issue and to Wackymole for helping figure out which method to patch).</li>
							<li>Changed method of adding custom pieces due to Null Exception error caused by adding pieces with Jotunn after ZNet.Start(), will probably switch back after Jotunn updates.</li>
						</ul>
					</td>
				</tr>
			</tbody>
		</table>
	</details>
	<div class="header">
		<h3>Versions 0.2.X</h3>
	</div>
	<details>
		<summary>Click to expand</summary>
		<table>
			<tbody>
				<tr>
					<td align="center">0.2.2</td>
					<td align="left">
						<ul>
							<li>Added null check to EnsureNoDuplicateZNetView(), should resolve issues caused when rejoining servers (Thanks to Cass on the Odinplus for reporting the bug).</li>
							<li>Mod now saves the cfg file on logout, should hopefully preserve changes made to it before reading from it when rejoining a server.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.2.1</td>
					<td align="left">
						<ul>
							<li>Fixed clipping and placement for several prefabs.</li>
							<li>Adjusted snap points on a few prefabs.</li>
							<li>Disabled CargoCrate prefab due to it deleting itself when placed because the inventory is empty.</li>
							<li>Code clean up.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.2.0</td>
					<td align="left">
						<ul>
							<li>Reduced load time from ~30 seconds to ~0.5 seconds (Thanks to onnan for reporting the issue and to Margmas on the OdinPlus discord for the tip on reducing config file load times).</li>
							<li>Switched to using ZNetScene for patch to trigger removal of custom pieces on logout.</li>
							<li>Internal code refactoring and clean up.</li>
						</ul>
					</td>
				</tr>
			</tbody>
		</table>
	</details>
	<div class="header">
		<h3>Versions 0.1.X</h3>
	</div>
	<details>
		<summary>Click to expand</summary>
		<table>
			<tbody>
				<tr>
					<td align="center">0.1.4</td>
					<td align="left">
						<ul>
							<li>Updated for patch 0.217.22</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.1.3</td>
					<td align="left">
						<ul>
							<li>Updated for Jotunn 2.14.2</li>
							<li>Removed three prefabs that caused a crash when re-logging (should fix compatibility issues with the Multiverse mod).</li>
							<li>Improved load times when re-logging.</li>
							<li>Changed method of adding custom build pieces to respect server configuration when changing between servers without restarting the game.</li>
							<li>
								Added configuration option to restrict placement of CreatorShop pieces to Admins.
							</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.1.1/0.1.2</td>
					<td align="left">
						<ul>
							<li>Fixed ILRepacker not merging ServerSync assembly when creating Release version of Thunderstore mod package (Thanks to BLUBBSON on Github for letting me know about the bug).</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.1.0</td>
					<td align="left">
						<b>Major Updates</b>
						<ul>
							<li>Implemented configuration syncing with server.</li>
							<li>Added a setting to allow admins to deconstruct CreatorShop pieces built by other players.</li>
							<li>Add a configuration option for each prefab that enables a generic collision patch to allow users to possibly fix placing prefabs that have not been custom patched yet.</li>
							<li>Improved configuration file to provide configuration descriptions and a list of acceptable values for each configuration option.</li>
							<li>Crafting station names in configuration settings are now descriptive instead of based on the item_id in-game.</li>
						</ul>
						<b>Minor updates</b>
						<ul>
							<li>Tweaked resource requirements for better balance.</li>
							<li>Enabled more build pieces by default after tweaking the resource requirements to prevent them unlocking several biomes before they would normally be encountered by players.</li>
							<li>Fixed Github link in Thunderstore manifest (had copied the wrong template manifest when I remade it).</li>
							<li>Improved README formatting and fixed spelling/grammar in various places.</li>
						</ul>
					</td>
				</tr>
			</tbody>
		</table>
	</details>
	<div class="header">
		<h3>Versions 0.0.X</h3>
	</div>
	<details>
		<summary>Click to expand</summary>
		<table>
			<tbody>
				<tr>
					<td align="center">0.0.3</td>
					<td align="left">
						<ul>
							<li>World-generated pieces now drop only their default resource drops while player-built pieces drop only the resources used to build them.</li>
							<li>README updated and cleaned up (that's what I get for writing it at 1am last time).</li>
							<li>
								Configuration file naming scheme changed due to automating the process. <b>You need to regenerate your configuration file and copy over any customizations you made.</b>
							</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.0.2</td>
					<td align="left">
						<ul>
							<li>Updated README and added links to source code.</li>
						</ul>
					</td>
				</tr>
				<tr>
					<td align="center">0.0.1</td>
					<td align="left">
						<ul>
							<li>Initial release.</li>
						</ul>
					</td>
				</tr>
			</tbody>
		</table>
	</details>
</details>
