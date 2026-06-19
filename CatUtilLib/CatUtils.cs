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
        
        public static float[] Roll(GameObject gameObject, float min, float max, int count)
        {
            float[] result = new float[count];
            
            if (SaveLoader.Instance == null || SaveLoader.Instance.clusterDetailSave == null)
            {
                for (int i=0; i<count; i++) result[i] = -1f;
                return result;
            }
            
            KRandom randomSource =  new KRandom(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)gameObject.transform.GetPosition().x + (int)gameObject.transform.GetPosition().y);
            for (int i=0; i<count; i++) result[i] = (float)(randomSource.NextDouble() * (max - min)) + min;
            return result;
        }
        
        public static float Roll(GameObject gameObject, float min, float max)
        {
            if (SaveLoader.Instance == null || SaveLoader.Instance.clusterDetailSave == null) return -1f;
            
            KRandom randomSource =  new KRandom(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)gameObject.transform.GetPosition().x + (int)gameObject.transform.GetPosition().y);
            return (float)(randomSource.NextDouble() * (max - min)) + min;
        }
    }
}