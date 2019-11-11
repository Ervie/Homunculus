using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using MarekMotykaBot.Services.Core;
using MarekMotykaBot.Services.External;
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

		[Command("Skeleton"), Alias("sk", "skmeme"), Summary("Skeleton meme")]
		public async Task OldMemeAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromAlbum("HuEZ2QX");

			await ReplyAsync(gifUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("Skeletor"), Alias("skq"), Summary("Ancient wisdom... of Skeletor")]
		public async Task QuoteAsync()
		{
			string gifUrl;
			gifUrl = await _imgur.GetRandomImageFromAlbum("lfnXIbc");

			await ReplyAsync(gifUrl);

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("MarekSkeletorMeme"), Alias("meme3"), Summary("Create your own Marek meme image, text split by semicolon - skeletor version")]
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
	}
}
