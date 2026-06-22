using System.Collections.Generic;
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
            if (CatUtils.IsModLoaded("MapOverlay"))
            {
                var type = AccessTools.TypeByName("MapOverlay.MapOverlay");
                if (type == null) return;
                
                var method = AccessTools.Method(type, "UpdateMapEntry", new []{typeof(GameObject), typeof(object)});
                if (method == null) return;

                harmony.Patch(
                    method,
                    new HarmonyMethod(
                        typeof(Patches.CompatibilityPatches), 
                        nameof(Patches.CompatibilityPatches.MapOverlayCompatibility)));
                
                LogUtil.Info("MapOverlay Compatibility Patched");
            }
            LogUtil.Info("Compatibility Process Finished");
        }

        public static bool IsDebug = false;
    }
}