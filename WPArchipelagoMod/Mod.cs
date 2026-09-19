using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WPArchipelagoMod
{
    [BepInPlugin("TechnicalityCreations.WPArchipelagoMod", "Archipelago", "0.1.0")]
    public class Mod : BaseUnityPlugin
    {
        static Mod inst;
        public static void Log(object s, LogLevel level = LogLevel.Message)
        {
            if(inst != null && inst.Logger != null)
                inst.Logger.Log(level, s);
        }
        public Mod()
        {
            inst = this;
        }
        void Awake()
        {
            Log("Archipelago is loading");
            var h = new Harmony("TechnicalityCreations.WPArchipelagoMod");
            h.PatchAll();
            SceneManager.activeSceneChanged += SceneChanged;
            Log("Archipelago has loaded successfully");
        }
        public void SceneChanged(Scene ignoreMe, Scene s)
        {
            Log($"Title Screen Loaded");
            var oPanel = GameObject.Find("Other Panel");
            var aPanel = Instantiate(oPanel, oPanel.transform.parent.parent.parent);
            aPanel.name = "Archipelago Panel";
            aPanel.transform.localPosition = new Vector3(420, -540, 0);
            var pTitle = GameObject.Find("Canvas - Main/Archipelago Panel/Label Area/Label/Title").GetComponent<TextMeshProUGUI>();
            pTitle.text = "Archipelago";
        }
        public static Texture2D LoadImage(string name)
        {
            var a = Assembly.GetExecutingAssembly();
            using(Stream s = a.GetManifestResourceStream("WPArchipelagoMod.Assets." +name + ".png"))
            {
                if(s == null)
                {
                    Log($"Couldnt find resource {name}", LogLevel.Error);
                    return null;
                }
                var buffer = new byte[s.Length];
                s.Read(buffer);
                
                var t = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(t, buffer);
                Log($"Loaded image {name}");
                return t;
            }
        }
    }
}