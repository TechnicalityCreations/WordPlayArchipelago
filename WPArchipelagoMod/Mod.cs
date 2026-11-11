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
using System.Collections.Generic;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using System;
using System.Linq;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using UnityEngine.Playables;
using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Models;
namespace WPArchipelagoMod
{
    public class NotificationUpdater : MonoBehaviour
        {
            private void Awake()
        {
            Mod.Log("Notification Updater Awake");
            DontDestroyOnLoad(gameObject);
        }
            void Update()
            {
                if(Mod.CurrentScene == SceneType.Game)
                {
                    if(Mod.TimeSinceDeathlink > 0) Mod.TimeSinceDeathlink -= Time.deltaTime;
                    if(Mod.timeLeft > 0)
                    {
                        Mod.timeLeft-= Time.deltaTime;
                        if(Mod.timeLeft <= 0)
                            Mod.Notification.HideTutorialMessage();
                    }
                    else
                    {
                        if(Mod.Messages.TryDequeue(out var m))
                        {
                            Mod.Notification.TutorialMessage(m, true, true, true);
                            Mod.timeLeft = 3;
                        }
                    }
                    if(GameplayManager.QueueDL) GameplayManager.DeathLink();
                }
            }
        }
    [BepInPlugin("TechnicalityCreations.WPArchipelagoMod", "Archipelago", "0.1.0")]
    public class Mod : BaseUnityPlugin
    {
        #nullable enable
        internal static DeathLinkService? DeathLink;
        #nullable disable
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
        internal static ConcurrentQueue<string> Messages;
        internal static float timeLeft;
        
        void Awake()
        {
            Log("Archipelago is loading");
            var h = new Harmony("TechnicalityCreations.WPArchipelagoMod");
            TileManager.Patch(h);
            GameplayManager.Patch(h);
            SceneManager.activeSceneChanged += SetUpTitleScreenUI;
            h.Patch(AccessTools.Method(typeof(KeyboardControl), "OnTextInput"), new HarmonyMethod(typeof(Mod), nameof(BlockTextInput)));
            Log("Archipelago has loaded successfully");
            
        }
        public static SceneType CurrentScene;
        public static bool Has(string name) => Count(name) > 0;
        public static int Count(string name) => Session.Items.AllItemsReceived.Where((i)=>i.ItemName == name).Count();
        public static void ItemRecieved(Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper helper)
        {
            while (helper.Any())
            {
                    try
                    {
                        
                    var i = helper.DequeueItem();
                    Log("Item Recieved: " + i.ItemName);
                    if(CurrentScene == SceneType.Game)
                    {
                        var wordBar = GameObject.FindAnyObjectByType<LastWordArea>();
                        if(i.Player.Name != Session.Players.ActivePlayer.Name)
                        {
                            NotificationMessage($"Received '{i.ItemDisplayName}' from {i.Player}");
                        }
                        else
                        {
                            NotificationMessage($"Found '{i.ItemDisplayName}'");
                        }
                    }
                    if(i.ItemName == "Progressive Difficulty" && CurrentScene == SceneType.Title)
                    {
                        DifficultyManager.UpdateDifficultyButtons();
                    }
                    if(CurrentScene == SceneType.Game)
                    {
                        if(i.ItemName == "Play") GameplayManager.AddPlay();
                        if(i.ItemName == "Refresh") GameplayManager.AddRefresh();
                        if(i.ItemName.Contains("Trap")) GameplayManager.AddTrap(i.ItemName);
                    }
                    if(i.ItemName.Replace("ERS", "3").Replace("ING", "1").Length == 1)
                    {
                        if(CurrentScene == SceneType.Game)
                        {
                            TileManager.RecieveLetterItem(i.ItemName.Replace("ERS", "3").Replace("ING", "1"));
                        }
                        else
                            LetterBagQueue.Add(i.ItemName);
                    }
                }
                catch(Exception e)
                {
                    Log(e, LogLevel.Error);
                }
            }
        }
        int attempt = 0;
        public static LastWordArea Notification;
        public void SetUpGame()
        {
            Log("Game Scene Loaded");
            var a = GameObject.FindAnyObjectByType<LastWordArea>();
            a.transform.localScale = Vector2.one * 0.75f;
            Notification = GameObject.Instantiate(a, a.transform.parent);
            Notification.transform.position = new Vector3(960, 940, 0);
            Notification.transform.SetSiblingIndex(2);
            Notification.name = "AP Notification";
            Messages = new ConcurrentQueue<string>();
            CurrentScene = SceneType.Game;
            var queue = LetterBagQueue.ToArray();
            foreach(var l in queue)
            {
                TileManager.RecieveLetterItem(l);
            }
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
            if (isConnected)
            {
                var r = GameObject.Find("Canvas - Main/Title Screen/Buttons/Resume Button").GetComponent<Button>();
                var b2 = GameObject.Find("Canvas - Main/Title Screen/Buttons/Resume Button/Resume Info/Bubble");
                Destroy(r.gameObject);
                Destroy(b2.gameObject);
                DifficultyManager.InitialiseDifficultyOptions();
                return;
            }
            var u = new GameObject("Notification Updater");
            u.AddComponent<NotificationUpdater>();
            DontDestroyOnLoad(u);
            var oPanel = GameObject.Find("Other Panel");
            var aPanel = Instantiate(oPanel, GameObject.Find("Canvas - Main/Title Screen").transform);
            aPanel.name = "Archipelago Panel";
            aPanel.transform.localPosition = new Vector3(420, -540, 0);
            aPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 558);
            aPanel.AddComponent<Draggable>();
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
            hostField.caretColor = UnityEngine.Color.black;
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
            var resumeButton = GameObject.Find("Canvas - Main/Title Screen/Buttons/Resume Button").GetComponent<Button>();
            var resumeBubble = GameObject.Find("Canvas - Main/Title Screen/Buttons/Resume Button/Resume Info/Bubble");

            playButton.interactable = false;
            playButton.enabled = false;
            Destroy(resumeBubble.gameObject);
            Destroy(resumeButton.gameObject);
            Log("Connected to Connect Button");
        }
        static string Server, PortNumber, Password, SlotName;
        internal static async void CompleteCheck(string name)
        {
            Log($"Completing Check "+name);
            var id = Session.Locations.GetLocationIdFromName("Word Play", name);
            if(id == -1)
            {
                Log($"Location {name} not found");
                return;
            }
            var item = await Session.Locations.ScoutLocationsAsync(id);
            Log($"Check ID is "+id);
            if (Session.Locations.AllLocationsChecked.Contains(id))
            {
                Log("Check already complete");
                return;
            }
            await Session.Locations.CompleteLocationChecksAsync(id);
            Log($"Completed Check "+name);
            if(CurrentScene == SceneType.Game)
            {
                await Task.Delay(1500);
                var i = item[id];
                if(i.Player.Name != Session.Players.ActivePlayer.Name) NotificationMessage($"Sent '{i.ItemDisplayName}' to {i.Player}");
            }
        }
        static void NotificationMessage(string m)
        {
            Messages.Enqueue(m);
        }
        static TextMeshProUGUI Console;
        public static bool BlockTextInput()
        {
            if(CommandField.isFocused) return false;
            return true;
        }
        static bool isHintToggled = false;
        static string consoleString, hintString;
        static Button hintButton, SendButton;
        public static void ToggleHint()
        {
            isHintToggled = !isHintToggled;
            if (isHintToggled)
            {
                Console.text = hintString;
                hintButton.GetComponentInChildren<TextMeshProUGUI>().text = "Console";
                SendButton.GetComponent<RectTransform>().localPosition = new Vector3(160000, 63, 0);
                var hTrans = hintButton.GetComponent<RectTransform>();
                hTrans.sizeDelta = new Vector2(450, 75);
                hTrans.localPosition = new Vector3(275, 63, 0);
            }
            else
            {
                hintButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hints";
                Console.text = consoleString;
                SendButton.GetComponent<RectTransform>().localPosition = new Vector3(160, 63, 0);
                var hTrans = hintButton.GetComponent<RectTransform>();
                hTrans.sizeDelta = new Vector2(225, 75);
                hTrans.localPosition = new Vector3(390, 63, 0);
                
            }
        }
        public static void TurnAPPanelIntoConsole()
        {
            Log("Turning AP Panel into Console");
            var options = GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Options");
            Destroy(options.GetComponent<VerticalLayoutGroup>());
            var text = options.AddComponent<TextMeshProUGUI>();
            text.font = GameObject.Find("Canvas - Main/Title Screen/Buttons/Play Button").GetComponentInChildren<TextMeshProUGUI>().font;
            text.fontSize = 18;
            text.enableWordWrapping = false;
            Log("Added Text Component to Console");
            for(int i = 0; i <options.transform.childCount; i++)
            {
                options.transform.GetChild(i).gameObject.SetActive(false);
            }
            var canvas = Instantiate(GameObject.Find("Canvas - Main"));
            for(int i = 0; i <canvas.transform.childCount; i++)
            {
                canvas.transform.GetChild(i).gameObject.SetActive(false);
            }
            var slot = GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Options/Slot");
            slot.SetActive(true);
            var stext = slot.GetComponent<TMP_InputField>();
            stext.placeholder.gameObject.SetActive(false);
            stext.placeholder = null;
            stext.text = "";
            CommandField = stext;
            slot.name = "Text Input";
            var connect = GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Options/Connect Button");
            connect.SetActive(true);
            SendButton = connect.GetComponent<Button>();
            SendButton.onClick.RemoveAllListeners();
            var bTrans = SendButton.GetComponent<RectTransform>();
            bTrans.sizeDelta = new Vector2(225, 75);
            SendButton.interactable = true;
            hintButton = Instantiate(SendButton, SendButton.transform.parent);
            SendButton.onClick.AddListener(new UnityAction(SendMessage));
            SendButton.GetComponentInChildren<TextMeshProUGUI>().text = "Send";
            
            hintButton.name = "Hint Button";
            
            hintButton.onClick.AddListener(new UnityAction(ToggleHint));
            hintButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hints";

            var panel = GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel");
            var title = GameObject.Find("Canvas - Main/Title Screen/Archipelago Panel/Label Area");
            stext.transform.SetParent(panel.transform, true);
            SendButton.transform.SetParent(panel.transform, true);
            hintButton.transform.SetParent(panel.transform, true);
            hintButton.transform.localPosition = new Vector3(390, 63, 0);
            bTrans.localPosition = new Vector3(160, 63, 0);

            Log("Finished Creating Console");
            title.GetComponent<Image>().raycastTarget = text.raycastTarget = false;
            panel.transform.SetParent(canvas.transform, true);
            Destroy(panel.GetComponent<Draggable>());
            panel.AddComponent<Draggable>();
            DontDestroyOnLoad(canvas);
            Log("Made Console Permanent");
            Console = text;
        }
        internal static TMP_InputField CommandField;
        public static void SendMessage()
        {
            CommandField.DeactivateInputField();
            Session.Say(CommandField.text);
            CommandField.text = "";
        }
        public static Dictionary<string, object> SlotData;
        internal static float TimeSinceDeathlink;
        public static void OnHint(Hint[] hints)
        {
            hintString = "";
            hints = hints.SkipWhile((h)=>h.Found).OrderBy(GetHintValue).ToArray();
            int lines = 0;
            foreach(var h in hints)
            {
                if(lines > 15) break;
                var locationName = Session.Locations.GetLocationNameFromId(h.LocationId);
                var gameName = Session.Players.GetPlayerInfo(h.ReceivingPlayer).Game;
                var player = Session.Players.GetPlayerInfo(h.ReceivingPlayer).Alias;
                var itemName = Session.Items.GetItemName(h.ItemId, gameName);
                hintString+= $"<colour=blue>{locationName}</colour> - {player}'s {itemName}\n";
                lines++;
            }
            if(isHintToggled) Console.text = hintString;
            else Console.text = consoleString;
            
        }
        public static int GetHintValue(Hint h)
        {
            var lid = h.LocationId;
            var l = Session.Locations.GetLocationNameFromId(lid);
            float locationValue = 1;
            bool addLocValue = false;
            if (l.Contains("Easy"))
            {
                locationValue = 1;
            }
            if (l.Contains("Normal"))
            {
                if(DifficultyManager.ProgDiffs > 0) locationValue = 2;
                else locationValue = 1001;
            }
            if (l.Contains("Hard"))
            {
                if(DifficultyManager.ProgDiffs > 1) locationValue = 3;
                else locationValue = 1002;
            }
            if (l.Contains("Legendary"))
            {
                if(DifficultyManager.ProgDiffs > 2) locationValue = 4;
                else locationValue = 1003;
            }
            if (l.Contains("UsedLetter"))
            {
                var letter = l.Replace("UsedLetter", "");
                if(Session.Items.AllItemsReceived.Where((i)=>i.ItemName==letter).Count()>0) locationValue = 0.1f;
                else locationValue = 1000;
            }
            if (l.Contains("PointWord"))
            {
                var num = l.Replace("PointWord", "");
                if(!int.TryParse(num, out int n)) return 100000;
                locationValue = n /5f;
                addLocValue = true;
            }
            if (l.Contains("LetterWord"))
            {
                var num = l.Replace("LetterWord", "");
                if(!int.TryParse(num, out int n)) return 100000;
                locationValue = n * 2;
                addLocValue = true;
            }
            int itemVal =1;
            var i = h.ItemFlags;
            if(i== ItemFlags.Advancement) itemVal = 1;
            if(i== ItemFlags.NeverExclude) itemVal = 3;
            if(i== ItemFlags.None) itemVal = 5;
            if(i== ItemFlags.Trap) itemVal = 1000;
            if(addLocValue) return (int)locationValue + itemVal;
            return (int)(locationValue * itemVal);
        }
        public static async void ConnectToMultiworld()
        {
            Log("Connecting");
            var apPanelPath = "Canvas - Main/Title Screen/Archipelago Panel/Options/";
            var button = GameObject.Find(apPanelPath + "Connect Button");

            var b = button.GetComponent<Button>();
            var bText = button.GetComponentInChildren<TextMeshProUGUI>();
            bText.text = "Connecting";
            bText.color = UnityEngine.Color.black;
            b.enabled = false;
            Server = GameObject.Find(apPanelPath + "Host").GetComponent<TMP_InputField>().text;
            PortNumber = GameObject.Find(apPanelPath + "Port").GetComponent<TMP_InputField>().text;
            Password = GameObject.Find(apPanelPath + "Password").GetComponent<TMP_InputField>().text;
            SlotName = GameObject.Find(apPanelPath + "Slot").GetComponent<TMP_InputField>().text;
            Session = ArchipelagoSessionFactory.CreateSession(Server + ":" + PortNumber);
            Session.Items.ItemReceived += ItemRecieved;
            try
            {
                var connectResult = await Session.ConnectAsync();
            }
            catch (Exception e)
            {
                Log(e);
                goto Failure;
            }
            var result = await Session.LoginAsync("Word Play", SlotName, ItemsHandlingFlags.AllItems, password: Password);
            b.enabled = true;
            if (result.Successful)
            {
                SlotData = (result as LoginSuccessful).SlotData;
                Log(string.Join(", ", SlotData.Keys));
                Log(string.Join(", ", SlotData.Values));
                var playButton = GameObject.Find("Canvas - Main/Title Screen/Buttons/Play Button").GetComponent<Button>();
                playButton.interactable = true;
                playButton.enabled = true;
                b.interactable = false;
                DifficultyManager.InitialiseDifficultyOptions();
                LogRecievedItems();
                SaveFilePath.SetValue(MetaGameSave.Instance, Path.Combine(Application.persistentDataPath, $"archipelago{PortNumber}{SlotName}savedata.json"));
                MetaGameSave.Instance.LoadData();
                TurnAPPanelIntoConsole();
                Session.MessageLog.OnMessageReceived += AddMessageToConsole;
                Session.Hints.TrackHints(OnHint);
                if ((long)SlotData["deathlink"] == 1)
                {
                    Log("Deathlink");
                    DeathLink = Session.CreateDeathLinkService();
                    DeathLink.OnDeathLinkReceived += GameplayManager.OnDeathLink;
                    DeathLink.EnableDeathLink();
                }
                return;
            }
        Failure:
            bText.text = "Couldn't Connect";
            bText.color = UnityEngine.Color.red;
            b.interactable = true;
            b.enabled = true;
            Log("Failure to Connect");
            Session = null;
        }
        static List<string> ConsoleLines = new List<string>();
        public static void AddMessageToConsole(LogMessage message)
        {
            var s = "";
            foreach(var part in message.Parts)
            {
                var c = part.Color;
                var colourHex = $"#{BitConverter.ToString(new byte[]{c.R, c.G, c.B})}".Replace("-", "");
                s += $"<color={colourHex}>{part.Text}</color> ";
            }
            ConsoleLines.Add(s);
            if(ConsoleLines.Count() > 15) ConsoleLines.RemoveAt(0);
            consoleString = string.Join("\n", ConsoleLines);
            if(!isHintToggled) Console.text = consoleString;
            else Console.text = hintString;
            Log(s);
        }
        static FieldInfo SaveFilePath = AccessTools.Field(typeof(MetaGameSave), "saveFilePath");
        static List<string> LetterBagQueue = new List<string>();
        static async void LogRecievedItems()
        {
            await Task.Delay(200);
            Log("Current Inventory:", LogLevel.Info);
            foreach(var item in Session.Items.AllItemsReceived)
            {
                Log(item.ItemName, LogLevel.Info);
            }
            Log(Session.Items.Any());
            Log(Session.Items.DequeueItem());
        }
        static async void Reconnect()
        {
            await Task.Delay(5000);
            if (!Session.Socket.Connected)
            {
                try
                {
                    var connectResult = await Session.ConnectAsync();
                }
                catch
                {
                    Reconnect();
                }
            var result = await Session.LoginAsync("Word Play", SlotName, ItemsHandlingFlags.AllItems, password: Password);
            }
            Reconnect();
        }
    }
    public enum SceneType
    {
        Title,
        Game
    }
}