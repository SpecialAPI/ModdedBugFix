﻿using Dungeonator;
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
    public static class Prismatism
    {
        public static MethodInfo pff_cbu_p = AccessTools.Method(typeof(Prismatism), nameof(ParrotsFeatherFix_CanBeUsed_Prefix));

        public static MethodInfo bf_ec_p = AccessTools.Method(typeof(Prismatism), nameof(BraveryFix_EnemiesCheck_Prefix));

        public static MethodInfo mpf_oph_t = AccessTools.Method(typeof(Prismatism), nameof(MaidenPlatingFix_OnPlayerHit_Transpiler));
        public static MethodInfo mpf_oph_rp = AccessTools.Method(typeof(Prismatism), nameof(MaidenPlatingFix_OnPlayerHit_ReplaceProjectile));

        public static MethodInfo caf_od_t = AccessTools.Method(typeof(Prismatism), nameof(ColdAmmoFix_OnDestroy_Transpiler));
        public static MethodInfo caf_od_u = AccessTools.Method(typeof(Prismatism), nameof(ColdAmmoFix_OnDestroy_Unsubscribe));

        public const string Guid = "blazeykat.etg.prismatism";
        public static readonly Version SupportedVersion = new(2, 7, 6);

        public static void Patch()
        {
            if (!Plugin.CheckModLoadedAndVersion(Guid, SupportedVersion))
                return;

            var parrotsFeatherClass = AccessTools.TypeByName("katmod.ParrotsFeather");
            if(parrotsFeatherClass != null)
            {
                var canBeUsed = AccessTools.Method(parrotsFeatherClass, "CanBeUsed");

                if (canBeUsed != null)
                    Plugin.HarmonyInstance.Patch(canBeUsed, prefix: new(pff_cbu_p));
            }

            var braveryClass = AccessTools.TypeByName("katmod.Bravery");
            if(braveryClass != null)
            {
                var enemiesCheck = AccessTools.Method(braveryClass, "EnemiesCheck");

                if (enemiesCheck != null)
                    Plugin.HarmonyInstance.Patch(enemiesCheck, prefix: new(bf_ec_p));
            }

            var maidenPlatingClass = AccessTools.TypeByName("katmod.MaidenPlating");
            if(maidenPlatingClass != null)
            {
                var onPlayerHit = AccessTools.Method(maidenPlatingClass, "OnPlayerHit");

                if (onPlayerHit != null)
                    Plugin.HarmonyInstance.Patch(onPlayerHit, ilmanipulator: new(mpf_oph_t));
            }

            var coldAmmoClass = AccessTools.TypeByName("katmod.ColdAmmo");
            if(coldAmmoClass != null)
            {
                var onDestroy = AccessTools.Method(coldAmmoClass, "OnDestroy");

                if (onDestroy != null)
                    Plugin.HarmonyInstance.Patch(onDestroy, ilmanipulator: new(caf_od_t));
            }

            var itemsWithBrokenOnDestroy = new string[]
            {
                "TwoOfHearts"
            };

            foreach (var i in itemsWithBrokenOnDestroy)
                OnDestroyGeneralFix.FixOnDestroy($"katmod.{i}");
        }

        public static bool ParrotsFeatherFix_CanBeUsed_Prefix(ref bool __result, PlayerController user)
        {
            if(user == null || user.CurrentRoom == null || Camera.main == null)
            {
                __result = false;
                return false;
            }

            return true;
        }

        public static bool BraveryFix_EnemiesCheck_Prefix(PassiveItem __instance)
        {
            if (__instance == null || __instance.Owner == null || __instance.Owner.CurrentRoom == null)
                return false;

            if(__instance.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) == null)
                return false;

            return true;
        }

        public static void MaidenPlatingFix_OnPlayerHit_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<BraveBehaviour>($"get_{nameof(BraveBehaviour.projectile)}")))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.Emit(OpCodes.Call, mpf_oph_rp);
        }

        public static Projectile MaidenPlatingFix_OnPlayerHit_ReplaceProjectile(Projectile _, Projectile proj)
        {
            return proj;
        }

        public static void ColdAmmoFix_OnDestroy_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<PlayerController>($"add_{nameof(PlayerController.GunChanged)}")))
                return;

            var instr = crs.Prev;
            var unsubLabel = crs.MarkLabel();

            crs.Emit(OpCodes.Call, caf_od_u);

            crs.Goto(instr, MoveType.Before);
            crs.Emit(OpCodes.Br, unsubLabel);
        }

        public static void ColdAmmoFix_OnDestroy_Unsubscribe(PlayerController player, Action<Gun, Gun, bool> act)
        {
            if(player == null)
                return;

            player.GunChanged -= act;
        }
    }
}
