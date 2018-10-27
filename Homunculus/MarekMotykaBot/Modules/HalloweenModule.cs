using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class HalloweenModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		public string ServiceName { get => "HalloweenModule"; }

		public ILoggingService _loggingService { get; }

		private readonly string skeletors = AppContext.BaseDirectory + "/Resources/Images/Skeletor/";

		private readonly JSONSerializerService _serializer;
		private readonly ImgurService _imgur;
		private readonly ImgFlipService _imgFlip;
		private readonly Random _rng;
		private readonly YTService _youtube;

		public HalloweenModule(LoggingService loggingService, JSONSerializerService serializerService, ImgurService imgurService, ImgFlipService imgFlip, Random random, YTService ytService)
		{
			_loggingService = loggingService;
			_serializer = serializerService;
			_imgur = imgurService;
			_imgFlip = imgFlip;
			_rng = random;
			_youtube = ytService;
		}

		[Command("MarekMeme"), Alias("meme"), Summary("Skeleton meme")]
		public async Task OldMemeAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromAlbum("HuEZ2QX");

			await ReplyAsync(gifUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("Sowa"), Alias("owl"), Summary("Accually skeleton meme")]
		public async Task SowaAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromAlbum("HuEZ2QX");

			await ReplyAsync(gifUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("quote"), Alias("cytat", "q"), Summary("Ancient wisdom...")]
		public async Task QuoteAsync()
		{
			int randomImgNumber = _rng.Next(1, 16);

			await Context.Channel.SendFileAsync(skeletors + randomImgNumber + ".jpg");

			//string gifUrl;
			//gifUrl = await _imgur.GetRandomImageFromAlbum("lfnXIbc");

			//await ReplyAsync(gifUrl);

			//_loggingService.CustomCommandLog(Context.Message, ServiceName);

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

			string resultUrl = await _imgFlip.CreateSkeletorMarekMeme(toptext, bottomtext);

			await ReplyAsync(resultUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName, string.Join(' ', captions));
		}

		[Command("Joke"), Summary("Marek's joke - you know the drill")]
		public async Task JokeAsync()
		{
			List<OneLinerJoke> jokes = _serializer.LoadFromFile<OneLinerJoke>("skeletonJokes.json");

			int randomJokeIndex = _rng.Next(1, jokes.Count);

			OneLinerJoke selectedJoke = jokes[randomJokeIndex];

			await Context.Channel.SendMessageAsync($"{selectedJoke.Question}");
			await Task.Delay(3000);
			await Context.Channel.SendMessageAsync($"{selectedJoke.Punchline}");

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("Youtube"), Alias("yt"), Summary("Search for video with given query.")]
		public async Task SearchYTvideo(params string[] args)
		{
			int randomNumber = _rng.Next(1, 5);

			string query; 

			switch (randomNumber)
			{
				case (1):
					query = "The Skeleton Dance";
					break;
				case (2):
					query = "ivan el trolazo";
					break;
				case (3):
					query = "skeletor myah";
					break;
				case (4):
					query = "Nyeh Heh Heh 10 Hours";
					break;
				default:
					query = "Undertale Megalovania";
					break;
			}

			var video = await _youtube.SearchYoutubeAsync(query, "youtube#video");

			if (video == null)
				await ReplyAsync(string.Format(StringConsts.YtNotFound, query));
			else
				await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}
	}
}
