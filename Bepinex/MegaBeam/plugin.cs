using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace BEAM
{
    [BepInPlugin("com.Melon.MegaBeam", "MegaBeam!", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("MegaBeam has loaded!");

            Harmony harmony = new Harmony("com.Melon.MegaBeam");


            MethodInfo original = AccessTools.Method(typeof(Beam), "Awake");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "MegaBeam");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        public class myPatches
        {
            public static void MegaBeam(Beam __instance)
            {
                __instance.angularAimSpeed = (Fix)100f;
                __instance.maxTime = (Fix)10000;
                __instance.maxTimeAir = (Fix)10000;
            }
        }
    }
}
