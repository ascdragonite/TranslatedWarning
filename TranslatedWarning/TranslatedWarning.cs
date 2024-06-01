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
                Transform buttonsList = GameObject.Find("Canvas").transform.GetChild(0).GetChild(3);

                foreach (Transform button in buttonsList) 
                {
                    TranslateButtonText(button);
                    var componentList = button.GetComponentInChildren<TextMeshProUGUI>().gameObject.GetComponents<Component>();
                    foreach (var component in componentList)
                    {
                        if (component.GetType() == typeof(LocalizeStringEvent))
                        {
                            Debug.Log($"FOUND {component.GetType().ToString()}!!!! DELETEING THIS NIGGER");
                            Destroy(component);
                        }
                    }
                }
            }

        }
        bool activated = false;
        void Update()
        {
            //if (GameObject.Find("Canvas").transform.GetChild(2).gameObject.activeSelf == false && activated == false)
            //{
            //    StartCoroutine(MainMenuMain());
            //    activated = true;
            //}

            if (Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(MainMenuMain());
            }
        }

        private IEnumerator MainMenuMain()
        {

            Transform buttonsList = GameObject.Find("Canvas").transform.GetChild(0).GetChild(3);



           int i = 0;
           while (i < buttonsList.childCount)
           {
                Debug.Log("CURRENT DIALOG: " + buttonsList.GetChild(i).gameObject.GetComponentInChildren<TextMeshProUGUI>().text);
                yield return null;
                if (buttonsList.GetChild(i).gameObject.GetComponentInChildren<TextMeshProUGUI>().text != InjectTranslation.translatedDict[buttonsList.GetChild(i).gameObject.name])
                {
                    TranslateButtonText(buttonsList.GetChild(i));
                    var componentList = buttonsList.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().gameObject.GetComponents<Component>();
                    foreach (var component in componentList)
                    {
                        Debug.Log($"Components: {component}");
                        if (component.GetType() == typeof(LocalizeStringEvent))
                        {
                            Debug.Log($"FOUND {component.GetType().ToString()}");
                            Destroy(component);
                        }
                    }

                }
                else
                {
                    i++;
                }
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
        public static void TranslateButtonText(Transform button)
        {
            TextMeshProUGUI buttonText = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                Debug.Log(buttonText.text + "!!!!!!!!!!!!!");
                buttonText.text = InjectTranslation.translatedDict[button.gameObject.name];
            }
        }
        internal static void HookAll()
        {
            Logger.LogDebug("Hooking...");

            //DialogRipper.Init();

            InjectTranslation.Init();

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