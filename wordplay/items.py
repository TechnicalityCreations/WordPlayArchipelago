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
    # 200-500 = Special Prog
    "Progressive Difficulty": 200,
    # 500+ Filler
    "Refresh": 501,
    "Play": 502,
}


DEFAULT_ITEM_CLASSIFICATIONS = {
    
}


class WordPlayItem(Item):
    game = "APQuest"


def get_random_filler_item_name(world: WordPlayWorld) -> str:
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
    "LANE",
    "LINE",
    "LOAN",
    "RAIN",
    "SINE",
    "SITE",
    "RISE",
    "TONE",
    "TUNE",
    "TUNA",
    "LONE",
]
def create_all_items(world: WordPlayWorld) -> None:
    startingWord = startingWords[world.random.randint(0, len(startingWords) - 1)]
    items = ITEM_NAME_TO_ID.keys()
    progItems = [i for i in items if ITEM_NAME_TO_ID[i] < 200]
    progPool = []
    for i in progItems:
        if i in startingWord:
            continue
        progPool.append(create_item_with_correct_classification(world, i))
    
    for i in range(0,3):
        progPool.append(create_item_with_correct_classification(world, "Progressive Difficulty"))
    itempool: list[Item] = progPool

    number_of_items = len(itempool)

    number_of_unfilled_locations = len(world.multiworld.get_unfilled_locations(world.player))

    needed_number_of_filler_items = number_of_unfilled_locations - number_of_items

    itempool += [world.create_filler() for _ in range(needed_number_of_filler_items)]

    world.multiworld.itempool += itempool

    for c in startingWord:
        letter = world.create_item(c)
        world.push_precollected(letter)
