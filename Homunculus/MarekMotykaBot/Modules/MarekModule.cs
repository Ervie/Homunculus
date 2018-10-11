using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.DataTypes.Caches;
using MarekMotykaBot.DataTypes.Enumerations;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class MarekModule : ModuleBase<SocketCommandContext>
	{
		private readonly ImgurService _imgur;
		private readonly JSONSerializerService _serializer;
		private readonly ImgFlipService _imgFlip;
		private readonly Random _rng;
		private readonly IConfiguration _configuration;

		private readonly List<string> eightBallResponses;

		private const byte cacheSize = 10;

		public MarekModule(IConfiguration configuration, ImgurService imgur, JSONSerializerService serializer, ImgFlipService imgFlip, Random random)
		{
			_configuration = configuration;
			_imgur = imgur;
			_serializer = serializer;
			_rng = random;
			_imgFlip = imgFlip;

			eightBallResponses = _serializer.LoadFromFile<string>("8ballResponses.json");
		}

		[Command("NoCoSeMoge"), Alias("no"), Summary("He will tell you what you can do")]
		public async Task CoSeMogeAsync()
		{
			int randomNumer = _rng.Next(0, 15);

			await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
			await Task.Delay(3000);

			switch (randomNumer)
			{
				case (1):
					await Context.Channel.SendMessageAsync($"...");
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync($"...");
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync($"**{StringConsts.RunAway}**");
					break;

				case (2):
				case (3):
					string victim = Context.Guild.GetRandomUserName(_rng, Context.User.DiscordId());
					await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
					await Task.Delay(1000);
					await Context.Channel.SendMessageAsync(string.Format(StringConsts.MissedThrow, victim));
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

			List<DeclineCache> declineCache = _serializer.LoadFromFile<DeclineCache>("declineCache.json");

			declineCache.RemoveAll(x => x.DiscordUsername.Equals(Context.User.DiscordId()));

			_serializer.SaveToFile("declineCache.json", declineCache);
		}

		[Command("Sowa"), Alias("owl"), Summary("Post random owl image")]
		public async Task SowaAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromGallery("CbtU3");

			await ReplyAsync(gifUrl);
		}

		[Command("MarekMeme"), Alias("meme"), Summary("Post random old Marek meme image")]
		public async Task OldMemeAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromAlbum("V5CPd");

			await ReplyAsync(gifUrl);
		}

		[Command("MarekMeme"), Alias("meme"), Summary("Create your own Marek meme image, text split by semicolon - marekface version")]
		public async Task NewMemeAsync(params string[] text)
		{			
			var captions = string.Join(" ", text).Split(';').ToList();
			
			if (captions.Count < 2)
				return;

			for (int i = 0; i < captions.Count; i++)
			{
				captions[i] = captions[i].RemoveEmojisAndEmotes();
			}


			if (string.IsNullOrWhiteSpace(captions[0]) || string.IsNullOrWhiteSpace(captions[1]))
				return;

			string toptext = captions[0].ToUpper();
			string bottomtext = captions[1].ToUpper();

			string resultUrl = await _imgFlip.CreateMarekMeme(toptext, bottomtext);

			await ReplyAsync(resultUrl);
		}

		[Command("MarekMeme2"), Alias("meme2"), Summary("Create your own Marek meme image, text split by semicolon - laughing version")]
		public async Task NewMeme2Async(params string[] text)
		{
			var captions = string.Join(" ", text).Split(';').ToList();

			if (captions.Count < 2)
				return;

			for (int i = 0; i < captions.Count; i++)
			{
				captions[i] = captions[i].RemoveEmojisAndEmotes();
			}


			if (string.IsNullOrWhiteSpace(captions[0]) || string.IsNullOrWhiteSpace(captions[1]))
				return;

			string toptext = captions[0].ToUpper();
			string bottomtext = captions[1].ToUpper();

			string resultUrl = await _imgFlip.CreateLaughingMarekMeme(toptext, bottomtext);

			await ReplyAsync(resultUrl);
		}

		[Command("Joke"), Summary("Marek's joke - you know the drill")]
		public async Task JokeAsync()
		{
			List<OneLinerJoke> jokes = _serializer.LoadFromFile<OneLinerJoke>("oneLiners.json");

			int randomJokeIndex = _rng.Next(1, jokes.Count);

			OneLinerJoke selectedJoke = jokes[randomJokeIndex];

			await Context.Channel.SendMessageAsync($"{selectedJoke.Question}");
			await Task.Delay(3000);
			await Context.Channel.SendMessageAsync($"{selectedJoke.Punchline}");
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
				List<EightBallCache> cache = _serializer.LoadFromFile<EightBallCache>("cache8ball.json");
				
				// Check if message was not received earlier; If yes, send same answer
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

					_serializer.SaveToFile<EightBallCache>("cache8ball.json", cache);

					await Context.Channel.SendMessageAsync($"{string.Format(selectedResponse, selectedUser)}");
				}
			}
		}

		[Command("quote"), Alias("cytat", "q"), Summary("Ancient wisdom...")]
		public async Task QuoteAsync(params string[] category)
		{
			QuoteCategory filtercategory = QuoteCategory.None;

			if (category.Length != 0)
			{
				switch (category[0].ToLower())
				{
					case ("i"):
					case ("p"):
					case ("insult"):
					case ("pocisk"):
						filtercategory = QuoteCategory.Insult;
						break;
					case ("m"):
					case ("w"):
					case ("mądrość"):
					case ("wisdom"):
						filtercategory = QuoteCategory.Wisdom;
						break;
					case ("t"):
					case ("thought"):
						filtercategory = QuoteCategory.Thought;
						break;
					case ("f"):
					case ("fiutt"):
						filtercategory = QuoteCategory.Fiutt;
						break;
					case ("reaction"):
					case ("reakcja"):
					case ("r"):
						filtercategory = QuoteCategory.Reaction;
						break;
					case ("d"):
						filtercategory = QuoteCategory.OfTheDay;
						break;
					default:
						break;
				}
			}

			Quote selectedQuote;

			if (filtercategory != QuoteCategory.OfTheDay)
			{
				List<Quote> quotes = _serializer.LoadFromFile<Quote>("quotes.json");

				if (filtercategory != QuoteCategory.None)
					quotes = quotes.Where(x => x.Categories.Contains(filtercategory)).ToList();

				int randomQuoteIndex = _rng.Next(0, quotes.Count);

				 selectedQuote = quotes[randomQuoteIndex];
			}
			else
			{
				selectedQuote = _serializer.LoadFromFile<Quote>("quoteOfTheDay.json").First();
			}

			var builder = new EmbedBuilder();

			builder.WithFooter(selectedQuote.Author);
			builder.WithTitle(selectedQuote.QuoteBody);

			string intro = string.Empty;

			switch (_rng.Next(1, 4))
			{
				case 1:
					intro = StringConsts.DerpQuote;
					break;

				case 2:
					intro = StringConsts.DerpQuote2;
					break;

				case 3:
					intro = StringConsts.DerpQuote3;
					break;
			}

			await Context.Channel.SendMessageAsync(intro);
			await Task.Delay(3000);
			await ReplyAsync("", false, builder.Build());
		}

		[Command("blueribbon"), Summary("Passes for hidden gift")]
		public async Task UnityAsync()
		{
			if (!Context.User.DiscordId().Equals("Tarlfgar#9358"))
			{
				await Context.Channel.SendMessageAsync(String.Format(StringConsts.SecretGiftDeny, "Lonka!"));
			}
			else
			{
				await Context.Channel.SendMessageAsync("Helion user e-mail: " + _configuration["credentials:helionUser"]);
				await Context.Channel.SendMessageAsync("Helion password: " + _configuration["credentials:helionPassword"]);
			}
		}
	}
}