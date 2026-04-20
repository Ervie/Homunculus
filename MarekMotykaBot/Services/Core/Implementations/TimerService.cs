using Cronos;
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
using System.Threading;
using System.Threading.Tasks;

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
		private readonly ulong _destinationServerId;
		private readonly ulong _destinationChannelId;
		private readonly ulong _streamAliasId;
		private readonly Dictionary<string, Func<Task>> _taskDispatch;

		private CancellationTokenSource _cts;
		private readonly object _schedulerLock = new object();

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

			_taskDispatch = new Dictionary<string, Func<Task>>
			{
				{ nameof(StreamMondaySchedule), StreamMondaySchedule },
				{ nameof(StreamMondayScheduleWithMention), StreamMondayScheduleWithMention },
				{ nameof(QuoteOfTheDay), QuoteOfTheDay },
				{ nameof(SwearWordCount), SwearWordCount },
				{ nameof(ResetUTMapRotation), () => { ResetUTMapRotation(); return Task.CompletedTask; } },
				{ nameof(NyaaWeeklySearch), NyaaWeeklySearch }
			};
		}

		public void StartTimer()
		{
			lock (_schedulerLock)
			{
				if (_cts != null)
				{
					return;
				}

				_cts = new CancellationTokenSource();
				_ = Task.Run(() => RunSchedulerLoopAsync(_cts.Token));
			}
		}

		private void RestartSchedulerLoop()
		{
			lock (_schedulerLock)
			{
				_cts?.Cancel();
				_cts?.Dispose();
				_cts = new CancellationTokenSource();
				_ = Task.Run(() => RunSchedulerLoopAsync(_cts.Token));
			}
		}

		private async Task RunSchedulerLoopAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					var now = DateTime.UtcNow;
					DateTime? earliest = null;

					foreach (var task in TimedTasks)
					{
						var cron = CronExpression.Parse(task.CronExpression);
						var next = cron.GetNextOccurrence(now, TimeZoneInfo.Local);
						if (next.HasValue && (!earliest.HasValue || next.Value < earliest.Value))
						{
							earliest = next.Value;
						}
					}

					if (!earliest.HasValue)
					{
						await Task.Delay(TimeSpan.FromHours(1), token);
						continue;
					}

					var delay = earliest.Value - DateTime.UtcNow;
					if (delay > TimeSpan.Zero)
					{
						await Task.Delay(delay, token);
					}

					if (token.IsCancellationRequested)
					{
						break;
					}

					var fireTime = DateTime.UtcNow;
					foreach (var task in TimedTasks)
					{
						var cron = CronExpression.Parse(task.CronExpression);
						var next = cron.GetNextOccurrence(now, TimeZoneInfo.Local);
						if (next.HasValue && next.Value <= fireTime && _taskDispatch.TryGetValue(task.Name, out var action))
						{
							try
							{
								await action();
							}
							catch
							{
							}
						}
					}
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch
				{
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
		var streamMonday = await _serializer.LoadSingleFromFileAsync<StreamMondayBacklog>("streamMonday.json");
		if (streamMonday.BacklogEntries == null)
		{
			streamMonday.BacklogEntries = new List<BacklogEntry>();
		}

		var trackedEntries = await _serializer.LoadFromFileAsync<NyaaBacklogEntry>("nyaaBacklog.json") ?? new List<NyaaBacklogEntry>();

		foreach (var entry in trackedEntries)
		{
			var phrase = entry.SearchPhrase;
			if (string.IsNullOrWhiteSpace(phrase))
			{
				continue;
			}

			string lastKnownTitle = entry.LastKnownTitle;

			try
			{
				var newResults = await _nyaaService
					.GetNewTorrentDownloadsSinceAsync(phrase, lastKnownTitle, 20);

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

		await _serializer.SaveToFileAsync("nyaaBacklog.json", trackedEntries).ConfigureAwait(false);

		var completeEntries = await _serializer.LoadFromFileAsync<NyaaCompleteBacklogEntry>("nyaaCompleteBacklog.json") ?? new List<NyaaCompleteBacklogEntry>();

		foreach (var entry in completeEntries)
		{
			if (entry.NextEpisodeNumber > entry.TotalEpisodes)
			{
				continue;
			}

			string episodeNumber = entry.NextEpisodeNumber.ToString().PadLeft(2, '0');
			string episodeName = $"{entry.DisplayName}ep{episodeNumber}";

			if (!streamMonday.BacklogEntries.Any(x => x.Name == episodeName))
			{
				streamMonday.BacklogEntries.Add(new BacklogEntry(episodeName, entry.TorrentUrl));
			}

			entry.NextEpisodeNumber++;
			entry.LastUpdated = DateTime.UtcNow;
		}

		await _serializer.SaveToFileAsync("nyaaCompleteBacklog.json", completeEntries).ConfigureAwait(false);
		await _serializer.SaveSingleToFileAsync("streamMonday.json", streamMonday).ConfigureAwait(false);
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

		public async Task ChangeStreamDayAsync(DayOfWeek dayOfWeek, int? hour)
		{
			int cronDow = (int)dayOfWeek;

			foreach (var task in TimedTasks.Where(t => t.Name.StartsWith("StreamMondaySchedule")))
			{
				var parts = task.CronExpression.Split(' ');
				parts[4] = cronDow.ToString();

				if (hour.HasValue)
				{
					var newHour = task.Name.EndsWith("WithMention") ? hour.Value - 1 : hour.Value - 4;
					parts[1] = newHour.ToString();
				}

				task.CronExpression = string.Join(" ", parts);
			}

			foreach (var task in TimedTasks.Where(t => t.Name == nameof(NyaaWeeklySearch)))
			{
				var parts = task.CronExpression.Split(' ');
				parts[4] = cronDow.ToString();

				if (hour.HasValue)
				{
					parts[1] = (hour.Value - 5).ToString();
				}

				task.CronExpression = string.Join(" ", parts);
			}

			await _serializer.SaveToFileAsync<TimedTask>("timedTasks.json", TimedTasks.ToList());

			RestartSchedulerLoop();
		}
	}
}
