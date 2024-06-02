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
                typeof(PlayerEmoteContentEvent), typeof(PlayerDeadContentEvent),
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
                    TranslateText(settings.GetChild(1), key: settings.gameObject.name); //Settings title

                    Transform tabs = settings.GetChild(2).GetChild(0);
                    foreach (Transform tab in tabs)
                    {
                        TranslateText(tab);
                    }

                    TranslatedWarning.seenList.Add(currentPage);
                    break;
            }
        }

        public static bool mainMenuMainActive = false;


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


        public static void TranslateText(Transform textObject, string key = "")
        {
            TextMeshProUGUI text = textObject.gameObject.GetComponentInChildren<TextMeshProUGUI>(); //find TextMeshPro
            if (text != null)
            {
                Debug.Log(text.text + "!!!!!!!!!!!!!");
                if (key.IsNullOrWhiteSpace()) { text.text = InjectTranslation.translatedDict[textObject.gameObject.name]; }
                else { text.text = InjectTranslation.translatedDict[key]; } //change text

                var componentList = text.gameObject.GetComponents<Component>(); //destroy unecessary components
                foreach (var component in componentList)
                {
                    if (component.GetType() == typeof(LocalizeStringEvent) || component.GetType() == typeof(GameObjectLocalizer))
                    {
                        TranslatedWarning.Delete(component);
                    }
                }

            }

        }
    }

}
