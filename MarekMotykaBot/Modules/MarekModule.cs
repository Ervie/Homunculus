using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.DataTypes.Caches;
using MarekMotykaBot.DataTypes.Enumerations;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class MarekModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly IImgurService _imgurService;
		private readonly IJSONSerializerService _jsonSerializer;
		private readonly IImgFlipService _imgFlipService;
		private readonly IEmbedBuilderService _embedBuilderService;
		private readonly Random _rng;
		private readonly IConfiguration _configuration;

		private readonly List<string> eightBallResponses;

		private const byte cacheSize = 10;
		private const string lonkDiscordId = "LonkDeveloper#9358";

		public string ModuleName { get => "MarekModule"; }

		public ILoggingService LoggingService { get; }

		public MarekModule(
			IConfiguration configuration,
			IImgurService imgur,
			IJSONSerializerService serializer,
			IImgFlipService imgFlip,
			IEmbedBuilderService embedBuilderService,
			Random random,
			ILoggingService loggingService)
		{
			_configuration = configuration;
			_imgurService = imgur;
			_jsonSerializer = serializer;
			_rng = random;
			_imgFlipService = imgFlip;
			_embedBuilderService = embedBuilderService;
			LoggingService = loggingService;

			eightBallResponses = _jsonSerializer.LoadFromFile<string>("8ballResponses.json");
		}

		[Command("NoCoSeMoge"), Alias("no", "co"), Summary("He will tell you what you can do")]
		public async Task CoSeMogeAsync()
		{
			await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
			await Task.Delay(3000);

			int hitNumber = _rng.Next(0, 15);

			if (hitNumber == 1)
			{
				await Context.Channel.SendMessageAsync($"...");
				await Task.Delay(1000);
				await Context.Channel.SendMessageAsync($"...");
				await Task.Delay(1000);
				await Context.Channel.SendMessageAsync($"**{StringConsts.RunAway}**");
				return;
			}

			int damageNumber = _rng.Next(0, 15);
			switch (damageNumber)
			{
				case int n when (n == 1):
					await Context.Channel.SendMessageAsync($"**{StringConsts.EggString}**");
					break;

				case (2):
				case (3):
					await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
					await Task.Delay(1500);
					await Context.Channel.SendMessageAsync($"**{StringConsts.InTheAss}**");
					break;

				case (4):
					await Context.Channel.SendMessageAsync($"**{StringConsts.FinishingBlow}**");
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync($"https://i.imgur.com/f0F9eqK.png");
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync($"**{StringConsts.GenkiDama}**");
					break;

				case (5):
				case (6):
				case (7):
					await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
					await Task.Delay(1500);
					await Context.Channel.SendMessageAsync($"**{StringConsts.InTheJar}**");
					break;

				default:
					await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
					break;
			}

			switch (hitNumber)
			{
				case (2):
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync(StringConsts.MissedThrow2);
					break;

				case (3):
				case (4):
					string victim = Context.Guild.GetRandomUserName(_rng, Context.User.DiscordId());
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync(string.Format(StringConsts.MissedThrow, victim));
					break;

				default:
					break;
			}

			var declineCache = await _jsonSerializer.LoadFromFileAsync<DeclineCache>("declineCache.json");

			declineCache.RemoveAll(x => x.DiscordUsername.Equals(Context.User.DiscordId()));

			await _jsonSerializer.SaveToFileAsync("declineCache.json", declineCache);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("Sowa"), Alias("owl"), Summary("Post random owl image")]
		public async Task SowaAsync()
		{
			string gifUrl = await _imgurService.GetRandomImageFromGallery("CbtU3");

			await ReplyAsync(gifUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("LonkMeme"), Alias("lonk"), Summary("Post random Lonk meme image")]
		public async Task LonkMemeAsync()
		{
			string picUrl = await _imgurService.GetRandomImageFromAlbum("w5dzWtL");

			await ReplyAsync(picUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("MarekMeme"), Alias("meme"), Summary("Post random old Marek meme image")]
		public async Task OldMemeAsync()
		{
			string gifUrl = await _imgurService.GetRandomImageFromAlbum("V5CPd");

			await ReplyAsync(gifUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("MarekMeme"), Alias("meme"), Summary("Create your own Marek meme image, text split by semicolon - marekface version")]
		public async Task NewMemeAsync(params string[] text)
		{
			string resultUrl = await _imgFlipService.CreateMarekFace(text);

			await ReplyAsync(resultUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', string.Join(" ", text)));
		}

		[Command("DrakeMeme"), Alias("drake"), Summary("Create your own Marek meme image, text split by semicolon - Marek Drake version")]
		public async Task DrakeMemeAsync(params string[] text)
		{
			string resultUrl = await _imgFlipService.CreateDrakeMarekMeme(text);

			await ReplyAsync(resultUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', string.Join(" ", text)));
		}

		[Command("MarekMeme2"), Alias("meme2"), Summary("Create your own Marek meme image, text split by semicolon - laughing version")]
		public async Task NewMeme2Async(params string[] text)
		{
			string resultUrl = await _imgFlipService.CreateLaughingMarekMeme(text);

			await ReplyAsync(resultUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', string.Join(" ", text)));
		}

		[Command("NosaczMeme"), Alias("nosacz"), Summary("Create your own nosacz meme image, text split by semicolon")]
		public async Task NosaczMemeAsync(params string[] text)
		{
			string resultUrl = await _imgFlipService.CreateNosaczMeme(text);

			await ReplyAsync(resultUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', string.Join(" ", text)));
		}

		[Command("Joke"), Summary("Marek's joke - you know the drill")]
		public async Task JokeAsync()
		{
			var jokes = await _jsonSerializer.LoadFromFileAsync<OneLinerJoke>("oneLiners.json");

			int randomJokeIndex = _rng.Next(1, jokes.Count);

			OneLinerJoke selectedJoke = jokes[randomJokeIndex];

			await Context.Channel.SendMessageAsync($"{selectedJoke.Question}");
			await Task.Delay(3000);
			await Context.Channel.SendMessageAsync($"{selectedJoke.Punchline}");

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("8ball"), Summary("Binary answer for all your questions")]
		public async Task EightBallAsync(params string[] text)
		{
			string messageKey = string.Join(" ", text);
			string userKey = Context.User.DiscordId();

			if (string.IsNullOrWhiteSpace(messageKey))
			{
				await Context.Channel.SendMessageAsync(StringConsts.WrongQuestion);
			}
			else
			{
				var cache = await _jsonSerializer.LoadFromFileAsync<EightBallCache>("cache8ball.json");

				if (cache.Exists(x => x.Question == messageKey && x.DiscordUsername == userKey))
				{
					await Context.Channel.SendMessageAsync(cache.Find(x => x.Question == messageKey).Answer);
				}
				else
				{
					int randomResponseIndex = _rng.Next(1, eightBallResponses.ToList().Count);

					string selectedResponse = eightBallResponses.ElementAt(randomResponseIndex);

					string selectedUser = Context.Guild.GetRandomUserName(_rng);

					if (cache.Count > cacheSize)
						cache.RemoveAt(0);

					cache.Add(new EightBallCache(userKey, messageKey, string.Format(selectedResponse, selectedUser)));

					await _jsonSerializer.SaveToFileAsync("cache8ball.json", cache);

					await Context.Channel.SendMessageAsync($"{string.Format(selectedResponse, selectedUser)}");
				}
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', messageKey));
		}

		[Command("quote"), Alias("cytat", "q"), Summary("Ancient wisdom...")]
		public async Task QuoteAsync(params string[] category)
		{
			QuoteCategory filtercategory = DetectQuoteCategory(category);

			Quote selectedQuote;

			switch (filtercategory)
			{
				case QuoteCategory.OfTheDay:
					selectedQuote = (await _jsonSerializer.LoadFromFileAsync<Quote>("quoteOfTheDay.json")).First();
					break;

				case QuoteCategory.Insult:
					var remorseChance = _rng.Next(0, 20);
					if (remorseChance == 3)
					{
						await Context.Channel.SendMessageAsync("...");
						await Task.Delay(3000);
						await Context.Channel.SendMessageAsync(StringConsts.WhyWouldIDoThat);
						return;
					}
					selectedQuote = await GetRandomQuoteAsync(filtercategory);
					break;

				default:
					selectedQuote = await GetRandomQuoteAsync(filtercategory);
					break;
			}

			string intro = _rng.Next(1, 4) switch
			{
				1 => StringConsts.DerpQuote,
				2 => StringConsts.DerpQuote2,
				3 => StringConsts.DerpQuote3,
				_ => StringConsts.DerpQuote
			};

			int lateArrivalProbability = _rng.Next(0, 10);
			if (lateArrivalProbability == 1)
			{
				await Task.Delay(5000);
				int lateMessageId = _rng.Next(0, 2);
				string lateArrivalMessage = lateMessageId == 0
					? StringConsts.SorryForLateArrivalMessage1
					: StringConsts.SorryForLateArrivalMessage2;
				await Context.Channel.SendMessageAsync(lateArrivalMessage);
				await Task.Delay(3000);
			}

			await Context.Channel.SendMessageAsync(intro);
			await Task.Delay(3000);
			await ReplyAsync("", false, _embedBuilderService.BuildQuote(selectedQuote));

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', category));
		}

		[Command("blueribbon"), Summary("Passes for hidden gift")]
		public async Task UnityAsync()
		{
			if (!Context.User.DiscordId().Equals(lonkDiscordId))
			{
				await Context.Channel.SendMessageAsync(string.Format(StringConsts.SecretGiftDeny, "Lonka!"));
			}
			else
			{
				await Context.Channel.SendMessageAsync($"{StringConsts.HelionUser} {_configuration["credentials:helionUser"]}");
				await Context.Channel.SendMessageAsync($"{StringConsts.HelionPassword} {_configuration["credentials:helionPassword"]}");
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("lastContact"), Alias("lc", "lastMessage", "lm"), Summary("Last message by Marek")]
		public async Task LastContactAsync()
		{
			var lastMessage = await _jsonSerializer.LoadSingleFromFileAsync<LastMarekMessage>("marekLastMessage.json");

			if (lastMessage != null)
			{
				await ReplyAsync("", false, _embedBuilderService.BuildLastContact(lastMessage));
			}
		}

		[Command("suchar"), Alias("pun"), Summary("A derpish pun from a derpish member")]
		public async Task DerpPunAsync()
		{
			var suchars = await _jsonSerializer.LoadFromFileAsync<OneLinerJoke>("derpSuchars.json");
			var selectedSucharIndex = _rng.Next(0, suchars.Count);
			var selectedSuchar = suchars[selectedSucharIndex];

			await Context.Channel.SendMessageAsync($"{selectedSuchar.Question}");
			await Task.Delay(3000);
			await Context.Channel.SendMessageAsync($"{selectedSuchar.Punchline}");

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		private async Task<Quote> GetRandomQuoteAsync(QuoteCategory filtercategory)
		{
			// secret quote...
			var secretQuoteResult = _rng.Next(1, 50);
			if (secretQuoteResult == 2)
			{
				var secretQuote = string.Format(StringConsts.DeclineCommand, "!q");
				var author = "Sztuczny Murzyn";
				return new Quote(secretQuote, author, null);
			}
			else
			{
				var quotes = await _jsonSerializer.LoadFromFileAsync<Quote>("quotes.json");

				if (filtercategory != QuoteCategory.None)
					quotes = quotes
						.Where(x => x.Categories.Contains(filtercategory))
						.ToList();

				int randomQuoteIndex = _rng.Next(0, quotes.Count);

				return quotes[randomQuoteIndex];
			}
		}

		private QuoteCategory DetectQuoteCategory(params string[] category)
		{
			QuoteCategory filtercategory = QuoteCategory.None;

			if (category.Any())
			{
				filtercategory = category[0].ToLower() switch
				{
					("i") => QuoteCategory.Insult,
					("p") => QuoteCategory.Insult,
					("insult") => QuoteCategory.Insult,
					("pocisk") => QuoteCategory.Insult,

					("m") => QuoteCategory.Wisdom,
					("w") => QuoteCategory.Wisdom,
					("mądrość") => QuoteCategory.Wisdom,
					("wisdom") => QuoteCategory.Wisdom,

					("t") => QuoteCategory.Thought,
					("thought") => QuoteCategory.Thought,

					("f") => QuoteCategory.Fiutt,
					("fiutt") => QuoteCategory.Fiutt,

					("reaction") => QuoteCategory.Reaction,
					("reakcja") => QuoteCategory.Reaction,
					("r") => QuoteCategory.Reaction,

					("d") => QuoteCategory.OfTheDay,
					_ => QuoteCategory.None
				};
			}

			return filtercategory;
		}
	}
}