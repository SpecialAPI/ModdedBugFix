using HarmonyLib;
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
        public static MethodInfo pff_cu_p = AccessTools.Method(typeof(Prismatism), nameof(ParrotsFeatherFix_CanUse_Prefix));

        public static void Patch()
        {
            var parrotsFeatherClass = AccessTools.TypeByName("katmod.ParrotsFeather");
            if(parrotsFeatherClass != null)
            {
                var canuse = AccessTools.Method(parrotsFeatherClass, "CanBeUsed");

                if (canuse != null)
                    Plugin.HarmonyInstance.Patch(canuse, prefix: new(pff_cu_p));
            }
        }

        public static bool ParrotsFeatherFix_CanUse_Prefix(ref bool __result, PlayerController user)
        {
            if(user == null || user.CurrentRoom == null || Camera.main == null)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
