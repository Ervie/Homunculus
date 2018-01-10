using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class DropboxService
    {
        private readonly DropboxClient _client;

        private readonly IConfiguration _configuration;

        private readonly string resourcesPath;

        public DropboxService(IConfiguration configuration)
        {
            _configuration = configuration;

            resourcesPath = AppContext.BaseDirectory + configuration["configValues:resourcePath"];

            _client = new DropboxClient(configuration["tokens:dropbox"]);
        }

        public async Task DownloadFileAsync(string fileName, string folderPath = "")
        {
            using (var response = await _client.Files.DownloadAsync(folderPath + "/" + fileName))
            {
                using (var fileStream = File.Create(resourcesPath + "/" + fileName))
                {
                    (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
                }
            }
        }

        public async Task UploadFileAsync(string fileNameLocal, string fileNameRemote, string folderPath = "")
        {
            using (var file = new FileStream(resourcesPath + "/" + fileNameLocal, FileMode.OpenOrCreate))
            using (var mem = new MemoryStream())
            {
                file.CopyTo(mem);
                mem.Position = 0;
                var updated = await _client.Files.UploadAsync(
                    folderPath + "/" + fileNameRemote,
                    WriteMode.Overwrite.Instance,
                    body: mem);
            }
        }
    }
}