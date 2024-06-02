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

namespace TranslatedWarning
{
    [ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class TranslatedWarning : BaseUnityPlugin
    {
        public static TranslatedWarning Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;

        string plan = $"{Directory.GetCurrentDirectory()}\\plan.png";

        public List<string> commentList = new List<string>();

        string[] values = new string[50];

        string[] upgradeTranslations = { "WATER", "BIOS SYSTEM COOKIES", "GREAT PODCAST", "TOURISM SERVICES", "A MILITARY BASE" };


        Texture2D? translatedPlan;


        private void Start()
        {
            translatedPlan = new Texture2D(1024, 512, TextureFormat.ARGB32, false);
            Logger.LogInfo("TEXTURE CREATED");

            byte[] bytes = File.ReadAllBytes(plan);
            Logger.LogInfo("IMAGE BECAME BYTES");

            ImageConversion.LoadImage(translatedPlan, bytes);

            Logger.LogInfo("IMAGE LOADED!!!!!!!!!!!!!!!!!!!!!");


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
            Logger.LogInfo("OnSceneLoaded: " + scene.name);
            Logger.LogInfo(mode);

            if(scene.name == "SurfaceScene")
            {
                Logger.LogInfo("THIS IS THE SurfaceScene, RUNNING COROUTINE");


                // =============== THE PLAN ===============
                Transform thePlan = GameObject.Find("House").transform.GetChild(3).GetChild(0); //find the Transform

                Renderer rend = thePlan.GetComponent<Renderer>(); //get the MeshRenderer

                Material material = new Material(Shader.Find("Universal Render Pipeline/Unlit")); //create and apply material
                material.color = Color.white;
                material.mainTexture = translatedPlan;

                rend.material = material;


                // COROUTINE 
                StartCoroutine(Surface());
            }

            if (scene.name == "NewMainMenu")
            {
                Debug.Log("CLEARING LIST");
                seenList.Clear();

            }

        }
        
        void Update()
        {

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
                TranslateText(button);
            }

            // =============== SETTINGS ===============


            Transform settings = GameObject.Find("Canvas").transform.GetChild(3).GetChild(2);

            TranslateText(settings.GetChild(0)); //BackButton
            TranslateText(settings.GetChild(1), key: settings.gameObject.name); //Settings title

            Transform tabs = settings.GetChild(2).GetChild(0);
            foreach (Transform tab in tabs)
            {
                TranslateText(tab);
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

            
        }

        internal static void HookAll()
        {
            Logger.LogDebug("Hooking...");

            DialogRipper.Init();

            //InjectTranslation.Init();

            Logger.LogDebug("Finished Hooking!");

            

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
                        Destroy(component);
                    }
                }

            }

        }

        public static void Delete(Component component)
        {
            Debug.Log($"FOUND {component.GetType().ToString()}!!!! DELETEING THIS NIGGER");
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