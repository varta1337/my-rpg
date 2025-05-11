using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyDiscordBot;
using System.Linq;

namespace MyDiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private Dictionary<ulong, Character> _playerCharacters;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            _client = new DiscordSocketClient(config);
            _playerCharacters = new Dictionary<ulong, Character>();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            // Get token from environment variable
            string token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Error: Discord bot token not found. Please set the DISCORD_BOT_TOKEN environment variable.");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Keep the bot running
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            if (message.Content.StartsWith("!"))
            {
                string command = message.Content.Substring(1).ToLower();
                string response = "";

                switch (command)
                {
                    case "hello":
                        response = $"Hello {message.Author.Username}!";
                        break;

                    case "help":
                        response = "🎮 !start - Start your RPG adventure\n" +
                                 "📊 !status - Check your character's stats and inventory\n" +
                                 "👀 !look - Look around your current location\n" +
                                 "🎒 !inventory - Check your inventory\n" +
                                 "🗺️ !move [direction] - Move in a direction (north/south/east/west)\n" +
                                 "🗣️ !talk [npc name] - Talk to nearby characters\n" +
                                 "📥 !take [item name] - Pick up items\n" +
                                 "⚔️ !attack [enemy name] - Attack a target\n" +
                                 "📜 !quests - Show your active and completed quests\n" +
                                 "📜 !quest - Accept a new quest\n" +
                                 "❓ !help - Show this help message";
                        break;

                    case "start":
                        if (!_playerCharacters.ContainsKey(message.Author.Id))
                        {
                            var character = new Character(message.Author.Username);
                            _playerCharacters[message.Author.Id] = character;
                            response = $"🎮 Welcome to the RPG, {message.Author.Username}! Your character has been created.";
                        }
                        else
                        {
                            response = "❌ You already have a character!";
                        }
                        break;

                    case "status":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character player))
                        {
                            response = $"📊 Character Status:\n" +
                                     $"❤️ Health: {player.Health}\n" +
                                     $"⚡ Stamina: {player.Stamina}\n" +
                                     $"🍖 Hunger: {player.Hunger}\n" +
                                     $"💧 Thirst: {player.Thirst}\n" +
                                     $"📈 Level: {player.Level}\n" +
                                     $"✨ Experience: {player.Experience}";
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "look":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character looker))
                        {
                            response = looker.LookAround();
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "inventory":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character invCharacter))
                        {
                            response = invCharacter.ShowInventory();
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "move":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character mover))
                        {
                            string[] parts = message.Content.Split(' ');
                            if (parts.Length < 2)
                            {
                                response = "❌ Please specify a direction: north, south, east, or west";
                            }
                            else
                            {
                                response = mover.Move(parts[1]);
                            }
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "talk":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character talker))
                        {
                            string[] parts = message.Content.Split(' ');
                            if (parts.Length < 2)
                            {
                                response = "❌ Please specify who to talk to.";
                            }
                            else
                            {
                                string npcName = string.Join(" ", parts.Skip(1));
                                response = talker.TalkTo(npcName);
                            }
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "take":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character taker))
                        {
                            string[] parts = message.Content.Split(' ');
                            if (parts.Length < 2)
                            {
                                response = "❌ Please specify an item to take.";
                            }
                            else
                            {
                                string itemName = string.Join(" ", parts.Skip(1));
                                response = taker.TakeItem(itemName);
                            }
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "attack":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character attacker))
                        {
                            string[] parts = message.Content.Split(' ');
                            if (parts.Length < 2)
                            {
                                response = "❌ Please specify a target to attack.";
                            }
                            else
                            {
                                string targetName = string.Join(" ", parts.Skip(1));
                                response = attacker.Attack(targetName);
                            }
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "quests":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character questCharacter))
                        {
                            response = questCharacter.ShowQuests();
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "quest":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character questAccepter))
                        {
                            response = questAccepter.AcceptQuest();
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "skills":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character skiller))
                        {
                            response = skiller.ShowSkills();
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "craft":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character crafter))
                        {
                            string[] parts = message.Content.Split(' ');
                            if (parts.Length < 2)
                            {
                                response = "❌ Please specify what to craft.";
                            }
                            else
                            {
                                string itemName = string.Join(" ", parts.Skip(1));
                                response = crafter.Craft(itemName);
                            }
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "buy":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character buyer))
                        {
                            string[] parts = message.Content.Split(' ');
                            if (parts.Length < 2)
                            {
                                response = "❌ Please specify what to buy.";
                            }
                            else
                            {
                                string itemName = string.Join(" ", parts.Skip(1));
                                var merchant = buyer.NearbyNPCs.FirstOrDefault(n => n.Name == "Merchant");
                                response = buyer.Buy(itemName, merchant);
                            }
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    case "sell":
                        if (_playerCharacters.TryGetValue(message.Author.Id, out Character seller))
                        {
                            string[] parts = message.Content.Split(' ');
                            if (parts.Length < 2)
                            {
                                response = "❌ Please specify what to sell.";
                            }
                            else
                            {
                                string itemName = string.Join(" ", parts.Skip(1));
                                var merchant = seller.NearbyNPCs.FirstOrDefault(n => n.Name == "Merchant");
                                response = seller.Sell(itemName, merchant);
                            }
                        }
                        else
                        {
                            response = "❌ You haven't created a character yet! Use !start to begin.";
                        }
                        break;

                    default:
                        response = "❓ Unknown command. Type !help for available commands.";
                        break;
                }

                await message.Channel.SendMessageAsync(response);
            }
        }
    }
}
