using System.Collections.Generic;
using System.Linq;
using CatUtilLib;
using HarmonyLib;
using KMod;
using UnityEngine;

namespace BetterReef
{
    public class ModEntry: UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            
            LogUtil.Info("Loaded");
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
        {
            LogUtil.Info("All Mods Loaded, Start Compatibility Process");

            if (mods.Any(mod => "MapOverlay".Contains(mod.staticID)))
            {
                harmony.Patch(
                    AccessTools.Method(typeof(MapOverlay.MapOverlay), "UpdateMapEntry",
                        new[] { typeof(GameObject), typeof(object) }),
                    prefix: new HarmonyMethod(typeof(Patches.CompatibilityPatches),
                        nameof(Patches.CompatibilityPatches.MapOverlayCompatibility)));
            }
        }
    }
}