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
        static string path = "";
        static string category;
        internal static void Init()
        {

            //fun things
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++) 
            {
                string line = lines[i];

                switch (category)
                {
                    case "COMMENTS":
                        Debug.Log("something");
                        break;
                    case "EMOTE":

                        break;
                    case "":

                        break;

                }

                if (line.StartsWith("//"))
                {
                    category = line.TrimStart('/');
                    Debug.Log($"Created category: {category}");
                }

            }

        }


    }
}
