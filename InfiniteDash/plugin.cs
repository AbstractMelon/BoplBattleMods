using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace BetterDash
{
    [BepInPlugin("com.Melon.BetterDash", "BetterDash", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Plugin BetterDash is loaded!");

            Harmony harmony = new Harmony("com.Melon.BetterDash");


            MethodInfo original = AccessTools.Method(typeof(Dash), "Awake");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "Awake_BetterDash_Plug");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        public class myPatches
        {
            public static void Awake_BetterDash_Plug(Dash __instance)
            {
                __instance.dashLength = (Fix)500f;
            }
        }
    }
}
