using System.Collections.Generic;
using CatUtilLib;
using Database;
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
                if (go != null) go.AddOrGet<ReefGeneratorModifier>();
            }

            [HarmonyPatch(typeof(Tinkerable), "OnCompleteWork")]
            public static class Tinkerable_OnCompleteWork_Patch
            {
                public static void Postfix(Tinkerable __instance)
                {
                    if (__instance.gameObject.GetComponent<ReefGeneratorModifier>() != null) __instance.gameObject.GetComponent<ReefGeneratorModifier>().TuneupStart();
                }
            }

            [HarmonyPatch(typeof(Tinkerable), "OnEffectRemoved")]
            public static class Tinkerable_OnEffectRemoved_Patch
            {
                public static void Postfix(Tinkerable __instance)
                {
                    if (__instance.gameObject.GetComponent<ReefGeneratorModifier>() != null)  __instance.gameObject.GetComponent<ReefGeneratorModifier>().TuneupEnd();
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
                __result.GetComponent<InfoDescription>().description = STRINGS.CREATURES.SPECIES.GEYSER.UNDERWATERVENT.DESC;
            }

            [HarmonyPatch(typeof(UnderwaterVent.Def), nameof(UnderwaterVent.Def.GetDescriptors))]
            public static class UnderwaterVent_GetDescriptors_Patch
            {
                public static void Postfix(ref List<Descriptor> __result)
                {
                    __result = new List<Descriptor>();
                }
            }
            
            [HarmonyPatch(typeof(MiscStatusItems), "CreateStatusItems")]
            public static class MiscStatusItems_CreateStatusItems_Patch
            {
                public static void Postfix(MiscStatusItems __instance)
                {
                    UnderwaterVentRandomizer.ModifyVanillaDynamicStrings(__instance);
                }
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
                UnderwaterVentRandomizer.ModifyVanillaStaticStrings();
            }
        }

        public static class CompatibilityPatches
        {
            public static void MapOverlayCompatibility(GameObject go, ref object colorReference)
            {
                if (go.GetComponent<ReefGeyserRandomizer>() != null)
                    colorReference = "Small Reef Geyser";
                else if (go.GetComponent<UnderwaterVentRandomizer>() != null)
                    colorReference = "Tidal Spring";
            }
        }
    }
}