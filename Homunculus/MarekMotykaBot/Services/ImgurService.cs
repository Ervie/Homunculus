using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class ImgurService
    {
        private readonly IConfiguration _configuration;
        private readonly Random _rng;
        private readonly ImgurClient _client;

        public ImgurService(IConfiguration configuration, Random random)
        {
            _configuration = configuration;
            _rng = random;

            _client = new ImgurClient(_configuration["tokens:imgurClient"], _configuration["tokens:imgurSecret"]);
        }

        public async Task<string> GetRandomImageFromGallery(string galleryId)
        {
            var endpoint = new GalleryEndpoint(_client);
            var album = await endpoint.GetGalleryAlbumAsync(galleryId);

            int randomImageIndex = _rng.Next(1, album.Images.Count());

            var imageToPost = album.Images.ElementAt(randomImageIndex);

            return imageToPost.Link.Remove(imageToPost.Link.Length - 4);
        }

        public async Task<string> GetRandomImageFromAlbum(string albumId)
        {
            var endpoint = new AlbumEndpoint(_client);
            var album = await endpoint.GetAlbumAsync(albumId);

            int randomImageIndex = _rng.Next(1, album.Images.Count());

            var imageToPost = album.Images.ElementAt(randomImageIndex);

            return imageToPost.Link.Remove(imageToPost.Link.Length - 4);
        }
    }
}