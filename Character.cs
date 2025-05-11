using System;
using System.Collections.Generic;
using System.Linq;

namespace MyDiscordBot
{
    public class Quest
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int ExperienceReward { get; private set; }
        public List<Item> ItemRewards { get; private set; }
        public bool IsCompleted { get; private set; }
        public QuestType Type { get; private set; }
        public int TargetCount { get; private set; }
        public int CurrentCount { get; private set; }

        public Quest(string title, string description, int expReward, QuestType type, int targetCount)
        {
            Title = title;
            Description = description;
            ExperienceReward = expReward;
            ItemRewards = new List<Item>();
            IsCompleted = false;
            Type = type;
            TargetCount = targetCount;
            CurrentCount = 0;
        }

        public void AddItemReward(Item item)
        {
            ItemRewards.Add(item);
        }

        public void UpdateProgress(int amount = 1)
        {
            if (!IsCompleted)
            {
                CurrentCount = Math.Min(CurrentCount + amount, TargetCount);
                if (CurrentCount >= TargetCount)
                {
                    IsCompleted = true;
                }
            }
        }
    }

    public enum QuestType
    {
        KillEnemies,
        CollectItems,
        TalkToNPCs,
        ExploreAreas
    }

    public class Character
    {
        public string Name { get; private set; }
        public Stats Stats { get; private set; }
        public Inventory Inventory { get; private set; }
        public Equipment Equipment { get; private set; }
        public float Health { get; private set; }
        public float Stamina { get; private set; }
        public float Hunger { get; private set; }
        public float Thirst { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public Vector2Int Position { get; private set; }
        public Dictionary<EquipmentSlot, EquipmentItem> EquippedItems { get; private set; }
        public List<Item> NearbyItems { get; private set; }
        public List<Character> NearbyNPCs { get; private set; }
        public List<Character> NearbyEnemies { get; private set; }
        public List<Quest> ActiveQuests { get; private set; }
        public List<Quest> CompletedQuests { get; private set; }
        public Dictionary<SkillType, int> Skills { get; private set; }
        public Dictionary<SkillType, int> SkillExperience { get; private set; }
        public int Gold { get; private set; }

        public Character(string name)
        {
            Name = name;
            Stats = new Stats();
            Inventory = new Inventory(100); // Max weight of 100
            Equipment = new Equipment();
            Health = 100;
            Stamina = 100;
            Hunger = 100;
            Thirst = 100;
            Level = 1;
            Experience = 0;
            Position = new Vector2Int(0, 0);
            EquippedItems = new Dictionary<EquipmentSlot, EquipmentItem>();
            NearbyItems = new List<Item>();
            NearbyNPCs = new List<Character>();
            NearbyEnemies = new List<Character>();
            ActiveQuests = new List<Quest>();
            CompletedQuests = new List<Quest>();
            Skills = new Dictionary<SkillType, int>();
            SkillExperience = new Dictionary<SkillType, int>();
            
            // Initialize all skills at level 1
            foreach (SkillType skill in Enum.GetValues(typeof(SkillType)))
            {
                Skills[skill] = 1;
                SkillExperience[skill] = 0;
            }

            // Give starting items
            Inventory.Add(new ConsumableItem("Health Potion", "Restores 25 health", 0.5f, 10, 25, 0, 0, 0));
            Inventory.Add(new Weapon("Rusty Knife", "A basic weapon", 1.0f, 5, 10, 100, WeaponType.Melee));
            Gold = 100; // Starting gold
        }

        public void Update()
        {
            Stats.Update();
            UpdateNeeds();
        }

        private void UpdateNeeds()
        {
            // Decrease needs over time
            Hunger = Math.Max(0, Hunger - 0.1f);
            Thirst = Math.Max(0, Thirst - 0.2f);

            // If needs are too low, take damage
            if (Hunger <= 0)
            {
                TakeDamage(0.5f);
            }
            if (Thirst <= 0)
            {
                TakeDamage(1.0f);
            }
        }

        public void TakeDamage(float damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public void Heal(float amount)
        {
            Health = Math.Min(100, Health + amount);
        }

        public void RestoreStamina(float amount)
        {
            Stamina = Math.Min(100, Stamina + amount);
        }

        public void Eat(float amount)
        {
            Hunger = Math.Min(100, Hunger + amount);
        }

        public void Drink(float amount)
        {
            Thirst = Math.Min(100, Thirst + amount);
        }

        public bool UseStamina(float amount)
        {
            if (Stamina >= amount)
            {
                Stamina -= amount;
                return true;
            }
            return false;
        }

        public void AddExperience(int amount)
        {
            Experience += amount;
            while (Experience >= Level * 100)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            Health = 100;
            Stamina = 100;
            Experience -= (Level - 1) * 100;
        }

        public string ShowInventory()
        {
            if (Inventory.Items.Count == 0)
            {
                return "Your inventory is empty.";
            }

            var inventoryText = "**Inventory:**\n";
            foreach (var item in Inventory.Items)
            {
                inventoryText += $"• {item.Name} - {item.Description}\n";
            }

            inventoryText += "\n**Equipped Items:**\n";
            foreach (var slot in EquippedItems)
            {
                inventoryText += $"• {slot.Key}: {slot.Value.Name}\n";
            }

            return inventoryText;
        }

        public string ShowQuests()
        {
            if (ActiveQuests.Count == 0 && CompletedQuests.Count == 0)
            {
                return "You have no quests.";
            }

            var response = "📜 **Your Quests:**\n\n";
            
            if (ActiveQuests.Count > 0)
            {
                response += "**Active Quests:**\n";
                foreach (var quest in ActiveQuests)
                {
                    response += $"• {quest.Title}\n";
                    response += $"  {quest.Description}\n";
                    response += $"  Progress: {quest.CurrentCount}/{quest.TargetCount}\n";
                    response += $"  Reward: {quest.ExperienceReward} XP";
                    if (quest.ItemRewards.Count > 0)
                    {
                        response += $" + {string.Join(", ", quest.ItemRewards.Select(i => i.Name))}";
                    }
                    response += "\n\n";
                }
            }

            if (CompletedQuests.Count > 0)
            {
                response += "**Completed Quests:**\n";
                foreach (var quest in CompletedQuests)
                {
                    response += $"• {quest.Title} ✓\n";
                }
            }

            return response;
        }

        public string AcceptQuest()
        {
            if (ActiveQuests.Count >= 3)
            {
                return "❌ You can only have 3 active quests at a time.";
            }

            var random = new Random();
            var questTypes = new[] { QuestType.KillEnemies, QuestType.CollectItems, QuestType.TalkToNPCs, QuestType.ExploreAreas };
            var type = questTypes[random.Next(questTypes.Length)];

            Quest newQuest = type switch
            {
                QuestType.KillEnemies => new Quest(
                    "Bandit Hunt",
                    "Defeat bandits terrorizing the area",
                    100,
                    QuestType.KillEnemies,
                    3
                ),
                QuestType.CollectItems => new Quest(
                    "Herb Gathering",
                    "Collect healing herbs for the village healer",
                    75,
                    QuestType.CollectItems,
                    5
                ),
                QuestType.TalkToNPCs => new Quest(
                    "Village Survey",
                    "Talk to villagers about recent events",
                    50,
                    QuestType.TalkToNPCs,
                    3
                ),
                QuestType.ExploreAreas => new Quest(
                    "Area Exploration",
                    "Explore new areas of the map",
                    150,
                    QuestType.ExploreAreas,
                    4
                ),
                _ => throw new ArgumentException("Invalid quest type")
            };

            // Add some random item rewards
            if (random.Next(100) < 50)
            {
                newQuest.AddItemReward(new ConsumableItem("Health Potion", "Restores 25 health", 0.5f, 10, 25, 0, 0, 0));
            }

            ActiveQuests.Add(newQuest);
            return $"📜 New quest accepted: {newQuest.Title}\n{newQuest.Description}";
        }

        public void UpdateQuestProgress(QuestType type, int amount = 1)
        {
            foreach (var quest in ActiveQuests.Where(q => q.Type == type && !q.IsCompleted))
            {
                quest.UpdateProgress(amount);
                if (quest.IsCompleted)
                {
                    CompleteQuest(quest);
                }
            }
        }

        private void CompleteQuest(Quest quest)
        {
            ActiveQuests.Remove(quest);
            CompletedQuests.Add(quest);
            AddExperience(quest.ExperienceReward);
            
            foreach (var item in quest.ItemRewards)
            {
                Inventory.Add(item);
            }
        }

        public string Move(string direction)
        {
            if (!UseStamina(5))
            {
                return "❌ You're too tired to move!";
            }

            Vector2Int newPosition = Position;
            switch (direction.ToLower())
            {
                case "north":
                    newPosition.Y += 1;
                    break;
                case "south":
                    newPosition.Y -= 1;
                    break;
                case "east":
                    newPosition.X += 1;
                    break;
                case "west":
                    newPosition.X -= 1;
                    break;
                default:
                    return "❌ Invalid direction! Use north, south, east, or west.";
            }

            Position = newPosition;
            UpdateNearbyEntities();
            UpdateQuestProgress(QuestType.ExploreAreas);
            return $"🗺️ You moved {direction}. {LookAround()}";
        }

        public string LookAround()
        {
            var response = "👀 You look around:\n";
            
            if (NearbyItems.Count > 0)
            {
                response += "\n📦 Items nearby:\n";
                foreach (var item in NearbyItems)
                {
                    response += $"• {item.Name} - {item.Description}\n";
                }
            }

            if (NearbyNPCs.Count > 0)
            {
                response += "\n👥 NPCs nearby:\n";
                foreach (var npc in NearbyNPCs)
                {
                    response += $"• {npc.Name}\n";
                }
            }

            if (NearbyEnemies.Count > 0)
            {
                response += "\n⚔️ Enemies nearby:\n";
                foreach (var enemy in NearbyEnemies)
                {
                    response += $"• {enemy.Name} (Health: {enemy.Health})\n";
                }
            }

            if (NearbyItems.Count == 0 && NearbyNPCs.Count == 0 && NearbyEnemies.Count == 0)
            {
                response += "\nThe area appears to be empty.";
            }

            return response;
        }

        public string TakeItem(string itemName)
        {
            var item = NearbyItems.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                return "❌ That item is not here.";
            }

            if (Inventory.Add(item))
            {
                NearbyItems.Remove(item);
                UpdateQuestProgress(QuestType.CollectItems);
                return $"📥 You picked up {item.Name}.";
            }
            else
            {
                return "❌ Your inventory is too heavy to carry that item.";
            }
        }

        public string Attack(string targetName)
        {
            var target = NearbyEnemies.FirstOrDefault(e => e.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));
            if (target == null)
            {
                return "❌ That target is not here.";
            }

            if (!UseStamina(10))
            {
                return "❌ You're too tired to attack!";
            }

            float damage = 5; // Base damage
            if (EquippedItems.TryGetValue(EquipmentSlot.Weapon, out var equippedItem) && equippedItem is Weapon weapon)
            {
                damage = weapon.Damage;
            }

            target.TakeDamage(damage);
            string response = $"⚔️ You attack {target.Name} for {damage} damage!";

            if (target.Health <= 0)
            {
                NearbyEnemies.Remove(target);
                AddExperience(50);
                UpdateQuestProgress(QuestType.KillEnemies);
                response += $"\n🎯 You defeated {target.Name}!\n✨ Gained 50 experience!";
            }
            else
            {
                response += $"\n{target.Name} has {target.Health} health remaining.";
            }

            return response;
        }

        public string TalkTo(string npcName)
        {
            var npc = NearbyNPCs.FirstOrDefault(n => n.Name.Equals(npcName, StringComparison.OrdinalIgnoreCase));
            if (npc == null)
            {
                return "❌ That person is not here.";
            }

            UpdateQuestProgress(QuestType.TalkToNPCs);
            return $"🗣️ {npc.Name}: Hello, {Name}! How can I help you today?";
        }

        private void UpdateNearbyEntities()
        {
            // In a real game, this would check the game world for entities near the player's position
            // For now, we'll just simulate some random entities
            var random = new Random();
            NearbyItems.Clear();
            NearbyNPCs.Clear();
            NearbyEnemies.Clear();

            if (random.Next(100) < 30)
            {
                NearbyItems.Add(new ConsumableItem("Health Potion", "Restores 25 health", 0.5f, 10, 25, 0, 0, 0));
            }

            if (random.Next(100) < 20)
            {
                var npc = new Character("Villager");
                npc.Health = 50;
                NearbyNPCs.Add(npc);
            }

            if (random.Next(100) < 15)
            {
                var enemy = new Character("Bandit");
                enemy.Health = 75;
                NearbyEnemies.Add(enemy);
            }
        }

        public void AddSkillExperience(SkillType skill, int amount)
        {
            SkillExperience[skill] += amount;
            while (SkillExperience[skill] >= Skills[skill] * 100)
            {
                Skills[skill]++;
                SkillExperience[skill] -= (Skills[skill] - 1) * 100;
            }
        }

        public string ShowSkills()
        {
            var response = "🎯 **Your Skills:**\n";
            foreach (var skill in Skills)
            {
                response += $"• {skill.Key}: Level {skill.Value} ({SkillExperience[skill.Value]} XP)\n";
            }
            return response;
        }

        public string Craft(string itemName)
        {
            // Check if player has required crafting level
            if (Skills[SkillType.Crafting] < 2)
            {
                return "❌ You need at least level 2 in Crafting to create items.";
            }

            // Define recipes
            var recipes = new Dictionary<string, (string[] materials, Item result)>
            {
                ["health potion"] = (
                    new[] { "herb", "water" },
                    new ConsumableItem("Health Potion", "Restores 25 health", 0.5f, 10, 25, 0, 0, 0)
                ),
                ["wooden sword"] = (
                    new[] { "wood", "string" },
                    new Weapon("Wooden Sword", "A basic training weapon", 2.0f, 15, 8, 50, WeaponType.Melee)
                )
            };

            if (!recipes.TryGetValue(itemName.ToLower(), out var recipe))
            {
                return "❌ Unknown recipe.";
            }

            // Check if player has all required materials
            foreach (var material in recipe.materials)
            {
                if (!Inventory.Items.Any(i => i.Name.ToLower() == material))
                {
                    return $"❌ You need {material} to craft this item.";
                }
            }

            // Remove materials and add crafted item
            foreach (var material in recipe.materials)
            {
                var item = Inventory.Items.First(i => i.Name.ToLower() == material);
                Inventory.Remove(item);
            }

            Inventory.Add(recipe.result);
            AddSkillExperience(SkillType.Crafting, 25);
            return $"✨ Successfully crafted {recipe.result.Name}!";
        }

        public string Buy(string itemName, Character merchant)
        {
            if (merchant == null || !NearbyNPCs.Contains(merchant))
            {
                return "❌ There is no merchant nearby.";
            }

            var item = merchant.Inventory.Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                return "❌ The merchant doesn't have that item.";
            }

            if (Gold < item.Value)
            {
                return $"❌ You need {item.Value} gold to buy this item.";
            }

            Gold -= item.Value;
            merchant.Gold += item.Value;
            merchant.Inventory.Remove(item);
            Inventory.Add(item);
            AddSkillExperience(SkillType.Trading, 10);
            return $"💰 You bought {item.Name} for {item.Value} gold.";
        }

        public string Sell(string itemName, Character merchant)
        {
            if (merchant == null || !NearbyNPCs.Contains(merchant))
            {
                return "❌ There is no merchant nearby.";
            }

            var item = Inventory.Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                return "❌ You don't have that item.";
            }

            int sellValue = (int)(item.Value * 0.7f); // 70% of original value
            if (merchant.Gold < sellValue)
            {
                return "❌ The merchant doesn't have enough gold.";
            }

            Gold += sellValue;
            merchant.Gold -= sellValue;
            Inventory.Remove(item);
            merchant.Inventory.Add(item);
            AddSkillExperience(SkillType.Trading, 5);
            return $"💰 You sold {item.Name} for {sellValue} gold.";
        }
    }

    public enum SkillType
    {
        Combat,
        Stealth,
        Medicine,
        Crafting,
        Trading,
        Survival
    }
} 