using Imgur.API.Authentication;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External
{
	public class ImgurService : IDiscordService, IImgurService
	{
		private readonly Random _rng;
		private readonly IImgurClient _imgurClient;

		public IConfiguration Configuration { get; set; }

		public ImgurService(IConfiguration configuration, Random random)
		{
			Configuration = configuration;
			_rng = random;

			_imgurClient = new ImgurClient(Configuration["tokens:imgurClient"], Configuration["tokens:imgurSecret"]);
		}

		public async Task<string> GetRandomImageFromGallery(string galleryId)
		{
			var endpoint = new GalleryEndpoint(_imgurClient);
			var album = await endpoint.GetGalleryAlbumAsync(galleryId);

			int randomImageIndex = _rng.Next(1, album.Images.Count());

			var imageToPost = album.Images.ElementAt(randomImageIndex);

			return imageToPost.Link.Remove(imageToPost.Link.Length - 4);
		}

		public async Task<string> GetRandomImageFromAlbum(string albumId)
		{
			var endpoint = new AlbumEndpoint(_imgurClient);
			var album = await endpoint.GetAlbumAsync(albumId);

			int randomImageIndex = _rng.Next(1, album.Images.Count());

			var imageToPost = album.Images.ElementAt(randomImageIndex);

			return imageToPost.Link.Remove(imageToPost.Link.Length - 4);
		}
	}
}