using ImgFlipAPI.APISource.Core;
using ImgFlipAPI.APISource.Core.Models;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External
{
	public class ImgFlipService : IDiscordService, IImgFlipService
	{
		private const int MarekTemplateId = 114558777;
		private const int LaughingMarekTemplateId = 152110002;
		private const int SkeletorMarekTemplateId = 156362598;
		private const int DrakeMarekTemplateId = 160994886;
		private const int NosaczTemplateId = 119511407;

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

		public async Task<string> CreateMarekFace(params string[] text)
		{
			string topText, bottomText;

			(topText, bottomText) = FormatMemeText(text);

			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(MarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}

		public async Task<string> CreateLaughingMarekMeme(params string[] text)
		{
			string topText, bottomText;

			(topText, bottomText) = FormatMemeText(text);

			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(LaughingMarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}

		public async Task<string> CreateSkeletorMarekMeme(params string[] text)
		{
			string topText, bottomText;

			(topText, bottomText) = FormatMemeText(text);

			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(SkeletorMarekTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}

		public async Task<string> CreateDrakeMarekMeme(params string[] text)
		{
			string topText, bottomText;

			(topText, bottomText) = FormatMemeText(text);

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

		public async Task<string> CreateNosaczMeme(params string[] text)
		{
			string topText, bottomText;

			(topText, bottomText) = FormatMemeText(text);

			CaptionMemeRoot freshMeme = await _imgFlipClient.CaptionMemeAsync(NosaczTemplateId, _imgFlipUsername, _imgFlipPassword, topText, bottomText);

			return freshMeme.success ? freshMeme.data.url : string.Empty;
		}

		private (string topText, string bottomText) FormatMemeText(params string[] text)
		{
			var captions = string.Join(" ", text).Split(';').ToList();

			if (captions.Count < 2)
				return ("", "");

			for (int i = 0; i < captions.Count; i++)
			{
				captions[i] = captions[i].RemoveEmojisAndEmotes();
			}


			if (string.IsNullOrWhiteSpace(captions[0]) || string.IsNullOrWhiteSpace(captions[1]))
				return ("", "");

			return (captions[0].ToUpper(), captions[1].ToUpper()); ;
		}
	}
}