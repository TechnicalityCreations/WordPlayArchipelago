using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace WPArchipelagoMod
{
    public static class GameplayManager
    {
        static Dictionary<string, string> AchievementIDToLocation = new Dictionary<string, string>
        {
            {"achieve-00-easy", "Achievementsanity - No Problem"},
            {"achieve-01-normal", "Achievementsanity - Average Gamer"},
            {"achieve-02-hard", "Achievementsanity - Tough as Nails"},
            {"achieve-03-legendary", "Achievementsanity - Total Wordsmith"},
            {"achieve-06-alphabet", "Achievementsanity - A to Z"},
            {"achieve-07-10letter", "Achievementsanity - Incredible"},
            {"achieve-08-20letter", "Achievementsanity - Beyond Incredible"},
            {"achieve-09-plus", "Achievementsanity - Mouthful"},
            {"achieve-10-magnet", "Achievementsanity - Mind Over..."},
            {"achieve-11-wordplay", "Achievementsanity - Hey, that's the game's name!"},
            {"achieve-12-200", "Achievementsanity - Big Points"},
            {"achieve-13-400", "Achievementsanity - Super Scorer"},
            {"achieve-14-refresh", "Achievementsanity - Hoarder"},
            {"achieve-15-letterbag", "Achievementsanity - Stuffed"},
            {"achieve-16-wildcard", "Achievementsanity - W*ldc*rd"},
            {"achieve-17-emerald", "Achievementsanity - Super Lucky"},
            {"achieve-18-diamond", "Achievementsanity - Uncut"},
            {"achieve-19-golden", "Achievementsanity - Midas Touch"}
        };
        public static void Patch(Harmony h)
        {
            h.Patch(AccessTools.Method(typeof(SubmitWord), "WordWasAccepted"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(CheckWordLength)));
            h.Patch(AccessTools.Method(typeof(SubmitWord), "FinishSubmission"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(CheckWordPoints)));
            h.Patch(AccessTools.Method(typeof(LogicManagerScript), "FinishRound"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(CheckRound)));
            h.Patch(AccessTools.Method(typeof(LogicManagerScript), "StartFreshGame"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(NewGame)));
            h.Patch(AccessTools.Method(typeof(LogicManagerScript), "LoadGameState"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(LoadGame)));
            h.Patch(AccessTools.Method(typeof(SaveData), "GetAllLogicData"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(SaveGame)));
            h.Patch(AccessTools.Method(typeof(AchievementManager), nameof(AchievementManager.UnlockAchievement)), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(CheckAchievement)));
            h.Patch(AccessTools.Method(typeof(BonusStore), nameof(BonusStore.PickThreeRandomUpgrades)), prefix: new HarmonyMethod(typeof(GameplayManager), nameof(EditUpgradesAvailable)));
            h.Patch(AccessTools.Method(typeof(uAddTile), "PickTileRandomly"), prefix: new HarmonyMethod(typeof(GameplayManager), nameof(EditRandomTiles)), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(RestoreRandomTiles)));

        }
        public static void CheckAchievement(object[] __args)
        {
            if(!(bool)(Mod.SlotData["Achievementsanity"])) return;
            if (AchievementIDToLocation.ContainsKey((string) __args[0]))
            {
                Mod.CompleteCheck(AchievementIDToLocation[(string)__args[0]]);
            }
        }
        public static void AddTrap(string Trap)
        {
            switch (Trap)
            {
                default:
                    break;
            }
        }
        public static void EditRandomTiles(List<string> ___allowedLetters, out string[] __state)
        {
            __state = ___allowedLetters.ToArray();
            foreach(var s in __state)
            {
                if(!Mod.Has(s)) ___allowedLetters.Remove(s);
            }
            if(___allowedLetters.Count == 0)
            {
                if(Mod.Has("A")) ___allowedLetters.Add("A");
                if(Mod.Has("E")) ___allowedLetters.Add("E");
                if(Mod.Has("I")) ___allowedLetters.Add("I");
                if(Mod.Has("O")) ___allowedLetters.Add("O");
                if(Mod.Has("U")) ___allowedLetters.Add("U");
            }
        }
        public static void RestoreRandomTiles(List<string> ___allowedLetters, string[] __state)
        {
            ___allowedLetters.Clear();
            foreach(var s in __state)
            {
                ___allowedLetters.Add(s);
            }
        }
        public static void EditUpgradesAvailable(BonusStore __instance)
        {
            if(!(bool)Mod.SlotData["Randomise_Bonuses"]) return;
            MetaGameRules.Instance.levelCompletion[1] = true;
            var list = __instance.everyBonus;
            __instance.everyBonus.Clear();
            list.Add(Bonus("Gift - Add Refreshes"));
            list.Add(Bonus("Gift - Add Random Standard"));
            list.Add(Bonus("Gift - Add Random Standard High Value"));
            if (Mod.Has("*"))
            {
                list.Add(Bonus("Gift - Add Wildcards"));
                list.Add(Bonus("Up - Change to Wildcard"));
            }
            if(Mod.Has("ING")) list.Add(Bonus("Gift - Add Ing"));
            if(Mod.Has("ERS")) list.Add(Bonus("Gift - Add Ers"));
            if(Mod.Has("+")) list.Add(Bonus("Gift - Add +"));
            if(Mod.Has("!")) list.Add(Bonus("Gift - Add !"));
            if(Mod.Has("Upgrades - Shuffle"))
            {
                list.Add(Bonus("Up - Shuffle for Consonant"));
                list.Add(Bonus("Up - Shuffle for Vowel"));
                if(Mod.Has("E")) list.Add(Bonus("Up - Shuffle Tiles for E"));
                list.Add(Bonus("Up - Shuffle All of Type"));
                list.Add(Bonus("Up - Refresh Others"));
                list.Add(Bonus("Up - Shuffle Vowels or Consonants"));
            }
            if(Mod.Has("Special Tiles - Glass")) list.Add(Bonus("Up - Clone to Glass"));
            if(Mod.Has("Modifiers - Hold Zone")) list.Add(Bonus("Modifier - Hold Zone"));
            if(Mod.Has("Modifiers - Refresh Ignores Submitted")) list.Add(Bonus("Modifier - Refresh Ignores Submitted"));
            if(Mod.Has("Modifiers - Hold Zone")) list.Add(Bonus("Modifier - Hold Zone"));
            if(Mod.Has("Q") && Mod.Has("U")) list.Add(Bonus("Modifier - Qs are Us"));
            if(Mod.Has("U") && Mod.Has("N")) list.Add(Bonus("Modifier - UnFree"));
            if(Mod.Has("S") && Mod.Has("Z")) list.Add(Bonus("Modifier - S and Z Interchangeable"));
            if(Mod.Has("R") && Mod.Has("E"))
            {
                list.Add(Bonus("Modifier - Anything Start with RE"));
                list.Add(Bonus("Modifier - Re Gives Refresh"));
            }
            if(Mod.Has("Y")) list.Add(Bonus("Modifier - Y is a Vowel"));
            if(Mod.Has("Modifiers - Multiplier"))
            {
                list.Add(Bonus("Modifier - Tile Multi"));
                list.Add(Bonus("Modifier - Repeat Multi"));
                list.Add(Bonus("Modifier - 4 Letter First Last"));
                list.Add(Bonus("Modifier - Same Two Letters"));
                list.Add(Bonus("Modifier - Specific Length"));
                list.Add(Bonus("Modifier - Specific Letter"));
                list.Add(Bonus("Modifier - First and Last"));
                list.Add(Bonus("Modifier - Two Vowels"));
                list.Add(Bonus("Modifier - Start With Vowel"));
                list.Add(Bonus("Modifier - Special Multiplier"));
                list.Add(Bonus("Modifier - Different Special Tiles"));
                list.Add(Bonus("Modifier - No E Boost"));
                list.Add(Bonus("Modifier - Multi After Refresh"));
                list.Add(Bonus("Modifier - Multi After Upgrade"));
                list.Add(Bonus("Modifier - Lowest Scoring Const"));
                list.Add(Bonus("Modifier - Long Word Booster"));
                list.Add(Bonus("Modifier - Unique Vowels"));
                list.Add(Bonus("Modifier - First is Previous Last"));
                if(Mod.Has("R")) list.Add(Bonus("Modifier - R Multi"));
                if(Mod.Has("E")) list.Add(Bonus("Modifier - E Multi"));
                list.Add(Bonus("Modifier - Specific Last Letter"));
                list.Add(Bonus("Modifier - No Repeats"));
                list.Add(Bonus("Modifier - 3x if Only Mod"));
                list.Add(Bonus("Modifier - No Consonants in a Row"));
                list.Add(Bonus("Modifier - Start and End with a Vowel"));
                list.Add(Bonus("Modifier - Multiply by Unsubmitted Specials"));
                list.Add(Bonus("Modifier - First Vowel 5X"));
                list.Add(Bonus("Modifier - Tile Multi Part 2"));
                list.Add(Bonus("Modifier - Different Special Tiles 2"));
            }
            if(Mod.Has("Modifiers - Second Chance")) list.Add(Bonus("Modifier - Saviour"));
            if(Mod.Has("Modifiers - Refresh Zone")) list.Add(Bonus("Modifier - Refresh Zone"));
            if(Mod.Has("Modifiers - Special Tiles Return to Bag")) list.Add(Bonus("Modifier - Special Into Bag"));
            if(Mod.Has("Modifiers - Refresh Ignores Special")) list.Add(Bonus("Modifier - Refresh Ignores Special"));
            if(Mod.Has("Modifiers - Refresh on Upgrade Exhaust")) list.Add(Bonus("Modifier - Refresh on Upgrade Exhaust"));
            if(Mod.Has("Modifiers - Free 4 Letter Word"))
            {
                list.Add(Bonus("Modifier - Free 4 Letters"));
                list.Add(Bonus("Modifier - Free if Length"));
                list.Add(Bonus("Modifier - 4 Letters, Free Refreshes"));
            }
            if(Mod.Has("Modifiers - Double Refreshes"))
            {
                list.Add(Bonus("Modifier - Double Refreshes"));
            }
            if(Mod.Has("Modifiers - Return Tiles to Grid"))
            {
                list.Add(Bonus("Modifier - Return First Tile"));
                list.Add(Bonus("Modifier - Return Last Tile"));
            }
            if(Mod.Has("Modifiers - Free Refresh"))
            {
                list.Add(Bonus("Modifier - First Refresh Is Free"));
                list.Add(Bonus("Modifier - Refresh Is Free if No E"));
                list.Add(Bonus("Modifier - Free Refresh if Over 10"));
            }
            if(Mod.Has("Modifiers - Auto Refresh")) list.Add(Bonus("Modifier - Auto Refresh"));
            if(Mod.Has("Modifiers - Refill Upgrades")) list.Add(Bonus("Modifier - Refill Upgrades"));
            if(Mod.Has("Modifiers - See Next Tiles")) list.Add(Bonus("Modifier - Show Next 4"));
            if(Mod.Has("Modifiers - Upgrade Extra Uses")) list.Add(Bonus("Modifier - More Upgrades"));
            if(Mod.Has("Modifiers - Free Reroll")) list.Add(Bonus("Modifier - First Reroll Free"));
            if(Mod.Has("Modifiers - Value Increases Each Round")) list.Add(Bonus("Modifier - Value Increase Per Round"));
            if(Mod.Has("Modifiers - Copy Sold Modifier")) list.Add(Bonus("Modifier - Clone Sold Modifier"));
            if(Mod.Has("Upgrades - Destroy Tiles"))
            {
                list.Add(Bonus("Up - Destroy Tiles"));
                list.Add(Bonus("Up - Destroy and Set Lives"));
                if(Mod.Has("Upgrades - Increase Tile Score")) list.Add(Bonus("Up - Destroy Then Give"));
            }
            if(Mod.Has("Modifiers - Better Bonus Selection"))
            {
                list.Add(Bonus("Modifier - Rare and Legendary More Likely"));
                list.Add(Bonus("Modifier - Shop Size"));
                list.Add(Bonus("Modifier - Plays When Skipping Shop"));
                list.Add(Bonus("Modifier - Force Four Shop"));
            }
            if(Mod.Has("Upgrades - Increase Tile Score"))
            {
                list.Add(Bonus("Up - Add 5 Points"));
                list.Add(Bonus("Up - Add Random Points"));
                list.Add(Bonus("Up - Add 10"));
                list.Add(Bonus("Up - Add Refresh Count"));
                list.Add(Bonus("Modifier - Length Same As Before"));
                list.Add(Bonus("Modifier - All +1 on Four"));
                list.Add(Bonus("Modifier - If Last Is Vowel Add Perm"));
                list.Add(Bonus("Modifier - More Vowels than Consonants"));
                if(Mod.Has("Upgrades - Shuffle")) list.Add(Bonus("Up - 5 if Consonant"));
            }
            if(Mod.Has("Upgrades - Lock Tiles")) list.Add(Bonus("Up - Make Locked Tiles"));
            if(Mod.Has("Modifiers - Gain Play on Refresh")) list.Add(Bonus("Modifier - Gain Play on Refresh"));
            if(Mod.Has("Modifiers - Refresh on Sell")) list.Add(Bonus("Modifier - Refresh on Sell"));
            if(Mod.Has("Modifiers - Turn Refresh Into Plays")) list.Add(Bonus("Modifier - Sell to Turn Refreshes into Plays"));
            if(Mod.Has("Upgrades - Clone Tiles"))
            {
                list.Add(Bonus("Up - 10 Clones"));
                list.Add(Bonus("Up - Duplicate Tile"));
                list.Add(Bonus("Up - 3 Standard Clones"));
                list.Add(Bonus("Modifier - Duplicate More Than 3 Special"));
                list.Add(Bonus("Modifier - 4 Tile Word, Dupe 1st"));
            }
            if(Mod.Has("Modifiers - Bonus Points"))
            {
                list.Add(Bonus("Modifier - Add ReRolls"));
                list.Add(Bonus("Modifier - Increase on Modifier Sell"));
                list.Add(Bonus("Modifier - Add Refresh Count to Word Score"));
                list.Add(Bonus("Modifier - Decrease on Submit"));
                list.Add(Bonus("Modifier - Add Sell Price"));
                list.Add(Bonus("Modifier - Add on New Tile"));
                list.Add(Bonus("Modifier - Unused Upgrades"));
                list.Add(Bonus("Modifier - Use Upgrade"));
                list.Add(Bonus("Modifier - Specific Length Unchanging"));
                list.Add(Bonus("Modifier - Special Round Boost"));
                list.Add(Bonus("Modifier - Boost Per Word"));
                list.Add(Bonus("Modifier - Add Last Tile Score to BS"));
                list.Add(Bonus("Modifier - Build with Letters"));
                list.Add(Bonus("Modifier - Build with Letter"));
                list.Add(Bonus("Modifier - Alphabet Challenge"));
                list.Add(Bonus("Modifier - I Run Challenge"));
                list.Add(Bonus("Modifier - E Run Challenge"));
                list.Add(Bonus("Modifier - O Run Challenge"));
                list.Add(Bonus("Modifier - If Refresh Specials"));
                list.Add(Bonus("Modifier - Specific Letter Unchange"));
                list.Add(Bonus("Modifier - Specific Last Letter Unchange"));
                list.Add(Bonus("Modifier - Odd or Even"));
                list.Add(Bonus("Modifier - Boost with Letter Pairs"));
                list.Add(Bonus("Modifier - Double Down Challenge"));
                list.Add(Bonus("Modifier - Boost with Numbers"));
                list.Add(Bonus("Modifier - Leech Special"));
            }
            if(Mod.Has("Special Tiles - Emerald"))
            {
                list.Add(Bonus("Gift - Add Random Emerald"));
                list.Add(Bonus("Gift - Make All Emerald"));
                list.Add(Bonus("Up - Make Emerald Tiles"));
                list.Add(Bonus("Modifier - Emerald Touch"));
                list.Add(Bonus("Modifier - Emeralds Twice As Lucky"));
                list.Add(Bonus("Modifier - Emeralds Can Break"));
                list.Add(Bonus("Modifier - Convert 6th to Emerald"));
            }
            if(Mod.Has("Special Tiles - Diamond"))
            {
                list.Add(Bonus("Gift - Add Random Diamond"));
                list.Add(Bonus("Up - Make Diamond Tiles"));
                list.Add(Bonus("Modifier - Diamond Boost"));
                list.Add(Bonus("Modifier - Convert Unsubmitted To Diamond"));
            }
            if(Mod.Has("Special Tiles - Dots"))
            {
                list.Add(Bonus("Gift - Add Random Diamond"));
                list.Add(Bonus("Up - Make Dot Tiles"));
                list.Add(Bonus("Modifier - Dot Boost"));
            }
            if(Mod.Has("Special Tiles - Potion"))
            {
                list.Add(Bonus("Gift - Add Random Potion (High Score)"));
                list.Add(Bonus("Gift - Add Random Potion (Low Score)"));
            }
            if(Mod.Has("Special Tiles - Gold"))
            {
                list.Add(Bonus("Gift - Make All Gold"));
                list.Add(Bonus("Up - Make Golden Tiles"));
                list.Add(Bonus("Modifier - Midas Touch"));
                list.Add(Bonus("Modifier - Convert 7th to Golden"));
                list.Add(Bonus("Modifier - Refresh into Gold"));
                if(Mod.Has("Upgrades - Clone Tiles")) list.Add(Bonus("Up - 5 Golden Clones"));
            }
            if(Mod.Has("Special Tiles - Mirror"))
            {
                list.Add(Bonus("Gift - Add Mirror Tiles"));
            }
            Mod.Log($"Entering shop with {list.Count} options");
        }
        static BaseBonus Bonus(string name)
        {
            return GameObject.Find("Managers/Bonuses/Bonuses /"+name).GetComponent<BaseBonus>();
        }
        public static void SaveGame(SaveData __instance)
        {
            Mod.Log("Editing Save Data");
            var items = Mod.Session.Items.AllItemsReceived;
            var refreshCount = (from i in items where i.ItemName == "Refresh" select i).Count();
            var playCount = (from i in items where i.ItemName == "Play" select i).Count();
            __instance.livesToSave -= playCount;
            __instance.refreshesToSave -= refreshCount;
            Mod.Log($"Took away {refreshCount} refreshes and {playCount} plays");
        }
        public static void LoadGame(LogicManagerScript __instance)
        {
            Mod.Log("Adding AP Plays/Refreshes");
            var items = Mod.Session.Items.AllItemsReceived;
            var refreshCount = (from i in items where i.ItemName == "Refresh" select i).Count();
            var playCount = (from i in items where i.ItemName == "Play" select i).Count();
            __instance.SetLives(__instance.lives + playCount);
            __instance.ChangeNumberOfRefreshes(refreshCount);
            Mod.Log($"Added {refreshCount} refreshes and {playCount} plays");
        }
        public static void AddPlay()
        {
            var logic = GameObject.Find("Managers/Game Logic").GetComponent<LogicManagerScript>();
            var prevLives = logic.lives;
            logic.AddLives(1);
            Mod.Log($"Recieved Play: Lives increased from {prevLives} to {logic.lives}");
        }
        public static void AddRefresh()
        {
            var logic = GameObject.Find("Managers/Game Logic").GetComponent<LogicManagerScript>();
            var prevRefresh = logic.numberOfRefreshes;
            logic.ChangeNumberOfRefreshes(1);
            Mod.Log($"Recieved Refresh: Increased from {prevRefresh} to {logic.numberOfRefreshes}");
        }
        public static void CheckWordLength(SubmitWord __instance)
        {
            var length = Mathf.Min(__instance.currentWordLength, 9);
            if(length > 4) Mod.CompleteCheck("5LetterWord");
            if(length > 5) Mod.CompleteCheck("6LetterWord");
            if(length > 6) Mod.CompleteCheck("7LetterWord");
            if(length > 7) Mod.CompleteCheck("8LetterWord");
            if(length > 8) Mod.CompleteCheck("9LetterWord");
        }
        public static void CheckWordPoints(SubmitWord __instance)
        {
            var p = __instance.currentFinalScore;
            foreach(var n in new int[] {10, 15, 20, 25,30, 50, 75, 100, 125, 150, 175, 200, 225, 250})
            {
                if(p >= n)
                {
                    Mod.CompleteCheck($"{n}PointWord");
                }
            }
        }
        public static void NewGame(LogicManagerScript __instance)
        {
            var items = Mod.Session.Items.AllItemsReceived;
            var playCount = (from i in items where i.ItemName == "Play" select i).Count();
            __instance.SetLives(__instance.lives + playCount);
            var refreshCount = (from i in items where i.ItemName == "Refresh" select i).Count();
            __instance.ChangeNumberOfRefreshes(refreshCount);
        }
        public static void CheckRound(LogicManagerScript __instance)
        {
            var r = __instance.currentRound;
            Mod.Log($"Level {MetaGameRules.Instance.inProgressRule} Round {r}");
            switch (MetaGameRules.Instance.inProgressRule)
            {
                case 1:
                    if(r == __instance.roundsToWin)
                        Mod.CompleteCheck($"EasyFinished");
                    else
                        Mod.CompleteCheck($"EasyRound{r}");
                    break;
                case 2:
                    if(r == __instance.roundsToWin)
                        Mod.CompleteCheck($"NormalFinished");
                    else
                        Mod.CompleteCheck($"NormalRound{r}");
                    break;
                case 3:
                    if(r == __instance.roundsToWin)
                    {
                        Mod.CompleteCheck($"HardFinished");
                        if((int)Mod.SlotData["goal"] == 0) Mod.Session.SetGoalAchieved();
                    }
                    else
                        Mod.CompleteCheck($"HardRound{r}");
                    break;
                case 4:
                    if(r == __instance.roundsToWin)
                    {
                        Mod.CompleteCheck($"LegendaryFinished");
                        if((int)Mod.SlotData["goal"] == 1) Mod.Session.SetGoalAchieved();
                    }
                        
                    else
                        Mod.CompleteCheck($"LegendaryRound{r}");
                    break;
            }
        }
    }
}