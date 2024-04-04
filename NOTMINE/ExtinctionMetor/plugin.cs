using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ExtinctionMetor
{
    [BepInPlugin("com.savaGe.ExtinctionMetor", "ExtinctionMetor", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Plugin ExtinctionMetor is loaded!");

            Harmony harmony = new Harmony("com.savaGe.ExtinctionMetor");


            MethodInfo original = AccessTools.Method(typeof(MeteorSmash), "Awake");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "Awake_ExtinctionMetor_Plug");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        public class myPatches
        {
            public static void Awake_ExtinctionMetor_Plug(MeteorSmash __instance)
            {
                __instance.timeBeforeDrop = (Fix)0.1;
                __instance.maxHoldTime = (Fix)2;
                __instance.minHoldTime = (Fix)0.001;
                __instance.vibrationStr = (Fix)1.5;
                __instance.groundForce = (Fix)3000;
                __instance.playerPushForce = (Fix)800;
            }
        }
    }
}
