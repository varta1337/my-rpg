using System.Collections.Generic;
using MyDiscordBot;

namespace MyDiscordBot
{
    public class Equipment
    {
        private Dictionary<EquipmentSlot, EquipmentItem> equippedItems;

        public Equipment()
        {
            equippedItems = new Dictionary<EquipmentSlot, EquipmentItem>();
        }

        public bool Equip(EquipmentItem item)
        {
            if (equippedItems.ContainsKey(item.Slot))
            {
                return false; // Slot already occupied
            }

            equippedItems[item.Slot] = item;
            return true;
        }

        public bool Unequip(EquipmentSlot slot)
        {
            return equippedItems.Remove(slot);
        }

        public EquipmentItem GetEquippedItem(EquipmentSlot slot)
        {
            return equippedItems.TryGetValue(slot, out var item) ? item : null;
        }

        public Dictionary<EquipmentSlot, EquipmentItem> GetAllEquippedItems()
        {
            return new Dictionary<EquipmentSlot, EquipmentItem>(equippedItems);
        }

        public Stats GetEquipmentStats()
        {
            var stats = new Stats();
            foreach (var item in equippedItems.Values)
            {
                foreach (var stat in item.Stats)
                {
                    stats.IncreaseStat(stat.Key, stat.Value);
                }
            }
            return stats;
        }

        public override string ToString()
        {
            var result = "Equipped Items:\n";
            foreach (var kvp in equippedItems)
            {
                result += $"{kvp.Key}: {kvp.Value.Name}\n";
            }
            return result;
        }
    }
} 