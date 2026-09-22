from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import ItemClassification, Location

from . import items

if TYPE_CHECKING:
    from .world import WordPlayWorld


LOCATION_NAME_TO_ID = {
    "EasyRound1": 1,
    "EasyRound2": 2,
    "EasyRound3": 3,
    "EasyRound4": 4,
    "EasyRound5": 5,
    "EasyRound6": 6,
    "EasyRound7": 7,
    "EasyRound8": 8,
    "EasyRound9": 9,
    "EasyFinished": 10,
    "NormalRound1": 21,
    "NormalRound2": 22,
    "NormalRound3": 23,
    "NormalRound4": 24,
    "NormalRound5": 25,
    "NormalRound6": 26,
    "NormalRound7": 27,
    "NormalRound8": 28,
    "NormalRound9": 29,
    "NormalRound10": 30,
    "NormalRound11": 31,
    "NormalFinished": 32,
    "HardRound1": 41,
    "HardRound2": 42,
    "HardRound3": 43,
    "HardRound4": 44,
    "HardRound5": 45,
    "HardRound6": 46,
    "HardRound7": 47,
    "HardRound8": 48,
    "HardRound9": 49,
    "HardRound10": 50,
    "HardRound11": 51,
    "HardFinished": 52,
    "LegendaryRound1": 61,
    "LegendaryRound2": 62,
    "LegendaryRound3": 63,
    "LegendaryRound4": 64,
    "LegendaryRound5": 65,
    "LegendaryRound6": 66,
    "LegendaryRound7": 67,
    "LegendaryRound8": 68,
    "LegendaryRound9": 69,
    "LegendaryRound10": 70,
    "LegendaryRound11": 71,
    "LegendaryRound12": 72,
    "LegendaryRound13": 73,
    "LegendaryFinished": 74,
    "5LetterWord": 105,
    "6LetterWord": 106,
    "7LetterWord": 107,
    "8LetterWord": 108,
    "9LetterWord": 109,
    "10PointWord": 210,
    "15PointWord": 215,
    "20PointWord": 220,
    "25PointWord": 225,
    "30PointWord": 230,
    "50PointWord": 250,
    "75PointWord": 275,
    "100PointWord": 300,
    "125PointWord": 325,
    "150PointWord": 350,
    "175PointWord": 375,
    "200PointWord": 400,
    "225PointWord": 425,
    "250PointWord": 450,
}



class WordPlayLocation(Location):
    game = "Word Play"


def get_location_names_with_ids(location_names: list[str]) -> dict[str, int | None]:
    return {location_name: LOCATION_NAME_TO_ID[location_name] for location_name in location_names}


def create_all_locations(world: WordPlayWorld) -> None:
    create_regular_locations(world)


def create_regular_locations(world: WordPlayWorld) -> None:
    menu = world.get_region("Menu")
    locations = []
    for i in range(1,14):
        if i < 10:
            locations.append(f"EasyRound{i}")
        if i < 12:
            locations.append(f"NormalRound{i}")
            locations.append(f"HardRound{i}")
            
        locations.append(f"LegendaryRound{i}")

    locations.append(f"EasyFinished")
    locations.append(f"NormalFinished")
    locations.append(f"HardFinished")
    locations.append(f"LegendaryFinished")

    for i in range(5,10):
        locations.append(f"{i}LetterWord")

    for i in [10, 15, 20, 25, 30, 50, 75, 100, 125, 150, 175, 200, 225, 250]:
        locations.append(f"{i}PointWord")

    menu.add_locations(get_location_names_with_ids(locations), WordPlayLocation)
