from dataclasses import dataclass

from Options import Choice, OptionGroup, PerGameCommonOptions, Range, Toggle

class Goal(Choice):
    """
    Which game mode you need to complete to goal.
    """

    display_name = "Goal"

    option_hard = 0
    option_legendary = 1

    default = option_hard

class RandomiseBonuses(Toggle):
    """
    Whether or not to include the bonuses you get at the end of a round as items.
    Receiving one of these items will add what you need to the pool.
    """
    default = True
    display_name = "Randomise Bonuses"


class AchievementSanity(Toggle):
    """
    Add checks for every achievement in the game
    """
    default = False
    display_name = "Achievementsanity"

class TrapChance(Range):
    """
    The percentage chance of getting a trap instead of a filler item.
    """
    default = 10
    range_start = 0
    range_end = 100
    display_name = "Trap Chance"
    

@dataclass
class WordPlayOptions(PerGameCommonOptions):
    goal : Goal 
    randomise_bonuses: RandomiseBonuses
    Achievementsanity: AchievementSanity
    Trap_Chance: TrapChance


option_groups = [

]

option_presets = {
    "easy": {
        "goal": Goal.option_hard,
        "randomise_bonuses" : False,
        "Trap_Chance": 5,
        "Achievementsanity": False
    },
    "hard": {
        "goal": Goal.option_legendary,
        "randomise_bonuses" : True,
        "Trap_Chance": 20,
        "Achievementsanity": True
    }
}
