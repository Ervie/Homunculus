using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes.Caches;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class CommandHandlingService : IDiscordService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
		private readonly JSONSerializerService _jsonSerializer;
        private readonly MessageScannerService _scanner;
        private readonly Random _rng;
        private readonly IServiceProvider _provider;

        public IConfiguration Configuration { get; set; }

        // DiscordSocketClient, CommandService, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandlingService(IConfiguration configuration, DiscordSocketClient discord, CommandService commands, MessageScannerService scanner, Random random, IServiceProvider provider, JSONSerializerService jSONSerializer)
        {
            _discord = discord;
            _commands = commands;
            _scanner = scanner;
            _provider = provider;
			_jsonSerializer = jSONSerializer;
            Configuration = configuration;
            _rng = random;

            SetStartingState();
        }

        public CommandService Commands => _commands;
        

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
			if (!(s is SocketUserMessage msg)) return;
			if (msg.Author == _discord.CurrentUser) return;

            var context = new SocketCommandContext(_discord, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(Configuration["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                if (!DeclineCommand(context, msg.Content).Result)
                {
                    var result = await Commands.ExecuteAsync(context, argPos, _provider);

                    if (!result.IsSuccess &&
						result.Error != CommandError.UnknownCommand &&
						result.Error != CommandError.BadArgCount &&
						result.Error != CommandError.ParseFailed)
                        await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }
        
        private void SetStartingState()
        {
            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageReceived += _scanner.ScanMessage;
            _discord.MessageUpdated += _scanner.ScanUpdateMessage;
        }

        private async Task<bool> DeclineCommand(SocketCommandContext context, string messageContent)
        {
            string commandName = messageContent.Split(' ').FirstOrDefault();
            bool commandDeclined = false;

            if (!string.IsNullOrWhiteSpace(commandName))
            {
				if (BlockCommand(context, commandName).Result)
				{
					return true;
				}

                string meanResponse = string.Format(StringConsts.DeclineCommand, commandName);
                
                int randomInt = _rng.Next(1, 20);

                // bad luck, you suck
                if (randomInt == 1)
                {
                    commandDeclined = true;
					addDeclineCache(context.User.DiscordId(), commandName);
                    await context.Channel.SendMessageAsync(meanResponse);
                }
            }
            return commandDeclined;
        }

		private void addDeclineCache(string discordId, string commandName)
		{
			List<DeclineCache> declineCache = _jsonSerializer.LoadFromFile<DeclineCache>("declineCache.json");

			declineCache.Add(new DeclineCache(discordId, commandName));

			_jsonSerializer.SaveToFile<DeclineCache>("declineCache.json", declineCache);
		}

		private async Task<bool> BlockCommand(SocketCommandContext context, string commandName)
		{
			List<DeclineCache> declineCache = _jsonSerializer.LoadFromFile<DeclineCache>("declineCache.json");
			
			if (declineCache.Count > 0 && declineCache.FirstOrDefault(x => x.DiscordUsername == context.User.DiscordId() && x.CommandName == commandName) != null)
			{
				string meanerResponse = string.Format(StringConsts.ToldYou, context.User.Username, commandName);
				await context.Channel.SendMessageAsync(meanerResponse);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}