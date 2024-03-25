using Splotch;
using System;
using HarmonyLib;
using JetBrains.Annotations;
using System.Reflection;
using BoplFixedMath;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

namespace TimestopTimer
{
    public class TimestopTimer : SplotchMod
    {
        private static Harmony harmony;

        public override void OnLoad()
        {

            Logger.Log($"Plugin TimestopTimer is loaded!");

            Harmony harmony = new Harmony("com.antimality.TimestopTimer");

            harmony.PatchAll(typeof(Patch));
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
            TimerTextbox.DisposeAll();
        }

        public static string currentScene;

        private void Update()
        {
            // Delete old textboxes on scene change
            if (currentScene != SceneManager.GetActiveScene().name)
            {
                currentScene = SceneManager.GetActiveScene().name;
                TimerTextbox.DisposeAll();
            }
        }
    }

    [HarmonyPatch]
    public class Patch
    {
        /// <summary>
        /// Creates a textbox when a player casts Time Stop
        /// </summary>
        [HarmonyPatch(typeof(CastSpell), nameof(CastSpell.OnEnterAbility))]
        [HarmonyPostfix]
        public static void OnCast(ref GameObject ___spell, ref PlayerInfo ___playerInfo)
        {
            // Only activate on the intended ability
            if (___spell.name == "TimeStopSphere")
            {
                new TimerTextbox(___playerInfo.playerId);
            }
        }

        /// <summary>
        /// Deletes the textbox when the ability ends (or when it is canceled)
        /// </summary>
        [HarmonyPatch(typeof(CastSpell), nameof(CastSpell.ExitAbility), typeof(AbilityExitInfo))]
        [HarmonyPrefix]
        public static void OnExit(ref PlayerInfo ___playerInfo)
        {
            // Find the matching textbox by player and type
            foreach (TimerTextbox box in TimerTextbox.Textboxes)
            {
                if (box.playerID == ___playerInfo.playerId && !box.casting)
                {
                    box.Dispose();
                    break;
                }
            }
        }

        /// <summary>
        /// Update the textbox during the casting of Time Stop
        /// </summary>
        /// <param name="___castTime">The time required to cast</param>
        /// <param name="___timeSinceActivation">How long the player has been casting</param>
        [HarmonyPatch(typeof(CastSpell), nameof(CastSpell.UpdateSim))]
        [HarmonyPrefix]
        public static void CastingTimeStop(ref Fix ___castTime, ref Fix ___timeSinceActivation, ref PlayerInfo ___playerInfo)
        {
            // Find the matching textbox by player and type
            foreach (TimerTextbox box in TimerTextbox.Textboxes)
            {
                if (box.playerID == ___playerInfo.playerId && !box.casting)
                {
                    // Display time left. The added 1.25s is to accomidate the enter animation keyframes
                    box.Update((float)(___castTime - ___timeSinceActivation) + 1.25f);
                    break;
                }
            }

        }

        /// <summary>
        /// Creates a textbox when time freezes
        /// </summary>
        [HarmonyPatch(typeof(TimeStop), nameof(TimeStop.Init))]
        [HarmonyPostfix]
        public static void StartTimeStop(ref int ___casterId)
        {
            new TimerTextbox(___casterId, casting: true);
        }

        /// <summary>
        /// Deletes the textbox when time unfeezes 
        /// </summary>
        [HarmonyPatch(typeof(TimeStop), nameof(TimeStop.End))]
        [HarmonyPrefix]
        public static void EndTimeStop(ref int ___casterId)
        {
            // Find the matching textbox by player and type
            foreach (TimerTextbox box in TimerTextbox.Textboxes)
            {
                if (box.playerID == ___casterId && box.casting)
                {
                    box.Dispose();
                    break;
                }
            }
        }

        /// <summary>
        /// Update the textbox while time is frozen
        /// </summary>
        /// <param name="___duration">The time required to cast</param>
        /// <param name="___secondsElapsed">How long the player has been casting</param>
        [HarmonyPatch(typeof(TimeStop), nameof(TimeStop.UpdateSim))]
        [HarmonyPrefix]
        public static void DuringTimeStop(ref float ___duration, ref float ___secondsElapsed, ref int ___casterId)
        {
            // Find the matching textbox by player and type
            foreach (TimerTextbox box in TimerTextbox.Textboxes)
            {
                if (box.playerID == ___casterId && box.casting)
                {
                    // Display time left. The added 2s is to accomidate the exit animation keyframes
                    box.Update((float)(___duration - ___secondsElapsed) + 2f);
                    break;
                }
            }
        }
    }

    class TimerTextbox
    {
        // A list of all the textbox instances
        public static List<TimerTextbox> Textboxes = new List<TimerTextbox>();

        /// <summary>
        /// Id of the player using the ability
        /// </summary>
        public readonly int playerID;
        /// <summary>
        /// True: Casting time stop
        /// False: During time freeze
        /// </summary>
        public readonly bool casting;

        private Canvas canvas;
        private GameObject textObj;
        private TextMeshProUGUI textComp;
        private RectTransform location;

        public TimerTextbox(int playerID, bool casting = false)
        {
            Textboxes.Add(this);

            this.playerID = playerID;
            this.casting = casting;

            Summon();
        }

        /// <summary>
        /// Summon the textbox
        /// </summary>
        public void Summon()
        {
            // I don't understand why this is the correct Canvas, but it is
            canvas = GameObject.Find("AbilitySelectCanvas").GetComponent<Canvas>();

            // Only create textboxes on Level scenes
            if (canvas == null || !TimestopTimer.currentScene.Contains("Level"))
            {
                Logger.Error($"No suitable canvas! Canvas: {canvas}, Scene: {TimestopTimer.currentScene}");
                return;
            }

            textObj = new GameObject("TimeStopTimer", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(canvas.transform);

            textComp = textObj.GetComponent<TextMeshProUGUI>();

            // Dunno what this does
            textComp.raycastTarget = false;

            // Color of the casting player
            textComp.color = PlayerHandler.Get().GetPlayer(playerID).Color.GetColor("_ShadowColor");
            textComp.fontSize = 50f;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.font = LocalizedText.localizationTable.GetFont(Settings.Get().Language, false);

            location = textObj.GetComponent<RectTransform>();
            // Moves the refrence point of the textbox to the upper left corner, I think
            location.pivot = new Vector2(0, 1);

            textObj.SetActive(true);
        }

        public void Update(float time)
        {
            // Dispose on 0
            if (time < 1)
            {
                Dispose();
                return;
            }

            // Floor values
            textComp.text = ((int)time).ToString();

            // Height and width of the screen, roughly
            float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
            float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;

            // Ensures textboxes don't overlap
            int offset = (Textboxes.IndexOf(this)) * 100;
            location.anchoredPosition = new Vector2(canvasWidth / 2 - 200, canvasHeight / 2 - 100 - offset);
        }

        public void Dispose()
        {
            Textboxes.Remove(this);
            GameObject.Destroy(textObj);
        }

        /// <summary>
        /// Disposes of all the textboxes
        /// </summary>
        public static void DisposeAll()
        {
            for (int i = Textboxes.Count - 1; i >= 0; i--)
            {
                Textboxes[i].Dispose();
            }
        }
    }
}
