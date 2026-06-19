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
        public const string ID = "SweetAndSourRibs";
        public static ComplexRecipe CookingStationRecipe;
        
        private GameObject entityTemplates = EntityTemplates.CreateLooseEntity(
            ID,
            STRINGS.FOOD.SWEETSOURRIB.NAME,
            STRINGS.FOOD.SWEETSOURRIB.DESC,
            1f,
            false,
            Assets.GetAnim("food_sweet_sour_ribs_kanim"),
            // Assets.GetAnim("dinobrisket_kanim"),
            "object",
            Grid.SceneLayer.Front,
            EntityTemplates.CollisionShape.RECTANGLE,
            1f,
            0.6f,
            true,
            0
        );
        
        private EdiblesManager.FoodInfo foodInfo = new EdiblesManager.FoodInfo(
                ID,
                2400000f,
                3,
                255.15f,
                277.15f,
                1200f,
                true,
                DlcManager.DLC4
            );
        
        public GameObject CreatePrefab()
        {
            if (CatUtils.IsModLoaded("Ronivan.DupesCuisine")) foodInfo.AddEffects(new List<string> {"DupesCuisineSugarRush"});
            return EntityTemplates.ExtendEntityToFood(entityTemplates, foodInfo);
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
                new ComplexRecipe.RecipeElement("DinosaurMeat", 3f),
                CatUtils.IsModLoaded("RonivansLegacy_ChemicalProcessing")?
                    new ComplexRecipe.RecipeElement(new Tag[]
                    {
                        "Tallow",
                        "RefinedLipid",
                        "PhytoOil",
                        "LiquidVegeOil"
                    }, 2.4f):
                    new ComplexRecipe.RecipeElement(new Tag[]
                    {
                        "Tallow",
                        "RefinedLipid",
                        "PhytoOil"
                    }, 2.4f),
                new ComplexRecipe.RecipeElement("Sucrose", 0.36f)
            };

            private static readonly ComplexRecipe.RecipeElement[] Output = {
                new ComplexRecipe.RecipeElement(ID, 3.5f,
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
                    description = STRINGS.FOOD.SWEETSOURRIB.COOKSTATIONRECIEDESC,
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
                StringUtil.RegisterFoodStrings(ID, STRINGS.FOOD.SWEETSOURRIB.NAME,  STRINGS.FOOD.SWEETSOURRIB.DESC);
                StringUtil.RegisterFoodRecipeDescription(ID, new Dictionary<string, string>{ ["CookingStation"]=STRINGS.FOOD.SWEETSOURRIB.COOKSTATIONRECIEDESC});
            }
        }
    }
}