using System;
using System.IO;
using System.Reflection;

namespace CatUtilLib
{
    public class LocalizationUtil
    {
        public static void MakeLocalization(Type root)
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string transPath = Path.Combine(modPath, "Translations");
            string poPath = Path.Combine(transPath, (Localization.GetLocale()?.Code ?? "en")+".po");
            
            if (!Directory.Exists(transPath)) Directory.CreateDirectory(transPath);
            Localization.RegisterForTranslation(root);
            LocString.CreateLocStringKeys(root, null);
            if (CatUtils.isDebug) Localization.GenerateStringsTemplate(root, transPath);
            if (File.Exists(poPath)) Localization.OverloadStrings(Localization.LoadStringsFile(poPath, false));
        }
    }
}