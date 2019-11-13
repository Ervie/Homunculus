using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes.Caches;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core
{
	public class CommandHandlingService : IDiscordService, ICommandHandlingService
	{
		private readonly DiscordSocketClient _discord;
		private readonly CommandService _commands;
		private readonly IJSONSerializerService _jsonSerializer;
		private readonly IMessageScannerService _scanner;
		private readonly Random _rng;
		private readonly IServiceProvider _provider;

		public IConfiguration Configuration { get; set; }

		public CommandHandlingService(
			IConfiguration configuration,
			DiscordSocketClient discord, 
			CommandService commands, 
			IMessageScannerService scanner, 
			Random random, 
			IServiceProvider provider, 
			IJSONSerializerService jSONSerializer
			)
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

		public async Task OnMessageReceivedAsync(SocketMessage s)
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

		public void SetStartingState()
		{
			_discord.MessageReceived += OnMessageReceivedAsync;
			_discord.MessageReceived += _scanner.ScanMessage;
			_discord.MessageUpdated += _scanner.ScanUpdateMessage;
			_discord.MessageDeleted += _scanner.ScanDeletedMessage;
		}

		public async Task<bool> DeclineCommand(SocketCommandContext context, string messageContent)
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

				int randomInt = _rng.Next(1, 100);

				// bad luck, you suck
				if (randomInt < 6)
				{
					commandDeclined = true;
					AddDeclineCache(context.User.DiscordId(), commandName);
					await context.Channel.SendMessageAsync(meanResponse);
				}
				else if (randomInt == 6)
				{
					commandDeclined = true;
					await context.Channel.SendMessageAsync(StringConsts.PreemptiveAttack);
					await Task.Delay(2000);
					await context.Channel.SendMessageAsync(StringConsts.ShitString);
				}
			}
			return commandDeclined;
		}

		public void AddDeclineCache(string discordId, string commandName)
		{
			if (commandName != "!no" && commandName != "!nocosemoge")
			{
				List<DeclineCache> declineCache = _jsonSerializer.LoadFromFile<DeclineCache>("declineCache.json");

				declineCache.Add(new DeclineCache(discordId, commandName));

				_jsonSerializer.SaveToFile<DeclineCache>("declineCache.json", declineCache);
			}
		}

		public async Task<bool> BlockCommand(SocketCommandContext context, string commandName)
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