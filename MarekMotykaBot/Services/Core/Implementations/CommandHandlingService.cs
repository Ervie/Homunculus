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
		private readonly CommandService _commandService;
		private readonly Random _rng;
		private readonly IMessageScannerService _messageScanner;
		private readonly IJSONSerializerService _jsonSerializer;
		private readonly IServiceProvider _provider;

		public IConfiguration Configuration { get; set; }

		public CommandHandlingService(
			DiscordSocketClient discord, 
			CommandService commandService, 
			Random random, 
			IConfiguration configuration,
			IMessageScannerService messageScanner, 
			IServiceProvider provider, 
			IJSONSerializerService jSONSerializer
		)
		{
			_discord = discord;
			_commandService = commandService;
			_rng = random;
			Configuration = configuration;
			_messageScanner = messageScanner;
			_provider = provider;
			_jsonSerializer = jSONSerializer;

			SetStartingState();
		}

		public void SetStartingState()
		{
			_discord.MessageReceived += OnMessageReceivedAsync;
			_discord.MessageReceived += _messageScanner.ScanMessageAsync;
			_discord.MessageUpdated += _messageScanner.ScanUpdateMessageAsync;
			_discord.MessageDeleted += _messageScanner.ScanDeletedMessageAsync;
		}

		public async Task OnMessageReceivedAsync(SocketMessage s)
		{
			if (!(s is SocketUserMessage msg))
			{
				return;
			}

			if (msg.Author.Id == _discord.CurrentUser.Id)
			{
				return;
			}

			var context = new SocketCommandContext(_discord, msg);

			var argPos = 0;
			if (msg.HasStringPrefix(Configuration["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
			{
				if (! await DeclineCommandAsync(context, msg.Content))
				{
					IResult result = await _commandService.ExecuteAsync(context, argPos, _provider);

					if (!result.IsSuccess &&
						result.Error != CommandError.UnknownCommand &&
						result.Error != CommandError.BadArgCount &&
						result.Error != CommandError.ParseFailed)
					{
						await context.Channel.SendMessageAsync(result.ToString());
					}
				}
			}
			else if (RegexChecker.IsWhatWord(msg.Content))
			{
				var randomInt = _rng.Next(1, 10);
				// 20% chance
				if (randomInt < 3)
				{
					await context.Channel.SendMessageAsync($"{StringConsts.ShitString}!");
				}
			}
		}

		public async Task<bool> DeclineCommandAsync(SocketCommandContext context, string messageContent)
		{
			string commandName = messageContent.Split(' ').FirstOrDefault();
			bool commandDeclined = false;

			if (!string.IsNullOrWhiteSpace(commandName))
			{
				if (await BlockCommandAsync(context, commandName))
				{
					return true;
				}

				string meanResponse = string.Format(StringConsts.DeclineCommand, commandName);

				int randomInt = _rng.Next(1, 100);

				// bad luck, you suck
				if (randomInt < 6)
				{
					commandDeclined = true;
					await AddDeclineCacheAsync(context.User.DiscordId(), commandName);
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

		public async Task AddDeclineCacheAsync(string discordId, string commandName)
		{
			if (commandName != "!no" && commandName != "!nocosemoge" && commandName != "!co")
			{
				var declineCache = await _jsonSerializer.LoadFromFileAsync<DeclineCache>("declineCache.json");

				declineCache.Add(new DeclineCache(discordId, commandName));

				await _jsonSerializer.SaveToFileAsync("declineCache.json", declineCache);
			}
		}

		public async Task<bool> BlockCommandAsync(SocketCommandContext context, string commandName)
		{
			var declineCache = await _jsonSerializer.LoadFromFileAsync<DeclineCache>("declineCache.json");

			if (declineCache.Any() && declineCache.FirstOrDefault(x => x.DiscordUsername == context.User.DiscordId() && x.CommandName == commandName) != null)
			{
				string meanerResponse = string.Format(StringConsts.ToldYou, context.User.Username, commandName);
				await context.Channel.SendMessageAsync(meanerResponse);
				return true;
			}

			return false;
		}
	}
}