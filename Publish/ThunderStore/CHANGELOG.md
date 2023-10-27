<div class="header">
	<h2>Versions 0.4.X</h2>
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Version</th>
			<th align="center">Notes</th>
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

<div class="header">
	<h2>Versions 0.3.X</h2>
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
	<h2>Versions 0.2.X</h2>
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
	<h2>Versions 0.1.X</h2>
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
	<h2>Versions 0.0.X</h2>
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