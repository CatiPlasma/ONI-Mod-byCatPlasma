using HarmonyLib;
using System.IO;
using System.Reflection;
using static Localization;

namespace Chinese_Food
{
    public class Patches
    {
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            static string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Translations");
            
            public static void Postfix()
            {
                RegisterForTranslation(typeof(STRINGS));
                LoadStrings();
                LocString.CreateLocStringKeys(typeof(STRINGS), null);
                GenerateStringsTemplate(typeof(STRINGS), path);
            }

            private static void LoadStrings()
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (File.Exists(Path.Combine(path, $"{GetLocale()?.Code}.po"))) OverloadStrings(LoadStringsFile(path, false));
            }
        }
    }
}