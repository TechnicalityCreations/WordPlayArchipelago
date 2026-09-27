from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import Item, ItemClassification

if TYPE_CHECKING:
    from .world import WordPlayWorld

ITEM_NAME_TO_ID = {
    # 1-199 Single Items
    "A": 1,
    "B": 2,
    "C": 3,
    "D": 4,
    "E": 5,
    "F": 6,
    "G": 7,
    "H": 8,
    "I": 9,
    "J": 10,
    "K": 11,
    "L": 12,
    "M": 13,
    "N": 14,
    "O": 15,
    "P": 16,
    "Q": 17,
    "R": 18,
    "S": 19,
    "T": 20,
    "U": 21,
    "V": 22,
    "W": 23,
    "X": 24,
    "Y": 25,
    "Z": 26,
    # Special Tiles
    "*": 27,
    "ING": 28,
    "ERS": 29,
    "+": 30,
    "!": 31,
    # Upgrades
    "Upgrades - Shuffle": 50,
    "Modifiers - Hold Zone": 51,
    "Modifiers - Refresh Ignores Submitted": 52,
    "Modifiers - Multiplier": 53,
    "Modifiers - Second Chance": 54,
    "Modifiers - Free 4 Letter Word": 55,
    "Modifiers - Free Refresh": 56,
    "Modifiers - Auto Refresh": 57,
    "Modifiers - Refill Upgrades": 58,
    "Modifiers - See Next Tiles": 59,
    "Modifiers - Upgrade Extra Uses": 60,
    "Modifiers - Free Reroll": 61,
    "Modifiers - Bonus Points": 62,
    "Upgrades - Destroy Tiles": 63,
    "Upgrades - Lock Tiles": 64,
    "Upgrades - Clone Tiles": 65,
    "Upgrades - Increase Tile Score": 66,
    "Modifiers - Value Increases Each Round": 67,
    "Modifiers - Copy Sold Modifier": 68,
    "Modifiers - Better Bonus Selection": 69,
    "Modifiers - Gain Play on Refresh": 70,
    "Modifiers - Refresh on Sell": 71,
    "Modifiers - Special Tiles Return to Bag": 72,
    "Modifiers - Refresh Ignores Special": 73,
    "Modifiers - Turn Refresh Into Plays": 74,
    "Modifiers - Refresh on Upgrade Exhaust": 75,
    "Modifiers - Double Refreshes": 77,
    "Modifiers - Refresh Zone": 78,
    "Modifiers - Return Tiles to Grid": 79,
    "Special Tiles - Glass": 80,
    "Special Tiles - Emerald": 81,
    "Special Tiles - Diamond": 82,
    "Special Tiles - Dots": 83,
    "Special Tiles - Potion": 84,
    "Special Tiles - Gold": 85,
    "Special Tiles - Mirror": 86,
    # 200-500 = Special Prog
    "Progressive Difficulty": 200,
    "Progressive Extra Tiles": 201,
    "Progressive Max Word Length": 202,
    # 500+ Filler
    "Refresh": 501,
    "Play": 502,
    "Polyglot Trap": 503,
    "Special Trap": 504
}


DEFAULT_ITEM_CLASSIFICATIONS = {
    
}


class WordPlayItem(Item):
    game = "Word Play"


def get_random_filler_item_name(world: WordPlayWorld) -> str:
    if world.random.randint(0, 100) < world.options.Trap_Chance:
        if world.random.randint(0, 100) < 75:
            return "Polyglot Trap"
        else:
            return "Special Trap"

    if world.random.randint(0, 100) < 50:
        return "Refresh"
    

    return "Play"


def create_item_with_correct_classification(world: WordPlayWorld, name: str) -> WordPlayItem:
    if len(name) == 1:
        classification = ItemClassification.progression
    elif ITEM_NAME_TO_ID[name] > 500:
        classification = ItemClassification.filler
    elif ITEM_NAME_TO_ID[name] <= 200:
        classification = ItemClassification.progression
    else:
        classification = DEFAULT_ITEM_CLASSIFICATIONS[name]


    return WordPlayItem(name, classification, ITEM_NAME_TO_ID[name], world.player)

startingWords = [
    "LOAN",
    "RAIN",
    "TONE",
    "TUNE",
    "LONE",
]
startingBonuses = [
    "Upgrades - Shuffle",
    "Modifiers - Multiplier",
    "Modifiers - Bonus Points"
]
def create_all_items(world: WordPlayWorld) -> None:
    startingWord = startingWords[world.random.randint(0, len(startingWords) - 1)]
    startingBonus = startingBonuses[world.random.randint(0, len(startingBonuses) - 1)]
    items = ITEM_NAME_TO_ID.keys()
    cap = 200
    if not world.options.Randomise_Bonuses:
        cap = 50
    progItems = [i for i in items if ITEM_NAME_TO_ID[i] < cap]
    progPool = []
    for i in progItems:
        if i in startingWord:
            continue
        if startingBonus == i:
            continue
        progPool.append(create_item_with_correct_classification(world, i))
    progPool.append(create_item_with_correct_classification(world, startingBonus))
    
    for i in range(0,3):
        progPool.append(create_item_with_correct_classification(world, "Progressive Difficulty"))

    if world.options.Trap_Chance > 0:
        progPool.append(create_item_with_correct_classification(world, "Polyglot Trap"))
        progPool.append(create_item_with_correct_classification(world, "Special Trap"))
    progPool.append(create_item_with_correct_classification(world, "Play"))
    progPool.append(create_item_with_correct_classification(world, "Refresh"))
    itempool: list[Item] = progPool

    number_of_items = len(itempool)

    number_of_unfilled_locations = len(world.multiworld.get_unfilled_locations(world.player))

    needed_number_of_filler_items = number_of_unfilled_locations - number_of_items

    itempool += [world.create_filler() for _ in range(needed_number_of_filler_items)]

    world.multiworld.itempool += itempool

    for c in startingWord:
        letter = world.create_item(c)
        world.push_precollected(letter)
