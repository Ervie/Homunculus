using ImgFlipAPI.APISource.Core;
using ImgFlipAPI.APISource.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class ImgFlipService: IDiscordService
    {
        private const int MarekTemplateId = 114558777;
		private const int LaughingMarekTemplateId = 152110002;
		private const int SkeletorMarekTemplateId = 156362598;

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

		public async Task<string> CreateLaughingMarekMeme(string topText, string bottomText)
		{
			CaptionMemeRoot freshMeme = await ImgFlipAPISource.Instance.CaptionMemeAsync(LaughingMarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			if (freshMeme.success)
				return freshMeme.data.url;
			else
				return string.Empty;
		}

		public async Task<string> CreateSkeletorMarekMeme(string topText, string bottomText)
		{
			CaptionMemeRoot freshMeme = await ImgFlipAPISource.Instance.CaptionMemeAsync(SkeletorMarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			if (freshMeme.success)
				return freshMeme.data.url;
			else
				return string.Empty;
		}
	}
}