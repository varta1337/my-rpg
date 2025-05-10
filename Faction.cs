using System;
using System.Collections.Generic;
using MyDiscordBot;
using PostApocalypticRPG.Models;

namespace MyDiscordBot
{
    public class Faction
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<Character> Members { get; private set; }
        public List<Quest> AvailableQuests { get; private set; }
        private HashSet<Vector2Int> territories;

        public Faction(string name, string description)
        {
            Name = name;
            Description = description;
            Members = new List<Character>();
            AvailableQuests = new List<Quest>();
            territories = new HashSet<Vector2Int>();
        }

        public void AddTerritory(Vector2Int position)
        {
            territories.Add(position);
        }

        public void RemoveTerritory(Vector2Int position)
        {
            territories.Remove(position);
        }

        public bool OwnsTerritory(Vector2Int position)
        {
            return territories.Contains(position);
        }

        public IReadOnlyList<Vector2Int> GetTerritories()
        {
            return territories.ToList().AsReadOnly();
        }

        public void AddMember(Character character)
        {
            if (!Members.Contains(character))
            {
                Members.Add(character);
            }
        }

        public void RemoveMember(Character character)
        {
            Members.Remove(character);
        }

        public void AddQuest(Quest quest)
        {
            AvailableQuests.Add(quest);
        }

        public void RemoveQuest(Quest quest)
        {
            AvailableQuests.Remove(quest);
        }
    }
} 