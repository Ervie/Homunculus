using Discord;
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
		private static readonly Color blueSidebarColor = new Color(114, 137, 218);
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

			DateTime today = DateTime.Today;
			int daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7;
			DateTime nextWednesday = today.AddDays(daysUntilWednesday);

			var builder = new EmbedBuilder()
			{
				Color = blueSidebarColor,
				Description = StringConsts.Backlog
			};

			if (schedule != null && schedule.Any())
			{
				builder.AddField(x =>
				{
					x.Name = nextWednesday.ToString("dd.MM");
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
				Color = blueSidebarColor,
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
				Color = blueSidebarColor,
			};

			builder.WithFooter(quote.Author);
			builder.WithTitle(quote.QuoteBody);

			return builder.Build();
		}

		public Embed BuildAbout()
		{
			StringBuilder sb = new StringBuilder();

			var builder = new EmbedBuilder()
			{
				Color = blueSidebarColor,
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
			StringBuilder sb = new StringBuilder();

			string prefix = Configuration["prefix"];

			var builder = new EmbedBuilder()
			{
				Color = blueSidebarColor,
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

						description += $"{sb.ToString()}\n";
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
				Color = blueSidebarColor
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
				Color = blueSidebarColor
			};

			if (maps is { } && maps.Any())
			{
				builder.AddField(x =>
				{
					x.Name = StringConsts.CurrentMapRotationHeader;
					x.Value = string.Join(Environment.NewLine, maps.ToArray());
					x.IsInline = false;
				});
			}

			return builder.Build();
		}
	}
}