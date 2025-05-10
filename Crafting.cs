using System;
using System.Collections.Generic;
using MyDiscordBot;

namespace MyDiscordBot
{
    public class CraftingRequirement
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }

        public CraftingRequirement(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    public class Recipe
    {
        public Item Result { get; set; }
        public List<CraftingRequirement> Requirements { get; set; }
        public string Description { get; set; }
        public Dictionary<string, int> RequiredSkills { get; set; }

        public Recipe(Item result, List<CraftingRequirement> requirements, string description, Dictionary<string, int> requiredSkills)
        {
            Result = result;
            Requirements = requirements;
            Description = description;
            RequiredSkills = requiredSkills;
        }
    }

    public class Crafting
    {
        private List<Recipe> knownRecipes;
        private Player owner;

        public Crafting(Player owner)
        {
            this.owner = owner;
            knownRecipes = new List<Recipe>();
        }

        public void LearnRecipe(Recipe recipe)
        {
            if (!knownRecipes.Contains(recipe))
            {
                knownRecipes.Add(recipe);
            }
        }

        public Recipe GetRecipe(string itemName)
        {
            return knownRecipes.FirstOrDefault(r => r.Result.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }

        public bool CanCraft(Recipe recipe)
        {
            if (recipe == null) return false;

            // Check required skills
            foreach (var skill in recipe.RequiredSkills)
            {
                if (!owner.Stats.Skills.TryGetValue(skill.Key, out int level) || level < skill.Value)
                {
                    return false;
                }
            }

            // Check required materials
            foreach (var requirement in recipe.Requirements)
            {
                if (owner.Inventory.GetItemCount(requirement.Item) < requirement.Quantity)
                {
                    return false;
                }
            }

            return true;
        }

        public Item CraftItem(Recipe recipe)
        {
            if (!CanCraft(recipe)) return null;

            // Remove materials
            foreach (var requirement in recipe.Requirements)
            {
                for (int i = 0; i < requirement.Quantity; i++)
                {
                    owner.Inventory.RemoveItem(requirement.Item);
                }
            }

            // Create the result
            return recipe.Result;
        }

        public List<Recipe> GetKnownRecipes()
        {
            return knownRecipes.ToList();
        }
    }
} 