import os
from pathlib import Path
from typing import Dict, List


class PrefabConfig:
    """Class to hold prefab configuration settings"""
    _tab = "    "

    _ordering = {
        "name": 1,
        "enabled": 2,
        "allowedInDungeons": 3,
        "category": 4,
        "craftingStation": 5,
        "requirements": 6,
        "clipEverything": 7,
        "clipGround": 8,
        "placementPatch": 9,
        "placementOffset": 10,
        "pieceName": 11,
        "pieceDesc": 12,
        "pieceGroup": 13,
        "playerBasePatch": 14,
        "spawnOnDestroyed": 15,
        "invWidth": 16,
        "invHeight": 17
    }

    _needs_quotes = set(
        [
            "name",
            "requirements",
            "pieceDesc",
            "pieceName",
            "spawnOnDestroyed"
        ]
    )

    def __init__(self):
        self.name = None
        self.enabled = None
        self.allowedInDungeons = None
        self.category = None
        self.craftingStation = None
        self.requirements = None
        self.clipEverything = None
        self.clipGround = None
        self.placementPatch = None
        self.placementOffset = None
        self.pieceName = None
        self.pieceDesc = None
        self.pieceGroup = None
        self.playerBasePatch = None
        self.spawnOnDestroyed = None
        self.invWidth = None
        self.invHeight = None

    def _sort_attr_names(self, name: str):
        if name not in self._ordering:
            raise KeyError(f"Could not find ordering value for: {name}")
        return self._ordering[name]

    def get_attr_names(self) -> List[str]:
        """Get sorted list of attribute names."""
        names = [x for x in dir(self) if self.__is_attr(x)]
        names.sort(key=self._sort_attr_names)
        return names

    def __is_attr(self, name: str) -> bool:
        return not name.startswith('_') and not callable(getattr(self, name))

    def is_valid(self):
        """Checks that name is not None"""
        return self.name is not None

    def __str__(self):
        result = [f"new {PrefabConfig.__name__}("]

        for name in self.get_attr_names():
            if getattr(self, name) is not None:
                val = getattr(self, name)
                if name in self._needs_quotes:
                    val = f'\"{val}\"'
                result.append(f'{self._tab}{name}: {val},')

        result[-1] = result[-1].strip(",")
        result.append(")")
        return "\n".join(result)


def main():
    """Function to read and generate new prefab config dictionary."""
    default_config_path = Path(__file__).parents[1].joinpath(
        "PrefabManagement",
        "PrefabConfigManager.cs"
    )
    start_line = (
        "    private static readonly Dictionary<string, PrefabConfig>"
        + " PrefabConfigMap = new()\n"
    )

    r2modman_profile_name = "Mod-Debug"
    config_file_name = "Searica.Valheim.MoreVanillaBuildPrefabs.cfg"
    cfg_file_path = Path(os.getenv('APPDATA')).joinpath(
        "r2modmanPlus-local",
        "Valheim",
        "profiles",
        r2modman_profile_name,
        "BepInEx",
        "config",
        config_file_name
    )
    default_req_string = "<PrefabName>,0"

    out_path = Path(__file__).parent.joinpath("DefaultConfigs.txt")

    default_cfg_settings = read_default_configs(default_config_path, start_line)
    cfg_file_settings = read_config_file(cfg_file_path, default_req_string)

    # Add entries from the config file to the default configs
    # Also overwrite any values in default configs with values from
    # the config file (iff values exist in config file)
    for key, val in cfg_file_settings.items():
        if key not in default_cfg_settings:
            default_cfg_settings.update({key: val})
            print(f"Added Prefab: {key}")
            print(val, "\n")
            continue

        for attr in val.get_attr_names():
            attr_val = getattr(val, attr)
            if attr_val is None:
                continue
            current_val = getattr(default_cfg_settings[key], attr)
            if current_val == attr_val:
                continue
            setattr(default_cfg_settings[key], attr, attr_val)
            if current_val is not None:
                print("Modified Prefab:", key)
                print(f"Changed {attr}: {current_val} to {attr_val}\n")

    # write modified default configs to output
    write_output(out_path, default_cfg_settings, start_line=start_line)


def read_default_configs(
    file_path: Path, start_line: str
) -> Dict[str, PrefabConfig]:
    """Reads the cs file containing the prefab
    configs and construct a dictionary of them."""
    default_configs = {}
    prefab_config = PrefabConfig()

    with open(file_path, "r", encoding="utf-8") as file:
        lines = file.readlines()
        i = 0
        while (lines[i] != start_line):
            i = i + 1

        prefab_config = PrefabConfig()

        for line in lines[i:]:
            if "name:" in line:
                # Encountering a "name:" line when prefab_config is valid
                # means this is a new prefab config settings so cache the
                # current prefab_config and make a new one
                if (prefab_config.is_valid()):
                    default_configs.update({prefab_config.name: prefab_config})
                prefab_config = PrefabConfig()
                prefab_config.name = get_line_value(line, "name:")

            for name in prefab_config.get_attr_names():
                if f"{name}:" in line:
                    setattr(
                        prefab_config,
                        name,
                        get_line_value(line, f"{name}:")
                    )

        if (prefab_config.is_valid()):
            default_configs.update({prefab_config.name: prefab_config})

    return default_configs


def read_config_file(
    file_path: Path, ignore_text: str
) -> Dict[str, PrefabConfig]:
    """Reads mod cfg file to create dictionary of prefab configs.
    Ignores the value for any setting where the value == `ignore_text`."""

    prefab_configs = {}
    prefab_config = PrefabConfig()

    with open(file_path, "r", encoding="utf-8") as file:
        lines = file.readlines()

        prefab_config = PrefabConfig()

        for line in lines[88:]:
            if line.startswith("["):  # prefab config section
                # add config to dictionary is it has been initialized
                if (prefab_config.is_valid()):
                    prefab_configs.update({prefab_config.name: prefab_config})
                    prefab_config = PrefabConfig()
                prefab_config.name = line[1:-2]  # strip ["name"]\n

            for name in prefab_config.get_attr_names():
                line_start = f"{capitalize_first_letter(name)} = "
                if line.startswith(line_start):
                    text = get_line_value(line, "=")
                    if text != ignore_text:
                        setattr(prefab_config, name, text)

    if (prefab_config.is_valid()):
        prefab_configs.update({prefab_config.name: prefab_config})

    return prefab_configs


def capitalize_first_letter(text: str):
    """Capitalizes first letter of string"""
    first = text[0].capitalize()
    return first + text[1:]


def get_line_value(line: str, split: str):
    """Gets value from line after the split string"""
    val = line.split(split)[-1].strip(",\n").strip().strip('"')
    val = val.replace("BuildingWorkbench", "Building")
    val = val.replace("BuildingStonecutter", "Stonecutter")
    if "Category = " in line:
        return f"HammerCategories.{val}"
    if "CraftingStation = " in line:
        val = f"nameof(CraftingStations.{val})"

    return val


def write_output(
    out_path: Path,
    prefab_configs: Dict[str, PrefabConfig],
    start_line: str,
    tab: str = "    "
):
    """Write output to a text file"""
    indent = " "*(len(start_line) - len(start_line.lstrip()))
    if len(indent) != 0:
        tab = indent

    with open(out_path, "w", encoding="utf-8") as out_file:
        out_file.write(start_line)
        out_file.write(indent+"{\n")

        open_key_value_pair = f"{indent}{tab}" + "{\n"
        close_key_value_pair = f"{indent}{tab}" + "},\n"
        kvp_indent = f"{indent}{tab*2}"
        sorted_keys = sorted(x for x in prefab_configs.keys())
        for key in sorted_keys:
            prefab_config = prefab_configs[key]
            out_file.write(open_key_value_pair)
            out_file.write(
                f'{kvp_indent}"{prefab_config.name}",\n'  # key for PrefabConfigMap
            )
            # write prefab config entry as Value in KeyValue pair
            prefab_str = str(prefab_config)
            lines = [f"{kvp_indent}{line}" for line in prefab_str.split("\n")]
            out_file.write("\n".join(lines) + "\n")
            out_file.write(close_key_value_pair)
        out_file.write(indent+"};\n")


if __name__ == "__main__":
    main()
