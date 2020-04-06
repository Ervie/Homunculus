using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class GamesModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly Random _rng;
		private readonly IEmbedBuilderService _embedBuilderService;
		private readonly IUnrealTournamentService _unrealTournamentService;
		private readonly IJSONSerializerService _jsonSerializer;

		public string ModuleName { get => "GamesModule"; }

		public ILoggingService LoggingService { get; }

		public GamesModule(
			Random random,
			IJSONSerializerService serializer,
			IEmbedBuilderService embedBuilderService,
			IUnrealTournamentService unrealTournamentService,
			ILoggingService loggingService
		)
		{
			_rng = random;
			_jsonSerializer = serializer;
			_embedBuilderService = embedBuilderService;
			_unrealTournamentService = unrealTournamentService;
			LoggingService = loggingService;
		}

		[Command("Someone"), Alias("s"), Summary("Get random username from server")]
		public async Task RandomUserAsync()
		{
			await ReplyAsync(Context.Guild.GetRandomUserName(_rng));

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("Roll_k6"), Alias("k6"), Summary("Roll the k6 dice.")]
		public async Task RollK6Async()
		{
			int rolledNumber = _rng.Next(0, 6) + 1;

			await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("Roll_k100"), Alias("k100"), Summary("Roll the k100 dice.")]
		public async Task RollK100Async()
		{
			int rolledNumber = _rng.Next(0, 100) + 1;

			await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("Roll"), Alias("k"), Summary("Roll customizable dice.")]
		public async Task RollDiceAsync(params string[] diceSizes)
		{
			const int defaultMaxNumber = 6;

			try
			{
				string diceSize = diceSizes?.FirstOrDefault();

				if (diceSize is { })
				{
					int.TryParse(diceSize, out int parsedNumber);

					if (parsedNumber < 0)
					{
						await ReplyAsync(StringConsts.NoNumber);

						int rolledNumber = _rng.Next(0, defaultMaxNumber) + 1;

						await ReplyAsync($"{Context.User.Username}: {rolledNumber}");
					}
					else
					{
						int rolledNumber = _rng.Next(0, parsedNumber) + 1;

						await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
					}
				}
				else
				{
					await ReplyAsync(StringConsts.NoNumber);

					int rolledNumber = _rng.Next(0, defaultMaxNumber) + 1;

					await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
				}
				LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', diceSize.ToString() ?? defaultMaxNumber.ToString()));
			}
			catch (Exception)
			{
			}
		}

		[Command("Flip_coin"), Alias("flip"), Summary("Flip the coin.")]
		public async Task FlipCoinAsync()
		{
			int result = _rng.Next(0, 2);

			string resultString = result == 1 ? StringConsts.Head : StringConsts.Tails;

			await ReplyAsync(Context.User.Username + ": " + resultString);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("Charade"), Alias("kalambury", "c"), Summary("Draw a random entry for charade game.")]
		public async Task CharadeAsync()
		{
			var charadeCollection = await _jsonSerializer.LoadFromFileAsync<CharadeEntry>("Animes.json");
			var charadeCache = await _jsonSerializer.LoadFromFileAsync<int>("charadeCache.json");

			if (charadeCache.Count() == charadeCollection.Count())
			{
				await Context.Channel.SendMessageAsync($"{StringConsts.CharadeEnd}");
				return;
			}

			while (true)
			{
				int randomCharadeEntryIndex = _rng.Next(0, charadeCollection.Count);

				if (charadeCache.Contains(charadeCollection[randomCharadeEntryIndex].Series.Id))
					continue;

				CharadeEntry selectedEntry = charadeCollection[randomCharadeEntryIndex];

				await ReplyAsync("", false, _embedBuilderService.BuildCharadeEntry(selectedEntry));

				charadeCache.Add(selectedEntry.Series.Id);

				await _jsonSerializer.SaveToFileAsync<int>("charadeCache.json", charadeCache);

				break;
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("ResetCharade"), Alias("reset", "r"), Summary("Reset charade cache")]
		public async Task CharadeResetAsync()
		{
			await _jsonSerializer.SaveToFileAsync("charadeCache.json", new List<int>());

			await ReplyAsync(StringConsts.CharadeReset);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("CurrentRotation"), Alias("utr"), Summary("List current map rotation")]
		public async Task PrintMapRotationAsync()
		{
			var mapList = await _unrealTournamentService.GetCurrentRotationMapList();

			await ReplyAsync("", false, _embedBuilderService.BuildMapList(mapList));

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}
	}
}