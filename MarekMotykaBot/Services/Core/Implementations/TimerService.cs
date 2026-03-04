using Discord;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.DataTypes.Enumerations;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace MarekMotykaBot.Services.Core
{
	public class TimerService : IDiscordService, ITimerService
	{
		private readonly IJSONSerializerService _serializer;
		private readonly IEmbedBuilderService _embedBuilderService;
		private readonly IUnrealTournamentService _unrealTournamentService;
		private readonly INyaaService _nyaaService;
		private readonly DiscordSocketClient _client;
		private readonly Random _rng;
		private readonly Timer _timer;
		private readonly ulong _destinationServerId;
		private readonly ulong _destinationChannelId;
		private readonly ulong _streamAliasId;

		public IConfiguration Configuration { get; set; }

		private ICollection<TimedTask> TimedTasks { get; set; }

		public TimerService(
			IConfiguration configuration,
			IJSONSerializerService serializer,
			IEmbedBuilderService statisticsService,
			IUnrealTournamentService unrealTournamentService,
			INyaaService nyaaService,
			DiscordSocketClient client,
			Random rng
		)
		{
			Configuration = configuration;
			_serializer = serializer;
			_client = client;
			_embedBuilderService = statisticsService;
			_unrealTournamentService = unrealTournamentService;
			_nyaaService = nyaaService;
			_rng = rng;

			_destinationServerId = (ulong)long.Parse(Configuration["tokens:destinationServerId"]);
			_destinationChannelId = (ulong)long.Parse(Configuration["tokens:destinationChannelId"]);
			_streamAliasId = (ulong)long.Parse(Configuration["configValues:streamAliasId"]);

			TimedTasks = _serializer.LoadFromFile<TimedTask>("timedTasks.json");

			_timer = new Timer(60 * 1000);

			_timer.Elapsed += new ElapsedEventHandler(TimerTick);
		}

		public void StartTimer()
		{
			if (!_timer.Enabled)
				_timer.Start();
		}

		public void TimerTick(object src, ElapsedEventArgs e)
		{
			DateTime currentDateTime = DateTime.Now;

			foreach (TimedTask task in TimedTasks)
			{
				if (task.Hours.Contains(currentDateTime.Hour) &&
					task.Minutes.Contains(currentDateTime.Minute) &&
					(task.DaysOfWeek.Contains(currentDateTime.DayOfWeek) || task.DaysOfMonth.Contains(currentDateTime.Day)))
				{
					GetType().GetMethod(task.Name).Invoke(this, null);
				}
			}
		}

		public async Task StreamMondaySchedule()
		{
			var channelToPost = _client.GetChannel(_destinationChannelId) as IMessageChannel;

			var streamBacklog = await _embedBuilderService.BuildStreamMondayScheduleAsync();

			await channelToPost.SendMessageAsync("", false, streamBacklog);
		}

		public async Task StreamMondayScheduleWithMention()
		{
			var channelToPost = _client.GetChannel(_destinationChannelId) as IMessageChannel;

			var guild = _client.Guilds.FirstOrDefault(x => x.Id.Equals(_destinationServerId));

			if (guild != null)
			{
				var alias = guild.Roles.FirstOrDefault(x => x.Id.Equals(_streamAliasId));

				if (alias != null)
				{
					await channelToPost.SendMessageAsync(alias.Mention);
				}
			}

			var streamBacklog = await _embedBuilderService.BuildStreamMondayScheduleAsync();

			await channelToPost.SendMessageAsync("", false, streamBacklog);
		}

		public async Task QuoteOfTheDay()
		{
			var allQuotes = await _serializer.LoadFromFileAsync<Quote>("quotes.json");
			var availableQuotes = allQuotes
				.Where(x => x.Categories.Contains(QuoteCategory.Wisdom) || x.Categories.Contains(QuoteCategory.Thought))
				.ToList();

			var randomQuoteIndex = _rng.Next(0, availableQuotes.Count);
			var selectedQuote = availableQuotes[randomQuoteIndex];

			await _serializer.SaveToFileAsync("quoteOfTheDay.json", new List<Quote> { selectedQuote });

			var channelToPost = _client.GetChannel(_destinationChannelId) as IMessageChannel;

			await channelToPost.SendMessageAsync(StringConsts.QuoteForToday);
			await Task.Delay(1000);
			await channelToPost.SendMessageAsync("", false, _embedBuilderService.BuildQuote(selectedQuote));
		}

		public async Task SwearWordCount()
		{
			var channelToPost = _client.GetChannel(_destinationChannelId) as IMessageChannel;
			var swearWordCountRanking = await _embedBuilderService.BuildSwearWordCountRankingAsync();

			await channelToPost.SendMessageAsync("", false, swearWordCountRanking);
		}

		public void ResetUTMapRotation() 
			=> _unrealTournamentService.ChangeRotation(new UTRotationConfiguration()
				{
					Repeat = false,
					ExcludeMaps = true
				});

		public async Task NyaaWeeklySearch()
		{
			var trackedEntries = await _serializer.LoadFromFileAsync<NyaaBacklogEntry>("nyaaBacklog.json") ?? new List<NyaaBacklogEntry>();
			if (trackedEntries.Count == 0)
				return;

			var streamMonday = await _serializer.LoadSingleFromFileAsync<StreamMondayBacklog>("streamMonday.json");
			if (streamMonday.BacklogEntries == null)
				streamMonday.BacklogEntries = new List<BacklogEntry>();

			foreach (var entry in trackedEntries)
			{
				var phrase = entry.SearchPhrase;
				if (string.IsNullOrWhiteSpace(phrase))
					continue;

				string lastKnownTitle = entry.LastKnownTitle;

				try
				{
					var newResults = await _nyaaService
						.GetNewTorrentDownloadsSinceAsync(phrase, lastKnownTitle, 20)
						.ConfigureAwait(false);

					if (newResults == null || newResults.Count == 0)
					{
						continue;
					}

					// Add new episodes to StreamMonday backlog (oldest first for natural ordering)
					foreach (var result in newResults.Reverse())
					{
						string suffix = GetEpisodeSuffixFromTitle(result.Title);
						string episodeName;

						if (!string.IsNullOrWhiteSpace(entry.DisplayName) && !string.IsNullOrWhiteSpace(suffix))
						{
							episodeName = $"{entry.DisplayName} {suffix}";
						}
						else if (!string.IsNullOrWhiteSpace(entry.DisplayName))
						{
							episodeName = entry.DisplayName;
						}
						else
						{
							episodeName = GetEpisodeNameFromTitle(result.Title) ?? result.Title;
						}

						if (!streamMonday.BacklogEntries.Any(x => x.Name == episodeName))
						{
							streamMonday.BacklogEntries.Add(new BacklogEntry(episodeName, result.TorrentUrl));
						}
					}

					// Update entry with most recent result
					var newest = newResults[0];
					entry.LastKnownTitle = newest.Title;
					entry.LastUpdated = DateTime.UtcNow;
				}
				catch
				{
					// Skip failed phrase and continue with the rest
				}

				await Task.Delay(500).ConfigureAwait(false);
			}

			await _serializer.SaveSingleToFileAsync("streamMonday.json", streamMonday).ConfigureAwait(false);
			await _serializer.SaveToFileAsync("nyaaBacklog.json", trackedEntries).ConfigureAwait(false);
		}

		private static string GetEpisodeNameFromTitle(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				return title;
			}

			// Strip leading group tags like [ASW]
			string cleaned = Regex.Replace(title, @"^\[[^\]]+\]\s*", string.Empty).Trim();

			// Extract the main name and episode number: "Show Name S2 - 03 ..." or "Show Name - 21 ..."
			var match = Regex.Match(cleaned, @"^(?<name>.+?)\s*(?<season>S\d+)?\s*-\s*(?<ep>\d{1,3})\b");
			if (!match.Success)
			{
				return null;
			}

			string name = match.Groups["name"].Value.Trim();
			string seasonGroup = match.Groups["season"].Success ? match.Groups["season"].Value.TrimStart('S', 's') : null;
			string episodeNumber = match.Groups["ep"].Value.TrimStart('0');
			if (string.IsNullOrEmpty(episodeNumber))
			{
				episodeNumber = match.Groups["ep"].Value; // keep original if all zeros
			}

			if (!string.IsNullOrEmpty(seasonGroup))
			{
				return $"{name} s{seasonGroup}ep{episodeNumber}";
			}

			return $"{name} ep{episodeNumber}";
		}

		private static string GetEpisodeSuffixFromTitle(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				return null;
			}

			string cleaned = Regex.Replace(title, @"^\[[^\]]+\]\s*", string.Empty).Trim();
			var match = Regex.Match(cleaned, @"^(?<name>.+?)\s*(?<season>S\d+)?\s*-\s*(?<ep>\d{1,3})\b");
			if (!match.Success)
			{
				return null;
			}

			string seasonGroup = match.Groups["season"].Success ? match.Groups["season"].Value.TrimStart('S', 's') : null;
			string episodeNumber = match.Groups["ep"].Value.TrimStart('0');
			if (string.IsNullOrEmpty(episodeNumber))
			{
				episodeNumber = match.Groups["ep"].Value;
			}

			if (!string.IsNullOrEmpty(seasonGroup))
			{
				return $"s{seasonGroup}ep{episodeNumber}";
			}

			return $"ep{episodeNumber}";
		}

		public void ChangeStreamDay(DayOfWeek dayOfWeek, int? hour)
		{
			var streamMondayTasks = TimedTasks.Where(x => x.Name.StartsWith("StreamMondaySchedule"));

			foreach (var streamTask in streamMondayTasks)
			{
				streamTask.DaysOfWeek.Clear();
				streamTask.DaysOfWeek.Add(dayOfWeek);

				if (hour.HasValue)
				{
					var newReminderHour = streamTask.Name.EndsWith("WithMention") ? hour.Value - 1 : hour.Value - 4;

					streamTask.Hours.Clear();
					streamTask.Hours.Add(newReminderHour);
				}
			}

			// Keep Nyaa weekly search in sync: it should run 1 hour before StreamMondayScheduleWithMention
			var nyaaTasks = TimedTasks.Where(x => x.Name == nameof(NyaaWeeklySearch));
			foreach (var nyaaTask in nyaaTasks)
			{
				nyaaTask.DaysOfWeek.Clear();
				nyaaTask.DaysOfWeek.Add(dayOfWeek);

				if (hour.HasValue)
				{
					var nyaaHour = hour.Value - 5;

					nyaaTask.Hours.Clear();
					nyaaTask.Hours.Add(nyaaHour);
				}
			}
		}
	}
}