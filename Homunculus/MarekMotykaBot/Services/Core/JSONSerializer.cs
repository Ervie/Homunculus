using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarekMotykaBot.Services.Core
{
    public class JSONSerializerService: IDiscordService
    {
        private readonly string resourcesPath;

        public IConfiguration Configuration { get; set; }

        public JSONSerializerService(IConfiguration configuration)
        {
            Configuration = configuration;

            resourcesPath = AppContext.BaseDirectory + configuration["configValues:resourcePath"];
        }

		public T LoadSingleFromFile<T>(string fileName)
		{
			var text = File.ReadAllText(resourcesPath + fileName, Encoding.UTF8);

			var deserialized = JsonConvert.DeserializeObject<T>(text);

			return deserialized;
		}

		public List<T> LoadFromFile<T>(string fileName)
        {
            var text = File.ReadAllText(resourcesPath + fileName, Encoding.UTF8);

            var deserializedList = JsonConvert.DeserializeObject<List<T>>(text);

            return deserializedList ?? new List<T>();
        }

		public void SaveSingleToFile<T>(string fileName, T dataToSave)
		{
			var json = JsonConvert.SerializeObject(dataToSave);
			File.WriteAllText(resourcesPath + fileName, json, Encoding.UTF8);
		}

		public void SaveToFile<T>(string fileName, List<T> dataToSave)
        {
            var json = JsonConvert.SerializeObject(dataToSave);
            File.WriteAllText(resourcesPath + fileName, json, Encoding.UTF8);
        }
    }
}