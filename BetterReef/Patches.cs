using CatUtilLib;
using HarmonyLib;
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
                if (__result != null) __result.AddOrGet<ReefGeyserRandomizer>();
            }
        }

        [HarmonyPatch(typeof(ReefGeneratorConfig), nameof(ReefGeneratorConfig.DoPostConfigureComplete))]
        public static class ReefGeneratorConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
            {
                if (go != null) go.AddOrGet<ReefGeneratorMultiplier>();
            }
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

        [HarmonyPatch(typeof(UnderwaterVentConfig), nameof(UnderwaterVentConfig.CreatePrefab))]
        public static class UnderwaterVentConfig_CreatePrefab_Patch
        {
            public static void Postfix(GameObject __result)
            {
                if (__result != null) __result.AddOrGet<UnderwaterVentRandomizer>();
            }
        }
    }
}