using Archipelago.MultiClient.Net;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

namespace WPArchipelagoMod
{
    public static class DifficultyManager
    {
        static ArchipelagoSession Session => Mod.Session;
        static GameObject Normal, Hard, Legendary;
        internal static int ProgDiffs = 0;
        public static void InitialiseDifficultyOptions()
        {
            Mod.Log("Initialising Difficulty Manager");
            ModifyDifficultySelectUI();
            Normal = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - B Normal");
            Hard = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - C Hard");
            Legendary = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - L Legendary");
            UpdateDifficultyButtons();
            Mod.Log("Initialised Difficulty Manager");
        }
        public static void UpdateDifficultyButtons()
        {
            Mod.Log("Updating Difficulty Buttons");
            var Items = Session.Items.AllItemsReceived;
            var progDiff = from i in Items where i.ItemName == "Progressive Difficulty" select i;
            ProgDiffs = progDiff.Count();
            Mod.Log($"Prog Difficulties: {ProgDiffs}");
            Normal.SetActive(ProgDiffs >0);
            Hard.SetActive(ProgDiffs >1);
            Legendary.SetActive(ProgDiffs >2);
            Mod.Log("Updated Difficulty Buttons");
        }
        static void ModifyDifficultySelectUI()
        {
            Mod.Log("Modifying Select UI");
            GameObject.Destroy(GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Casual"));
            GameObject.Destroy(GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/Label - Strategic"));
            GameObject.Destroy(GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - M Marathon"));
            var picker = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy").GetComponent<RectTransform>();
            picker.sizeDelta = new Vector2(1235.89f, 219);
            picker.anchorMax = picker.anchorMin = new Vector2(0.5f, 0.5f);
            picker.offsetMax = new Vector2(617.9449f, 109.5f);
            picker.offsetMin = new Vector2(-617.9449f, -109.5f);
            Mod.Log("Modified Select UI");
        }
    }
}