using System.Collections.Generic;
using CatUtilLib;
using HarmonyLib;
using STRINGS;
using TUNING;
using UnityEngine;
using PeterHan.PLib;

namespace Chinese_Food.ItemConfigs.Foods
{
    public class SweetAndSourRibsConfig: IEntityConfig
    {
        public GameObject CreatePrefab()
        {
            var gameObject = EntityTemplates.ExtendEntityToFood(
                EntityTemplates.CreateLooseEntity(
                    ItemData.SweetAndSourRibs.ID,
                    Strings.Get(ItemData.SweetAndSourRibs.NameKey),
                    Strings.Get(ItemData.SweetAndSourRibs.DescKey),
                    ItemData.SweetAndSourRibs.Mass,
                    ItemData.SweetAndSourRibs.UnitMass,
                    ItemData.SweetAndSourRibs.Anim,
                    ItemData.SweetAndSourRibs.InitialAnim,
                    ItemData.SweetAndSourRibs.Layer,
                    ItemData.SweetAndSourRibs.Shape,
                    ItemData.SweetAndSourRibs.Width,
                    ItemData.SweetAndSourRibs.Height,
                    ItemData.SweetAndSourRibs.IsPickupable,
                    ItemData.SweetAndSourRibs.SortOrder,
                    ItemData.SweetAndSourRibs.Element
                ), CatUtils.IsModLoaded("Ronivan.DupesCuisine")?ItemData.SweetAndSourRibs.FoodInfo.AddEffects(new List<string>{"DupesCuisineSugarRush"}):ItemData.SweetAndSourRibs.FoodInfo
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
                        ItemData.SweetAndSourRibs.CookingStationInput,
                        ItemData.SweetAndSourRibs.CookingStationOutput
                    ),
                    ItemData.SweetAndSourRibs.CookingStationInput,
                    ItemData.SweetAndSourRibs.CookingStationOutput)
                {
                    time = 100f,
                    description = Strings.Get(ItemData.SweetAndSourRibs.CookingStationRecipeDescKey),
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
                    ItemData.SweetAndSourRibs.NameKey,
                    UI.FormatAsLink(ItemData.SweetAndSourRibs.Name, ItemData.SweetAndSourRibs.ID)
                );
                Strings.Add(
                    ItemData.SweetAndSourRibs.DescKey,
                    ItemData.SweetAndSourRibs.Desc
                );
                Strings.Add(
                    ItemData.SweetAndSourRibs.CookingStationRecipeDescKey,
                    ItemData.SweetAndSourRibs.CookingStationRecipeDesc
                );
            }
        }
    }
}