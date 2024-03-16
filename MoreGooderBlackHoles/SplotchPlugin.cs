using Splotch;
using HarmonyLib;
using BoplFixedMath;
using System.Reflection;
using UnityEngine;

namespace BiggerBlackHoles
{
    public class BiggerBlackHoles : SplotchMod
    {
        public override void OnLoad()
        {
            Logger.Log("Plugin BiggerBlackHoles is loaded!");

            Harmony harmony = new Harmony("com.Melon.BiggerBlackHoles");

            MethodInfo original = AccessTools.Method(typeof(BlackHole), "Awake");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "BiggerBlackHoles");
            harmony.Patch(original, new HarmonyMethod(patch));
        }
    }
    public class myPatches
    {
        public static void BiggerBlackHoles(BlackHole __instance)
        {
            __instance.startingMass = (Fix)1000f;
            __instance.SizeGrowthMultiplier = (Fix)1000;
        }
    }
}
