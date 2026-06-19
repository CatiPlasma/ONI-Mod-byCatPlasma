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
        public static string ID = "BoiledSpicyPork";
        public static ComplexRecipe CookingStationRecipe;

        private GameObject entityTemplate = EntityTemplates.CreateLooseEntity(
            ID,
            STRINGS.FOOD.BOILSPICEPORK.NAME,
            STRINGS.FOOD.BOILSPICEPORK.DESC,
            1f,
            false,
            Assets.GetAnim("food_boiled_spicy_pork_kanim"),
            "object",
            Grid.SceneLayer.Front,
            EntityTemplates.CollisionShape.RECTANGLE,
            1f,
            0.6f,
            true,
            23
        );
        
        private EdiblesManager.FoodInfo foodInfo = new EdiblesManager.FoodInfo(
            ID,
            800000f,
            3,
            255.15f,
            277.15f,
            4800f,
            true
        ).AddEffects(new List<string>{"HotStuff", "WarmTouchFood"});
        
        public GameObject CreatePrefab()
        {
            return EntityTemplates.ExtendEntityToFood(entityTemplate, foodInfo);
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
        
        [HarmonyPatch(typeof(CookingStationConfig), "ConfigureRecipes")]
        public static class AddRecipe
        {
            private static readonly ComplexRecipe.RecipeElement[] Input = {
                new ComplexRecipe.RecipeElement("Meat", 2f),
                new ComplexRecipe.RecipeElement("SpiceNut", 0.5f),
                new ComplexRecipe.RecipeElement("Water", 7.5f)
            };

            private static readonly ComplexRecipe.RecipeElement[] Output = {
                new ComplexRecipe.RecipeElement(ID, 5f,
                    ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
            };
            
            public static void Postfix()
            {
                CookingStationRecipe = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(
                        "CookingStation",
                        Input,
                        Output
                    ),
                    Input,
                    Output)
                {
                    time = 100f,
                    description = STRINGS.FOOD.BOILSPICEPORK.COOKSTATIONRECIPEDESC,
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
                StringUtil.RegisterFoodStrings(ID, STRINGS.FOOD.BOILSPICEPORK.NAME,  STRINGS.FOOD.BOILSPICEPORK.DESC);
                StringUtil.RegisterFoodRecipeDescription(ID, new Dictionary<string, string>{["CookingStation"]=STRINGS.FOOD.BOILSPICEPORK.COOKSTATIONRECIPEDESC});
            }
        }
    }
}