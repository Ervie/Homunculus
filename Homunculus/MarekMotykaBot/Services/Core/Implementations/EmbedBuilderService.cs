using Discord;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Implementations
{
	class EmbedBuilderService : IDiscordService, IEmbedBuilderService
	{

		private readonly IJSONSerializerService _serializer;

		public IConfiguration Configuration { get; set; }

		public EmbedBuilderService(
			IConfiguration configuration,
			IJSONSerializerService serializer
			)
		{
			Configuration = configuration;
			_serializer = serializer;
		}

		public Embed BuildStreamMondaySchedule()
		{
			List<string> schedule = _serializer.LoadFromFile<string>("streamMonday.json");

			var builder = new EmbedBuilder();

			DateTime today = DateTime.Today;
			int daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7;
			DateTime nextWednesday = today.AddDays(daysUntilWednesday);

			if (schedule != null && schedule.Any())
			{
				builder.AddField(x =>
				{
					x.Name = "Rozkładówka (backlog) na " + nextWednesday.ToString("dd.MM");
					x.Value = string.Join(Environment.NewLine, schedule.ToArray());
					x.IsInline = false;
				});
			}

			return builder.Build();
		}

		public Embed BuildSwearWordCountRanking()
		{
			StringBuilder sb = new StringBuilder();

			var counterList = _serializer.LoadFromFile<WordCounterEntry>("wordCounter.json");

			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = StringConsts.SwearWordCounterHeader
			};

			foreach (string swearWord in counterList.Select(x => x.Word).Distinct())
			{
				var specificSwearWordEntries = counterList.Where(x => x.Word.Equals(swearWord)).OrderByDescending(x => x.CounterValue);

				foreach (var entry in specificSwearWordEntries)
				{
					sb.AppendLine(string.Format(StringConsts.SwearWordCounterEntry, entry.DiscordNickname, entry.Word, entry.CounterValue));
				}

				if (!string.IsNullOrEmpty(sb.ToString()))
					builder.AddField(x =>
					{
						x.Name = swearWord;
						x.Value = sb.ToString();
						x.IsInline = true;
					});

				sb.Clear();
			}

			return builder.Build();
		}
	}
}
