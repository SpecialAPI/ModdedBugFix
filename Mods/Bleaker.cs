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
    public static class Bleaker
    {
        public static MethodInfo cf_u_p = AccessTools.Method(typeof(Bleaker), nameof(CarrotFix_Update_Prefix));
        public static MethodInfo cf_u_t = AccessTools.Method(typeof(Bleaker), nameof(CarrotFix_Update_Transpiler));
        public static MethodInfo cf_u_cn = AccessTools.Method(typeof(Bleaker), nameof(CarrotFix_Update_CameraNullcheck));

        public static MethodInfo coff_dtf_t = AccessTools.Method(typeof(Bleaker), nameof(ChamberOfFrogsFix_DoTongueFlick_Transpiler));
        public static MethodInfo coff_dtf_cn = AccessTools.Method(typeof(Bleaker), nameof(ChamberOfFrogsFix_DoTongueFlick_CheckNearest));
        public static MethodInfo coff_dtf_n = AccessTools.Method(typeof(Bleaker), nameof(ChamberOfFrogsFix_DoTongueflick_Null));

        public static MethodInfo whf_u_t = AccessTools.Method(typeof(Bleaker), nameof(WinchestersHatFix_Update_Transpiler));
        public static MethodInfo whf_u_n_1 = AccessTools.Method(typeof(Bleaker), nameof(WinchestersHatFix_Update_Nullcheck_1));
        public static MethodInfo whf_u_n_2 = AccessTools.Method(typeof(Bleaker), nameof(WinchestersHatFix_Update_Nullcheck_2));
        public static MethodInfo whf_sva_p = AccessTools.Method(typeof(Bleaker), nameof(WinchestersHatFix_SpawnVFXAttached_Prefix));

        public static void Patch()
        {
            var carrotClass = AccessTools.TypeByName("BleakMod.Carrot");
            if (carrotClass != null)
            {
                var update = AccessTools.Method(carrotClass, "Update");

                if(update != null)
                    Plugin.HarmonyInstance.Patch(update, prefix: new(cf_u_p), ilmanipulator: new(cf_u_t));
            }

            var frogTongueBehaviorClass = AccessTools.TypeByName("BleakMod.ChamberOfFrogs+FrogTongueBehavior");
            if(frogTongueBehaviorClass != null)
            {
                var doTongueFlick = AccessTools.Method(frogTongueBehaviorClass, "DoTongueFlick");

                if (doTongueFlick != null)
                    Plugin.HarmonyInstance.Patch(doTongueFlick, ilmanipulator: new(coff_dtf_t));
            }

            var winchestersHatClass = AccessTools.TypeByName("BleakMod.WinchestersHat");
            if(winchestersHatClass != null)
            {
                var update = AccessTools.Method(winchestersHatClass, "Update");
                var spawnVFXAttached = AccessTools.Method(winchestersHatClass, "SpawnVFXAttached");

                if (update != null)
                    Plugin.HarmonyInstance.Patch(update, ilmanipulator: new(whf_u_t));

                if (spawnVFXAttached != null)
                    Plugin.HarmonyInstance.Patch(spawnVFXAttached, prefix: new(whf_sva_p));
            }
        }

        public static bool CarrotFix_Update_Prefix(PassiveItem __instance)
        {
            if(__instance == null || !__instance.PickedUp || __instance.Owner == null)
                return false;

            if (__instance.Owner.passiveItems == null || __instance.Owner.inventory == null || __instance.Owner.inventory.AllGuns == null)
                return false;

            return true;
        }

        public static void CarrotFix_Update_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<UnityEngine.Object>("op_Implicit")))
                return;

            crs.Emit(OpCodes.Call, cf_u_cn);
        }

        public static bool CarrotFix_Update_CameraNullcheck(bool curr)
        {
            return curr && GameManager.Instance != null && GameManager.Instance.MainCameraController != null;
        }

        public static void ChamberOfFrogsFix_DoTongueFlick_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryFindNext(out var insts, x => x.MatchRet()))
                return;

            if (!crs.JumpToNext(x => x.MatchStloc(1)))
                return;

            var retInst = insts[0].Next;
            var nextInstr = crs.Next;

            crs.Emit(OpCodes.Ldloc_1);
            crs.Emit(OpCodes.Call, coff_dtf_cn);

            // If the nearest enemy exists ((nearestEnemy == null) == false), just move to the next instruction as it normally would.
            crs.Emit(OpCodes.Brfalse, nextInstr); 

            // Otherwise, load null as the return value and jump to the return instruction.
            crs.Emit(OpCodes.Call, coff_dtf_n);
            crs.Emit(OpCodes.Br, retInst); 
        }

        public static bool ChamberOfFrogsFix_DoTongueFlick_CheckNearest(AIActor ai)
        {
            return ai == null;
        }

        public static BeamController ChamberOfFrogsFix_DoTongueflick_Null()
        {
            return null;
        }

        public static void WinchestersHatFix_Update_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            ILLabel label = null;

            if(!crs.TryFindNext(out var c, x => x.MatchBneUn(out label)))
                return;

            if (label == null)
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, whf_u_n_1);
            crs.Emit(OpCodes.Call, whf_u_n_2);

            crs.Emit(OpCodes.Beq, label);
        }

        public static int WinchestersHatFix_Update_Nullcheck_1(PassiveItem p)
        {
            if(p == null || p.Owner == null || p.Owner.CurrentGun == null)
                return 0;

            return 1;
        }

        public static int WinchestersHatFix_Update_Nullcheck_2()
        {
            return 0;
        }

        public static bool WinchestersHatFix_SpawnVFXAttached_Prefix(PassiveItem __instance)
        {
            if (__instance == null || __instance.Owner == null)
                return false;

            if(__instance.Owner.specRigidbody == null || __instance.Owner.specRigidbody.PrimaryPixelCollider == null)
                return false;

            if(PhysicsEngine.Instance == null)
                return false;

            return true;
        }
    }
}
