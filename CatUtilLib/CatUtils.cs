using System;
using System.Linq;
using System.Reflection;
using KMod;
using UnityEngine;

namespace CatUtilLib
{
    public static class CatUtils
    {
        public static bool IsModLoaded(string modId)
        {
            return Global.Instance.modManager.mods.Any(mod => mod.staticID == modId && mod.IsActive());
        }
        
        public static float[] RollFloats(GameObject gameObject, float min, float max, int count)
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
        
        public static float RollFloats(GameObject gameObject, float min, float max)
        {
            return RollFloats(gameObject, min, max, 1)[0];
        }

        public static int[] RollInts(GameObject gameObject, int min, int max, int count)
        {
            int[] result = new int[count];

            if (SaveLoader.Instance == null || SaveLoader.Instance.clusterDetailSave == null)
            {
                for (int i=0; i<count; i++)  result[i] = -1;
                return result;
            }
            
            KRandom randomSource = new KRandom(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)gameObject.transform.GetPosition().x + (int)gameObject.transform.GetPosition().y);
            for (int i = 0; i < count; i++) result[i] = randomSource.Next() * (max - min) + min;
            return result;
        }

        public static int RollInts(GameObject gameObject, int min, int max)
        {
            return RollInts(gameObject, min, max, 1)[0];
        }
        
        public static bool HasWorldTrait(GameObject go, string traitId)
        {
            WorldContainer world = go.GetMyWorld();
            if (world == null) return false;
            return world.WorldTraitIds.Contains(traitId);
        }

        public static bool HasBiome(GameObject go, string biomeId)
        {
            foreach (string biome in go.GetMyWorld().Biomes)
            {
                if (biomeId == biome) return true;
            }

            return false;
        }

        public static void LogWorldTraits(int worldId)
        {
            WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
            if (world == null) return;
            foreach (string id in world.WorldTraitIds) LogUtil.Debug($"WorldTraitId: {id}");
        }

        public static void LogBiomeTraits(int worldId)
        {
            WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
            if (world == null) return;
            foreach (string id in world.Biomes) LogUtil.Debug($"BiomeId: {id}");
        }

        private static bool GetDebugState()
        {
            Type? type = asm.GetTypes().FirstOrDefault(t => typeof(UserMod2).IsAssignableFrom(t));
            if (type == null) return false;
            
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
            FieldInfo? field = type.GetField("IsDebug", flags);
            if (field == null || field.FieldType != typeof(bool)) return false;
            
            return (bool)field.GetValue(null);
        }

        private static readonly Assembly asm = Assembly.GetExecutingAssembly();
        internal static readonly bool isDebug = GetDebugState();
    }
}