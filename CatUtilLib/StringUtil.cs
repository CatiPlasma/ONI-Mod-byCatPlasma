using System;

namespace CatUtilLib
{
    public static class StringUtil
    {
        public static string GetFoodNameKey(string id)
        {
            return $"STRINGS.ITEMS.FOOD.{id.ToUpper()}.NAME";
        }
        public static string GetFoodDescKey(string id)
        {
            return $"STRINGS.ITEMS.FOOD.{id.ToUpper()}.DESC";
        }

        public static string GetFoodRecipeDescKey(string id, string cookingStationId = "")
        {
            return $"STRINGS.ITEMS.FOOD.{id.ToUpper()}{(cookingStationId.Length == 0 ? "" : ".")}{cookingStationId.ToUpper()}.RECIPEDESC";
        }

        public static string GetItemNameKey(string id)
        {
            return $"STRINGS.ITEMS.{id.ToUpper()}.NAME";
        }

        public static string GetItemDescKey(string id)
        {
            return $"STRINGS.ITEMS.{id.ToUpper()}.DESC";
        }
    }
}