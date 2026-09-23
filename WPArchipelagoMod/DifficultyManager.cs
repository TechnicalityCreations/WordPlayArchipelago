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

        public static void InitialiseDifficultyOptions()
        {
            var Items = Session.Items.AllItemsReceived;
            var progDiff = from i in Items where i.ItemName == "Progressive Difficulty" select i;
            var progressiveDifficultyCount = progDiff.Count();
            ModifyDifficultySelectUI();
            Normal = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - B Normal");
            Hard = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - C Hard");
            Normal = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - L Legendary");
        }
        static void ModifyDifficultySelectUI()
        {
            GameObject.Destroy(GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Casual"));
            GameObject.Destroy(GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/Label - Strategic"));
            GameObject.Destroy(GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy/The Tiles/Level - M Marathon"));
            var picker = GameObject.Find("Canvas - Main/Level Select/Picker/Level Tiles - Strategy").GetComponent<RectTransform>();
            picker.sizeDelta = new Vector2(1235.89f, 219);
            picker.anchorMax = picker.anchorMax = new Vector2(0.5f, 0.5f);
            picker.offsetMax = new Vector2(617.9449f, 109.5f);
            picker.offsetMin = new Vector2(-617.9449f, -109.5f);
        }
    }
}