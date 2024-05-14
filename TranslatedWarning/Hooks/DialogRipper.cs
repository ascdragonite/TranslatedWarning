using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TranslatedWarning.Patches
{
    public class DialogRipper
    {
        private static bool isWritten = false;

        internal static void Init()
        {
            /*
             *  Subscribe with 'On.Namespace.Type.Method += CustomMethod;' for each method you're patching.
             *  Or if you are writing an ILHook, use 'IL.' instead of 'On.'
             *  Note that not all types are in a namespace, especially in Unity games.
             */

            On.LocalizationKeys.GetLocalizedString += LocalizationKeys_GetLocalizedString;
        }

        private static string LocalizationKeys_GetLocalizedString(On.LocalizationKeys.orig_GetLocalizedString orig, LocalizationKeys.Keys key)
        {
            
            orig(key);

            if (!isWritten)
            {
                foreach (KeyValuePair<LocalizationKeys.Keys, string> kvp in LocalizationKeys.m_StringDictionary)
                {
                    using (StreamWriter writetext = new StreamWriter("D:\\repos\\TranslatedWarning\\TranslatedWarning\\Dialog.txt", true))
                    {
                        Debug.Log("[TranslatedWarning] Key: " + kvp.Key);
                        Debug.Log("[TranslatedWarning] Value: " + kvp.Value);
                        writetext.WriteLine($"- {kvp.Key}\n+{kvp.Value}\n");
                    }
                }
                isWritten = true;
            }

            return LocalizationKeys.m_StringDictionary[key];
        }
    }
}