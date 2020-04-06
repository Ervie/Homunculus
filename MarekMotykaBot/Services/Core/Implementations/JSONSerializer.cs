using MarekMotykaBot.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core
{
	public class JSONSerializerService : IDiscordService, IJSONSerializerService
	{
		private readonly string _resourcesPath;

		public IConfiguration Configuration { get; set; }

		public JSONSerializerService(IConfiguration configuration)
		{
			Configuration = configuration;

			_resourcesPath = AppContext.BaseDirectory + configuration["configValues:resourcePath"];
		}

		public async Task<T> LoadSingleFromFileAsync<T>(string fileName)
		{
			var text = await File.ReadAllTextAsync(_resourcesPath + fileName, Encoding.UTF8);
			return JsonConvert.DeserializeObject<T>(text);
		}

		public List<T> LoadFromFile<T>(string fileName)
		{
			var text = File.ReadAllText(_resourcesPath + fileName, Encoding.UTF8);
			var deserializedList = JsonConvert.DeserializeObject<List<T>>(text);

			return deserializedList ?? new List<T>();
		}

		public async Task<List<T>> LoadFromFileAsync<T>(string fileName)
		{
			var text = await File.ReadAllTextAsync(_resourcesPath + fileName, Encoding.UTF8);
			var deserializedList = JsonConvert.DeserializeObject<List<T>>(text);

			return deserializedList ?? new List<T>();
		}

		public async Task SaveSingleToFileAsync<T>(string fileName, T dataToSave)
		{
			var json = JsonConvert.SerializeObject(dataToSave);
			await File.WriteAllTextAsync(_resourcesPath + fileName, json, Encoding.UTF8);
		}

		public async Task SaveToFileAsync<T>(string fileName, List<T> dataToSave)
		{
			var json = JsonConvert.SerializeObject(dataToSave);
			await File.WriteAllTextAsync(_resourcesPath + fileName, json, Encoding.UTF8);
		}
	}
}