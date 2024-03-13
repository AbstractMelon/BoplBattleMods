using BepInEx;
using UnityEngine;
using HarmonyLib;

namespace MapPicker
{
    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameSession), nameof(GameSession.RandomBagLevel))]
        public static void Fixes(GameSession __instance, ref byte ___currentLevel, ref int __result)
        {
            if (int.TryParse(Plugin.mapIdInput, out int mapId))
            {
                if (mapId >= 0 && mapId <= 62)
                {
                    ___currentLevel = (byte)mapId;
                    __result = ___currentLevel;
                    Debug.Log("setting mapid to " + mapId);
                }
                else
                {
                    Debug.LogWarning("Invalid, do 0-62");
                }
            }
            else
            {
                Debug.LogWarning("Invalid");
            }
        }

    }

    [BepInPlugin("com.Melon_David.MapPicker", "Map Picker", "1.1.0")]
    [BepInProcess("BoplBattle.exe")] //bopl only
    public class Plugin : BaseUnityPlugin
    {
        public static string mapIdInput = "";
        private void Awake()
        {
            Harmony harmony = new("com.Melon_David.MapPicker");
            harmony.PatchAll();
        }
        public string GUIName = "Map Selecter";
        private Rect windowRect = new Rect(60, 20, 180, 70);
        public string GUIText = "Map Selector";
        public bool youturnnedmeon = false;

        void OnGUI()
        {
            GUI.backgroundColor = Color.black;
            GUI.color = Color.blue;
            if (GUI.Button(new Rect(20, 1050, 100, 20), GUIText))
            {
                if (youturnnedmeon == false)
                {
                    GUIText = "Close";
                    youturnnedmeon = true;
                }
                else
                {
                    GUIText = "Select Map";
                    youturnnedmeon = false;
                }
            }
            if (youturnnedmeon)
            {
                windowRect = GUI.Window(10000, windowRect, MainGUI, GUIName);
            }
        }
        public string InputText = "";
        public static int MapId = 1;
        void MainGUI(int windowID)
        {
            GUI.Label(new Rect(35, 40, 180, 70), "By Melon/David");

            GUI.Label(new Rect(10, 20, 100, 20), "Map ID:");
            mapIdInput = GUI.TextField(new Rect(70, 20, 100, 20), mapIdInput);
            GUI.DragWindow();
        }
    }
}
