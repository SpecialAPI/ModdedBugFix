using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
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
        public const string VERSION = "1.5.0";

        public static Harmony HarmonyInstance;
        public static ManualLogSource ModdedBugFixLogger;

        public void Start()
        {
            HarmonyInstance = new Harmony(GUID);
            ModdedBugFixLogger = Logger;

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
                FrostAndGunfire.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Frost and Gunfire: {ex}");
            }

            try
            {
                Prismatism.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Prismatism: {ex}");
            }

            try
            {
                RetrashItems.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Retrash's Items: {ex}");
            }

            try
            {
                Bleaker.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching A Bleaker Item Pack: {ex}");
            }

            try
            {
                Oddments.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching Oddments: {ex}");
            }

            try
            {
                ReferenceCollection.Patch();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed patching The Reference Collection: {ex}");
            }
        }

        public static bool CheckModLoadedAndVersion(string modGuid, Version supportedVersion)
        {
            if (!Chainloader.PluginInfos.TryGetValue(modGuid, out var info))
                return false;

            if(info?.Metadata is not BepInPlugin metadata || metadata.Version is not Version version)
                return false;

            if(version > supportedVersion)
                return false;

            if(version < supportedVersion)
            {
                ModdedBugFixLogger?.LogError($"You are using an older version ({version}) of {metadata.Name} than Modded Bug Fix supports ({supportedVersion}).");
                return false;
            }

            return true;
        }
    }
}
