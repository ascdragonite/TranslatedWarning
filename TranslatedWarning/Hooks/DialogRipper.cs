using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Unity.Collections.LowLevel.Unsafe.BurstRuntime;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;
using UnityEngine;
using Zorro.Core;

namespace TranslatedWarning.Patches
{
    public class DialogRipper
    {
        private static bool isWritten = false;
        private static bool isWrittenItem = false;

        public static List<string> commentList = new List<string>();

        static string[] values = new string[50];

        public static string prevName = "";
        internal static void Init()
        {
            /*
             *  Subscribe with 'On.Namespace.Type.Method += CustomMethod;' for each method you're patching.
             *  Or if you are writing an ILHook, use 'IL.' instead of 'On.'
             *  Note that not all types are in a namespace, especially in Unity games.
             */



            On.LocalizationKeys.GetLocalizedString += LocalizationKeys_GetLocalizedString;
            On.PlayerEmoteContentEvent.GenerateComment += PlayerEmoteContentEvent_GenerateComment;
            On.PropContentEvent.GenerateComment += PropContentEvent_GenerateComment;
            //On.ContentPolling.Poll += ContentPolling_Poll;
            On.ItemDatabase.TryGetItemFromID += ItemDatabase_TryGetItemFromID;

            DataCollection(); //collects most comments

            Print("//COMMENTS_MONSTER", append: false);
            foreach (string comment in commentList)
            {

                int hashCode = HashFunction(comment, values); //creates hashcode for comments
                Print(hashCode.ToString(), comment);
            }


            PropContent[] props = SingletonAsset<PropContentDatabase>.Instance.Objects;
            foreach (PropContent prop in props)
            {
                Debug.Log($"{prop.name}");
                int i = 0;
                foreach (string comment in prop.comments)
                {

                    Print(prop.name + i, comment); //collects prop comments
                    i++;
                }
            }



        }

        private static bool ItemDatabase_TryGetItemFromID(On.ItemDatabase.orig_TryGetItemFromID orig, byte id, out Item item)
        {
            Debug.Log("ItemDatabase.TryGetItemFromId ACTIVATED !!!!!!!!!!!");
            if (isWrittenItem)
            {
                return orig(id, out item);
            }

            Item[] items = SingletonAsset<ItemDatabase>.Instance.Objects;

            Debug.Log("Created Item List!!!!!!!!");
            Print("//COMMENTS_EMOTE");
            foreach (Item _item in items)
            {
                int i = 0;
                foreach(string comment in _item.emoteInfo.comments)
                {
                    Print(_item.name + "." + i, comment, log: true);
                    i++;
                }
            }
            isWrittenItem = true;
            return orig(id, out item);
        }



        //private static void ContentPolling_Poll(On.ContentPolling.orig_Poll orig, int x, int y, Camera camera)
        //{
        //    orig(x, y, camera);

        //    float x_ = (float)x / 20f;
        //    float y_ = (float)y / 20f;
        //    Ray laser = camera.ViewportPointToRay(new Vector3(x_, y_, 0f));
        //    float num = 200f;
        //    Vector3 last = laser.origin + laser.direction * num;
        //    RaycastHit hitInfo;
        //    bool thing = Physics.Raycast(laser, out hitInfo, num, int.MaxValue, QueryTriggerInteraction.Ignore);
        //    if (thing)
        //    {
        //        last = hitInfo.point;
        //        thing = false;
        //        string name = hitInfo.collider.gameObject.name;
        //        if(name != prevName)
        //        {
        //            ContentProvider contentProv = hitInfo.collider.GetComponentInParent<ContentProvider>();

        //            if (contentProv != null)
        //            {
        //                Debug.Log($"RAYCAST HAS HIT {name} !!!!!!!!!!!!!!!!!!!!!!!");
        //                thing = true;
        //            }
        //            prevName = name;
        //        }

        //    }
        //}

        private static void Print(string key, string value = "", bool append = true, bool log = false)
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

        private static Comment PropContentEvent_GenerateComment(On.PropContentEvent.orig_GenerateComment orig, PropContentEvent self)
        {

            return orig(self);
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
                    Print(emoteName, comment, log: true);
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

        static int HashFunction(string s, string[] array)
        {
            int total = 0;
            char[] c = s.ToCharArray();

            // Summing up all the ASCII values
            // of each alphabet in the string
            for (int k = 0; k <= c.GetUpperBound(0); k++)
                total += c[k]; //char can be converted into int??

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