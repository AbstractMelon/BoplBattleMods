using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace SlightCooldowns
{
    [BepInPlugin("com.Melon.SlightCooldowns", "SlightCooldowns", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Plugin SlightCooldowns is loaded!");

            Harmony harmony = new Harmony("com.Melon.SlightCooldowns");


            MethodInfo original = AccessTools.Method(typeof(Ability), "Awake");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "Awake_SlightCooldowns_Plug");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        public class myPatches
        {
            public static void Awake_SlightCooldowns_Plug(Ability __instance)
            {
                __instance.Cooldown = (Fix)1;
            }
        }
    }
}
