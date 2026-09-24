using HarmonyLib;
using UnityEngine;

namespace WPArchipelagoMod
{
    public static class WordSubmitManager
    {
        public static void Patch(Harmony h)
        {
            h.Patch(AccessTools.Method(typeof(SubmitWord), "WordWasAccepted"), postfix: new HarmonyMethod(typeof(WordSubmitManager), nameof(CheckWordLength)));
            h.Patch(AccessTools.Method(typeof(SubmitWord), "FinishSubmission"), postfix: new HarmonyMethod(typeof(WordSubmitManager), nameof(CheckWordPoints)));
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
    }
}