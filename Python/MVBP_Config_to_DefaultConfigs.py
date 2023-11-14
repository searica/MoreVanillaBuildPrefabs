from pathlib import Path
from typing import Dict


class PrefabConfig:
    """Class to hold prefab configuration settings"""
    tab = "    "

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
        self.pieceName = None
        self.pieceDesc = None
        self.pieceGroup = None
        self.playerBasePatch = None

    def iter_attr(self):
        return [x for x in dir(self) if not x.startswith('__') and not callable(getattr(self, x))]

    def is_valid(self):
        return self.name is not None

    def __str__(self):
        result = ["new PrefabDB("]
        if self.name is not None:
            result.append(f'{self.tab}name: "{self.name}",')

        if self.enabled is not None:
            result.append(f'{self.tab}enabled: {self.enabled},')

        if self.allowedInDungeons is not None:
            result.append(
                f'{self.tab}allowedInDungeons: {self.allowedInDungeons},'
            )

        if self.category is not None:
            result.append(f'{self.tab}category: {self.category},')

        if self.craftingStation is not None:
            result.append(f'{self.tab}craftingStation: {self.craftingStation},')

        if self.requirements is not None:
            result.append(f'{self.tab}requirements: "{self.requirements}",')

        if self.clipEverything is not None:
            result.append(f'{self.tab}clipEverything: {self.clipEverything},')

        if self.clipGround is not None:
            result.append(f'{self.tab}clipGround: {self.clipGround},')

        if self.pieceName is not None:
            result.append(f'{self.tab}pieceName: "{self.pieceName}",')

        if self.pieceDesc is not None:
            result.append(f'{self.tab}pieceDesc: "{self.pieceDesc}",')

        if self.pieceGroup is not None:
            result.append(f'{self.tab}pieceGroup: {self.pieceGroup},')

        if self.playerBasePatch is not None:
            result.append(f'{self.tab}playerBasePatch: {self.playerBasePatch},')

        result[-1] = result[-1].strip(",")
        result.append(")")
        return "\n".join(result)


def main():
    default_config_path = Path(r"C:\Users\TheUser\Documents\Projects\ValheimMods\MoreVanillaBuildPrefabs\Configs\PrefabConfigs.cs")

    cfg_file_path = Path(r"C:\Users\TheUser\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\Mod-Debug\BepInEx\config\Searica.Valheim.MoreVanillaBuildPrefabs.cfg")

    out_path = Path(__file__).parent.joinpath("DefaultConfigs.txt")

    start_line = "        internal static readonly Dictionary<string, PrefabDB> DefaultConfigValues = new()\n"

    default_configs = read_default_prefabs(default_config_path, start_line)
    prefab_configs = read_config_file(cfg_file_path)

    # Add entries from the config file to the default configs
    # Also overwrite any values in default configs with values from
    # the config file (iff values exist in config file)
    for key, val in prefab_configs.items():
        if key not in default_configs:
            default_configs.update({key: val})
            print("Added prefab")
            print(val)
        else:
            for attr in val.iter_attr():
                attr_val = getattr(val, attr)
                if attr_val is not None:
                    current_val = getattr(default_configs[key], attr)
                    if current_val != attr_val:
                        setattr(default_configs[key], attr, attr_val)
                        if current_val is not None:
                            print("Prefab:", key)
                            print(f"Changed {attr}: {current_val} to {attr_val}")

    # write modified default configs to output
    write_output(out_path, default_configs)


def read_default_prefabs(file_path, start_line) -> Dict[str, PrefabConfig]:
    """Reads the cs file containing the prefab
    configs and construct a dictionary of them."""
    default_configs = {}
    prefab_config = PrefabConfig()

    with open(file_path, "r") as file:
        lines = file.readlines()
        i = 0
        while (lines[i] != start_line):
            i = i + 1

        for line in lines[i:]:
            if "name:" in line:
                # add config to dictionary is it has been initialized
                if (prefab_config.is_valid()):
                    default_configs.update({prefab_config.name: prefab_config})
                    prefab_config = PrefabConfig()
                prefab_config.name = get_line_value(line, "name:")

            if "enabled:" in line:
                prefab_config.enabled = get_line_value(line, "enabled:")

            if "allowedInDungeons:" in line:
                prefab_config.allowedInDungeons = get_line_value(
                    line, "allowedInDungeons:"
                )

            if "category:" in line:
                prefab_config.category = get_line_value(line, "category:")

            if "craftingStation:" in line:
                prefab_config.craftingStation = get_line_value(
                    line, "craftingStation:"
                )

            if "requirements:" in line:
                prefab_config.requirements = get_line_value(
                    line, "requirements:"
                )

            if "clipEverything:" in line:
                prefab_config.clipEverything = get_line_value(
                    line, "clipEverything:"
                )

            if "clipGround:" in line:
                prefab_config.clipGround = get_line_value(
                    line, "clipGround:"
                )

            if "placementPatch:" in line:
                prefab_config.placementPatch = get_line_value(
                    line, "placementPatch:"
                )

            if "pieceName:" in line:
                prefab_config.pieceName = get_line_value(line, "pieceName:")

            if "pieceDesc:" in line:
                prefab_config.pieceDesc = get_line_value(line, "pieceDesc:")

            if "pieceGroup:" in line:
                prefab_config.pieceGroup = get_line_value(line, "pieceGroup:")

            if "playerBasePatch" in line:
                prefab_config.playerBasePatch = get_line_value(line, "playerBasePatch:")

        if (prefab_config.is_valid()):
            default_configs.update({prefab_config.name: prefab_config})

    return default_configs


def read_config_file(file_path) -> Dict[str, PrefabConfig]:
    """Reads mod cfg file to create dictionary of prefab configs"""

    prefab_configs = {}
    prefab_config = PrefabConfig()

    with open(file_path, "r") as file:
        lines = file.readlines()
        for line in lines[72:]:
            if line.startswith("["):
                # add config to dictionary is it has been initialized
                if (prefab_config.is_valid()):
                    prefab_configs.update({prefab_config.name: prefab_config})
                    prefab_config = PrefabConfig()
                prefab_config.name = line[1:-2]

            if "Enabled = " in line:
                prefab_config.enabled = get_line_value(line, "=")

            if "AllowedInDungeons = " in line:
                prefab_config.allowedInDungeons = get_line_value(line, "=")

            if "Category = " in line:
                val = get_line_value(line, "=")
                prefab_config.category = f"HammerCategories.{val}"

            if "CraftingStation = " in line:
                val = get_line_value(line, "=")
                val = f"nameof(CraftingStations.{val})"
                prefab_config.craftingStation = val

            if "Requirements = " in line:
                prefab_config.requirements = get_line_value(line, "=")

            if "ClipEverything = " in line:
                prefab_config.clipEverything = get_line_value(line, "=")

            if "ClipGround = " in line:
                prefab_config.clipGround = get_line_value(line, "=")

            if "PlacementPatch = " in line:
                prefab_config.placementPatch = get_line_value(line, "=")

            if "PieceName = " in line:
                prefab_config.pieceName = get_line_value(line, "=")

            if "PieceDesc = " in line:
                prefab_config.pieceDesc = get_line_value(line, "=")

            if "PieceGroup = " in line:
                prefab_config.pieceGroup = get_line_value(line, "=")

            if "PlayerBasePatch = " in line:
                prefab_config.pieceGroup = get_line_value(line, "=")

    if (prefab_config.is_valid()):
        prefab_configs.update({prefab_config.name: prefab_config})

    return prefab_configs


def get_line_value(line: str, split: str):
    """Gets value from line after the split string"""
    return line.split(split)[-1].strip(",\n").strip().strip('"')


def write_output(out_path: Path, prefab_configs: Dict[str, PrefabConfig]):
    """Write output to a text file"""
    tab = "    "
    with open(out_path, "w") as out_file:
        out_file.write(tab*2 + "internal static readonly Dictionary<string, PrefabDB> DefaultConfigValues = new()\n")
        out_file.write(tab*2 + "{\n")

        sorted_keys = sorted([x for x in prefab_configs.keys()])
        for key in sorted_keys:
            prefab_config = prefab_configs[key]
            out_file.write(tab*3 + "{\n")
            out_file.write(f'{tab*4}"{prefab_config.name}",\n')
            prefab_str = str(prefab_config)
            lines = [tab*4 + line for line in prefab_str.split("\n")]
            out_file.write("\n".join(lines) + "\n")
            out_file.write(tab*3 + "},\n")
        out_file.write(tab*2 + "};\n")


if __name__ == "__main__":
    main()



# with open(out_path, "w") as out_file:
#     i = 35

#     tab = "    "
#     out_file.write(tab*2 + "internal static readonly Dictionary<string, PrefabDB> DefaultConfigValues = new()\n")
#     out_file.write(tab*2 + "{\n")

#     nature_pieces = []
#     creator_shop_pieces = []

#     while i < len(lines):
#         line = lines[i]


#         if "Enabled = " in line:
#             val = line.split(" ")[-1].strip("\n")
#             out_file.write(tab*5 + f'{val},\n')

#         if line.startswith("AllowedInDungeons = "):
#             val = line.split(" ")[-1].strip("\n")
#             out_file.write(tab*5 + f'{val},\n')

#         if line.startswith("Category = "):
#             val = line.split(" ")[-1].strip("\n")
#             out_file.write(tab*5 + f'HammerCategories.{val},\n')
#             if (val == "Nature"):
#                 nature_pieces.append(name)
#             if (val == "CreatorShop"):
#                 creator_shop_pieces.append(name)

#         if line.startswith("CraftingStation = "):
#             val = line.split(" ")[-1].strip("\n")
#             out_file.write(tab*5 + f'nameof(CraftingStations.{val}),\n')

#         if line.startswith("Requirements = "):
#             val = line.split(" ")[-1].strip("\n")
#             out_file.write(tab*5 + f'\"{val}\"\n')
#             out_file.write(tab*4 + ")\n")
#             out_file.write(tab*3 + "},\n")

#         i = i + 1
#     out_file.write(tab*2 + "};")
