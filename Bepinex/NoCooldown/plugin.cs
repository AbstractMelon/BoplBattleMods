using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;

namespace NoCooldowns
{
    [BepInPlugin("com.Melon.NoCooldowns", "No Cooldowns", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("No More Cooldowns");

            Harmony harmony = new Harmony("com.Melon.NoCooldowns");

            MethodInfo Orignal = AccessTools.Method(typeof(Ability), "Awake");
            MethodInfo Patch = AccessTools.Method(typeof(myPatches), "NoMoreCooldowns");
            harmony.Patch(Orignal, new HarmonyMethod(Patch));
        }

        public class myPatches
        {
            public static void NoMoreCooldowns(ref Ability __instance)
            {
                __instance.Cooldown = (Fix)0;
            }
        }
    }
}
