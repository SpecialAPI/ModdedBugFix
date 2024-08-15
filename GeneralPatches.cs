using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace ModdedBugFix
{
    [HarmonyPatch]
    public class GeneralPatches
    {
        [HarmonyPatch(typeof(AudioAnimatorListener), nameof(AudioAnimatorListener.Start))]
        [HarmonyPrefix]
        public static bool AudioAnimatorListenerFix_Start_Prefix(AudioAnimatorListener __instance)
        {
            if (__instance == null || __instance.spriteAnimator == null)
                return false;

            return true;
        }

        [HarmonyPatch(typeof(PlayerOrbital), nameof(PlayerOrbital.OnDestroy))]
        [HarmonyPrefix]
        public static bool PlayerOrbitalFix_OnDestroy_Prefix(PlayerOrbital __instance)
        {
            if(__instance == null || __instance.Owner == null || __instance.Owner.orbitals == null)
                return false;

            return true;
        }
    }
}
