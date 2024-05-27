using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using TranslatedWarning.Patches;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace TranslatedWarning
{
    [ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class TranslatedWarning : BaseUnityPlugin
    {
        public static TranslatedWarning Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;



        public List<string> commentList = new List<string>();

        string[] values = new string[50];

        string[] upgradeTranslations = { "WATER", "BIOS SYSTEM COOKIES", "GREAT PODCAST", "TOURISM SERVICES", "A MILITARY BASE" };

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            HookAll();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        internal void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.LogInfo("OnSceneLoaded: " + scene.name);
            Logger.LogInfo(mode);

            if(scene.name == "SurfaceScene")
            {
                Logger.LogInfo("THIS IS THE SurfaceScene, RUNNING COROUTINE");
                //InjectTransEnviro.Init();
                StartCoroutine(Coroutine());
            }


        }



        private IEnumerator Coroutine()
        {
            yield return 0;

            GameObject addons = GameObject.Find("Addons");
            Debug.Log("FOUND Addons!!!!!!!!!!");

            int i = 0;

            foreach (Transform upgradeTransform in addons.transform)
            {
                if (i == 5) { break; }
                Transform upgrade = upgradeTransform;
                Debug.Log($"LOOKING IN {upgrade.name}!!!!!!!!!!");

                Transform locked = upgrade.GetChild(1);
                Debug.Log($"{upgrade.name} FOUND {locked.name}!!!!!!!!!!");

                Component[] objects = locked.GetComponentsInChildren<Component>(true);
                Debug.Log($"{upgrade.name} GRABBED ComponentsInChildren!!!!!!!!!!");

                GameObject title = new GameObject();
                foreach (Component thing in objects)
                {
                    Debug.Log($"{upgrade.name} COMPONENT {thing}!!!!!!!!!!");
                    if (thing.gameObject.name == "Title")
                    {
                        title = thing.gameObject;
                        Debug.Log($"{upgrade.name} FOUND TITLE!!!!!!!!!!");
                        break;
                    }
                }


                TMP_Text TMP = title.GetComponent<TextMeshPro>();


                Debug.Log(TMP.text);

                TMP.text = upgradeTranslations[i];

                Debug.Log(TMP.text);


                Debug.Log($"{upgrade.name} TRANSLATION INJECTED!!!!!!!!?!?!?");
                i++;
            }

            yield break;

        }


        internal static void HookAll()
        {
            Logger.LogDebug("Hooking...");

            DialogRipper.Init();

            //InjectTranslation.Init();

            Logger.LogDebug("Finished Hooking!");

            

        }

        internal static void UnhookAll()
        {
            Logger.LogDebug("Unhooking...");

            /*
             *  HookEndpointManager is from MonoMod.RuntimeDetour.HookGen, and is used by the MMHOOK assemblies.
             *  We can unhook all methods hooked with HookGen using this.
             *  Or we can unsubscribe specific patch methods with 'On.Namespace.Type.Method -= CustomMethod;'
             */
            HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());

            Logger.LogDebug("Finished Unhooking!");
        }
    }
}