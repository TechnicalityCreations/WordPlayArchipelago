using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
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
            
            Log("Title Screen Loaded");
            var oPanel = GameObject.Find("Other Panel");
            var aPanel = Instantiate(oPanel, GameObject.Find("Canvas - Main/Title Screen").transform);
            aPanel.name = "Archipelago Panel";
            aPanel.transform.localPosition = new Vector3(420, -540, 0);
            aPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 558);
            var pTitle = GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Label Area/Label/Title").GetComponent<TextMeshProUGUI>();
            pTitle.text = "Archipelago";
            Log("Created Archipelago Panel");
            Destroy(GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Options/Report an Issue"));
            Destroy(GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Options/Delete Save Button"));
            var buttonTemplate = GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Options/Credits Button");
            buttonTemplate.SetActive(true);
            Log("Discovered button templates");
            var host = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            host.name = "Host";
            DestroyImmediate(host.GetComponent<Button>());
            DestroyImmediate(host.GetComponent<ButtonTextColouriser>());
            Log("Disabled button components");
            var input = host.AddComponent<TMP_InputField>();
            input.textComponent = host.GetComponentInChildren<TextMeshProUGUI>();
            input.textViewport = input.textComponent.rectTransform;
            input.textComponent.raycastTarget = false;
            input.text = input.textComponent.text = "archipelago.gg";
            input.caretColor = Color.black;
            input.customCaretColor = true;
            input.caretWidth = 2;
            input.onFocusSelectAll = false;
            Log("Added InputField Components");
            input.enabled = false;
            input.enabled = true;
            
        }
        public void Update()
        {
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