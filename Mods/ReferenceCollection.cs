using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace ModdedBugFix.Mods
{
    public static class ReferenceCollection
    {
        public static MethodInfo sf_t_t = AccessTools.Method(typeof(ReferenceCollection), nameof(SnowfoxFix_Transforming_Transpiler));
        public static MethodInfo sf_t_n_1 = AccessTools.Method(typeof(ReferenceCollection), nameof(SnowfoxFix_Transforming_Nullcheck_1));
        public static MethodInfo sf_t_n_2 = AccessTools.Method(typeof(ReferenceCollection), nameof(SnowfoxFix_Transforming_Nullcheck_2));
        public static MethodInfo sf_t_ddp = AccessTools.Method(typeof(ReferenceCollection), nameof(SnowfoxFix_Transforming_DontDestroyPrefab));

        public static MethodInfo sf_uod_op_p = AccessTools.Method(typeof(ReferenceCollection), nameof(SnowfoxFix_UnsubscribeOnDestroy_OnPickup_Postfix));
        public static MethodInfo sf_uod_opd_p = AccessTools.Method(typeof(ReferenceCollection), nameof(SnowfoxFix_UnsubscribeOnDestroy_OnPostDrop_Postfix));

        public static void Patch()
        {
            var snowfoxes = new string[]
            {
                "SnowfoxL",
                "SnowfoxXL",
                "SnowfoxXXL",
                "SnowfoxXXXL",

                "Snowfoxrosegold"
            };

            foreach(var f in snowfoxes)
            {
                var type = AccessTools.TypeByName($"Dulsamthings.{f}");

                if (type == null)
                    continue;

                var transforming = AccessTools.Method(type, "Transforming");
                var onPickup = AccessTools.Method(type, "OnPickup");
                var onPostDrop = AccessTools.Method(type, "OnPostDrop");

                if (transforming != null)
                    Plugin.HarmonyInstance.Patch(transforming, ilmanipulator: new(sf_t_t));

                if (onPickup != null)
                    Plugin.HarmonyInstance.Patch(onPickup, postfix: new(sf_uod_op_p));

                if (onPostDrop != null)
                    Plugin.HarmonyInstance.Patch(onPostDrop, postfix: new(sf_uod_opd_p));
            }

            var itemsWithBrokenDisableEffect = new string[]
            {
                "ExpensiveBullets",
                "CheesyBullets",
                "DeterminationBullets"
            };

            foreach(var i in itemsWithBrokenDisableEffect)
                OnDestroyGeneralFix.FixOnDestroy($"dulsamthings.{i}");
        }

        public static void SnowfoxFix_Transforming_Transpiler(ILContext ctx, MethodBase mthd)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryFindNext(out var r, x => x.MatchRet()))
                return;

            var retInstr = r[r.Length - 1].Next;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, sf_t_n_1);
            crs.Emit(OpCodes.Call, sf_t_n_2);
            crs.Emit(OpCodes.Beq, retInstr);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<GunInventory>(nameof(GunInventory.DestroyGun))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, sf_t_ddp);
        }

        public static int SnowfoxFix_Transforming_Nullcheck_1(MonoBehaviour behav)
        {
            if (behav == null)
                return 0;

            if (behav.GetComponent<Gun>() == null)
                return 0;

            return 1;
        }

        public static int SnowfoxFix_Transforming_Nullcheck_2()
        {
            return 0;
        }

        public static Gun SnowfoxFix_Transforming_DontDestroyPrefab(Gun _, MonoBehaviour behav)
        {
            return behav.GetComponent<Gun>();
        }

        public static void SnowfoxFix_UnsubscribeOnDestroy_OnPickup_Postfix(MonoBehaviour __instance, GameActor owner)
        {
            if (__instance == null || owner == null || owner is not PlayerController player)
                return;

            var destroyBehav = __instance.gameObject.GetOrAddComponent<SnowfoxUnsubscribeOnDestroy>();

            destroyBehav.snowfox = __instance;
            destroyBehav.player = player;
        }


        public static void SnowfoxFix_UnsubscribeOnDestroy_OnPostDrop_Postfix(MonoBehaviour __instance)
        {
            if (__instance == null)
                return;

            var destroyBehav = __instance.gameObject.GetComponent<SnowfoxUnsubscribeOnDestroy>();

            if(destroyBehav == null)
                return;

            destroyBehav.player = null;
        }
    }

    public class SnowfoxUnsubscribeOnDestroy : MonoBehaviour
    {
        public PlayerController player;
        public MonoBehaviour snowfox;

        public void OnDestroy()
        {
            if (player == null || snowfox == null)
                return;

            var transformingDelegate = (Action<PlayerController>)Delegate.CreateDelegate(typeof(Action<PlayerController>), snowfox, "Transforming");
            player.OnKilledEnemy -= transformingDelegate;

            player = null;
        }
    }
}
