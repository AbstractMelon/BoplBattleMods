using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace InsaneRockBounce
{
    [BepInPlugin("com.Melon.BiggerBlackHoles", "BiggerBlackHoles", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Plugin BiggerBlackHoles is loaded!");

            Harmony harmony = new Harmony("com.Melon.BiggerBlackHoles");


            MethodInfo original = AccessTools.Method(typeof(BlackHole), "Awake");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "BiggerBlackHoles");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        public class myPatches
        {
            public static object BiggerBlackHoles(BlackHole __instance)
            {   
                __instance.startingMass = (Fix)1000f;
                __instance.SizeGrowthMultiplier = (Fix)1000;
                return true;
            }
        }
    }
}
