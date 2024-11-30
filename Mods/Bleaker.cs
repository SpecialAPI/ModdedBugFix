using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Linq;
using System.Reflection;
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

        public static MethodInfo gcf_dmpm_t = AccessTools.Method(typeof(Bleaker), nameof(GoldenCircletFix_DontMultPickupModifier_Transpiler));
        public static MethodInfo gcf_dmpm_rm = AccessTools.Method(typeof(Bleaker), nameof(GoldenCircletFix_DontMultPickupModifier_ReplaceMult));

        public static MethodInfo hbf_u_p = AccessTools.Method(typeof(Bleaker), nameof(HealthyBulletsFix_Unsubscribe_Prefix));

        public static MethodInfo lcf_od_t = AccessTools.Method(typeof(Bleaker), nameof(LeadCrownFix_OnDestroy_Transpiler));
        public static MethodInfo lcf_od_puc = AccessTools.Method(typeof(Bleaker), nameof(LeadCrownFix_OnDestroy_PickedUpCheck));
        public static MethodInfo lcf_od_p = AccessTools.Method(typeof(Bleaker), nameof(LeadCrownFix_OnDestroy_Pop));

        public static MethodInfo ssf_u_t = AccessTools.Method(typeof(Bleaker), nameof(ShadesShadesFix_Update_Transpiler));
        public static MethodInfo ssf_u_puc = AccessTools.Method(typeof(Bleaker), nameof(ShadesShadesFix_Update_PickedUpCheck));

        public static MethodInfo plf_a_t = AccessTools.Method(typeof(Bleaker), nameof(PouchLauncherFix_Add_Transpiler));
        public static MethodInfo plf_a_scp = AccessTools.Method(typeof(Bleaker), nameof(PouchLauncherFix_Add_SetCoinProjectile));

        public static MethodInfo psf_sdfps_t = AccessTools.Method(typeof(Bleaker), nameof(ProjectileSpriteFix_SetupDefinitionForProjectileSprite_Transpiler));
        public static MethodInfo psf_sdfps_md = AccessTools.Method(typeof(Bleaker), nameof(ProjectileSpriteFix_SetupDefinitionForProjectileSprite_ModifyDefinition));

        public static MethodInfo ssf_a_t = AccessTools.Method(typeof(Bleaker), nameof(StarSplitterFix_Add_Transpiler));
        public static MethodInfo ssf_a_si = AccessTools.Method(typeof(Bleaker), nameof(StarSplitterFix_Add_SaveID));
        public static MethodInfo ssf_b_t = AccessTools.Method(typeof(Bleaker), nameof(StarSplitterFix_Bounce_Transpiler));
        public static MethodInfo ssf_b_ssi = AccessTools.Method(typeof(Bleaker), nameof(StarSplitterFix_Bounce_StarSplitterId));

        public static MethodInfo bgsf_aeb_pnt_t = AccessTools.Method(typeof(Bleaker), nameof(BabyGoodShellicopterFix_ApproachEnemiesBehavior_PickNewTarget_Transpiler));
        public static MethodInfo bgsf_aeb_pnt_b = AccessTools.Method(typeof(Bleaker), nameof(BabyGoodShellicopterFix_ApproachEnemiesBehavior_PickNewTarget_Beq));
        public static MethodInfo bgsf_aeb_pnt_aa = AccessTools.Method(typeof(Bleaker), nameof(BabyGoodShellicopterFix_ApproachEnemiesBehavior_PickNewTarget_AIAnimator));

        public static MethodInfo oooodgf_od_t = AccessTools.Method(typeof(Bleaker), nameof(OutOfOrderOnDestroyGeneralFix_OnDestroy_Transpiler));
        public static MethodInfo oooodgf_od_p = AccessTools.Method(typeof(Bleaker), nameof(OutOfOrderOnDestroyGeneralFix_OnDestroy_Pop));

        public static int StarSplitterId;

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

            var goldenCircletClass = AccessTools.TypeByName("BleakMod.GoldenCirclet");
            if(goldenCircletClass != null)
            {
                var pickup = AccessTools.Method(goldenCircletClass, "Pickup");

                if (pickup != null)
                    Plugin.HarmonyInstance.Patch(pickup, ilmanipulator: new(gcf_dmpm_t));
            }

            var healthyBulletsClass = AccessTools.TypeByName("BleakMod.HealthyBullets");
            if(healthyBulletsClass != null)
            {
                var drop = AccessTools.Method(healthyBulletsClass, "Drop");
                var onDestroy = AccessTools.Method(healthyBulletsClass, "OnDestroy");

                if (drop != null)
                    Plugin.HarmonyInstance.Patch(drop, prefix: new(hbf_u_p));

                if (onDestroy != null)
                    Plugin.HarmonyInstance.Patch(onDestroy, prefix: new(hbf_u_p));
            }

            var leadCrownClass = AccessTools.TypeByName("BleakMod.LeadCrown");
            if(leadCrownClass != null)
            {
                var onDestroy = AccessTools.Method(leadCrownClass, "OnDestroy");

                if (onDestroy != null)
                    Plugin.HarmonyInstance.Patch(onDestroy, ilmanipulator: new(lcf_od_t));
            }

            var shadesShadesClass = AccessTools.TypeByName("BleakMod.ShadesShades");
            if(shadesShadesClass != null)
            {
                var update = AccessTools.Method(shadesShadesClass, "Update");

                if (update != null)
                    Plugin.HarmonyInstance.Patch(update, ilmanipulator: new(ssf_u_t));
            }

            var pouchLauncherClass = AccessTools.TypeByName("BleakMod.PouchLauncher");
            if(pouchLauncherClass != null)
            {
                var add = AccessTools.Method(pouchLauncherClass, "Add");

                if (add != null)
                    Plugin.HarmonyInstance.Patch(add, ilmanipulator: new(plf_a_t));

                var bleakerAssembly = pouchLauncherClass.Assembly;

                if(bleakerAssembly != null)
                {
                    var gunToolsClass = bleakerAssembly.GetType("ItemAPI.GunTools");

                    if (gunToolsClass != null)
                    {
                        var setupDefinitionForProjectileSprite = AccessTools.Method(gunToolsClass, "SetupDefinitionForProjectileSprite");

                        if (setupDefinitionForProjectileSprite != null)
                            Plugin.HarmonyInstance.Patch(setupDefinitionForProjectileSprite, ilmanipulator: new(psf_sdfps_t));
                    }
                }
            }

            var startStrikerClass = AccessTools.TypeByName("BleakMod.StartStriker");
            if(startStrikerClass != null)
            {
                var add = AccessTools.Method(startStrikerClass, "Add");
                var bounce = AccessTools.Method(startStrikerClass, "bounce");

                if (add != null)
                    Plugin.HarmonyInstance.Patch(add, ilmanipulator: new(ssf_a_t));

                if (bounce != null)
                    Plugin.HarmonyInstance.Patch(bounce, ilmanipulator: new(ssf_b_t));
            }

            var babyGoodShellicopter_ApproachEnemiesBehaviorClass = AccessTools.TypeByName("BleakMod.BabyGoodShellicopter+ApproachEnemiesBehavior");
            if(babyGoodShellicopter_ApproachEnemiesBehaviorClass != null)
            {
                var pickNewTarget = AccessTools.Method(babyGoodShellicopter_ApproachEnemiesBehaviorClass, "PickNewTarget");

                if (pickNewTarget != null)
                    Plugin.HarmonyInstance.Patch(pickNewTarget, ilmanipulator: new(bgsf_aeb_pnt_t));
            }

            var itemsWithBrokenOnDestroy_OutOfOrder = new string[]
            {
                "ChamberOfFrogs",
                "CatchingMitts",
                "Protractor",
                "TrickShot",
                "FittedTankBarrel",
                "PendantOfTheFirstOrder",
                "GoldenCirclet",
                "CheeseAmmolet",
                "Distribullets",
                "GatlingGullets",
                "HungryClips",
                "HeroicCape",
                "Popcorn",
                "RepurposedShellCasing",
                "WhiteBulletCell",
                "StrawberryJam",
                "Bleaker",
                "WinchestersHat",
                "JammomancersHat",
                "ShowoffBullets",
                "FriendshipBracelet"
            };

            foreach(var i in itemsWithBrokenOnDestroy_OutOfOrder)
            {
                var type = AccessTools.TypeByName($"BleakMod.{i}");

                if (type == null)
                    continue;

                var onDestroy = AccessTools.Method(type, "OnDestroy");

                if (onDestroy == null)
                    continue;

                Plugin.HarmonyInstance.Patch(onDestroy, ilmanipulator: new(oooodgf_od_t));
            }

            var activesWithBrokenOnDestroy = new string[]
            {
                "SuspiciousLookingBell",
                "ShadesShades",
                "AmmocondasNest",
                "Rewind",
                "Overpill"
            };

            foreach (var i in activesWithBrokenOnDestroy)
                OnDestroyGeneralFix.FixOnDestroy($"BleakMod.{i}");
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

        public static void GoldenCircletFix_DontMultPickupModifier_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchLdcR4(100f)))
                return;

            crs.Emit(OpCodes.Call, gcf_dmpm_rm);
        }

        public static float GoldenCircletFix_DontMultPickupModifier_ReplaceMult(float _)
        {
            return 1f;
        }

        public static void HealthyBulletsFix_Unsubscribe_Prefix(PassiveItem __instance)
        {
            if (__instance == null || __instance.Owner == null)
                return;

            var type = __instance.GetType();
            var enteredCombatDelegate = (Action)Delegate.CreateDelegate(typeof(Action), __instance, "OnEnteredCombat");

            __instance.Owner.OnEnteredCombat -= enteredCombatDelegate;
        }

        public static void LeadCrownFix_OnDestroy_Transpiler(ILContext ctx, MethodBase mthd)
        {
            var baseClass = mthd.DeclaringType.BaseType;

            if (baseClass == null)
                return;

            var crs = new ILCursor(ctx);

            if (!crs.TryFindNext(out var d, x => x.MatchCall(baseClass, nameof(PassiveItem.OnDestroy))) || d.Length <= 0)
                return;

            if (!crs.TryFindNext(out var r, x => x.MatchRet()) || r.Length <= 0)
                return;

            var onDestroyInstr = d[0].Next;
            var retInstr = r[r.Length - 1].Next;

            var puLoc = crs.DeclareLocal<bool>();
            var afterOnDestroyLabel = crs.DefineLabel();

            // Check if the item is picked up. If it is, true will be saved to puLoc. If it's not, false will be saved.
            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldloca, puLoc);
            crs.Emit(OpCodes.Call, lcf_od_puc);

            // Load the item to potentially use for OnDestroy.
            crs.Emit(OpCodes.Ldarg_0);

            // Load puLoc for the brfalse instruction.
            crs.Emit(OpCodes.Ldloc, puLoc);

            // If puLoc is false, move to OnDestroy.
            crs.Emit(OpCodes.Brfalse, onDestroyInstr);
            // If puLoc is true, brfalse will do nothing and there will be a leftover reference to the item on the stack. Pop that reference.
            crs.Emit(OpCodes.Call, lcf_od_p);

            crs.Goto(onDestroyInstr, MoveType.Before);
            // Before OnDestroy, unconditionally branch to the instruction after OnDestroy (skip OnDestroy).
            crs.Emit(OpCodes.Br, afterOnDestroyLabel);

            crs.Goto(onDestroyInstr, MoveType.After);
            // After OnDestroy happens, unconditionally branch to the ret instruction to end the method.
            crs.Emit(OpCodes.Br, retInstr);

            // Mark the afterOnDestroyLabel after OnDestroy.
            crs.MarkLabel(afterOnDestroyLabel);
            // Pop the item reference loaded for OnDestroy.
            crs.Emit(OpCodes.Call, lcf_od_p);

            crs.Goto(retInstr, MoveType.Before);
            // Before normal ret happens, jump to the OnDestroy instruction if puLoc is true. After that, the previously emitted br instruction should jump back to ret.
            crs.Emit(OpCodes.Ldloc, puLoc);
            crs.Emit(OpCodes.Brtrue, onDestroyInstr);
        }

        public static void LeadCrownFix_OnDestroy_PickedUpCheck(PassiveItem item, out bool pu)
        {
            pu = true;

            if (item == null)
                pu = true;

            else if (item.Owner == null)
                pu = false;
        }

        public static void LeadCrownFix_OnDestroy_Pop(PassiveItem _) { }

        public static void ShadesShadesFix_Update_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCall<PlayerItem>(nameof(PlayerItem.Update))))
                return;

            if (!crs.TryFindNext(out var r, x => x.MatchRet()) || r.Length <= 0)
                return;

            var retInstr = r[r.Length - 1].Next;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, ssf_u_puc);
            crs.Emit(OpCodes.Brfalse, retInstr);
        }

        public static bool ShadesShadesFix_Update_PickedUpCheck(PlayerItem item)
        {
            if (item == null)
                return true;

            if (item.LastOwner == null || !item.PickedUp)
                return false;

            return true;
        }

        public static void PouchLauncherFix_Add_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchStloc(2)))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.Emit(OpCodes.Call, plf_a_scp);
        }

        public static void PouchLauncherFix_Add_SetCoinProjectile(Projectile proj)
        {
            var pouchLauncherClass = AccessTools.TypeByName("BleakMod.PouchLauncher");
            if (pouchLauncherClass == null)
                return;

            var coinProjectile = AccessTools.Field(pouchLauncherClass, "coinProjectile");
            if (coinProjectile == null || !coinProjectile.IsStatic)
                return;

            coinProjectile.SetValue(null, proj);
        }

        public static void ProjectileSpriteFix_SetupDefinitionForProjectileSprite_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchStloc(8)))
                return;

            crs.Emit(OpCodes.Ldloc, 8);
            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Call, psf_sdfps_md);
        }

        public static void ProjectileSpriteFix_SetupDefinitionForProjectileSprite_ModifyDefinition(tk2dSpriteDefinition def, int id)
        {
            def.materialInst.mainTexture = ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[id].materialInst.mainTexture;
            def.uvs = [.. ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[id].uvs];
        }

        public static void StarSplitterFix_Add_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<ItemDB>(nameof(ItemDB.Add))))
                return;

            crs.Emit(OpCodes.Call, ssf_a_si);
        }

        public static int StarSplitterFix_Add_SaveID(int id)
        {
            StarSplitterId = id;

            return id;
        }

        public static void StarSplitterFix_Bounce_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            foreach(var m in crs.MatchAfter(x => x.MatchLdcI4(881)))
            {
                crs.Emit(OpCodes.Call, ssf_b_ssi);
            }
        }

        public static int StarSplitterFix_Bounce_StarSplitterId(int _)
        {
            return StarSplitterId;
        }

        public static void BabyGoodShellicopterFix_ApproachEnemiesBehavior_PickNewTarget_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchLdsfld("BleakMod.BabyGoodShellicopter", "ChopperPrefab")))
                return;

            if (!crs.TryFindNext(out var l, x => x.MatchStloc(3)) || l.Length <= 0)
                return;

            var ldsfldInstr = crs.Next;
            var afterStlocInstr = l[0].Next.Next;

            crs.Emit(OpCodes.Call, bgsf_aeb_pnt_b);
            crs.Emit(OpCodes.Call, bgsf_aeb_pnt_b);
            crs.Emit(OpCodes.Beq, afterStlocInstr);

            foreach(var m in crs.MatchBefore(x => x.MatchCallOrCallvirt<BraveBehaviour>($"get_{nameof(BraveBehaviour.aiAnimator)}")))
            {
                var aiAnimatorInstr = crs.Next;
                var afterAiAnimatorLabel = crs.DefineLabel();

                crs.Emit(OpCodes.Call, bgsf_aeb_pnt_b);
                crs.Emit(OpCodes.Call, bgsf_aeb_pnt_b);
                crs.Emit(OpCodes.Beq, afterAiAnimatorLabel);

                crs.Goto(aiAnimatorInstr, MoveType.After);
                crs.MarkLabel(afterAiAnimatorLabel);

                crs.Emit(OpCodes.Ldarg_0);
                crs.Emit(OpCodes.Call, bgsf_aeb_pnt_aa);
            }
        }

        public static int BabyGoodShellicopterFix_ApproachEnemiesBehavior_PickNewTarget_Beq()
        {
            return 0;
        }

        public static AIAnimator BabyGoodShellicopterFix_ApproachEnemiesBehavior_PickNewTarget_AIAnimator(CompanionController _, BehaviorBase aa)
        {
            return aa.m_aiAnimator;
        }

        public static void OutOfOrderOnDestroyGeneralFix_OnDestroy_Transpiler(ILContext ctx, MethodBase mthd)
        {
            var baseClass = mthd.DeclaringType.BaseType;

            if (baseClass == null)
                return;

            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCall(baseClass, nameof(PassiveItem.OnDestroy))))
                return;

            if (!crs.TryFindNext(out var r, x => x.MatchRet()) || r.Length <= 0)
                return;

            var onDestroyInstr = crs.Next;
            var retInstr = r[r.Length - 1].Next;

            var afterOnDestroyLabel = crs.DefineLabel();

            // Before OnDestroy, unconditionally branch to the instruction after OnDestroy (skip OnDestroy).
            crs.Emit(OpCodes.Br, afterOnDestroyLabel);

            crs.Goto(onDestroyInstr, MoveType.After);
            // After OnDestroy happens, unconditionally branch to the ret instruction to end the method.
            crs.Emit(OpCodes.Br, retInstr);

            // Mark the afterOnDestroyLabel after OnDestroy.
            crs.MarkLabel(afterOnDestroyLabel);
            // Pop the item reference loaded for OnDestroy.
            crs.Emit(OpCodes.Call, oooodgf_od_p);

            crs.Goto(retInstr, MoveType.Before);
            // Before normal ret happens, jump to the OnDestroy instruction. After that, the previously emitted br instruction should jump back to ret.
            crs.Emit(OpCodes.Br, onDestroyInstr);
        }

        public static void OutOfOrderOnDestroyGeneralFix_OnDestroy_Pop(PassiveItem _) { }
    }
}
