using Discord.Commands;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class YoutubeModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly IYTService _youtube;

		public string ModuleName { get => "YoutubeModule"; }

		public ILoggingService LoggingService { get; }

		public YoutubeModule(
			IYTService youtube,
			ILoggingService loggingService
		)
		{
			_youtube = youtube;
			LoggingService = loggingService;
		}

		[Command("Youtube"), Alias("yt"), Summary("Search for video with given query.")]
		public async Task SearchYTvideo(params string[] args)
		{
			string query = string.Join(" ", args);
			var video = await _youtube.SearchYoutubeAsync(query, "youtube#video");

			if (video == null)
			{
				await ReplyAsync(string.Format(StringConsts.YtNotFound, query));
			}
			else
			{
				await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}
	}
}