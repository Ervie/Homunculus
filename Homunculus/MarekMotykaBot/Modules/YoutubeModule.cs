using Discord.Commands;
using MarekMotykaBot.Services;
using System;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class YoutubeModule : ModuleBase<SocketCommandContext>
    {
        private readonly YTService _youtube;
        private readonly IServiceProvider _provider;

        public YoutubeModule(YTService youtube, IServiceProvider provider)
        {
            _youtube = youtube;
            _provider = provider;
        }

        [Command("BlokEkipa"), Alias("be"), Summary("Get newest episode of Blok Ekipa from YT.")]
        public async Task BlokEkipaSync()
        {
            var video = await _youtube.SearchYoutubeAsync("Blok ekipa", "youtube#video");

            if (video == null)
                await ReplyAsync($"I could not find a video like Blok Ekipa");
            else
                await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");
        }

        [Command("Youtube"), Alias("yt"), Summary("Search for video with given query.")]
        public async Task SearchYTvideo(params string[] args)
        {
            string query = string.Join(" ", args);
            var video = await _youtube.SearchYoutubeAsync(query, "youtube#video");

            if (video == null)
                await ReplyAsync($"I could not find a video like {query}");
            else
                await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");
        }
    }
}