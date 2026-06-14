using System.Collections.Generic;
using HarmonyLib;
using STRINGS;
using TUNING;
using UnityEngine;

namespace Chinese_Food.ItemConfigs
{
    public class TestItemConfig: IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            var gameObject = EntityTemplates.CreateLooseEntity(
                ItemData.TestItem.ID,
                Strings.Get(ItemData.TestItem.NameKey),
                Strings.Get(ItemData.TestItem.DescKey),
                ItemData.TestItem.Mass,
                true,
                ItemData.TestItem.Anim,
                "object",
                ItemData.TestItem.Layer,
                ItemData.TestItem.Shape,
                1f,
                1f,
                true,
                0,
                SimHashes.Creature
            );
            return gameObject;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public static ComplexRecipe Recipe;

        [HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
        public static class AddRecipe
        {
            public static void Posfix()
            {
                Recipe = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(
                        "CookingStation",
                        ItemData.TestItem.Input,
                        ItemData.TestItem.Output
                    ),
                    ItemData.TestItem.Input,
                    ItemData.TestItem.Output)
                {
                    time = 20f,
                    description = Strings.Get(ItemData.TestItem.RecipeDescKey),
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                    fabricators = new List<Tag>
                    {
                        "CraftingTable"
                    },
                    sortOrder = 0
                };
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Locale
        {
            public static void Postfix()
            {
                Strings.Add(ItemData.TestItem.NameKey, UI.FormatAsLink(ItemData.TestItem.NameKey, ItemData.TestItem.ID));
                Strings.Add(ItemData.TestItem.DescKey, ItemData.TestItem.Desc);
            }
        }
    }
}