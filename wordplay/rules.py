from __future__ import annotations

from typing import TYPE_CHECKING

from rule_builder.options import OptionFilter
from rule_builder.rules import *

from .locations import LOCATION_NAME_TO_ID
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
            world.set_rule(loc, legend)#&HasFromList("A","B","C","D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", count=16))

def set_completion_condition(world: WordPlayWorld) -> None:
    world.set_completion_rule(Has("LegendaryFinished"))