using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Unity.Collections.LowLevel.Unsafe.BurstRuntime;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;
using Zorro.Core;
using TMPro;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using IL;

namespace TranslatedWarning.Patches
{
    public class DialogRipper
    {
        private static bool isWritten = false;
        private static bool isWrittenItem = false;

        public static Dictionary<string, string> commentList = new Dictionary<string, string>();

        static string[] values = new string[50];

        public static string prevName = "";

        public static Dictionary<string, string> uiDict = new Dictionary<string, string>();

        internal static void Init()
        {
            /*
             *  Subscribe with 'On.Namespace.Type.Method += CustomMethod;' for each method you're patching.
             *  Or if you are writing an ILHook, use 'IL.' instead of 'On.'
             *  Note that not all types are in a namespace, especially in Unity games.
             */



            On.LocalizationKeys.GetLocalizedString += LocalizationKeys_GetLocalizedString;


            On.PlayerEmoteContentEvent.GenerateComment += PlayerEmoteContentEvent_GenerateComment;


            On.ItemDatabase.TryGetItemFromID += ItemDatabase_TryGetItemFromID;

            On.MainMenuMainPage.Awake += MainMenuMainPage_Awake;
            On.MainMenuSettingsPage.Awake += MainMenuSettingsPage_Awake;

            On.ShopHandler.Awake += ShopHandler_Awake;


            DataCollection(); //collects most comments

            Print("//COMMENTS", append: false);
            foreach (KeyValuePair<string, string> kvp in commentList)
            {
                //int hashCode = HashFunction(comment, values); //creates hashcode for comments
                Print(kvp.Key, kvp.Value);
            }


            PropContent[] props = SingletonAsset<PropContentDatabase>.Instance.Objects;
            Print("//PROP");
            foreach (PropContent prop in props)
            {
                Debug.Log($"{prop.name}");
                int i = 0;
                foreach (string comment in prop.comments)
                {

                    Print(prop.name + "." + i, comment); //collects prop comments
                    i++;
                }
            }

        }




        // ====== UI ======

        //Shop
        private static void ShopHandler_Awake(On.ShopHandler.orig_Awake orig, ShopHandler self)
        {
            var shop = self.gameObject.transform;
            string text = shop.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text;
            Debug.Log(text);
            uiDict.Add(shop.name, text);
            orig(self);
        }


        //Settings
        private static bool isWrittenButtons2 = false;
        private static void MainMenuSettingsPage_Awake(On.MainMenuSettingsPage.orig_Awake orig, MainMenuSettingsPage self)
        {
            //back button
            if (!isWrittenButtons2) 
            {
                GrabButtonText(self.backButton);
                isWrittenButtons2 = true;
            }
            Transform settings = self.backButton.transform.parent;

            //settings text
            string settingsText = settings.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            uiDict.Add(settings.gameObject.name, settingsText);

            //tabs

            Transform tabs = settings.GetChild(2).GetChild(0);
            foreach (Transform tab in tabs) 
            { 
                Debug.Log("LOOKING IN "+ tab.gameObject.name + "!!!!!!!!!!!");
                string text = tab.GetChild(1).GetComponent<TextMeshProUGUI>().text;
                uiDict.Add(tab.gameObject.name, text);
            }

            orig(self);
        }

        //MainPage
        private static bool isWrittenButtons = false;
        private static void MainMenuMainPage_Awake(On.MainMenuMainPage.orig_Awake orig, MainMenuMainPage self)
        {
            if (!isWrittenButtons)
            {
                GrabButtonText(self.hostButton);
                GrabButtonText(self.joinButton);
                GrabButtonText(self.settingsButton);
                GrabButtonText(self.creditsButton);
                GrabButtonText(self.quitButton);
                isWrittenButtons = true;
            }
            orig(self);
        }

        public static void GrabButtonText(Button button)
        {
            TextMeshProUGUI buttonText = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log(button.gameObject.name + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            if (buttonText != null)
            {
                Debug.Log($"FOUND TextMeshProUGUI IN {button.gameObject.name} IT SAYS {buttonText.text} !!!!!!!!!!");
                Debug.Log($"Key: {button.gameObject.name}");
                Debug.Log($"Value: {buttonText.text}");
                uiDict.Add(button.gameObject.name, buttonText.text);
            }
        }
        
        //======= EMOTES ========

        private static bool ItemDatabase_TryGetItemFromID(On.ItemDatabase.orig_TryGetItemFromID orig, byte id, out Item item)
        {
            if (isWrittenItem)
            {
                return orig(id, out item);
            }

            Item[] items = SingletonAsset<ItemDatabase>.Instance.Objects;

            Print("//EMOTE");
            foreach (Item _item in items)
            {
                int i = 0;
                foreach(string comment in _item.emoteInfo.comments)
                {
                    Print(_item.name + "." + i, comment);
                    i++;
                }
            }
            isWrittenItem = true;
            return orig(id, out item);
        }




        public static void Print(string key, string value = "", bool append = true, bool log = false)
        {
            using (StreamWriter writetext = new StreamWriter("D:\\repos\\TranslatedWarning\\TranslatedWarning\\Dialog.txt", append))
            {
                if (value.IsNullOrWhiteSpace())
                {
                    if (log) { Debug.Log(key);};
                    writetext.WriteLine(key);
                    return;
                }
                if (log)
                {
                    Debug.Log("Key: " + key);
                    Debug.Log("Key: " + value);
                }
                writetext.WriteLine($"-{key}\n+{value}\n");
            }
        }


       

        private static Comment PlayerEmoteContentEvent_GenerateComment(On.PlayerEmoteContentEvent.orig_GenerateComment orig, PlayerEmoteContentEvent self)
        {
            orig(self);

            PlayerEmoteContentEvent playerEmoteContentEvent = self;

            if (playerEmoteContentEvent.item.emoteInfo.comments != null)
            {
                int i = 1;
                string emoteName = playerEmoteContentEvent.item.emoteInfo.displayName;
                Debug.Log($"emote comments found for {emoteName} !!!!!!!!");
                foreach (string comment in playerEmoteContentEvent.item.emoteInfo.comments)
                {
                    Print(emoteName, comment);
                    i++;
                }
            }
            else
            {
                Debug.Log("emote comments does not EXIST!!!!!!!!");
            }

            List<string> list = new List<string>();
            list.AddRange(playerEmoteContentEvent.item.emoteInfo.comments);
            return new Comment(playerEmoteContentEvent.item.emoteInfo.comments![0]);
        }

        //======= COMMENTS ========

        //amazing code

        private static void DataCollection()
        {
            Debug.Log("DATA COLLECTION IS RUNNING");
            int i = 0;
            foreach (var comment in BarnacleBallContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("BarnacleBallContentEvent."+i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in BigSlapAgroContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("BigSlapAgroContentEvent." + i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in BigSlapPeacefulContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("BigSlapPeacefulContentEvent."+i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in BlackHoleBotContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("BlackHoleBotContentEvent."+i, comment);
                i++;
            }
            
            i = 0;
            commentList.Add("BombContentEvent", "omg <playername> is holding the bomb!");
            foreach (var comment in BombsContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("BombsContentEvent."+i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in CamCreepContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("CamCreepContentEvent."+i,comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in DogContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("DogContentEvent."+i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in EarContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("EarContentEvent."+i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in EyeGuyContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("EyeGuyContentEvent."+i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in FireMonsterContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("FireMonsterContentEvent."+i,comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in FlickerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("FlickerContentEvent."+i,comment);
                i++;
            }
            
            i = 0;
            GoodCatchContentEvent goodCatch = new GoodCatchContentEvent();
            foreach (var comment in goodCatch.GOOD_CATCH_COMMENTS)
            {
                commentList.Add("GoodCatchContentEvent."+ i, comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in HarpoonerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("HarpoonerContentEvent."+i,comment);
                i++;
            }
            
            i = 0;
            InterviewEvent interview = new InterviewEvent();
            foreach (var comment in interview.INTERVIEW_COMMENTS)
            {
                commentList.Add(interview.GetType().ToString(), comment);
                i++;
            }
            
            i = 0;
            foreach (var comment in JelloContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("JelloContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in KnifoContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("KnifoContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in LarvaContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("LarvaContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in MimeContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("MimeContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in MouthContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("MouthContentEvent."+i, comment);
                i++;
            }
            i = 0;
            commentList.Add("MultiMonsterContentEvent", "I saw a bunch of monsters in this video! It was really cool!");
            foreach (var comment in PlayerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("PlayerContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in PlayerDeadContentEvent.DEAD_COMMENTS)
            {
                commentList.Add("PlayerDeadContentEvent."+i, comment);
                i++;
            }

            //PlayerEmote is nowhere to be found
            i = 0;
            foreach (var comment in PlayerFallingContentEvent.BIG_FALL_COMMENTS)
            {
                commentList.Add("PlayerFallingContentEvent.BIG." + i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in PlayerFallingContentEvent.SMALL_FALL_COMMENTS)
            {
                commentList.Add("PlayerFallingContentEvent.SMALL." + i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in PlayerHoldingMicContentEvent.HOLDING_MIC_COMMENTS)
            {
                commentList.Add("PlayerHoldingMicContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in PlayerRagdollContentEvent.RAGDOLL_COMMENTS)
            {
                commentList.Add("PlayerRagdollContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in PlayerTookDamageContentEvent.TOOK_DMG_COMMENTS)
            {
                commentList.Add("PlayerTookDamageContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in PuffoContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("PuffoContentEvent."+i,comment);
                i++;
            }
            i = 0;
            foreach (var comment in RobotButtonContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("RobotButtonContentEvent."+i,comment);
                i++;
            }
            i = 0;
            foreach (var comment in PlayerShroomContentEvent.comments)
            {
                commentList.Add("PlayerShroomContentEvent."+i,comment);
                i++;
            }
            i = 0;
            foreach (var comment in SlurperContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("SlurperContentEvent."+i,comment);
                i++;
            }
            i = 0;
            foreach (var comment in SnailSpawnerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("SnailSpawnerContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in SnatchoContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("SnatchoContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in SpiderContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("SpiderContentEvent."+i,comment);
                i++;
            }
            i = 0;
            foreach (var comment in StreamerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("StreamerContentEvent."+i,comment);
                i++;
            }
            
            i = 0;
            TauntEvent taunt = new TauntEvent();
            foreach (var comment in taunt.INTERVIEW_COMMENTS)
            {
                commentList.Add("TauntEvent" + i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in ToolkitWhiskContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("ToolkitWhiskContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in WalloContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("WalloContentEvent."+i,comment);
                i++;
            }
            i = 0;
            foreach (var comment in WeepingContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("WeepingContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in WeepingContentEventCaptured.CAPTURED_COMMENTS)
            {
                commentList.Add("WeepingContentEventCaptured."+i,comment);
                i++;
            }
            i = 0;
            foreach (var comment in WeepingContentEventFail.FAIL_COMMENTS)
            {
                commentList.Add("WeepingContentEventFail."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in WeepingContentEventSuccess.SUCCESS_COMMENTS)
            {
                commentList.Add("WeepingContentEventSuccess."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in WormContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("WormContentEvent."+i, comment);
                i++;
            }
            i = 0;
            foreach (var comment in ZombieContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add("ZombieContentEvent."+i, comment);
                i++;
            }
        }

        //======= MAIN ========
        private static string LocalizationKeys_GetLocalizedString(On.LocalizationKeys.orig_GetLocalizedString orig, LocalizationKeys.Keys key)
        {
            
            orig(key);

            if (!isWritten)
            {
                using (StreamWriter writetext = new StreamWriter("D:\\repos\\TranslatedWarning\\TranslatedWarning\\Dialog.txt", true))
                {
                    Debug.Log("//MAIN");
                    writetext.WriteLine("//MAIN\n");
                }
                foreach (KeyValuePair<LocalizationKeys.Keys, string> kvp in LocalizationKeys.m_StringDictionary)
                {
                    using (StreamWriter writetext = new StreamWriter("D:\\repos\\TranslatedWarning\\TranslatedWarning\\Dialog.txt", true))
                    {
                        writetext.WriteLine($"-{kvp.Key}\n+{kvp.Value}\n");
                    }
                }
                isWritten = true;
            }

            return LocalizationKeys.m_StringDictionary[key];
        }
    }
}