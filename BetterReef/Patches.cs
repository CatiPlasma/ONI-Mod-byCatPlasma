using System;
using System.Collections.Generic;
using CatUtilLib;
using HarmonyLib;
using KMod;
using UnityEngine;

namespace BetterReef
{
    public class Patches
    {
        
        [HarmonyPatch(typeof(SmallReefGeyserConfig), nameof(SmallReefGeyserConfig.CreatePrefab))]
        public static class SmallReefGeyserConfig_CreatePrefab_Patch
        {
            public static void Postfix(GameObject __result)
            {
                if (__result == null) return;
                __result.AddOrGet<UserNameable>();
                __result.AddOrGet<ReefGeyserRandomizer>();
            }
        }

        [HarmonyPatch(typeof(ReefGeneratorConfig), nameof(ReefGeneratorConfig.DoPostConfigureComplete))]
        public static class ReefGeneratorConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
            {
                if (go != null) go.AddOrGet<ReefGeneratorMultiplier>();
            }

            [HarmonyPatch(typeof(Tinkerable), "OnCompleteWork")]
            public static class Tinkerable_OnCompleteWork_Patch
            {
                public static void Postfix(Tinkerable __instance)
                {
                    if (__instance.gameObject.GetComponent<ReefGeneratorMultiplier>() != null) __instance.gameObject.GetComponent<ReefGeneratorMultiplier>().TuneupStart();
                }
            }

            [HarmonyPatch(typeof(Tinkerable), "OnEffectRemoved")]
            public static class Tinkerable_OnEffectRemoved_Patch
            {
                public static void Postfix(Tinkerable __instance)
                {
                    if (__instance.gameObject.GetComponent<ReefGeneratorMultiplier>() != null)  __instance.gameObject.GetComponent<ReefGeneratorMultiplier>().TuneupEnd();
                }
            }
        }

        [HarmonyPatch(typeof(UnderwaterVentConfig), nameof(UnderwaterVentConfig.CreatePrefab))]
        public static class UnderwaterVentConfig_CreatePrefab_Patch
        {
            public static void Postfix(GameObject __result)
            {
                if (__result == null) return;
                __result.AddOrGet<UnderwaterVentRandomizer>();
                __result.AddOrGet<UserNameable>();
            }
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalizationUtil.MakeLocalization(typeof(STRINGS));
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public static class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                ReefGeyserRandomizer.RegisterReefGeyserString();
                UnderwaterVentRandomizer.RegisterUnderwaterVentString();
            }
        }

        public static class CompatibilityPatches
        {
            public static void MapOverlayCompatibility(GameObject go, ref object colorReference)
            {
                if (go.GetComponent<ReefGeyserRandomizer>() != null)
                {
                    colorReference = "Thermal Gas Fissure";
                    return;
                }
                if (go.GetComponent<UnderwaterVentRandomizer>() != null) colorReference = "Tidal Spring";
            }
        }
    }
}