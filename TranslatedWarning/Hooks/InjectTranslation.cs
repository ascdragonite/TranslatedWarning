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

            On.PlayerBaseEvent.FixPlayerName += PlayerBaseEvent_FixPlayerName;
        }

        static PlayerBaseEvent? playerBaseEvent;
        private static string PlayerBaseEvent_FixPlayerName(On.PlayerBaseEvent.orig_FixPlayerName orig, PlayerBaseEvent self, string comment)
        {
            playerBaseEvent = self;
            Debug.Log("FixPlayerName ACTIVATED!!!!!!");
            return orig(self, comment);
        }

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

                    comment.Text = TranslateComment(cEvent); //magic happens

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
            string[] eventCommentArray = translatedDict.Where(kvp => kvp.Key.Contains(pattern)).Select(kvp => kvp.Value).ToArray(); //gets all values (maybe) that has the ContentEvent in it

            System.Random random = new System.Random();
            int index = random.Next(0, eventCommentArray.Length); //make a random number from 0 to array length
            Debug.Log($"COMMENT CHOSEN: {pattern}.{index}"); 
            string comment = eventCommentArray[index];
            if (content.GetType().IsSubclassOf(typeof(PlayerBaseEvent)))
            {
                PlayerBaseEvent playerBaseEvent = (PlayerBaseEvent)content;

            }

            return eventCommentArray[index]; //return random comment
        }

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
