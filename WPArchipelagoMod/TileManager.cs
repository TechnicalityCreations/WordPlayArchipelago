using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace WPArchipelagoMod
{
    public static class TileManager
    {
        static Dictionary<string, int> LetterCounts = new Dictionary<string, int>()
        {
            {"A", 9},
            {"B", 2},
            {"C", 2},
            {"D", 4},
            {"E", 12},
            {"F", 2},
            {"G", 3},
            {"H", 2},
            {"I", 8},
            {"J", 1},
            {"K", 1},
            {"L", 4},
            {"M", 2},
            {"N", 6},
            {"O", 8},
            {"P", 2},
            {"Q", 1},
            {"R", 6},
            {"S", 4},
            {"T", 6},
            {"U", 4},
            {"V", 2},
            {"W", 2},
            {"X", 1},
            {"Y", 1},
            {"Z", 1}
        };
        public static void Patch(Harmony h)
        {
            h.Patch(AccessTools.Method(typeof(LetterBagManager), "LoadLetterBag"), prefix: new HarmonyMethod(typeof(TileManager), nameof(LoadLetterBag)));
        }
        public static void RecieveLetterItem(string item)
        {
            if(item.Length != 1) return;
            var m = GameObject.Find("Managers/Letter Bag").GetComponent<LetterBagManager>();
            var letterBag = m.LetterBag;
            if(letterBag.Contains(item + "S0000")) return;
            var count = LetterCounts[item];
            for(int i = 0; i < count; i++)
            {
                letterBag.Add(item + "S0000");
                m.LettersInPlay.Add(item + "S0000");
            }
            Mod.Log($"Added {count} of {item} to letter bag");
        }
        public static bool LoadLetterBag(LetterBagManager __instance)
        {
            Mod.Log("Loading Letter Bag");
            var LettersUnlocked = from i in Mod.Session.Items.AllItemsReceived where i.ItemName.Length == 1 select i.ItemName;
            __instance.LetterBag = new List<string>();
            foreach(string l in LettersUnlocked)
            {
                var count = LetterCounts[l];
                for(int i = 0; i < count; i++)
                    __instance.LetterBag.Add(l + "S0000");
                Mod.Log($"Added {count} of {l} to letter bag");
            }
            return false;
        }
    }
}