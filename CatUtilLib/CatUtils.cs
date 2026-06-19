using System.Linq;
using UnityEngine;

namespace CatUtilLib
{
    public static class CatUtils
    {
        public static bool IsModLoaded(string modId)
        {
            return Global.Instance.modManager.mods.Any(mod => mod.staticID == modId && mod.IsActive());
        }
        
        public static float Roll(GameObject gameObject, float min, float max)
        {
            if (SaveLoader.Instance == null || SaveLoader.Instance.clusterDetailSave == null) return -1f;
            KRandom randomSource =  new KRandom(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)gameObject.transform.GetPosition().x + (int)gameObject.transform.GetPosition().y);
            return (float)(randomSource.NextDouble() * (max - min)) + min;
        }
    }
}