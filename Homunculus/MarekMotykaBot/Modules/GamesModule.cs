using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
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
		private readonly IJSONSerializerService _jsonSerializer;

		public string ModuleName { get => "GamesModule"; }

		public ILoggingService LoggingService { get; }

		public GamesModule(
			Random random,
			IJSONSerializerService serializer,
			IEmbedBuilderService embedBuilderService,
			ILoggingService loggingService
		)
		{
			_rng = random;
			_jsonSerializer = serializer;
			_embedBuilderService = embedBuilderService;
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
		public async Task RollDiceAsync(params string[] diceSize)
		{
			try
			{
				var diceSizes = diceSize.ToList();
				int i = 0;
				int maxNumber = -1;

				if (diceSizes.Count > 0)
				{
					while (i < diceSizes.Count && !Int32.TryParse(diceSizes[i], out maxNumber))
					{
						i++;
					}
					if (maxNumber == -1 || maxNumber == 0)
					{
						await ReplyAsync(StringConsts.NoNumber);

						int rolledNumber = _rng.Next(0, 6) + 1;

						await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
					}
					else
					{
						int rolledNumber = _rng.Next(0, maxNumber) + 1;

						await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
					}
				}
				else
				{
					await ReplyAsync(StringConsts.NoNumber);

					int rolledNumber = _rng.Next(0, 6) + 1;

					await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
				}
			}
			catch (NullReferenceException)
			{
				await ReplyAsync(StringConsts.NoNumber);

				int rolledNumber = _rng.Next(0, 6) + 1;

				await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
			}
			catch (IndexOutOfRangeException)
			{
				await ReplyAsync(StringConsts.NoNumber);

				int rolledNumber = _rng.Next(0, 6) + 1;

				await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
			}
			catch (FormatException)
			{
				await ReplyAsync(StringConsts.NoNumber);

				int rolledNumber = _rng.Next(0, 6) + 1;

				await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
				await ReplyAsync(StringConsts.Impossible);
			}
			catch (OverflowException)
			{
				await ReplyAsync(StringConsts.TooMuch);
			}
			catch (Exception)
			{
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', diceSize));
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
			List<CharadeEntry> charadeCollection = _jsonSerializer.LoadFromFile<CharadeEntry>("Animes.json");
			List<int> charadeCache = _jsonSerializer.LoadFromFile<int>("charadeCache.json");

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

				_jsonSerializer.SaveToFile<int>("charadeCache.json", charadeCache);

				break;
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("ResetCharade"), Alias("reset", "r"), Summary("Reset charade cache")]
		public async Task CharadeResetAsync()
		{
			_jsonSerializer.SaveToFile("charadeCache.json", new List<int>());

			await ReplyAsync(StringConsts.CharadeReset);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}
	}
}