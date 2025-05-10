using System;
using System.Collections.Generic;

namespace MyDiscordBot
{
    public abstract class Item
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public float Weight { get; protected set; }
        public int Value { get; protected set; }
        public bool IsStackable { get; protected set; }
        public int MaxStack { get; protected set; }

        protected Item(string name, string description, float weight, int value, bool isStackable = false, int maxStack = 1)
        {
            Name = name;
            Description = description;
            Weight = weight;
            Value = value;
            IsStackable = isStackable;
            MaxStack = maxStack;
        }

        public abstract void Use(Character character);
    }

    public class Weapon : Item
    {
        public float Damage { get; private set; }
        public float Durability { get; private set; }
        public WeaponType Type { get; private set; }

        public Weapon(string name, string description, float weight, int value, float damage, float durability, WeaponType type)
            : base(name, description, weight, value)
        {
            Damage = damage;
            Durability = durability;
            Type = type;
        }

        public override void Use(Character character)
        {
            // Weapons are used through combat system
            throw new NotImplementedException("Weapons should be used through the combat system.");
        }

        public void TakeDamage(float amount)
        {
            Durability = Math.Max(0, Durability - amount);
        }

        public void Repair(float amount)
        {
            Durability = Math.Min(100, Durability + amount);
        }
    }

    public class Armor : Item
    {
        public float Defense { get; private set; }
        public float Durability { get; private set; }
        public ArmorType Type { get; private set; }

        public Armor(string name, string description, float weight, int value, float defense, float durability, ArmorType type)
            : base(name, description, weight, value)
        {
            Defense = defense;
            Durability = durability;
            Type = type;
        }

        public override void Use(Character character)
        {
            // Armor is equipped through equipment system
            throw new NotImplementedException("Armor should be equipped through the equipment system.");
        }

        public void TakeDamage(float amount)
        {
            Durability = Math.Max(0, Durability - amount);
        }

        public void Repair(float amount)
        {
            Durability = Math.Min(100, Durability + amount);
        }
    }

    public class ConsumableItem : Item
    {
        public float HealthRestore { get; private set; }
        public float StaminaRestore { get; private set; }
        public float HungerRestore { get; private set; }
        public float ThirstRestore { get; private set; }

        public ConsumableItem(string name, string description, float weight, int value,
            float healthRestore, float staminaRestore, float hungerRestore, float thirstRestore)
            : base(name, description, weight, value, true, 10)
        {
            HealthRestore = healthRestore;
            StaminaRestore = staminaRestore;
            HungerRestore = hungerRestore;
            ThirstRestore = thirstRestore;
        }

        public override void Use(Character character)
        {
            character.Heal(HealthRestore);
            character.RestoreStamina(StaminaRestore);
            character.Eat(HungerRestore);
            character.Drink(ThirstRestore);
        }
    }

    public enum WeaponType
    {
        Melee,
        Ranged,
        Explosive
    }

    public enum ArmorType
    {
        Head,
        Chest,
        Legs,
        Feet,
        Shield
    }

    public class EquipmentItem : Item
    {
        public EquipmentSlot Slot { get; private set; }
        public Dictionary<string, int> Stats { get; private set; }

        public EquipmentItem(string name, string description, float weight, int value, EquipmentSlot slot, Dictionary<string, int> stats)
            : base(name, description, weight, value)
        {
            Slot = slot;
            Stats = stats;
        }

        public override void Use(Character character)
        {
            // Equipment is equipped through equipment system
            throw new NotImplementedException("Equipment should be equipped through the equipment system.");
        }
    }
} 