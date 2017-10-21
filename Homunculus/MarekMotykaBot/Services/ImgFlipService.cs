﻿using ImgFlipAPI.APISource.Core;
using ImgFlipAPI.APISource.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class ImgFlipService
    {
        private const int MarekTemplateId = 114558777;

        private readonly ImgFlipAPISource _source;
        private readonly IConfiguration _configuration;

        private readonly string _imgFlipUsername;
        private readonly string _imgFlipPassword;

        public ImgFlipService(IConfiguration configuration)
        {
            _configuration = configuration;
            _source = ImgFlipAPISource.Instance;

            _imgFlipUsername = _configuration["credentials:imgFlipUser"];
            _imgFlipPassword = _configuration["credentials:imgFlipPassword"];
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