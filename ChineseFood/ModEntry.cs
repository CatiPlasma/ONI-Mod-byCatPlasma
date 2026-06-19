using CatUtilLib;
using HarmonyLib;
using KMod;

namespace Chinese_Food
{
    public class ModEntry: UserMod2
    {
        public static string ModId = "ChineseFood";
        
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            
            LogUtil.Info("Loaded");
        }
    }
}