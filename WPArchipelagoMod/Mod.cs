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
            var hostField = host.AddComponent<TMP_InputField>();
            var t = host.GetComponentInChildren<TextMeshProUGUI>();
            hostField.textComponent = t;
            hostField.textViewport = t.rectTransform;
            t.raycastTarget = false;
            var placeHolder = Instantiate(t, t.transform.parent);
            var c = placeHolder.color;
            c.a = 0.75f;
            placeHolder.color = c;
            placeHolder.text = "Server";
            hostField.placeholder = placeHolder;
            hostField.caretColor = Color.black;
            hostField.customCaretColor = true;
            hostField.caretWidth = 2;
            hostField.onFocusSelectAll = false;
            Log("Added InputField Components");
            hostField.enabled = false;
            hostField.enabled = true;
            var port = Instantiate(hostField, hostField.transform.parent);
            port.name = "Port";
            var portField = port.GetComponent<TMP_InputField>();
            portField.characterValidation = TMP_InputField.CharacterValidation.Digit;
            portField.characterLimit = 5;
            ((TextMeshProUGUI)portField.placeholder).text = "Port";
            var password = Instantiate(hostField, hostField.transform.parent);
            password.name = "Password";
            var passwordField = password.GetComponent<TMP_InputField>();
            ((TextMeshProUGUI)passwordField.placeholder).text = "Password";
            var slot = Instantiate(hostField, hostField.transform.parent);
            slot.name = "Slot";
            var slotField = slot.GetComponent<TMP_InputField>();
            ((TextMeshProUGUI)slotField.placeholder).text = "Slot";
            hostField.text = "archipelago.gg";

            var connect = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            Destroy(buttonTemplate);
            connect.name = "Connect Button";
            connect.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
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