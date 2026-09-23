using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using UnityEngine.Events;
using Unity.VisualScripting;
namespace WPArchipelagoMod
{
    [BepInPlugin("TechnicalityCreations.WPArchipelagoMod", "Archipelago", "0.1.0")]
    public class Mod : BaseUnityPlugin
    {
        internal static ArchipelagoSession Session;
        static Mod inst;
        public static void Log(object s, LogLevel level = LogLevel.Message)
        {
            if(inst != null && inst.Logger != null)
                inst.Logger.Log(level, s);
            else
            {
                BepInEx.Logging.Logger.CreateLogSource("WordPlay").Log(level, s);
            }
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
            SceneManager.activeSceneChanged += SetUpTitleScreenUI;
            Log("Archipelago has loaded successfully");
        }
        public static SceneType CurrentScene;
        public static void ItemRecieved(Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                var i = helper.DequeueItem();
                if(i.ItemName == "Progressive Difficulty" && CurrentScene == SceneType.Title)
                {
                    DifficultyManager.UpdateDifficultyButtons();
                }
            }
        }
        int attempt = 0;
        public void SetUpGame()
        {
            Log("Game Scene Loaded");
            CurrentScene = SceneType.Game;
        }
        public void SetUpTitleScreenUI(Scene ignoreMe, Scene s)
        {
            if(s.name == "Game")
            {
                SetUpGame();
                return;
            }
            var isConnected = Session != null;
            CurrentScene = SceneType.Title;
            attempt++;
            if(attempt <= 2)
            {
                Log("Blocked SetUpTitleScreenUI");
                return;
            }
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
            passwordField.contentType = TMP_InputField.ContentType.Password;
            ((TextMeshProUGUI)passwordField.placeholder).text = "Password";
            var slot = Instantiate(hostField, hostField.transform.parent);
            slot.name = "Slot";
            var slotField = slot.GetComponent<TMP_InputField>();
            ((TextMeshProUGUI)slotField.placeholder).text = "Slot";
            hostField.text = "archipelago.gg";
            Log("Loaded UI");

            var connect = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            Destroy(buttonTemplate);
            connect.name = "Connect Button";
            connect.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
            Log("Loaded UI");
            //await Task.Delay(500);
            if(connect == null)
            {
                Log("Connect is null");
                return;
            }
            var b = connect.GetComponent<Button>();
            Log("Gotten b");
            if(b == null)
            {
                Log("b is null", LogLevel.Error);
                return;
            }
            if(b.onClick == null) Log("b.onClick is null", LogLevel.Error);
            var onClick = new UnityAction(ConnectToMultiworld);
            Log("Created Unity Action");
            b.onClick.AddListener(onClick);
            Log("Added Listener");
            var playButton = GameObject.Find("Canvas - Main/Title Screen/Buttons/Play Button").GetComponent<Button>();
            playButton.interactable = isConnected;
            if (isConnected)
            {
                hostField.text = Server;
                portField.text = PortNumber;
                passwordField.text = Password;
                slotField.text = SlotName;
                connect.GetComponentInChildren<TextMeshProUGUI>().text = "Success";
                b.interactable = false;
                DifficultyManager.InitialiseDifficultyOptions();
            }
            Log("Connected to Connect Button");
        }
        static string Server, PortNumber, Password, SlotName;
        public static void ConnectToMultiworld()
        {
            Log("Connecting");
            var apPanelPath = "Canvas - Main/Title Screen/Archipelago Panel/Options/";
            var button = GameObject.Find(apPanelPath + "Connect Button");
            var b = button.GetComponent<Button>();
            var bText = button.GetComponentInChildren<TextMeshProUGUI>();
            bText.text = "Connecting";
            b.enabled = false;
            Server = GameObject.Find(apPanelPath + "Host").GetComponent<TMP_InputField>().text;
            PortNumber = GameObject.Find(apPanelPath + "Port").GetComponent<TMP_InputField>().text;
            Password = GameObject.Find(apPanelPath + "Password").GetComponent<TMP_InputField>().text;
            SlotName = GameObject.Find(apPanelPath + "Slot").GetComponent<TMP_InputField>().text;
            Session = ArchipelagoSessionFactory.CreateSession(Server + ":" + PortNumber);
            var result = Session.TryConnectAndLogin("Word Play", SlotName, ItemsHandlingFlags.IncludeOwnItems, password: Password);
            b.enabled = true;
            if (result.Successful)
            {
                bText.text = "Success";
                Log("Successfully Connected");
                var playButton = GameObject.Find("Canvas - Main/Title Screen/Buttons/Play Button").GetComponent<Button>();
                playButton.interactable = true;
                b.interactable = false;
                Session.Items.ItemReceived += ItemRecieved;
                DifficultyManager.InitialiseDifficultyOptions();
            }
            else
            {
                bText.text = "Couldn't Connect";
                Log("Failure to Connect");
                Session = null;
            }
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
                using var b = new BinaryReader(s);
                b.Read(buffer);
                var t = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(t, buffer);
                Log($"Loaded image {name}");
                return t;
            }
        }
    }
    public enum SceneType
    {
        Title,
        Game
    }
}