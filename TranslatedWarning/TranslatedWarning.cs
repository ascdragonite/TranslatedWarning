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
using System.Linq;
using UnityEngine.UI;
using Zorro.Core;
using UnityEngine.UIElements;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.PropertyVariants;
using static System.Net.Mime.MediaTypeNames;


namespace TranslatedWarning
{

    [ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class TranslatedWarning : BaseUnityPlugin
    {

        public static TranslatedWarning Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;

        string plan = $"{InjectTranslation.assemblyPath}\\Resources\\plan.png";
        string title = $"{InjectTranslation.assemblyPath}\\Resources\\title.png";

        public List<string> commentList = new List<string>();

        string[] values = new string[50];

        string[] upgradeTranslations = { "WATER", "BIOS SYSTEM COOKIES", "GREAT PODCAST", "TOURISM SERVICES", "A MILITARY BASE" };


        public static Texture2D? translatedPlan;
        public static Texture2D? titleTexture;
        public static Sprite? titleTranslated;

        private void Start()
        {
            translatedPlan = new Texture2D(1024, 512, TextureFormat.ARGB32, false);
            titleTexture = new Texture2D(1024, 348, TextureFormat.ARGB32, false);
            //Logger.LogInfo("TEXTURE CREATED");

            byte[] bytes = File.ReadAllBytes(plan);
            byte[] titleBytes = File.ReadAllBytes(title);
            //Logger.LogInfo("IMAGE BECAME BYTES");

            ImageConversion.LoadImage(translatedPlan, bytes);
            ImageConversion.LoadImage(titleTexture, titleBytes);

            titleTranslated = Sprite.Create(titleTexture, new Rect(0.0f, 0.0f, titleTexture.width, titleTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            //Logger.LogInfo("IMAGE LOADED!!!!!!!!!!!!!!!!!!!!!");


        }

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            HookAll();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public static List<string> seenList = new List<string>();
        internal void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Logger.LogInfo("OnSceneLoaded: " + scene.name);
            //Logger.LogInfo(mode);

            if(scene.name == "SurfaceScene")
            {
                //Logger.LogInfo("THIS IS THE SurfaceScene, RUNNING COROUTINE");

                // COROUTINE 
                StartCoroutine(Surface());
            }

            if (scene.name == "NewMainMenu")
            {
                Debug.Log("CLEARING LIST");
                seenList.Clear();

            }

        }
        

        private IEnumerator MainMenu()
        {

            yield return null;
            foreach (var things in GameObject.Find("Canvas").transform)
            {
                Debug.Log(things);
            }

            // =============== MAIN MENU ===============
            Transform buttonsList = GameObject.Find("Canvas").transform.GetChild(0).GetChild(3);

            foreach (Transform button in buttonsList)
            {
                InjectTranslation.TranslateText(button);
            }

            // =============== SETTINGS ===============


            Transform settings = GameObject.Find("Canvas").transform.GetChild(3).GetChild(2);

            InjectTranslation.TranslateText(settings.GetChild(0)); //BackButton
            InjectTranslation.TranslateText(settings.GetChild(1), key: settings.gameObject.name); //Settings title

            Transform tabs = settings.GetChild(2).GetChild(0);
            foreach (Transform tab in tabs)
            {
                InjectTranslation.TranslateText(tab);
            }

            yield break;
        }

        private IEnumerator Surface()
        {
            yield return 0;

            // =============== ADDONS ===============
            GameObject addons = GameObject.Find("Addons");

            int i = 0;

            foreach (Transform upgradeTransform in addons.transform)
            {
                if (i == 5) { break; }
                Transform upgrade = upgradeTransform;

                Transform locked = upgrade.GetChild(1);

                Component[] objects = locked.GetComponentsInChildren<Component>(true);

                GameObject title = new GameObject();
                foreach (Component thing in objects)
                {
                    if (thing.gameObject.name == "Title")
                    {
                        title = thing.gameObject;
                        break;
                    }
                }

                TMP_Text TMP = title.GetComponent<TextMeshPro>();

                Debug.Log(TMP.text);

                TMP.text = upgradeTranslations[i];

                Debug.Log(TMP.text);

                i++;
            }


            yield break;
        }

        internal static void HookAll()
        {
            Logger.LogDebug("Hooking...");

            //DialogRipper.Init();

            InjectTranslation.Init();

            Logger.LogDebug("Finished Hooking!");

            

        }
        

        public static void Delete(Component component)
        {
            //Debug.Log($"FOUND {component.GetType().ToString()}!!!!");
            Destroy(component);
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