using System;
using System.Collections.Generic;
using MyDiscordBot;

namespace MyDiscordBot
{
    public class World
    {
        public Dictionary<Vector2Int, Region> Regions { get; private set; }
        public TimeSystem TimeSystem { get; private set; }
        public WeatherSystem WeatherSystem { get; private set; }
        public List<Faction> Factions { get; private set; }
        public List<Quest> ActiveQuests { get; private set; }

        public World()
        {
            Regions = new Dictionary<Vector2Int, Region>();
            TimeSystem = new TimeSystem();
            WeatherSystem = new WeatherSystem();
            Factions = new List<Faction>();
            ActiveQuests = new List<Quest>();
        }

        public void AddRegion(Region region)
        {
            Regions[region.Position] = region;
        }

        public Region GetRegion(Vector2Int position)
        {
            return Regions.TryGetValue(position, out Region region) ? region : null;
        }

        public void AddFaction(Faction faction)
        {
            Factions.Add(faction);
        }

        public Faction GetFaction(string name)
        {
            return Factions.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void AddQuest(Quest quest)
        {
            ActiveQuests.Add(quest);
        }

        public void RemoveQuest(Quest quest)
        {
            ActiveQuests.Remove(quest);
        }

        public void Update()
        {
            TimeSystem.Update();
            WeatherSystem.Update();

            foreach (var region in Regions.Values)
            {
                region.Update();
            }

            // Update quests
            for (int i = ActiveQuests.Count - 1; i >= 0; i--)
            {
                var quest = ActiveQuests[i];
                quest.UpdateProgress();
                if (quest.Status == QuestStatus.Completed || quest.Status == QuestStatus.Failed)
                {
                    ActiveQuests.RemoveAt(i);
                }
            }
        }

        public void Save()
        {
            // TODO: Implement save functionality
        }

        public void Load()
        {
            // TODO: Implement load functionality
        }
    }
} 