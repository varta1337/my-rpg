using System;
using System.Collections.Generic;
using MyDiscordBot;

namespace MyDiscordBot
{
    public class Quest
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public QuestStatus Status { get; private set; }
        public List<QuestObjective> Objectives { get; private set; }
        public List<Item> Rewards { get; private set; }
        public int ExperienceReward { get; private set; }
        public Faction Giver { get; private set; }
        public bool IsRepeatable { get; private set; }
        public int LevelRequirement { get; private set; }

        public Quest(string name, string description, Faction giver, bool isRepeatable = false, int levelRequirement = 1)
        {
            Name = name;
            Description = description;
            Status = QuestStatus.NotStarted;
            Objectives = new List<QuestObjective>();
            Rewards = new List<Item>();
            ExperienceReward = 0;
            Giver = giver;
            IsRepeatable = isRepeatable;
            LevelRequirement = levelRequirement;
        }

        public void AddObjective(QuestObjective objective)
        {
            Objectives.Add(objective);
        }

        public void AddReward(Item item)
        {
            Rewards.Add(item);
        }

        public void SetExperienceReward(int amount)
        {
            ExperienceReward = amount;
        }

        public void Start()
        {
            if (Status == QuestStatus.NotStarted)
            {
                Status = QuestStatus.InProgress;
            }
        }

        public void Complete()
        {
            if (Status == QuestStatus.InProgress && AreAllObjectivesComplete())
            {
                Status = QuestStatus.Completed;
            }
        }

        public void Fail()
        {
            if (Status == QuestStatus.InProgress)
            {
                Status = QuestStatus.Failed;
            }
        }

        public bool AreAllObjectivesComplete()
        {
            return Objectives.TrueForAll(o => o.IsComplete);
        }

        public void UpdateProgress()
        {
            if (Status == QuestStatus.InProgress)
            {
                if (AreAllObjectivesComplete())
                {
                    Complete();
                }
            }
        }
    }

    public class QuestObjective
    {
        public string Description { get; private set; }
        public int RequiredAmount { get; private set; }
        public int CurrentAmount { get; private set; }
        public bool IsComplete => CurrentAmount >= RequiredAmount;

        public QuestObjective(string description, int requiredAmount)
        {
            Description = description;
            RequiredAmount = requiredAmount;
            CurrentAmount = 0;
        }

        public void UpdateProgress(int amount)
        {
            CurrentAmount = Math.Min(CurrentAmount + amount, RequiredAmount);
        }

        public void SetProgress(int amount)
        {
            CurrentAmount = Math.Clamp(amount, 0, RequiredAmount);
        }
    }
} 