using BepInEx;
using HarmonyLib;

namespace CustomReplayAmount
{
    [BepInPlugin("com.Melon.CustomReplayAmount", "Custom Replay Amount", "1.0.0")]
    public class CustomReplayAmount : BaseUnityPlugin
    {
        private const int DefaultMaxReplays = 1000;
        private int maxReplays;

        void Awake()
        {
            maxReplays = Config.Bind<int>("General", "MaxReplays", DefaultMaxReplays, "Maximum number of replays").Value;
            Harmony harmony = new Harmony("com.Melon.CustomReplayAmount");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Host), nameof(Host.maxReplays))]
        class Patches
        {
            static void Postfix(ref int __result)
            {
                __result = CustomReplayAmount.DefaultMaxReplays;
            }
        }
    }
}
