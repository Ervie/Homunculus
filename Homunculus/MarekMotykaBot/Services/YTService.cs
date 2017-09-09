using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Linq;
using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace MarekMotykaBot.Services
{
    public class YTService
    {
        private readonly YouTubeService _youTubeService;

        public YTService()
        {
            _youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "toBeReplaced",
                MaxUrlLength = 256
            });
        }

        public async Task<Youtube.SearchResult> SearchYoutubeAsync(string query, string dataType)
        {
            var request = _youTubeService.Search.List("snippet");
            request.Q = query;
            request.MaxResults = 3;

            var result = await request.ExecuteAsync();
            return result.Items.FirstOrDefault(x => x.Id.Kind == dataType);
        }
    }
}