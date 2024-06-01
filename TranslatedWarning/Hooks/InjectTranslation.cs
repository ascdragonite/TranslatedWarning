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
