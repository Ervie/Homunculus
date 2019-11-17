using ImgFlipAPI.APISource.Core;
using ImgFlipAPI.APISource.Core.Models;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External
{
	public class ImgFlipService : IDiscordService, IImgFlipService
	{
		private const int MarekTemplateId = 114558777;
		private const int LaughingMarekTemplateId = 152110002;
		private const int SkeletorMarekTemplateId = 156362598;
		private const int DrakeMarekTemplateId = 160994886;

		private readonly ImgFlipAPISource _imgFlipClient;

		private readonly string _imgFlipUsername;
		private readonly string _imgFlipPassword;

		public IConfiguration Configuration { get; set; }

		public ImgFlipService(IConfiguration configuration)
		{
			Configuration = configuration;
			_imgFlipClient = ImgFlipAPISource.Instance;

			_imgFlipUsername = Configuration["credentials:imgFlipUser"];
			_imgFlipPassword = Configuration["credentials:imgFlipPassword"];
		}

		public async Task<string> CreateMarekFace(string topText, string bottomText)
		{
			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(MarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}

		public async Task<string> CreateLaughingMarekMeme(string topText, string bottomText)
		{
			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(LaughingMarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}

		public async Task<string> CreateSkeletorMarekMeme(string topText, string bottomText)
		{
			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(SkeletorMarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}

		public async Task<string> CreateDrakeMarekMeme(string topText, string bottomText)
		{
			List<TextBox> textBoxes = new List<TextBox>
			{
				new TextBox()
				{
					text = topText,
					x = 320,
					y = 10,
					height = 350,
					width = 300,
					color = "#ffffff",
					outline_color = "#000000"
				},

				new TextBox()
				{
					text = bottomText,
					x = 320,
					y = 220,
					height = 300,
					width = 300,
					color = "#ffffff",
					outline_color = "#000000"
				}
			};

			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(DrakeMarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText, textBoxes.ToArray());

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}
	}
}