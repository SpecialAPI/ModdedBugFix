using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModdedBugFix.Mods
{
    public static class RetrashItems
    {
        public static MethodInfo cbf_sh_t = AccessTools.Method(typeof(RetrashItems), nameof(CrystalBallFix_SoulHook_Transpiler));
        public static MethodInfo cbf_sh_n = AccessTools.Method(typeof(RetrashItems), nameof(CrystalBallFix_SoulHook_Nullcheck));
        public static MethodInfo cbf_sh_rp1 = AccessTools.Method(typeof(RetrashItems), nameof(CrystalBallFix_SoulHook_ReplaceP1));
        public static MethodInfo cbf_sh_rp2 = AccessTools.Method(typeof(RetrashItems), nameof(CrystalBallFix_SoulHook_ReplaceP2));

        public static void Patch()
        {
            var crystalBallClass = AccessTools.TypeByName("Blunderbeast.CrystalBall");
            if(crystalBallClass != null)
            {
                var soulHook = AccessTools.Method(crystalBallClass, "SoulHook");

                if(soulHook != null)
                    Plugin.HarmonyInstance.Patch(soulHook, ilmanipulator: new(cbf_sh_t));
            }
        }

        public static void CrystalBallFix_SoulHook_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryFindNext(out var insts, x => x.MatchRet()))
                return;

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt(out var mthd) && mthd.DeclaringType.Is(typeof(Action<LifeOrbGunModifier, bool>)) && mthd.Name == "Invoke"))
                return;

            var retInst = insts[0].Next;

            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Call, cbf_sh_n);
            crs.Emit(OpCodes.Brtrue, retInst);

            if (!crs.JumpBeforeNext(x => x.MatchStloc(0)))
                return;

            crs.Emit(OpCodes.Call, cbf_sh_rp1);

            if (!crs.JumpBeforeNext(x => x.MatchStloc(1)))
                return;

            crs.Emit(OpCodes.Call, cbf_sh_rp2);
        }

        public static bool CrystalBallFix_SoulHook_Nullcheck(LifeOrbGunModifier orb)
        {
            if(orb == null)
                return true;

            if(GameManager.Instance == null)
                return true;

            var firstValid = GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer.CurrentGun != null;
            var secondValid = GameManager.Instance.SecondaryPlayer != null && GameManager.Instance.SecondaryPlayer.CurrentGun != null;

            if (!firstValid && !secondValid)
                return true;

            return false;
        }

        public static PlayerController CrystalBallFix_SoulHook_ReplaceP1(PlayerController curr)
        {
            if (curr != null && curr.CurrentGun != null)
                return curr;

            if (GameManager.Instance == null || GameManager.Instance.SecondaryPlayer == null || GameManager.Instance.SecondaryPlayer.CurrentGun == null)
                return curr;

            return GameManager.Instance.SecondaryPlayer;
        }

        public static PlayerController CrystalBallFix_SoulHook_ReplaceP2(PlayerController curr)
        {
            if (curr != null && curr.CurrentGun != null)
                return curr;

            if (GameManager.Instance == null || GameManager.Instance.PrimaryPlayer == null || GameManager.Instance.PrimaryPlayer.CurrentGun == null)
                return curr;

            return GameManager.Instance.PrimaryPlayer;
        }
    }
}
