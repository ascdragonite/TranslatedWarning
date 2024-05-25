using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Unity.Collections.LowLevel.Unsafe.BurstRuntime;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace TranslatedWarning.Patches
{
    public class InjectTranslation
    {
        static string path = "D:\\repos\\TranslatedWarning\\TranslatedWarning\\TranslatedDialog.txt";

        static Dictionary<string, string> translatedDict = new Dictionary<string, string>();
        static List<string> keyList = new List<string>();
        static List<string> valueList = new List<string>();
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
                    keyList.Add(line.Substring(1).Trim());
                    keyAssign = i;
                }
                if (line.StartsWith("+"))
                {
                    valueList.Add(line.Substring(1).Trim());
                    translatedDict.Add(lines[keyAssign], line.Substring(1).Trim());
                    Debug.Log($"Key: {lines[keyAssign]}");
                    Debug.Log($"Value: {line.Substring(1).Trim()}");
                }

            }

        }


    }
}
