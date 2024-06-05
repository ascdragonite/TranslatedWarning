using BepInEx;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.UI;
using static Unity.Collections.LowLevel.Unsafe.BurstRuntime;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System.Linq;
using MonoMod.Cil;
using DefaultNamespace.ContentProviders;
using DefaultNamespace.ContentEvents;
using UnityEngine.Localization;
using Photon.Pun;

namespace TranslatedWarning.Patches
{
    public class InjectTranslation
    {
        static string path = "D:\\repos\\TranslatedWarning\\TranslatedWarning\\DialogTranslated.txt";

        public static Dictionary<string, string> translatedDict = new Dictionary<string, string>();
        static int keyAssign;

        internal static void Init()
        {

            //fun things
            string[] lines = File.ReadAllLines(path);

            Debug.Log("====== DICTIONARY BEGIN ======");
            for (int i = 0; i < lines.Length; i++) 
            {
                string line = lines[i];

                if (line.StartsWith("-"))
                {
                    keyAssign = i;
                }
                if (line.StartsWith("+"))
                {
                    translatedDict.Add(lines[keyAssign].Substring(1).Trim(), line.Substring(1).Trim());
                    Debug.Log($"Key: {lines[keyAssign]}");
                    Debug.Log($"Value: {line.Substring(1).Trim()}");
                }

            }
            Debug.Log("====== DICTIONARY END ======");

            On.LocalizationKeys.GetLocalizedString += LocalizationKeys_GetLocalizedString;

            On.MainMenuUIHandler.OnTransistionedToPage += MainMenuUIHandler_OnTransistionedToPage;

            On.ContentBuffer.GenerateComments += ContentBuffer_GenerateComments;

            On.PlayerCustomizer.Awake += PlayerCustomizer_Awake;

            On.ThePlanLocalizer.LocalizeMe += ThePlanLocalizer_LocalizeMe;

            On.VideoCamera.Start += VideoCamera_Start;

            On.ShopHandler.InitShopHandler += ShopHandler_InitShopHandler;

            On.DivingBell.Start += DivingBell_Start;

            On.ExtractVideoMachine.Awake += ExtractVideoMachine_Awake;

            On.UploadVideoStation.Awake += UploadVideoStation_Awake;

            On.EscapeMenuMainPage.Awake += EscapeMenuMainPage_Awake;
            On.EscapeMenuSettingsPage.Awake += EscapeMenuSettingsPage_Awake;




            On.PlayerWearingHatContentEvent.GenerateComment += PlayerWearingHatContentEvent_GenerateComment;
            On.PlayerEmoteContentEvent.GenerateComment += PlayerEmoteContentEvent_GenerateComment;
            On.PlayerDeadContentEvent.GenerateComment += PlayerDeadContentEvent_GenerateComment;
            On.PlayerFallingContentEvent.GenerateComment += PlayerFallingContentEvent_GenerateComment;
            On.PlayerHoldingMicContentEvent.GenerateComment += PlayerHoldingMicContentEvent_GenerateComment;
            On.PlayerRagdollContentEvent.GenerateComment += PlayerRagdollContentEvent_GenerateComment;
            On.PlayerTookDamageContentEvent.GenerateComment += PlayerTookDamageContentEvent_GenerateComment;
            On.GoodCatchContentEvent.GenerateComment += GoodCatchContentEvent_GenerateComment;
            On.PlayerContentEvent.GenerateComment += PlayerContentEvent_GenerateComment;

        }


        // =============== ESCAPE MENU ===============
        private static void EscapeMenuSettingsPage_Awake(On.EscapeMenuSettingsPage.orig_Awake orig, EscapeMenuSettingsPage self)
        {
            orig(self);
            Transform settings = self.transform.GetChild(0);

            TranslateText(settings.GetChild(0)); //BackButton
            TranslateText(settings.GetChild(1), settings.gameObject.name); //Settings title

            Transform tabs = settings.GetChild(2).GetChild(0);
            foreach (Transform tab in tabs)
            {
                TranslateText(tab);
            }
        }

        private static void EscapeMenuMainPage_Awake(On.EscapeMenuMainPage.orig_Awake orig, EscapeMenuMainPage self)
        {
            orig(self);
            TranslateText(self.gameObject.transform.GetChild(0), "CONTENTWARNING");
            TranslateText(self.settingsButton.gameObject.transform);
            TranslateText(self.exitButton.gameObject.transform);
            TranslateText(self.resumeButton.gameObject.transform);
            TranslateText(self.inviteButton.gameObject.transform);
        }

        // =============== TV ===============
        private static void UploadVideoStation_Awake(On.UploadVideoStation.orig_Awake orig, UploadVideoStation self)
        {

            Debug.Log("UPLOADVIDEOSTATION ACTIVE!!!!!!!!!!!!!!!!");
            orig(self);
            Debug.Log(self.gameObject.name);
            var mcScreen = self.gameObject.transform.Find("McScreen");
            if (mcScreen != null)
            {
                Debug.Log("FOUND MCSCREEN!!!!!!!!!!!!!!!!");
                InjectTranslation.TranslateText(mcScreen.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0)); //tube
                InjectTranslation.TranslateText(mcScreen.GetChild(0).GetChild(0).GetChild(0).GetChild(1)); //spook
                Transform content = mcScreen.GetChild(1);
                TranslateText(content.GetChild(0).GetChild(0).GetChild(0), "UploadVideo"); //UploadState
                TranslateText(content.GetChild(0).GetChild(0).GetChild(2), "InsertDisc");

                TranslateText(content.GetChild(1)); //ClosedState

                TranslateText(content.GetChild(2)); //UploadingState

                TranslateText(content.GetChild(3).GetChild(1), "ShowVideoState"); //ShowVideoState
                TranslateText(content.GetChild(3).GetChild(0).GetChild(2).GetChild(1), "SaveVideo.text");
                TranslateText(content.GetChild(3).GetChild(0).GetChild(2).GetChild(2));
                TranslateText(content.GetChild(3).GetChild(0).GetChild(2).GetChild(3));
            }
        }

        // =============== VIDEO EXTRACTOR ===============
        private static void ExtractVideoMachine_Awake(On.ExtractVideoMachine.orig_Awake orig, ExtractVideoMachine self)
        {
            orig(self);
            Transform canvas = self.gameObject.transform.GetChild(2);
            for (int i = 1; i < 4; i++)
            {
                TranslateText(canvas.GetChild(i));
            }
        }

        // =============== DIVING BELL ===============
        private static void DivingBell_Start(On.DivingBell.orig_Start orig, DivingBell self)
        {
            orig(self);
            Transform canvas = self.gameObject.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(1);

            for (int i = 0; i < 3; i++)
            {
                TranslateText(canvas.GetChild(i));
            }
        }

        // =============== THE SHOP ===============
        private static void ShopHandler_InitShopHandler(On.ShopHandler.orig_InitShopHandler orig, ShopHandler self)
        {
            Debug.Log("ShopHandler ACTIVATE!!!!!!");
            
            orig(self);
            Debug.Log(self.gameObject.transform.GetChild(0));
            TranslateText(self.gameObject.transform, ugui: false);
            Debug.Log("ShopHandler ACTIVATE!!!!!!");
        }

        // =============== THE "REC" TEXT U SEE ON THE CAMERA ===============
        private static void VideoCamera_Start(On.VideoCamera.orig_Start orig, VideoCamera self)
        {
            orig(self);
            TranslateText(self.m_recordingUI.gameObject.transform);
        }

        // =============== THE PLAN ===============
        private static void ThePlanLocalizer_LocalizeMe(On.ThePlanLocalizer.orig_LocalizeMe orig, ThePlanLocalizer self)
        {
            if(TranslatedWarning.translatedPlan != null)
            {
                self.m_PlanRenderer.material.SetTexture("_Variation", TranslatedWarning.translatedPlan);
            }

        }

        // =============== PLAYER CUSTOMIZER ===============
        private static void PlayerCustomizer_Awake(On.PlayerCustomizer.orig_Awake orig, PlayerCustomizer self)
        {
            orig(self);
            TranslateText(self.applyButton.gameObject.transform.GetParent());
            TranslateText(self.quitButton.gameObject.transform.GetParent());

            TranslateText(self.clearHatButton.gameObject.transform);
            TranslateText(self.hatNameText.gameObject.transform);

            var customizer = self.gameObject.transform.GetChild(0).GetChild(0);
            TranslateText(customizer.GetChild(9), "PlayerCustomizerMachine");
            TranslateText(customizer.GetChild(2));

        }

        // =============== COMMENTS ===============

        private static Comment PlayerContentEvent_GenerateComment(On.PlayerContentEvent.orig_GenerateComment orig, PlayerContentEvent self)
        {
            string comment = FindTranslation(self.GetType().ToString());
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment GoodCatchContentEvent_GenerateComment(On.GoodCatchContentEvent.orig_GenerateComment orig, GoodCatchContentEvent self)
        {
            string comment = FindTranslation(self.GetType().ToString());
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment PlayerTookDamageContentEvent_GenerateComment(On.PlayerTookDamageContentEvent.orig_GenerateComment orig, PlayerTookDamageContentEvent self)
        {
            string comment = FindTranslation(self.GetType().ToString());
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment PlayerRagdollContentEvent_GenerateComment(On.PlayerRagdollContentEvent.orig_GenerateComment orig, PlayerRagdollContentEvent self)
        {
            string comment = FindTranslation(self.GetType().ToString());
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment PlayerHoldingMicContentEvent_GenerateComment(On.PlayerHoldingMicContentEvent.orig_GenerateComment orig, PlayerHoldingMicContentEvent self)
        {
            string comment = FindTranslation(self.GetType().ToString());
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment PlayerFallingContentEvent_GenerateComment(On.PlayerFallingContentEvent.orig_GenerateComment orig, PlayerFallingContentEvent self)
        {
            string key = self.IsBigFall ? self.GetType().ToString() + ".BIG" : self.GetType().ToString() + ".SMALL";
            string comment = FindTranslation(key);
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment PlayerDeadContentEvent_GenerateComment(On.PlayerDeadContentEvent.orig_GenerateComment orig, PlayerDeadContentEvent self)
        {
            string comment = FindTranslation(self.GetType().ToString());
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment PlayerEmoteContentEvent_GenerateComment(On.PlayerEmoteContentEvent.orig_GenerateComment orig, PlayerEmoteContentEvent self)
        {
            string comment = FindTranslation(self.item.name);
            Debug.Log("FINDING COMMENTS FOR: " + self.item.name);
            return new Comment(self.FixPlayerName(comment));
        }
        private static Comment PlayerWearingHatContentEvent_GenerateComment(On.PlayerWearingHatContentEvent.orig_GenerateComment orig, PlayerWearingHatContentEvent self)
        {
            string comment = FindTranslation(self.hatInDatabase.name);
            return new Comment(self.FixPlayerName(comment));
        }




        

        public static List<Type> excludedEvents =
        [
            typeof(PlayerWearingHatContentEvent),
            typeof(PlayerEmoteContentEvent), 
            typeof(PlayerDeadContentEvent),
            typeof(PlayerFallingContentEvent),
            typeof(PlayerHoldingMicContentEvent),
            typeof(PlayerRagdollContentEvent),
            typeof(PlayerTookDamageContentEvent),
            typeof(GoodCatchContentEvent),
            typeof(PlayerContentEvent)
        ];



        private static List<Comment> ContentBuffer_GenerateComments(On.ContentBuffer.orig_GenerateComments orig, ContentBuffer self)
        {
            try
            {
                List<Comment> list = new List<Comment>();
                foreach (ContentBuffer.BufferedContent item in self.buffer)
                {
                    var cEvent = item.frame.contentEvent;
                    Debug.Log($"FOUND EVENT: {cEvent.GetType().ToString()}");
                    Comment comment = cEvent.GenerateComment();

                    if (!excludedEvents.Contains(cEvent.GetType()))
                    {
                        comment.Text = TranslateComment(cEvent); //magic happens
                    }

                    comment.Likes = BigNumbers.GetScoreToViews(Mathf.RoundToInt(item.score), GameAPI.CurrentDay);
                    comment.Time = item.frame.time;
                    comment.Face = FaceDatabase.GetRandomFaceIndex();
                    comment.FaceColor = FaceDatabase.GetRandomColorIndex();
                    list.Add(comment);
                }
                list.Sort((Comment c1, Comment c2) => c1.Time.CompareTo(c2.Time));
                return list;
            }
            catch (Exception e)
            {
                Debug.Log("EPIC FAIL!!!!!");
                Debug.LogError(e.ToString());
                return orig(self);
            }

        }


        public static string TranslateComment(ContentEvent content)
        {
            string pattern;
            if (content.GetType() == typeof(PropContentEvent))
            {
                PropContentEvent propContent = (PropContentEvent)content;

                Debug.Log("PROP FOUND: " + propContent.content.name);
                pattern = propContent.content.name;
            }
            else if (content.GetType() == typeof(ArtifactContentEvent))
            {
                ArtifactContentEvent artifactContent = (ArtifactContentEvent)content;

                Debug.Log("ARTIFACT FOUND: " + artifactContent.content.name);
                pattern = artifactContent.content.name;
            }
            else
            {
                pattern = content.GetType().ToString();
            }
            return FindTranslation(pattern);
        }

        public static string FindTranslation(string pattern)
        {
            string[] eventCommentArray = translatedDict.Where(kvp => kvp.Key.Contains(pattern)).Select(kvp => kvp.Value).ToArray(); //gets all values (maybe) that has the ContentEvent in it

            System.Random random = new System.Random();
            int index = random.Next(0, eventCommentArray.Length); //make a random number from 0 to array length
            Debug.Log($"COMMENT CHOSEN: {pattern}.{index}");
            string comment = eventCommentArray[index];
            return eventCommentArray[index];
        }




        // =============== UI ===============
        // ----- MAIN MENU -----
        private static void MainMenuUIHandler_OnTransistionedToPage(On.MainMenuUIHandler.orig_OnTransistionedToPage orig, MainMenuUIHandler self, Zorro.UI.UIPage newPage)
        {
            orig(self, newPage);
            string currentPage = newPage.GetType().ToString();
            Debug.Log($"InjectTranslation: TRANSITION TO {currentPage}");

            switch (currentPage)
            {
                case "MainMenuMainPage":
                    if (TranslatedWarning.seenList.Contains(currentPage)) { Debug.Log($"InjectTranslation: ALREADY SEEN!!!!!");  break; }
                    Debug.Log("CASE MAIN MENU!!!!!!!!");
                    Transform buttonsList = newPage.gameObject.transform.GetChild(3);
                    foreach (Transform button in buttonsList) 
                    {
                        TranslateText(button);
                    }
                    TranslatedWarning.seenList.Add(currentPage);
                    break;

                case "MainMenuSettingsPage":
                    if (TranslatedWarning.seenList.Contains(currentPage)) { Debug.Log($"InjectTranslation: ALREADY SEEN!!!!!"); break; }
                    Debug.Log("CASE SETTINGS!!!!!!!!");
                    Transform settings = newPage.transform.GetChild(2);

                    TranslateText(settings.GetChild(0)); //BackButton
                    TranslateText(settings.GetChild(1), settings.gameObject.name); //Settings title

                    Transform tabs = settings.GetChild(2).GetChild(0);
                    foreach (Transform tab in tabs)
                    {
                        TranslateText(tab);
                    }

                    TranslatedWarning.seenList.Add(currentPage);
                    break;

                case "MainMenuHostPage":

                    if (TranslatedWarning.seenList.Contains(currentPage)) { Debug.Log($"InjectTranslation: ALREADY SEEN!!!!!"); break; }
                    Debug.Log("CASE HOST!!!!!!!!");

                    Transform hostPage = newPage.gameObject.transform;

                    TranslateText(hostPage.GetChild(2), "CW.HostPage"); //host h1
                    TranslateText(hostPage.GetChild(3), "CW (1).HostPage"); //host h2

                    foreach (Transform save in hostPage.GetChild(4))
                    {
                        TranslateText(save.GetChild(3), save.gameObject.name); //save numbers
                        TranslateText(save.GetChild(4).GetChild(0), "SaveCell." + save.GetChild(4).gameObject.name); //the text inside that says empty
                    }

                    TranslateText(hostPage.GetChild(5)); //back buttonm
                    TranslateText(hostPage.GetChild(6), "HostButton"); //host button

                    break;
            }
        }



        // =============== MAIN TEXT ===============

        private static bool m_MadeLocaleStrings = false;
        private static string LocalizationKeys_GetLocalizedString(On.LocalizationKeys.orig_GetLocalizedString orig, LocalizationKeys.Keys key)
        {
            if (!m_MadeLocaleStrings)
            {
                LocalizationKeys.MakeLocaleStrings();
                m_MadeLocaleStrings = true;
            }
            if (translatedDict == null || !translatedDict.ContainsKey(key.ToString()))
            {
                Debug.LogError("Cant find TRANSLATED key for: " + key);
                return orig(key);
            }
            return translatedDict[key.ToString()];
        }


        public static void TranslateText(Transform textObject, string key = "", bool ugui = true) 
        {
            TMP_Text? text = null;
            if (ugui) 
            {
                text = textObject.gameObject.GetComponentInChildren<TextMeshProUGUI>(); 
            }
            else 
            {
                text = textObject.gameObject.GetComponentInChildren<TextMeshPro>(); 
            }

            if (text != null)
            {
                Debug.Log(text.text + "!!!!!!!!!!!!!");
                try
                {
                    if (key.IsNullOrWhiteSpace())
                    {
                        key = textObject.gameObject.name;
                    }
                    text.text = translatedDict[key];  //change text

                    var componentList = text.gameObject.GetComponents<Component>(); //destroy unecessary components
                    foreach (var component in componentList)
                    {
                        if (component.GetType() == typeof(LocalizeStringEvent) || component.GetType() == typeof(GameObjectLocalizer))
                        {
                            TranslatedWarning.Delete(component);
                        }
                    }

                }
                catch (Exception e) 
                {
                    if (e is KeyNotFoundException)
                    {
                        Debug.LogError($"TRANSLATION KEY {key} NOT FOUND!!!!!");
                    }
                    else
                    {
                        Debug.LogError("EPIC FAILL!!!!!!!!!!!!!!!");
                        Debug.LogError(e.ToString());
                    }

                }

            }

        }
    }

}
