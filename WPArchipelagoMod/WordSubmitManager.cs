using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace WPArchipelagoMod
{
    public static class GameplayManager
    {
        public static void Patch(Harmony h)
        {
            h.Patch(AccessTools.Method(typeof(SubmitWord), "WordWasAccepted"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(CheckWordLength)));
            h.Patch(AccessTools.Method(typeof(SubmitWord), "FinishSubmission"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(CheckWordPoints)));
            h.Patch(AccessTools.Method(typeof(LogicManagerScript), "FinishRound"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(CheckRound)));
            h.Patch(AccessTools.Method(typeof(LogicManagerScript), "StartFreshGame"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(NewGame)));
            h.Patch(AccessTools.Method(typeof(LogicManagerScript), "LoadGameState"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(LoadGame)));
            h.Patch(AccessTools.Method(typeof(SaveData), "GetAllLogicData"), postfix: new HarmonyMethod(typeof(GameplayManager), nameof(SaveGame)));


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