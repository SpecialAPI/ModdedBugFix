using Gungeon;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModdedBugFix.Mods
{
    public static class FrostAndGunfire
    {
        public static MethodInfo bf_cu_p = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_CanUse_Prefix));
        public static MethodInfo bf_de_t = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_DoEffect_Transpiler));
        public static MethodInfo bf_de_sin = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_DoEffect_ShopItemNullcheck));
        public static MethodInfo bf_de_iln_f = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_DoEffect_ItemsListNullcheck));

        public static MethodInfo mmf_bp_p = AccessTools.Method(typeof(FrostAndGunfire), nameof(MiniMushboomFix_BuildPrefab_Postfix));

        public static void Patch()
        {
            var barterClass = AccessTools.TypeByName("FrostAndGunfireItems.Barter");
            if (barterClass != null)
            {
                var canuse = AccessTools.Method(barterClass, "CanBeUsed");
                var doeffect = AccessTools.Method(barterClass, "DoEffect");

                if (canuse != null)
                    Plugin.HarmonyInstance.Patch(canuse, prefix: new(bf_cu_p));
                if (doeffect != null)
                    Plugin.HarmonyInstance.Patch(doeffect, ilmanipulator: new(bf_de_t));
            }

            var miniMushboomClass = AccessTools.TypeByName("FrostAndGunfireItems.MiniMushboom");
            if(miniMushboomClass != null)
            {
                var buildprefab = AccessTools.Method(miniMushboomClass, "BuildPrefab");

                if (buildprefab != null)
                    Plugin.HarmonyInstance.Patch(buildprefab, postfix: new(mmf_bp_p));

                // If the MiniMushboom is already initialized for some reason
                MiniMushboomFix_BuildPrefab_Postfix();
            }
        }

        public static bool BarterFix_CanUse_Prefix(ref bool __result, PlayerController user)
        {
            if (user == null || user.CurrentRoom == null || user.carriedConsumables == null)
            {
                __result = false;
                return false;
            }
            return true;
        }

        public static void BarterFix_DoEffect_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (crs.JumpBeforeNext(x => x.MatchStloc(2)))
            {
                crs.Emit(OpCodes.Call, bf_de_iln_f);
            }

            crs.Index = 0;
            if (crs.JumpToNext(x => x.MatchCallOrCallvirt<UnityEngine.Object>("op_Inequality")))
            {
                crs.Emit(OpCodes.Ldloc_S, (byte)4);
                crs.Emit(OpCodes.Call, bf_de_sin);
            }
        }

        public static List<ShopItemController> BarterFix_DoEffect_ItemsListNullcheck(List<ShopItemController> itm)
        {
            return itm ?? [];
        }

        public static bool BarterFix_DoEffect_ShopItemNullcheck(bool curr, ShopItemController sic)
        {
            return curr && sic != null && sic.item != null;
        }

        public static void MiniMushboomFix_BuildPrefab_Postfix()
        {
            if (!Game.Enemies.ContainsID("kp:mini_mushboom"))
                return;

            var mushboom = Game.Enemies["kp:mini_mushboom"];

            if (mushboom == null || mushboom.aiAnimator == null)
                return;

            var anim = mushboom.aiAnimator;

            if (anim.IdleAnimation != null && (anim.IdleAnimation.AnimNames == null || anim.IdleAnimation.AnimNames.Length < 2))
                anim.IdleAnimation.AnimNames = ["idle", "idle"];

            if (anim.HitAnimation != null)
                anim.HitAnimation.Type = DirectionalAnimation.DirectionType.None;
        }
    }
}
