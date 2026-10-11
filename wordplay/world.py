from collections.abc import Mapping
from typing import Any

# Imports of base Archipelago modules must be absolute.
from worlds.AutoWorld import World

# Imports of your world's files must be relative.
from . import items, locations, regions, rules, web_world
from . import options as wordplay_options  # rename due to a name conflict with World.options

class WordPlayWorld(World):
    """
    Word Play is a Scrabble Roguelike
    """
    game = "Word Play"

    web = web_world.WordPlayWebWorld()

    options_dataclass = wordplay_options.WordPlayOptions
    options: wordplay_options.WordPlayOptions

    location_name_to_id = locations.LOCATION_NAME_TO_ID
    item_name_to_id = items.ITEM_NAME_TO_ID


    origin_region_name = "Menu"

    def create_regions(self) -> None:
        regions.create_and_connect_regions(self)
        locations.create_all_locations(self)

    def set_rules(self) -> None:
        rules.set_all_rules(self)

    def create_items(self) -> None:
        items.create_all_items(self)


    def create_item(self, name: str) -> items.WordPlayItem:
        return items.create_item_with_correct_classification(self, name)

    def get_filler_item_name(self) -> str:
        return items.get_random_filler_item_name(self)

    def fill_slot_data(self) -> Mapping[str, Any]:
        return self.options.as_dict(
            "goal", "randomise_bonuses", "deathlink", "achievementsanity"
        )
