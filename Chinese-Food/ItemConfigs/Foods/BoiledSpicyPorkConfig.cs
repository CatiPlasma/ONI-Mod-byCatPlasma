using System.Collections.Generic;
using CatUtilLib;
using HarmonyLib;
using STRINGS;
using TUNING;
using UnityEngine;
using PeterHan.PLib;

namespace Chinese_Food.ItemConfigs.Foods
{
    public class BoiledSpicyPorkConfig: IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            var gameObject = EntityTemplates.ExtendEntityToFood(
                EntityTemplates.CreateLooseEntity(
                    ItemData.BoiledSpicyPork.ID,
                    Strings.Get(ItemData.BoiledSpicyPork.NameKey),
                    Strings.Get(ItemData.BoiledSpicyPork.DescKey),
                    ItemData.BoiledSpicyPork.Mass,
                    ItemData.BoiledSpicyPork.UnitMass,
                    ItemData.BoiledSpicyPork.Anim,
                    ItemData.BoiledSpicyPork.InitialAnim,
                    ItemData.BoiledSpicyPork.Layer,
                    ItemData.BoiledSpicyPork.Shape,
                    ItemData.BoiledSpicyPork.Width,
                    ItemData.BoiledSpicyPork.Height,
                    ItemData.BoiledSpicyPork.IsPickupable,
                    ItemData.BoiledSpicyPork.SortOrder,
                    ItemData.BoiledSpicyPork.Element
                ), ItemData.BoiledSpicyPork.FoodInfo
            );
            return gameObject;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        public static ComplexRecipe CookingStationRecipe;
        
        [HarmonyPatch(typeof(CookingStationConfig), "ConfigureRecipes")]
        public static class AddRecipe
        {
            public static void Postfix()
            {
                CookingStationRecipe = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(
                        "CookingStation",
                        ItemData.BoiledSpicyPork.CookingStationInput,
                        ItemData.BoiledSpicyPork.CookingStationOutput
                    ),
                    ItemData.BoiledSpicyPork.CookingStationInput,
                    ItemData.BoiledSpicyPork.CookingStationOutput)
                {
                    time = 100f,
                    description = Strings.Get(ItemData.BoiledSpicyPork.CookingStationRecipeDescKey),
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                    fabricators = new List<Tag>
                    {
                        "CookingStation"
                    },
                    sortOrder = 23
                };
            }
        }
        
        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Locale
        {
            public static void Prefix()
            {
                Strings.Add(
                    ItemData.BoiledSpicyPork.NameKey,
                    UI.FormatAsLink(ItemData.BoiledSpicyPork.Name, ItemData.BoiledSpicyPork.ID)
                );
                Strings.Add(
                    ItemData.BoiledSpicyPork.DescKey,
                    ItemData.BoiledSpicyPork.Desc
                );
                Strings.Add(
                    ItemData.BoiledSpicyPork.CookingStationRecipeDescKey,
                    ItemData.BoiledSpicyPork.CookingStationRecipeDesc
                );
            }
        }
    }
}