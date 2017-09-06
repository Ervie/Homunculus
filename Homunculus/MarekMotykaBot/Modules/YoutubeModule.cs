using Discord.Commands;
using Google.Apis.YouTube.v3;
using System.Linq;
using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace MarekMotykaBot.Modules
{
    //TODO: wykończyć

    //public class YoutubeModule : ModuleBase<SocketCommandContext>
    //{
    //    private readonly YouTubeService _youtube;

    //    public YoutubeModule(YouTubeService youtube)
    //    {
    //        _youtube = youtube;
    //    }

    //    [Command("BlokEkipa"), Alias("be"), Summary("Get newest episode of Blok Ekipa from YT.")]
    //    public async Task BlokEkipaSync()
    //    {
    //        var video = await SearchYoutubeAsync("Blok ekipa", "youtube#video");

    //        if (video == null)
    //            await ReplyAsync($"I could not find a video like Blok Ekipa");
    //        else
    //            await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");
    //    }

    //    private async Task<Youtube.SearchResult> SearchYoutubeAsync(string query, string dataType)
    //    {
    //        var request = _youtube.Search.List("snippet");
    //        request.Q = query;
    //        request.MaxResults = 1;

    //        var result = await request.ExecuteAsync();
    //        return result.Items.FirstOrDefault(x => x.Id.Kind == dataType);
    //    }
    //}

}
