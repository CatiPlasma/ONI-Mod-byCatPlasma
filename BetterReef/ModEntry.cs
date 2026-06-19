using System;
using CatUtilLib;
using HarmonyLib;
using KMod;

namespace BetterReef
{
    public class ModEntry: UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            
            LogUtil.Info("Loaded");
        }
    }
}