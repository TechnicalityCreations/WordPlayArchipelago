from BaseClasses import Tutorial
from worlds.AutoWorld import WebWorld

from .options import option_groups, option_presets


# For our game to display correctly on the website, we need to define a WebWorld subclass.
class WordPlayWebWorld(WebWorld):
    game = "Word Play"

    theme = "partyTime"



    # If we have option groups and/or option presets, we need to specify these here as well.
    option_groups = option_groups
    options_presets = option_presets
