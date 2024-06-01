using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.LowLevel.Unsafe.BurstRuntime;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

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
    }
}
