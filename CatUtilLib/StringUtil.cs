using System;
using System.Collections.Generic;

namespace CatUtilLib
{
    public static class StringUtil
    {
        public static void RegisterFoodStrings(string id, string name, string desc)
        {
            Strings.Add($"STRINGS.ITEMS.FOOD.{id.ToUpperInvariant()}.NAME", name);
            Strings.Add($"STRINGS.ITEMS.FOOD.{id.ToUpperInvariant()}.DESC", desc);
        }

        public static void RegisterFoodRecipeDescription(string id, Dictionary<string, string> recipeDescription)
        {
            foreach (string key in recipeDescription.Keys)
            {
                Strings.Add($"STRINGS.ITEMS.FOOD.{id.ToUpperInvariant()}.{key.ToUpperInvariant()}.RECIPEDESC",  recipeDescription[key]);
            }
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