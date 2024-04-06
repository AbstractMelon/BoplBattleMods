using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace InsaneRockBounce
{
    [BepInPlugin("com.Melon.InsaneRockBounce", "InsaneRockBounce", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Plugin InsaneRockBounce is loaded!");

            Harmony harmony = new Harmony("com.Melon.InsaneRockBounce");


            MethodInfo original = AccessTools.Method(typeof(BoplBody), "Awake");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "Awake_InsaneRockBounce_Plug");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        public class myPatches
        {
            public static object Awake_InsaneRockBounce_Plug(BoplBody __instance)
            {
                __instance.bounciness = (Fix)1000;
                return true;
            }
        }
    }
}
