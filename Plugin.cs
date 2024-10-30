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

            try
            {
                HarmonyInstance.PatchAll();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed applying general patches: {ex}");
            }

            try
            {
                if (Chainloader.PluginInfos.ContainsKey("kp.etg.frostandgunfire"))
                    FrostAndGunfire.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Frost and Gunfire: {ex}");
            }

            try
            {
                if (Chainloader.PluginInfos.ContainsKey("blazeykat.etg.prismatism"))
                    Prismatism.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Prismatism: {ex}");
            }

            try
            {
                if (Chainloader.PluginInfos.ContainsKey("Retrash"))
                    RetrashItems.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Retrash's Items: {ex}");
            }

            try
            {
                if (Chainloader.PluginInfos.ContainsKey("bleak.etg.abip"))
                    Bleaker.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching A Bleaker Item Pack: {ex}");
            }

            try
            {
                if (Chainloader.PluginInfos.ContainsKey("blazeykat.etg.oddments"))
                    Oddments.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Oddments: {ex}");
            }
        }
    }
}
