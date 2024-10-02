using Dungeonator;
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
    public static class Oddments
    {
        public static MethodInfo sbf_u_t = AccessTools.Method(typeof(Oddments), nameof(SpiderBootsFix_Update_Transpiler));
        public static MethodInfo sbf_u_dn = AccessTools.Method(typeof(Oddments), nameof(SpiderBootsFix_Update_DungeonNullcheck));

        public static MethodInfo gef_t = AccessTools.Method(typeof(Oddments), nameof(GoopEffectsFix_Transpiler));
        public static MethodInfo gef_aan_1 = AccessTools.Method(typeof(Oddments), nameof(GoopEffectsFix_AIAnimatorNullcheck_1));
        public static MethodInfo gef_aan_2 = AccessTools.Method(typeof(Oddments), nameof(GoopEffectsFix_AIAnimatorNullcheck_2));
        public static MethodInfo gef_an = AccessTools.Method(typeof(Oddments), nameof(GoopEffectsFix_ActorNullcheck));

        public static void Patch()
        {
            var spiderBootsClass = AccessTools.TypeByName("Oddments.SpiderBoots");
            if(spiderBootsClass != null)
            {
                var update = AccessTools.Method(spiderBootsClass, "Update");

                if (update != null)
                    Plugin.HarmonyInstance.Patch(update, ilmanipulator: new(sbf_u_t));
            }

            var customGoopEffectDoerClass = AccessTools.TypeByName("Oddments.CustomGoopEffectDoer");
            if(customGoopEffectDoerClass != null)
            {
                var coolNewCustomGoopEffects = AccessTools.Method(customGoopEffectDoerClass, "CoolNewCustomGoopEffects");

                if (coolNewCustomGoopEffects != null)
                    Plugin.HarmonyInstance.Patch(coolNewCustomGoopEffects, ilmanipulator: new(gef_t));
            }
        }

        public static void SpiderBootsFix_Update_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<UnityEngine.Object>("op_Implicit")))
                return;

            crs.Emit(OpCodes.Call, sbf_u_dn);
        }

        public static bool SpiderBootsFix_Update_DungeonNullcheck(bool curr)
        {
            if (!GameManager.HasInstance || GameManager.Instance.Dungeon == null)
                return false;

            if (Dungeon.IsGenerating)
                return false;

            return curr;
        }

        public static void GoopEffectsFix_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if(!crs.JumpToNext(x => x.MatchIsinst(out _), 2))
                return;

            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Call, gef_an);

            if (!crs.JumpToNext(x => x.MatchBrfalse(out _), 3))
                return;

            if (!crs.TryFindNext(out var c, x => x.MatchBrfalse(out _)) || c.Length <= 0)
                return;

            var cr = c[0];

            if(cr == null || cr.Next == null || !cr.Next.MatchBrfalse(out var skipAiAnimatorChecks))
                return;

            crs.Emit(OpCodes.Ldarg_1);

            crs.Emit(OpCodes.Call, gef_aan_1);
            crs.Emit(OpCodes.Call, gef_aan_2);

            crs.Emit(OpCodes.Beq, skipAiAnimatorChecks);
        }

        public static AIActor GoopEffectsFix_ActorNullcheck(AIActor curr, GameActor actor)
        {
            return actor == null ? null : curr;
        }

        public static int GoopEffectsFix_AIAnimatorNullcheck_1(GameActor actor)
        {
            if (actor == null)
                return 0;

            if (actor.aiAnimator != null)
                return 0;

            return 1;
        }

        public static int GoopEffectsFix_AIAnimatorNullcheck_2()
        {
            return 1;
        }
    }
}
