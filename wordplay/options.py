from dataclasses import dataclass

from Options import Choice, OptionGroup, PerGameCommonOptions, Range, Toggle

class Goal(Choice):
    """
    Which game mode you need to complete to goal.
    """

    display_name = "Goal"

    option_hard = 0
    option_legendary = 1

    default = option_legendary


@dataclass
class WordPlayOptions(PerGameCommonOptions):
    goal : Goal 


option_groups = [

]

option_presets = {
    "easy": {
        "goal": Goal.option_hard
    },
    "hard": {
        "goal": Goal.option_legendary
    }
}
