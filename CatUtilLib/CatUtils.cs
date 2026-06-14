using System.Linq;
using KMod;

namespace CatUtilLib
{
    public static class CatUtils
    {
        public static bool IsModLoaded(string modId)
        {
            return Global.Instance.modManager.mods.Any(mod => mod.staticID == modId && mod.IsActive());
        }
    }
}