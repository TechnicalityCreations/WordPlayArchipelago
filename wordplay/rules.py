from __future__ import annotations

from typing import TYPE_CHECKING

from rule_builder.options import OptionFilter
from rule_builder.rules import *

from locations import LOCATION_NAME_TO_ID
if TYPE_CHECKING:
    from .world import WordPlayWorld

def set_all_rules(world: WordPlayWorld) -> None:
    set_all_location_rules(world)
    set_completion_condition(world)

def set_all_location_rules(world: WordPlayWorld) -> None:
    normal : Rule = Has("Progressive Difficulty", 1)
    hard : Rule = Has("Progressive Difficulty", 2)
    legend : Rule = Has("Progressive Difficulty", 3)

    locations = LOCATION_NAME_TO_ID.keys()

    for l in locations:
        loc = world.get_location(l)
        if "Normal" in l:
            world.set_rule(loc, normal)
        if "Hard" in l:
            world.set_rule(loc, hard)
        if "Legendary" in l:
            world.set_rule(loc, legend)
        if "PointWord" in l:
            world.set_rule(loc, HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "M", "P", "Y", "H"))
    world.set_rule(world.get_location("10PointWord"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G"))
    world.set_rule(world.get_location("EasyRound3"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G"))
    world.set_rule(world.get_location("EasyRound4"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "P"))
    world.set_rule(world.get_location("EasyRound5"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "P", "M"))
    world.set_rule(world.get_location("EasyRound6"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "P", "M", "Y"))
    world.set_rule(world.get_location("EasyRound7"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "P", "M", "Y"))
    world.set_rule(world.get_location("EasyRound8"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "P", "M", "Y"))
    world.set_rule(world.get_location("EasyRound9"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "P", "M", "Y", "H"))
    world.set_rule(world.get_location("EasyFinished"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "P", "M", "Y", "H"))
    world.set_rule(world.get_location("15PointWord"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "M", "P"))
    world.set_rule(world.get_location("20PointWord"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "M", "P"))
    world.set_rule(world.get_location("25PointWord"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "M", "P", "Y"))
    world.set_rule(world.get_location("30PointWord"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G", "M", "P", "Y"))
    world.set_rule(world.get_location("9LetterWord"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", "D", "G"))
    world.set_rule(world.get_location("8LetterWord"), HasAll("A", "E", "I", "L", "N", "O", "R", "S", "T", "U"))
    world.set_rule(world.get_location("7LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", count=8))
    world.set_rule(world.get_location("6LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", count=7))
    world.set_rule(world.get_location("5LetterWord"), HasFromList("A", "E", "I", "L", "N", "O", "R", "S", "T", "U", count=5))


def set_completion_condition(world: WordPlayWorld) -> None:
    world.set_completion_rule(Has("LegendaryFinished"))