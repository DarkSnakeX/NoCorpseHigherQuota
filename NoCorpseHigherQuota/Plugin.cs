using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using NoCorpseHigherQuota.Patches;

namespace NoCorpseHigherQuota
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class NoCorpseHigherQuota : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static NoCorpseHigherQuota Instance;

        internal static ManualLogSource Mls;
        
        public static Config MyConfig { get; internal set; }



        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            MyConfig = new(Config);

            Mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            Mls.LogInfo("NoCorpseHigherQuota is loaded - version " + PluginInfo.PLUGIN_VERSION);

            _harmony.PatchAll(typeof(NoCorpseHigherQuota));
            _harmony.PatchAll(typeof(LeavingBodyPatch));
            _harmony.PatchAll(typeof(ReportPatch));


        }

        
        
        
    }
}