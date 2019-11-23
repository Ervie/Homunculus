using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MarekMotykaBot.Services.External.Implementations
{
	public class UnrealTournamentService : IDiscordService, IUnrealTournamentService
	{
		public IConfiguration Configuration { get; set; }

		public UTRotationConfiguration RotationConfiguration { get; set; }

		private readonly IJSONSerializerService _jsonSerializer;

		private const string _mapsLinePrefix = "Maps[";
		private const string _iniFileSubPath = "/System/UnrealTournament.ini";
		private const string _mapsCatalogSubPath = "/Maps";
		private const string _utServerRestartCommand = "/./ucc.init restart";
		private const int _maxMapRotationSize = 32;

		public UnrealTournamentService(IConfiguration configuration, IJSONSerializerService jSONSerializerService)
		{
			Configuration = configuration;
			_jsonSerializer = jSONSerializerService;
		}

		public void ChangeRotation(UTRotationConfiguration rotationConfiguration)
		{
			RotationConfiguration = rotationConfiguration;

			var availableMaps = LoadMapsNamesFromMapFolder();

			var mapsFromCurrentRotation = LoadMapsFromCurrentRotation();

			var mapsToExclude = LoadExcludedMapsList();

			var newRotation = SelectNewRotation(availableMaps, mapsFromCurrentRotation, mapsToExclude);

			SaveNewRotationToIniFile(newRotation);

			RestartUTServer();
		}

		private ICollection<string> LoadMapsNamesFromMapFolder()
			=> Directory
				.GetFiles(string.Concat(Configuration["configValues:UTfolderPath"], _mapsCatalogSubPath), "*.unr", SearchOption.AllDirectories)
				.Select(map => Path.GetFileName(map))
				.Where(mapName => mapName.StartsWith("DM"))
				.ToList();


		private ICollection<string> LoadMapsFromCurrentRotation()
			=> File
				.ReadAllLines(string.Concat(Configuration["configValues:UTfolderPath"], _iniFileSubPath))
				.Where(line => line.StartsWith(_mapsLinePrefix))
				.Select(line => line.Split('=')?[1])
				.Where(mapName => mapName is { })
				.ToList();

		private ICollection<string> LoadExcludedMapsList()
			=> RotationConfiguration.ExcludeMaps ?
				new List<string>() :
				_jsonSerializer.LoadFromFile<string>("excludedMaps.json");

		private ICollection<string> SelectNewRotation(ICollection<string> availableMaps, ICollection<string> oldRotation, ICollection<string> mapsToExclude)
		{
			Random rng = new Random();
			List<string> newRotation = new List<string>();

			int maxPossibleMapCount = RotationConfiguration.Repeat ?
				Math.Min(_maxMapRotationSize, availableMaps.Count) :
				Math.Min(_maxMapRotationSize, availableMaps.Count - oldRotation.Count);

			while (newRotation.Count < maxPossibleMapCount && availableMaps.Any())
			{
				string selectedMap = availableMaps.ElementAt(rng.Next(0, availableMaps.Count));

				if (newRotation.Contains(selectedMap))
					continue;

				if (!RotationConfiguration.Repeat && oldRotation.Contains(selectedMap))
					continue;

				if (mapsToExclude.Contains(selectedMap))
					continue;

				newRotation.Add(selectedMap);
				availableMaps.Remove(selectedMap);
			}

			return newRotation;
		}

		private void SaveNewRotationToIniFile(ICollection<string> newRotation)
		{
			if (!(newRotation is { }) || !newRotation.Any())
				return;

			string[] configFileLines = File.ReadAllLines(string.Concat(Configuration["configValues:UTfolderPath"], _iniFileSubPath));

			for (int i = 0; i < configFileLines.Length; i++)
			{
				if (configFileLines[i].StartsWith(_mapsLinePrefix))
				{
					configFileLines[i] = string.Concat(configFileLines[i].Split('=')[0], "=", newRotation.ElementAt(i % _maxMapRotationSize) ?? string.Empty);
				}
			}

			File.WriteAllLines(string.Concat(Configuration["configValues:UTfolderPath"], _iniFileSubPath), configFileLines);
		}

		private void RestartUTServer()
		{
			using Process process = new Process()
			{
				StartInfo = new ProcessStartInfo
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					FileName = "/bin/bash",
					Arguments = string.Concat(Configuration["configValues:UTfolderPath"], _utServerRestartCommand)
				}
			};
			process.Start();
			process.WaitForExit();
		}
	}
}