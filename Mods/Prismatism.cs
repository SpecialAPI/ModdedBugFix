﻿using Dungeonator;
using HarmonyLib;
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

        public static MethodInfo bf_ec_p = AccessTools.Method(typeof(Prismatism), nameof(Prefix));

        public static void Patch()
        {
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

        public static bool Prefix(PassiveItem __instance)
        {
            if (__instance == null || __instance.Owner == null || __instance.Owner.CurrentRoom == null)
                return false;

            if(__instance.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) == null)
                return false;

            return true;
        }
    }
}
