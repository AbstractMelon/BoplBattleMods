using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace FrictonlessArrows
{
    [BepInPlugin("com.Melon.FrictonlessArrows", "FrictonlessArrows", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic

            Harmony harmony = new Harmony("com.Melon.FrictonlessArrows");


            MethodInfo original = AccessTools.Method(typeof(BowTransform), "Shoot");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "Shoot_bowtrans_myplug");
            harmony.Patch(original, new HarmonyMethod(patch));

            original = AccessTools.Method(typeof(BowTransform), "Awake");
            patch = AccessTools.Method(typeof(myPatches), "Awake_bowtrans_myplug");
            harmony.Patch(original, new HarmonyMethod(patch));
        }
    }

    public class myPatches
    {

        public static bool Shoot_bowtrans_myplug(Vec2 dir, BowTransform __instance, ref Fix ___ArrowSpeed,
            ref PlayerBody ___body, ref RingBuffer<BoplBody> ___Arrows, ref BoplBody ___Arrow, ref bool ___hasFired,
            ref Vec2 ___FirepointOffset, ref Fix ___TimeBeforeArrowsHurtOwner, ref int ___loadingFrame,
            ref PlayerInfo ___playerInfo, ref int ___maxNumberOfArrows)
        {
            // Create a single arrow
            Vec2 pos = ___body.position + ___FirepointOffset.x * ___body.right + ___FirepointOffset.y * ___body.up;
            BoplBody boplBody = FixTransform.InstantiateFixed<BoplBody>(___Arrow, pos, ___body.rotation);

            // Set arrow properties
            boplBody.gravityScale = (Fix)0;
            boplBody.Scale = ___body.fixtrans.Scale;
            ___Arrows.Add(boplBody);
            boplBody.GetComponent<IPlayerIdHolder>().SetPlayerId(___playerInfo.playerId);
            boplBody.GetComponent<SpriteRenderer>().material = ___playerInfo.playerMaterial;

            ___ArrowSpeed = (Fix)30;

            boplBody.StartVelocity = dir * ___ArrowSpeed + ___body.selfImposedVelocity;

            // Enable hurt owner after a certain time
            boplBody.GetComponent<Projectile>().DelayedEnableHurtOwner(___TimeBeforeArrowsHurtOwner);

            // Mark as fired
            ___hasFired = true;

            return false;
        }




        public static bool Awake_bowtrans_myplug(BowTransform __instance, ref int ___maxNumberOfArrows)
        {
            ___maxNumberOfArrows = 5000;

            return true;
        }
    }
}
