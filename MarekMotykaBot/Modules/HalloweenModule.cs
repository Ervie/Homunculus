using Discord.Commands;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class HalloweenModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		public string ModuleName { get => "HalloweenModule"; }

		public ILoggingService LoggingService { get; }

		private readonly IImgurService _imgur;
		private readonly IImgFlipService _imgFlip;

		public HalloweenModule(
			ILoggingService loggingService,
			IImgurService imgurService,
			IImgFlipService imgFlip
		)
		{
			LoggingService = loggingService;
			_imgur = imgurService;
			_imgFlip = imgFlip;
		}

		[Command("Skeleton"), Alias("sk", "skmeme"), Summary("Skeleton meme")]
		public async Task OldMemeAsync()
		{
			string gifUrl = await _imgur.GetRandomImageFromAlbum("HuEZ2QX");

			await ReplyAsync(gifUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("Skeletor"), Alias("skq"), Summary("Ancient wisdom... of Skeletor")]
		public async Task QuoteAsync()
		{
			string gifUrl = await _imgur.GetRandomImageFromAlbum("lfnXIbc");

			await ReplyAsync(gifUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("MarekSkeletorMeme"), Alias("meme3"), Summary("Create your own Marek meme image, text split by semicolon - skeletor version")]
		public async Task NewMemeAsync(params string[] text)
		{
			string resultUrl = await _imgFlip.CreateSkeletorMarekMeme(text);

			await ReplyAsync(resultUrl);

			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', string.Join(" ", text)));
		}
	}
}