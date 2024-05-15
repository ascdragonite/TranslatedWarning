using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace TranslatedWarning.Patches
{
    public class DialogRipper
    {
        private static bool isWritten = false;


        public static List<string> commentList = new List<string>();

        static string[] values = new string[50];


        internal static void Init()
        {
            /*
             *  Subscribe with 'On.Namespace.Type.Method += CustomMethod;' for each method you're patching.
             *  Or if you are writing an ILHook, use 'IL.' instead of 'On.'
             *  Note that not all types are in a namespace, especially in Unity games.
             */



            On.LocalizationKeys.GetLocalizedString += LocalizationKeys_GetLocalizedString;



            DataCollection();

            using (StreamWriter writetext = new StreamWriter("D:\\repos\\TranslatedWarning\\TranslatedWarning\\Dialog.txt"))
            {
                Debug.Log("//COMMENTS");
                writetext.WriteLine("//COMMENTS\n");
            }

            foreach (string comment in commentList)
            {

                int hashCode = HashFunction(comment, values);
                using (StreamWriter writetext = new StreamWriter("D:\\repos\\TranslatedWarning\\TranslatedWarning\\Dialog.txt", true))
                {
                    writetext.WriteLine($"-{hashCode}\n+{comment}\n");
                }
                Debug.Log("Key: " + hashCode);
                Debug.Log("Value: " + comment);
            }

        }

        static int HashFunction(string s, string[] array)
        {
            int total = 0;
            char[] c = s.ToCharArray();

            // Summing up all the ASCII values
            // of each alphabet in the string
            for (int k = 0; k <= c.GetUpperBound(0); k++)
                total += c[k];

            return total;
        }

        private static void DataCollection()
        {
            foreach (var comment in BarnacleBallContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in BigSlapAgroContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in BigSlapPeacefulContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in BlackHoleBotContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            commentList.Add("omg <playername> is holding the bomb!");
            foreach (var comment in BombsContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in CamCreepContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in DogContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in EarContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in EyeGuyContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in FireMonsterContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in FlickerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            GoodCatchContentEvent goodCatch = new GoodCatchContentEvent();
            foreach (var comment in goodCatch.GOOD_CATCH_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in HarpoonerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            InterviewEvent interview = new InterviewEvent();
            foreach (var comment in interview.INTERVIEW_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in JelloContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in KnifoContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in LarvaContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in MimeContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in MouthContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            commentList.Add("I saw a bunch of monsters in this video! It was really cool!");
            foreach (var comment in PlayerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in PlayerDeadContentEvent.DEAD_COMMENTS)
            {
                commentList.Add(comment);
            }

            //PlayerEmote is nowhere to be found

            foreach (var comment in PlayerFallingContentEvent.BIG_FALL_COMMENTS)
            {
                commentList.Add(comment);
            }

            foreach (var comment in PlayerFallingContentEvent.SMALL_FALL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in PlayerHoldingMicContentEvent.HOLDING_MIC_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in PlayerRagdollContentEvent.RAGDOLL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in PlayerTookDamageContentEvent.TOOK_DMG_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in PuffoContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in RobotButtonContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in PlayerShroomContentEvent.comments)
            {
                commentList.Add(comment);
            }
            foreach (var comment in SlurperContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in SnailSpawnerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in SnatchoContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in SpiderContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in StreamerContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in BigSlapPeacefulContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            TauntEvent taunt = new TauntEvent();
            foreach (var comment in taunt.INTERVIEW_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in ToolkitWhiskContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in WalloContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in WeepingContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in WeepingContentEventCaptured.CAPTURED_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in WeepingContentEventFail.FAIL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in WeepingContentEventSuccess.SUCCESS_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in WormContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
            foreach (var comment in ZombieContentEvent.NORMAL_COMMENTS)
            {
                commentList.Add(comment);
            }
        }


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
                        writetext.WriteLine($"- {kvp.Key}\n+{kvp.Value}\n");
                    }
                }
                isWritten = true;
            }

            return LocalizationKeys.m_StringDictionary[key];
        }
    }
}