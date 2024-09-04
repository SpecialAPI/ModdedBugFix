using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using ModdedBugFix.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModdedBugFix
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.moddedbugfix";
        public const string NAME = "Modded Bugfix";
        public const string VERSION = "1.1.0";

        public static Harmony HarmonyInstance;

        public void Start()
        {
            HarmonyInstance = new Harmony(GUID);

            HarmonyInstance.PatchAll();

            if (Chainloader.PluginInfos.ContainsKey("kp.etg.frostandgunfire"))
                FrostAndGunfire.Patch();

            if (Chainloader.PluginInfos.ContainsKey("blazeykat.etg.prismatism"))
                Prismatism.Patch();

            if (Chainloader.PluginInfos.ContainsKey("Retrash"))
                RetrashItems.Patch();

            if(Chainloader.PluginInfos.ContainsKey("bleak.etg.abip"))
                Bleaker.Patch();
        }
    }
}
