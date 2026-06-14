using CatUtilLib;

namespace Chinese_Food
{
    public class ItemData
    {
        private static readonly Grid.SceneLayer DefaultLayer = Grid.SceneLayer.Front;
        private static readonly EntityTemplates.CollisionShape DefaultShape = EntityTemplates.CollisionShape.RECTANGLE;
        private static readonly SimHashes DefaultElement = SimHashes.Creature;
        private static readonly float DefaultPreserveTemperature = 255.15f;
        private static readonly float DefaultRotTemperature = 277.15f;

        public static class TestItem
        {
            public static readonly string
                ID = "CatTestItem",
                NameKey = StringUtil.GetItemNameKey(ID),
                Name = "测试物品",
                DescKey = StringUtil.GetItemDescKey(ID),
                Desc = "测试描述";
            public static readonly float Mass = 1f;
            public static readonly bool UnitMass = true;
            public static readonly KAnimFile Anim = Assets.GetAnim("dinobrisket_kanim");
            public static readonly string InitialAnim = "object";
            public static readonly Grid.SceneLayer Layer = DefaultLayer;
            public static readonly EntityTemplates.CollisionShape Shape = DefaultShape;
            public static readonly float Width = 1f;
            public static readonly float Height = 1f;
            public static readonly bool IsPickupable = true;
            public static readonly int SortOrder = 0;
            public static readonly SimHashes Element = DefaultElement;
            
            public static readonly ComplexRecipe.RecipeElement[] Input = {
                new ComplexRecipe.RecipeElement("Sucrose", 0.2f)
            };

            public static readonly ComplexRecipe.RecipeElement[] Output = {
                new ComplexRecipe.RecipeElement(ID, 3.2f,
                    ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
            };
            
            public static readonly string RecipeDescKey = StringUtil.GetFoodRecipeDescKey(ID, "");
            public static readonly string RecipeDesc = "测试配方描述";
        }
        
        public static class SweetAndSourRibs
        {
            public static readonly string ID = "SweetAndSourRibs";
            public static readonly string NameKey = StringUtil.GetFoodNameKey(ID);
            public static readonly string Name = "糖醋排骨";
            public static readonly string DescKey = StringUtil.GetFoodDescKey(ID);
            public static readonly string Desc = "复制人会得糖尿病吗？";
            public static readonly float Mass = 1f;
            public static readonly bool UnitMass = false;
            public static readonly KAnimFile Anim = Assets.GetAnim("food_sweet_sour_ribs_kanim");
            public static readonly string InitialAnim = "object";
            public static readonly Grid.SceneLayer Layer = DefaultLayer;
            public static readonly EntityTemplates.CollisionShape Shape = DefaultShape;
            public static readonly float Width = 1f;
            public static readonly float Height = 0.6f;
            public static readonly bool IsPickupable = true;
            public static readonly int SortOrder = 0;
            public static readonly SimHashes Element = DefaultElement;
            public static readonly EdiblesManager.FoodInfo FoodInfo = new EdiblesManager.FoodInfo(
                ID,
                2400000f,
                3,
                DefaultPreserveTemperature,
                DefaultRotTemperature,
                1200f,
                true,
                DlcManager.DLC4
                );

            public static readonly ComplexRecipe.RecipeElement[] CookingStationInput = {
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

            public static readonly ComplexRecipe.RecipeElement[] CookingStationOutput = {
                new ComplexRecipe.RecipeElement(ID, 3.5f,
                    ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
            };

            public static readonly string CookingStationRecipeDescKey = StringUtil.GetFoodRecipeDescKey(ID, "");
            public static readonly string CookingStationRecipeDesc = "看起来不好清理……";
        }
        
        public static class BoiledSpicyPork
        {
            public static readonly string ID = "BoiledSpicyPork";
            public static readonly string NameKey = StringUtil.GetFoodNameKey(ID);
            public static readonly string Name = "水煮肉片";
            public static readonly string DescKey = StringUtil.GetFoodDescKey(ID);
            public static readonly string Desc = "嘶哈，好辣！";
            public static readonly float Mass = 1f;
            public static readonly bool UnitMass = false;
            public static readonly KAnimFile Anim = Assets.GetAnim("food_boiled_spicy_pork_kanim");
            public static readonly string InitialAnim = "object";
            public static readonly Grid.SceneLayer Layer = DefaultLayer;
            public static readonly EntityTemplates.CollisionShape Shape = DefaultShape;
            public static readonly float Width = 1f;
            public static readonly float Height = 0.6f;
            public static readonly bool IsPickupable = true;
            public static readonly int SortOrder = 0;
            public static readonly SimHashes Element = DefaultElement;
            public static readonly EdiblesManager.FoodInfo FoodInfo = new EdiblesManager.FoodInfo(
                ID,
                800000f,
                3,
                DefaultPreserveTemperature,
                DefaultRotTemperature,
                4800f,
                true
                );

            public static readonly ComplexRecipe.RecipeElement[] CookingStationInput = {
                new ComplexRecipe.RecipeElement("Meat", 2f),
                new ComplexRecipe.RecipeElement("SpiceNut", 0.5f),
                new ComplexRecipe.RecipeElement("Water", 7.5f)
            };

            public static readonly ComplexRecipe.RecipeElement[] CookingStationOutput = {
                new ComplexRecipe.RecipeElement(ID, 5f,
                    ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
            };

            public static readonly string CookingStationRecipeDescKey = StringUtil.GetFoodRecipeDescKey(ID, "");
            public static readonly string CookingStationRecipeDesc = "汤真的留得住吗";
        }
    }
}