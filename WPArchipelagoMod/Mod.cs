using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Log("WPArchipelagoMod is loading");
            var h = new Harmony("TechnicalityCreations.WPArchipelagoMod");
            h.PatchAll();
            SceneManager.activeSceneChanged += SceneChanged;
            Log("WPArchipelagoMod has loaded successfully");
        }
        public void SceneChanged(Scene old, Scene newScene)
        {
            Log($"Scene Changed form {old.name} to {newScene.name}");
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