from __future__ import annotations

from typing import TYPE_CHECKING

from rule_builder.options import OptionFilter
from rule_builder.rules import *

from .locations import LOCATION_NAME_TO_ID
from .options import *
if TYPE_CHECKING:
    from .world import WordPlayWorld

def set_all_rules(world: WordPlayWorld) -> None:
    set_all_location_rules(world)
    set_completion_condition(world)

def set_all_location_rules(world: WordPlayWorld) -> None:
    normal : Rule = Has("Progressive Difficulty", 1)
    hard : Rule = Has("Progressive Difficulty", 2)
    legend : Rule = Has("Progressive Difficulty", 3)

    locations = [l for l in LOCATION_NAME_TO_ID.keys() if "Achievementsanity" not in l]

    for l in locations:
        loc = world.get_location(l)
        if "Normal" in l:
            world.set_rule(loc, normal&HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
        if "Hard" in l:
            world.set_rule(loc, hard&HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "M", "P", "Y", "H", count= 13))
        if "Legendary" in l:
            world.set_rule(loc, legend&HasFromList("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", count=16))
        if "PointWord" in l and l.removesuffix("PointWord") not in ["15", "20", "25", "30", "40"]:
            world.set_rule(loc, HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=13))
            
    if world.options.Achievementsanity:
        world.set_rule(world.get_location("Achievementsanity - Average Gamer"), normal&HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
        world.set_rule(world.get_location("Achievementsanity - Tough as Nails"), hard&HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "M", "P", "Y", "H", count= 13))
        world.set_rule(world.get_location("Achievementsanity - No Problem"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
        world.set_rule(world.get_location("Achievementsanity - Total Wordsmith"), legend&HasFromList("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", count=16))
        world.set_rule(world.get_location("Achievementsanity - Beyond Incredible"), HasFromList("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", count=20))
        world.set_rule(world.get_location("Achievementsanity - A to Z"), HasAll("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"))
        world.set_rule(world.get_location("Achievementsanity - Incredible"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
        world.set_rule(world.get_location("Achievementsanity - Mouthful"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", "+"))
        world.set_rule(world.get_location("Achievementsanity - Mind Over..."), HasAll("M", "A", "G", "N", "E", "T"))
        world.set_rule(world.get_location("Achievementsanity - Mind Over..."), HasAll("W", "O", "R", "D", "P", "L", "A", "Y", "+"))
        world.set_rule(world.get_location("Achievementsanity - Big Points"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=18))
        world.set_rule(world.get_location("Achievementsanity - Super Scorer"),HasFromList("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", count=20))
        world.set_rule(world.get_location("Achievementsanity - Stuffed"), HasAll("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "+", "!", "*", "ER", "ING"))
        world.set_rule(world.get_location("Achievementsanity - W*ldc*rd"), Has("*"))
        world.set_rule(world.get_location("Achievementsanity - Super Lucky"), Has("Special Tiles - Emerald"))
        world.set_rule(world.get_location("Achievementsanity - Uncut"), Has("Special Tiles - Diamond"))
        world.set_rule(world.get_location("Achievementsanity - Midas Touch"), Has("Special Tiles - Gold"))


    world.set_rule(world.get_location("10PointWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=1))
    world.set_rule(world.get_location("EasyRound2"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=5))
    world.set_rule(world.get_location("EasyRound3"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=5))
    world.set_rule(world.get_location("EasyRound4"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=6))
    world.set_rule(world.get_location("EasyRound5"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=6))
    world.set_rule(world.get_location("EasyRound6"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=7))
    world.set_rule(world.get_location("EasyRound7"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=8))
    world.set_rule(world.get_location("EasyRound8"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=9))
    world.set_rule(world.get_location("EasyRound9"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=10))
    world.set_rule(world.get_location("EasyFinished"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
    world.set_rule(world.get_location("15PointWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", count=10))
    world.set_rule(world.get_location("20PointWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
    world.set_rule(world.get_location("25PointWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
    world.set_rule(world.get_location("30PointWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
    world.set_rule(world.get_location("10LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=11))
    world.set_rule(world.get_location("9LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=10))
    world.set_rule(world.get_location("8LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=9))
    world.set_rule(world.get_location("7LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=8))
    world.set_rule(world.get_location("6LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=7))
    world.set_rule(world.get_location("5LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "D", "G", "P", "Y", "H", count=5))

def set_completion_condition(world: WordPlayWorld) -> None:
    if world.options.goal == Goal.option_legendary:
        world.set_completion_rule(Has("Progressive Difficulty", 3)&HasFromList("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", count=19))
    elif world.options.goal == Goal.option_hard:
        world.set_completion_rule(Has("Progressive Difficulty", 2)&HasFromList("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", count=12))