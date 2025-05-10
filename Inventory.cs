using System;
using System.Collections.Generic;
using System.Linq;
using MyDiscordBot;

namespace MyDiscordBot
{
    public class Inventory
    {
        public List<Item> Items { get; private set; }
        public float MaxWeight { get; private set; }
        public float CurrentWeight { get; private set; }

        public Inventory(float maxWeight)
        {
            Items = new List<Item>();
            MaxWeight = maxWeight;
            CurrentWeight = 0;
        }

        public bool Add(Item item)
        {
            if (CurrentWeight + item.Weight <= MaxWeight)
            {
                Items.Add(item);
                CurrentWeight += item.Weight;
                return true;
            }
            return false;
        }

        public bool Remove(Item item)
        {
            if (Items.Remove(item))
            {
                CurrentWeight -= item.Weight;
                return true;
            }
            return false;
        }

        public Item Find(string itemName)
        {
            return Items.Find(item => item.Name.ToLower() == itemName.ToLower());
        }

        public int GetItemCount(Item item)
        {
            return Items.Count(i => i.Name == item.Name);
        }

        public bool RemoveItem(Item item)
        {
            var found = Items.FirstOrDefault(i => i.Name == item.Name);
            if (found != null)
            {
                Items.Remove(found);
                CurrentWeight -= found.Weight;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"Inventory ({CurrentWeight}/{MaxWeight} kg):\n" +
                   string.Join("\n", Items.Select(item => $"• {item.Name} ({item.Weight} kg)"));
        }
    }
} 