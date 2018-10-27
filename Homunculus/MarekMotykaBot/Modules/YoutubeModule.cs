using Discord.Commands;
using MarekMotykaBot.Services;
using MarekMotykaBot.Resources;
using System;
using System.Threading.Tasks;
using MarekMotykaBot.Modules.Interface;

namespace MarekMotykaBot.Modules
{
    public class YoutubeModule : ModuleBase<SocketCommandContext>, IDiscordModule
    {
        private readonly YTService _youtube;
        private readonly IServiceProvider _provider;

		public string ServiceName { get => "YoutubeModule"; }

		public ILoggingService _loggingService { get; }

		public YoutubeModule(YTService youtube, IServiceProvider provider, LoggingService loggingService)
		{
			_youtube = youtube;
			_provider = provider;
			_loggingService = loggingService;
		}

		[Command("BlokEkipa"), Alias("be"), Summary("Get newest episode of Blok Ekipa from YT.")]
		public async Task BlokEkipaSync()
        {
            var video = await _youtube.SearchYoutubeAsync("Blok ekipa", "youtube#video");

            if (video == null)
                await ReplyAsync(string.Format(StringConsts.YtNotFound, "Blok Ekipa"));
            else
                await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");

			_loggingService.CustomCommandLog(Context.Message, ServiceName);
		}

  //      [Command("Youtube"), Alias("yt"), Summary("Search for video with given query.")]
  //      public async Task SearchYTvideo(params string[] args)
  //      {
  //          string query = string.Join(" ", args);
  //          var video = await _youtube.SearchYoutubeAsync(query, "youtube#video");

  //          if (video == null)
  //              await ReplyAsync(string.Format(StringConsts.YtNotFound, query));
  //          else
  //              await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");

		//	_loggingService.CustomCommandLog(Context.Message, ServiceName);
		//}
    }
}