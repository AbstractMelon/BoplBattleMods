using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;

namespace InstantRoll
{
    [BepInPlugin("com.Melon.InstantRoll", "Instant Roll", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Instant Roll Rolling");

            Harmony harmony = new Harmony("com.Melon.InstantRoll");

            MethodInfo Orignal = AccessTools.Method(typeof(Mine), "Awake");
            MethodInfo Patch = AccessTools.Method(typeof(myPatches), "Start_Rolling");
            harmony.Patch(Orignal, new HarmonyMethod(Patch));
        }

        public class myPatches
        {
            public static void Start_Rolling(ref Mine __instance)
            {
                __instance.TimeBeforeArmed = (Fix)0;
                __instance.chasingBlinkSpeed = (float)(Fix)0.75;
                __instance.chaseStrength = (Fix)50;
                __instance.chaseTime = (Fix)3;
                __instance.shakeForce = (Fix)25;
            }
        }
    }
}
