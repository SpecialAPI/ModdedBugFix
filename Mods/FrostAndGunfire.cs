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
        public static MethodInfo bf_cbu_p = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_CanBeUsed_Prefix));
        public static MethodInfo bf_de_t = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_DoEffect_Transpiler));
        public static MethodInfo bf_de_sin = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_DoEffect_ShopItemNullcheck));
        public static MethodInfo bf_de_iln_f = AccessTools.Method(typeof(FrostAndGunfire), nameof(BarterFix_DoEffect_ItemsListNullcheck));

        public static MethodInfo mmf_bp_p = AccessTools.Method(typeof(FrostAndGunfire), nameof(MiniMushboomFix_BuildPrefab_Postfix));

        public static MethodInfo bkf_cbu_p = AccessTools.Method(typeof(FrostAndGunfire), nameof(BloodiedKeyFix_CanBeUsed_Prefix));

        public static MethodInfo egf_p = AccessTools.Method(typeof(FrostAndGunfire), nameof(EnemyGeneralFix_Postfix));

        public static Dictionary<string, string> Enemies = new()
        {
            ["SuppressorShroom"]    = "kp:suppores",
            ["Observer"]            = "kp:observer",
            ["Gazer"]               = "kp:gazer",
            ["Globbulon"]           = "kp:globbulon",
            ["Bubbulon"]            = "kp:bubbulon",
            ["Humphrey"]            = "kp:humphrey",
            ["Ophaims"]             = "kp:ophaim",
            ["Mushboom"]            = "kp:mushboom",
            ["Firefly"]             = "kp:firefly",
            ["Spitter"]             = "kp:gunzooka",
            ["Spitfire"]            = "kp:spitfire",
            ["Salamander"]          = "kp:salamander",
            ["MiniMushboom"]        = "kp:mini_mushboom",
            ["Shellet"]             = "kp:skell",
            ["Silencer"]            = "kp:suppressor",
            ["CannonKin"]           = "kp:cannon_kin",
        };

        public const string Guid = "kp.etg.frostandgunfire";
        public static readonly Version SupportedVersion = new(1, 0, 0);

        public static void Patch()
        {
            if (!Plugin.CheckModLoadedAndVersion(Guid, SupportedVersion))
                return;

            var barterClass = AccessTools.TypeByName("FrostAndGunfireItems.Barter");
            if (barterClass != null)
            {
                var canBeUsed = AccessTools.Method(barterClass, "CanBeUsed");
                var doEffect = AccessTools.Method(barterClass, "DoEffect");

                if (canBeUsed != null)
                    Plugin.HarmonyInstance.Patch(canBeUsed, prefix: new(bf_cbu_p));
                if (doEffect != null)
                    Plugin.HarmonyInstance.Patch(doEffect, ilmanipulator: new(bf_de_t));
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

            var bloodiedKeyClass = AccessTools.TypeByName("FrostAndGunfireItems.BloodiedKey");
            if(bloodiedKeyClass != null)
            {
                var canBeUsed = AccessTools.Method(bloodiedKeyClass, "CanBeUsed");

                if (canBeUsed != null)
                    Plugin.HarmonyInstance.Patch(canBeUsed, prefix: new(bkf_cbu_p));
            }

            var itemsWithBrokenOnDestroy = new string[]
            {
                "SlimeyGuonStone",
                "Pods",
                "Greed",
                "Sack",
                "BulletSponge2",
                "GeminiGuonStone",
                "MirrorGuon"
            };

            foreach(var className in Enemies.Keys)
            {
                var enemyClass = AccessTools.TypeByName($"FrostAndGunfireItems.{className}");

                if (enemyClass == null)
                    continue;

                var buildPrefab = AccessTools.Method(enemyClass, "BuildPrefab");

                if (buildPrefab != null)
                    Plugin.HarmonyInstance.Patch(buildPrefab, postfix: new(egf_p));

                EnemyGeneralFix_Postfix(buildPrefab);
            }

            foreach (var i in itemsWithBrokenOnDestroy)
                OnDestroyGeneralFix.FixOnDestroy($"FrostAndGunfireItems.{i}");
        }

        public static void EnemyGeneralFix_Postfix(MethodBase __originalMethod)
        {
            if (__originalMethod?.DeclaringType?.Name is not string className || !Enemies.TryGetValue(className, out var enemyConsoleId))
                return;

            if (!Game.Enemies.ContainsID(enemyConsoleId))
                return;

            var enm = Game.Enemies[enemyConsoleId];

            if (enm == null)
                return;

            if (enm.spriteAnimator is tk2dSpriteAnimator anim && anim != null && anim.Library != null && anim.GetClipByName("awaken") == null && anim.GetClipByName("spawn") == null)
            {
                var idles = new string[]
                {
                    "idle",
                    "idle_left",
                    "idle_right"
                };
                var sid = -1;
                tk2dSpriteCollectionData coll = null;

                foreach(var anmName in idles)
                {
                    var clippy = anim.GetClipByName(anmName);

                    if(clippy == null || clippy.frames == null || clippy.frames.Length <= 0)
                        continue;

                    if (clippy.frames[0] is not tk2dSpriteAnimationFrame frame)
                        continue;

                    sid = frame.spriteId;
                    coll = frame.spriteCollection;
                    break;
                }

                if(sid != -1 && coll != null)
                {
                    anim.Library.clips = anim.Library.clips.AddToArray(new()
                    {
                        fps = 100,
                        name = "awaken",
                        wrapMode = tk2dSpriteAnimationClip.WrapMode.Once,
                        frames =
                        [
                            new()
                            {
                                spriteCollection = coll,
                                spriteId = sid,
                            }
                        ]
                    });
                }
            }

            enm.gameObject.GetOrAddComponent<ObjectVisibilityManager>();
        }

        public static bool BarterFix_CanBeUsed_Prefix(ref bool __result, PlayerController user)
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

        public static bool BloodiedKeyFix_CanBeUsed_Prefix(ref bool __result, PlayerController user)
        {
            if (user != null && user.CurrentRoom != null && user.healthHaver != null)
                return true;

            __result = false;
            return false;
        }
    }
}
