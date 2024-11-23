using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModdedBugFix
{
    public static class OnDestroyGeneralFix
    {
        public static MethodInfo odf_od_t = AccessTools.Method(typeof(OnDestroyGeneralFix), nameof(OnDestroyFix_OnDestroy_Transpiler));
        public static MethodInfo odf_od_puc_1 = AccessTools.Method(typeof(OnDestroyGeneralFix), nameof(OnDestroyFix_OnDestroy_PickedUpCheck_1));
        public static MethodInfo odf_od_puc_2 = AccessTools.Method(typeof(OnDestroyGeneralFix), nameof(OnDestroyFix_OnDestroy_PickedUpCheck_2));
        public static MethodInfo odf_od_p = AccessTools.Method(typeof(OnDestroyGeneralFix), nameof(OnDestroyFix_OnDestroy_Pop));
        public static MethodInfo odf_de_p = AccessTools.Method(typeof(OnDestroyGeneralFix), nameof(OnDestroyFix_DisableEffect_Pop));

        public static void FixOnDestroy(string className)
        {
            var type = AccessTools.TypeByName(className);

            if (type == null)
                return;

            var onDestroy = AccessTools.Method(type, nameof(BraveBehaviour.OnDestroy), []);

            if (onDestroy != null && onDestroy.IsDeclaredMember())
                Plugin.HarmonyInstance.Patch(onDestroy, ilmanipulator: new(odf_od_t));

            if (type.IsSubclassOf(typeof(PassiveItem)))
            {
                var disableEffect = AccessTools.Method(type, nameof(PassiveItem.DisableEffect), [typeof(PlayerController)]);

                if (disableEffect != null && disableEffect.IsDeclaredMember())
                    Plugin.HarmonyInstance.Patch(disableEffect, ilmanipulator: new(odf_od_t));
            }
        }

        public static void OnDestroyFix_OnDestroy_Transpiler(ILContext ctx, MethodBase mthd)
        {
            var baseClass = mthd.DeclaringType.BaseType;

            if (baseClass == null)
                return;

            var crs = new ILCursor(ctx);
            var disableEffect = false;

            if (!crs.TryFindNext(out var d, x => x.MatchCall(baseClass, nameof(BraveBehaviour.OnDestroy))) || d.Length <= 0)
            {
                disableEffect = true;

                // Make this patch also work for DisableEffect
                if (!crs.TryFindNext(out d, x => x.MatchCall(typeof(PassiveItem), nameof(PassiveItem.DisableEffect))))
                    return;
            }

            var onDestroyInstr = d[0].Next;

            var puLoc = crs.DeclareLocal<int>();

            // Check if the item is picked up. If it is, 1 will be saved to puLoc. If it's not, 0 will be saved.
            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldloca, puLoc);
            crs.Emit(OpCodes.Call, odf_od_puc_1);

            // Load the item to potentially use for OnDestroy.
            crs.Emit(OpCodes.Ldarg_0);

            // Load the player as well if this is a DisableEffect patch
            if(disableEffect)
                crs.Emit(OpCodes.Ldarg_1);

            // Load puLoc and 0 for the beq instruction.
            crs.Emit(OpCodes.Ldloc, puLoc);
            crs.Emit(OpCodes.Call, odf_od_puc_2);

            // If puLoc is equal to 0, move to OnDestroy.
            crs.Emit(OpCodes.Beq, onDestroyInstr);
            // If they're not equal, beq will do nothing and there will be a leftover reference to the item on the stack. Pop that reference.
            crs.Emit(OpCodes.Call, odf_od_p);

            // Pop the player as wel if this is a DisableEffect patch
            if (disableEffect)
                crs.Emit(OpCodes.Call, odf_de_p);

            crs.Goto(onDestroyInstr, MoveType.After);

            if (!crs.TryFindNext(out var r, x => x.MatchRet()) || r.Length <= 0)
                return;

            var retInstr = r[r.Length - 1].Next;

            // Load puLoc and 0 for the next beq instruction.
            crs.Emit(OpCodes.Ldloc, puLoc);
            crs.Emit(OpCodes.Call, odf_od_puc_2);

            // If puLoc is equal to 0, move to the return instruction (end of the method).
            crs.Emit(OpCodes.Beq, retInstr);
        }

        public static void OnDestroyFix_OnDestroy_PickedUpCheck_1(PickupObject item, out int pu)
        {
            // Asumme item is picked up by default.
            pu = 1;

            // If this *somehow* happens, you have bigger issues.
            if (item == null)
                pu = 1;

            // If the item is a PassiveItem and doesn't have an owner, it's not picked up.
            else if (item is PassiveItem pa && pa.Owner == null)
                pu = 0;

            // If the item is a PlayerItem and doesn't have an owner, it's not picked up.
            else if (item is PlayerItem aa && aa.LastOwner == null)
                pu = 0;
        }

        public static int OnDestroyFix_OnDestroy_PickedUpCheck_2() => 0;

        public static void OnDestroyFix_OnDestroy_Pop(PickupObject _) { }

        public static void OnDestroyFix_DisableEffect_Pop(PlayerController _) { }
    }
}
