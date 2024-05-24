using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TranslatedWarning.Patches;

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


        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            HookAll();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        }

        
        internal static void HookAll()
        {
            Logger.LogDebug("Hooking...");

            DialogRipper.Init();

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