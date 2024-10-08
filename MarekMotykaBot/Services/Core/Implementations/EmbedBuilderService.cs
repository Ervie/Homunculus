﻿using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Implementations
{
	internal class EmbedBuilderService : IDiscordService, IEmbedBuilderService
	{
		private static readonly Color BlueSidebarColor = new Color(114, 137, 218);
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

		public async Task<Embed> BuildStreamMondayScheduleAsync()
		{
			var schedule = await _serializer.LoadSingleFromFileAsync<StreamMondayBacklog>("streamMonday.json");

			DateTime today = DateTime.Today;
			int daysUntilStreamDay = ((int)schedule.DayOfTheStream - (int)today.DayOfWeek + 7) % 7;
			DateTime nexStreamDay = today.AddDays(daysUntilStreamDay);

			var builder = new EmbedBuilder()
			{
				Color = BlueSidebarColor,
				Description = StringConsts.Backlog
			};

			if (schedule != null && schedule.BacklogEntries != null && schedule.BacklogEntries.Any())
			{
				var formattedEntries = schedule.BacklogEntries.Select(x => x.FormatForEmbedded());

				builder.AddField(x =>
				{
					x.Name = $"{nexStreamDay.ToString("dd.MM")} ({schedule.HourOfTheStream}:00)";
					x.Value = string.Join(Environment.NewLine, formattedEntries.ToArray());
					x.IsInline = false;
				});
			}

			return builder.Build();
		}

		public async Task<Embed> BuildSwearWordCountRankingAsync()
		{
			StringBuilder sb = new StringBuilder();

			var counterList = await _serializer.LoadFromFileAsync<WordCounterEntry>("wordCounter.json");

			var builder = new EmbedBuilder()
			{
				Color = BlueSidebarColor,
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

		public Embed BuildQuote(Quote quote)
		{
			var builder = new EmbedBuilder()
			{
				Color = BlueSidebarColor,
			};

			builder.WithFooter(quote.Author);
			builder.WithTitle(quote.QuoteBody);

			return builder.Build();
		}

		public Embed BuildAbout()
		{
			var sb = new StringBuilder();

			var builder = new EmbedBuilder()
			{
				Color = BlueSidebarColor,
				Description = StringConsts.About
			};

			builder.AddField(x =>
			{
				x.Name = StringConsts.VersionHeader;
				x.Value = Assembly
					.GetEntryAssembly()
					.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
					.InformationalVersion;
				x.IsInline = true;
			});

			return builder.Build();
		}

		public async Task<Embed> BuildCommandListAsync(IEnumerable<ModuleInfo> moduleInfos, SocketCommandContext commandContext)
		{
			var sb = new StringBuilder();

			string prefix = Configuration["prefix"];

			var builder = new EmbedBuilder()
			{
				Color = BlueSidebarColor,
				Description = StringConsts.ListCommands
			};

			foreach (var module in moduleInfos)
			{
				string description = null;
				foreach (var command in module.Commands)
				{
					var result = await command.CheckPreconditionsAsync(commandContext);
					if (result.IsSuccess)
					{
						foreach (string alias in command.Aliases)
						{
							sb.Append(prefix);
							sb.Append(alias);

							if (alias != command.Aliases.Last())
								sb.Append(", ");
						}

						if (command.Parameters.Count > 0)
						{
							sb.Append(" (");

							foreach (Discord.Commands.ParameterInfo parameter in command.Parameters)
							{
								sb.Append(parameter.Name);
								if (parameter != command.Parameters.Last())
									sb.Append(", ");
							}

							sb.Append(") ");
						}

						if (!string.IsNullOrWhiteSpace(command.Summary))
						{
							sb.Append(" - ");
							sb.Append(command.Summary);
						}

						description += $"{sb}\n";
						sb.Clear();
					}
				}

				if (!string.IsNullOrWhiteSpace(description))
				{
					builder.AddField(x =>
					{
						x.Name = module.Name;
						x.Value = description;
						x.IsInline = false;
					});
				}
			}

			return builder.Build();
		}

		public Embed BuildCharadeEntry(CharadeEntry charadeEntry)
		{
			var builder = new EmbedBuilder()
			{
				Color = BlueSidebarColor
			};

			builder.WithImageUrl(charadeEntry.Series.ImageUrl);

			builder.AddField(x =>
			{
				x.Name = StringConsts.Title;
				x.Value = charadeEntry.Series.Title;
				x.IsInline = true;
			});

			string translations = charadeEntry.Series.Translation.ListTranslationsWithNewLine(charadeEntry.Series.Title);

			if (!string.IsNullOrWhiteSpace(translations))
			{
				builder.AddField(x =>
				{
					x.Name = StringConsts.Translations;
					x.Value = translations;
					x.IsInline = false;
				});
			}

			if (charadeEntry.KnownBy.Any())
			{
				builder.AddField(x =>
				{
					x.Name = StringConsts.WatchedRead;
					x.Value = string.Join(Environment.NewLine, charadeEntry.KnownBy);
					x.IsInline = false;
				});
			}

			return builder.Build();
		}

		public Embed BuildLastContact(LastMarekMessage lastMessage)
		{
			var builder = new EmbedBuilder();

			int daysDifference = (DateTime.Now.Date - lastMessage.DatePosted.Date).Days;

			string footerSuffix = daysDifference switch
			{
				(0) => StringConsts.Today,
				(1) => StringConsts.Yesterday,
				_ => string.Format(StringConsts.DaysAgo, daysDifference),
			};
			builder.WithFooter(lastMessage.DatePosted.ToString("yyyy-MM-dd HH:mm") + ", " + footerSuffix);

			if (lastMessage.IsImage)
				builder.WithImageUrl(lastMessage.MessageContent);
			else
				builder.WithTitle(lastMessage.MessageContent.Truncate(250));

			return builder.Build();
		}

		public Embed BuildMapList(ICollection<string> maps)
		{
			var builder = new EmbedBuilder()
			{
				Color = BlueSidebarColor
			};

			if (maps is { } && maps.Any())
			{
				var mapsWithoutFileExtension = maps.Select(x => x.Remove(x.Length - 4)).ToArray();

				builder.AddField(x =>
				{
					x.Name = StringConsts.CurrentMapRotationHeader;
					x.Value = string.Join(Environment.NewLine, mapsWithoutFileExtension);
					x.IsInline = false;
				});
			}

			return builder.Build();
		}
	}
}