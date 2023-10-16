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
            <td align="center">0.4.0</td>
            <td align="left">
                <ul>
                    <li> I just solved the pickables drop resources exploit last night as well. Now when you deconstruct something that has a pickables component it reduces the resources returned by the amount that the pickables drops (if it was already picked when deconstructing) and it accounts for world modifiers to loot drops.</li>
                    <li>I'm considering setting up the mod so that creating pieces with a pickables component always requires uses the pickables component and the amount has to be at least as much as the pickables would drop when picked (while accounting for world modifiers). This would override the resoucprce costs set in the cfg file.</li>
                    <li></li>
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
                        <li>Fixed compatiability with WackyDB, (my bad, while rewriting the code to add pieces I switched from a prefix to a postfix).</li>
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
                        <li>Removed treasure chests that were visual duplicates of each other.</li>
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
                                <li>Fixed issue with some black marble pieces moving after placement due to discrepency between colliders and rigid bodies.</li>
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
                        <li>item</li>
                        <li>item</li>
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
                        <li>item</li>
                    </ul>
                </td>
            </tr>
            <tr>
                <td align="center">0.2.0</td>
                <td align="left">
                    <ul>
                        <li>Reduced load time from ~30 seconds to ~0.5 seconds (Thanks to onnan for reporting the issue and to Margmas on the OdinPlus discord for the tip on reducing config file load times).</li>
                        <li>Switched to using ZNetScene for patch to trigger removal of custom pieces on logout.</li>
                        <li>item</li>
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
            <tr>
                <td align="center">0.0.3</td>
                <td align="left">
                    <ul>
                        <li>World-generated pieces now drop only their default resource drops while player-built pieces drop only the resources used to build them.</li>
                        <li>README updated and cleaned up (that's what I get for writing it at 1am last time).</li>
                        <li>
                            Configuration file naming scheme changed due to automating the process. <b>You need to regenerate your configuration file and copy over any customizations you made.</b>
                        </li>
                        <li>item</li>
                        <li>item</li>
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
