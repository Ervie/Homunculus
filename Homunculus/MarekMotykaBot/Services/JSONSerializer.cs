using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarekMotykaBot.Services
{
    public class JSONSerializer
    {
        private readonly string resourcesPath;

        private readonly IConfiguration _configuration;

        public JSONSerializer(IConfiguration configuration)
        {
            _configuration = configuration;

            resourcesPath = AppContext.BaseDirectory + configuration["configValues:resourcePath"];
        }

        public List<T> LoadFromFile<T>(string fileName)
        {
            var text = File.ReadAllText(resourcesPath + fileName, Encoding.UTF8);

            var deserializedList = JsonConvert.DeserializeObject<List<T>>(text);

            return deserializedList ?? new List<T>();
        }

        public void SaveToFile<T>(string fileName, List<T> dataToSave)
        {
            var json = JsonConvert.SerializeObject(dataToSave);
            File.WriteAllText(resourcesPath + fileName, json, Encoding.UTF8);
        }
    }
}