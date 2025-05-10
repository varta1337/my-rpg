namespace MyDiscordBot
{
    public class Stats
    {
        public int Strength { get; private set; }
        public int Dexterity { get; private set; }
        public int Intelligence { get; private set; }
        public int Vitality { get; private set; }
        public int Agility { get; private set; }
        public Dictionary<string, int> Skills { get; set; }

        public Stats()
        {
            Strength = 10;
            Dexterity = 10;
            Intelligence = 10;
            Vitality = 10;
            Agility = 10;
            Skills = new Dictionary<string, int>();
        }

        public void Update()
        {
            // Update stats based on equipment, buffs, etc.
        }

        public void IncreaseStat(string statName, int amount)
        {
            switch (statName.ToLower())
            {
                case "strength":
                    Strength += amount;
                    break;
                case "dexterity":
                    Dexterity += amount;
                    break;
                case "intelligence":
                    Intelligence += amount;
                    break;
                case "vitality":
                    Vitality += amount;
                    break;
                case "agility":
                    Agility += amount;
                    break;
            }
        }

        public override string ToString()
        {
            return $"Strength: {Strength}\n" +
                   $"Dexterity: {Dexterity}\n" +
                   $"Intelligence: {Intelligence}\n" +
                   $"Vitality: {Vitality}\n" +
                   $"Agility: {Agility}";
        }
    }
} 