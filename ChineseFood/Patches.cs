using HarmonyLib;
using System.IO;
using System.Reflection;
using CatUtilLib;
using static Localization;

namespace Chinese_Food
{
    public class Patches
    {
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalizationUtil.MakeLocalization(typeof(STRINGS));
            }
        }
    }
}