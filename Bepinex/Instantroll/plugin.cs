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

            MethodInfo Orignal = AccessTools.Method(typeof(Roll), "Awake");
            MethodInfo Patch = AccessTools.Method(typeof(myPatches), "Start_Rolling");
            harmony.Patch(Orignal, new HarmonyMethod(Patch));
        }

        public class myPatches
        {
            public static void Start_Rolling(ref Roll __instance)
            {
                __instance.chargeTime = (Fix)0;
            }
        }
    }
}
