using ImgFlipAPI.APISource.Core;
using ImgFlipAPI.APISource.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class ImgFlipService: IDiscordService
    {
        private const int MarekTemplateId = 114558777;

        private readonly ImgFlipAPISource _source;
        
        private readonly string _imgFlipUsername;
        private readonly string _imgFlipPassword;

        public IConfiguration Configuration { get; set; }

        public ImgFlipService(IConfiguration configuration)
        {
            Configuration = configuration;
            _source = ImgFlipAPISource.Instance;

            _imgFlipUsername = Configuration["credentials:imgFlipUser"];
            _imgFlipPassword = Configuration["credentials:imgFlipPassword"];
        }

        public async Task<string> CreateMarekMeme(string topText, string bottomText)
        {
            CaptionMemeRoot freshMeme = await ImgFlipAPISource.Instance.CaptionMemeAsync(MarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

            if (freshMeme.success)
                return freshMeme.data.url;
            else
                return string.Empty;
        }
    }
}