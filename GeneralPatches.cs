using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace ModdedBugFix
{
    [HarmonyPatch]
    public static class GeneralPatches
    {
        public static MethodInfo aaf_od_nvpn = AccessTools.Method(typeof(GeneralPatches), nameof(AIAnimatorFix_OnDestroy_NamedVFXPoolNullcheck));
        public static MethodInfo aaf_od_vpn = AccessTools.Method(typeof(GeneralPatches), nameof(AIAnimatorFix_OnDestroy_VFXPoolNullcheck));
        public static MethodInfo aaf_od_nvpln = AccessTools.Method(typeof(GeneralPatches), nameof(AIAnimatorFix_OnDestroy_NamedVFXPoolListNullcheck));

        public static MethodInfo l_aa_nvp = AccessTools.PropertyGetter(typeof(List<AIAnimator.NamedVFXPool>), "Item");

        public static AIAnimator.NamedVFXPool EmptyNamedVFXPool = new()
        {
            anchorTransform = null,
            name = string.Empty,
            vfxPool = new()
            {
                effects = [],
                type = VFXPoolType.None
            }
        };

        public static VFXPool EmptyVFXPool = new()
        {
            effects = [],
            type = VFXPoolType.None
        };

        public static List<AIAnimator.NamedVFXPool> EmptyNamedVFXPoolList = [];

        [HarmonyPatch(typeof(AIAnimator), nameof(AIAnimator.OnDestroy))]
        [HarmonyILManipulator]
        public static void AIAnimatorFix_OnDestroy_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt(l_aa_nvp)))
                return;

            crs.Emit(OpCodes.Call, aaf_od_nvpn);

            if (!crs.JumpToNext(x => x.MatchLdfld<AIAnimator.NamedVFXPool>(nameof(AIAnimator.NamedVFXPool.vfxPool))))
                return;

            crs.Emit(OpCodes.Call, aaf_od_vpn);

            if (!crs.JumpToNext(x => x.MatchLdfld<AIAnimator>(nameof(AIAnimator.OtherVFX))))
                return;

            crs.Emit(OpCodes.Call, aaf_od_nvpln);
        }

        public static AIAnimator.NamedVFXPool AIAnimatorFix_OnDestroy_NamedVFXPoolNullcheck(AIAnimator.NamedVFXPool curr)
        {
            if (curr == null)
                return EmptyNamedVFXPool;

            return curr;
        }

        public static VFXPool AIAnimatorFix_OnDestroy_VFXPoolNullcheck(VFXPool curr)
        {
            if (curr == null)
                return EmptyVFXPool;

            return curr;
        }

        public static List<AIAnimator.NamedVFXPool> AIAnimatorFix_OnDestroy_NamedVFXPoolListNullcheck(List<AIAnimator.NamedVFXPool> curr)
        {
            if(curr == null)
                return EmptyNamedVFXPoolList;

            return curr;
        }

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
