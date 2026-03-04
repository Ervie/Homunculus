using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class AdminModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly IJSONSerializerService _serializer;
		private readonly IEmbedBuilderService _embedBuilderService;
		private readonly IUnrealTournamentService _unrealTournamentService;
		private readonly ITimerService _timerService;

		public string ModuleName { get => "AdminModule"; }

		public ILoggingService LoggingService { get; }

		public AdminModule(
			IJSONSerializerService serializer,
			ILoggingService loggingService,
			IEmbedBuilderService embedBuilderService,
			IUnrealTournamentService unrealTournamentService,
			ITimerService timerService
			)
		{
			_serializer = serializer;
			_embedBuilderService = embedBuilderService;
			_unrealTournamentService = unrealTournamentService;
			_timerService = timerService;
			LoggingService = loggingService;
		}

		[Command("Penis"), Summary("This is a Christian server!"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task SwearWordCounterAsync()
		{
			await ReplyAsync("", false, await _embedBuilderService.BuildSwearWordCountRankingAsync());

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("addSMEntry"), Alias("sma"), Summary("Add entry to StreamMonday schedule"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task AddEntryToStreamBacklog(params string[] text)
		{
			var entry = FormatStreamMondayEntryText(text);

			if (string.IsNullOrWhiteSpace(entry.Name))
				return;

			var schedule = await _serializer.LoadSingleFromFileAsync<StreamMondayBacklog>("streamMonday.json");

			if (!schedule.BacklogEntries.Select(x => x.Name).Contains(entry.Name))
			{
				schedule.BacklogEntries.Add(entry);

				await _serializer.SaveSingleToFileAsync("streamMonday.json", schedule);

				await ReplyAsync(string.Format(StringConsts.Added, entry.Name));
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, entry.Name);
		}

		[Command("removeSMEntry"), Alias("smr"), Summary("Remove entry from StreamMonday schedule"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task RemoveEntryFromStreamBacklog(params string[] text)
		{
			string entryName = string.Join(" ", text);

			var schedule = await _serializer.LoadSingleFromFileAsync<StreamMondayBacklog>("streamMonday.json");

			if (schedule.BacklogEntries.Select(x => x.Name).Contains(entryName))
			{
				var entryToRemove = schedule.BacklogEntries.First(x => x.Name == entryName);

				schedule.BacklogEntries.Remove(entryToRemove);

				await _serializer.SaveSingleToFileAsync("streamMonday.json", schedule);

				await ReplyAsync(string.Format(StringConsts.Removed, entryName));
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, entryName);
		}

		[Command("changeSMDay"), Alias("smd"), Summary("Change day of the SM (1-7)"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task ChangeStreamDay(params string[] text)
		{
			Enum.TryParse(text.FirstOrDefault(), out DayOfWeek newDayOfWeek);
			int? newHour = int.TryParse(text.LastOrDefault(), out int result) ? result : (int?)null;
	
			if (newDayOfWeek is { })
			{
				_timerService.ChangeStreamDay(newDayOfWeek, newHour);
				var schedule = await _serializer.LoadSingleFromFileAsync<StreamMondayBacklog>("streamMonday.json");
				schedule.DayOfTheStream = newDayOfWeek;
				if (newHour.HasValue)
				{
					schedule.HourOfTheStream = newHour.Value;
				}
				await _serializer.SaveSingleToFileAsync("streamMonday.json", schedule);

				await ReplyAsync(string.Format(StringConsts.StreamDayChanged, newDayOfWeek));
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, newDayOfWeek.ToString());
		}

		[Command("ut"), Summary("Change map rotation of UT 1999 server."), RequireUserPermission(GuildPermission.Administrator)]
		public async Task UTRotationReset()
		{
			await _unrealTournamentService.ChangeRotation(new DataTypes.UTRotationConfiguration()
			{
				Repeat = false,
				ExcludeMaps = true
			});

			await ReplyAsync(StringConsts.UTRotationChange);

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("addNyaaPhrase"), Alias("nyaap"), Summary("Add tracked Nyaa.si entry: \"Display name; search phrase\""), RequireUserPermission(GuildPermission.Administrator)]
		public async Task AddNyaaPhraseAsync(params string[] text)
		{
			string input = string.Join(" ", text).Trim();
			if (string.IsNullOrWhiteSpace(input))
			{
				return;
			}

			var parts = input.Split(';')
				.Select(p => p.Trim())
				.Where(p => !string.IsNullOrWhiteSpace(p))
				.ToList();

			if (parts.Count < 2)
			{
				await ReplyAsync("Please use format: `Display name; search phrase`.");
				return;
			}

			string displayName = parts[0];
			string searchPhrase = parts[1];

			var backlog = await _serializer.LoadFromFileAsync<NyaaBacklogEntry>("nyaaBacklog.json") ?? new List<NyaaBacklogEntry>();

			if (!backlog.Any(x => string.Equals(x.SearchPhrase, searchPhrase, StringComparison.OrdinalIgnoreCase)))
			{
				backlog.Add(new NyaaBacklogEntry
				{
					DisplayName = displayName,
					SearchPhrase = searchPhrase
				});

				await _serializer.SaveToFileAsync("nyaaBacklog.json", backlog);
				await ReplyAsync(string.Format(StringConsts.Added, $"{displayName} ({searchPhrase})"));
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, input);
		}

		[Command("removeNyaaPhrase"), Alias("nyaar"), Summary("Remove tracked Nyaa.si entry by search phrase"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task RemoveNyaaPhraseAsync(params string[] text)
		{
			string phrase = string.Join(" ", text).Trim();
			var backlog = await _serializer.LoadFromFileAsync<NyaaBacklogEntry>("nyaaBacklog.json") ?? new List<NyaaBacklogEntry>();
			if (backlog.Count == 0)
			{
				return;
			}

			var toRemove = backlog.FirstOrDefault(x => string.Equals(x.SearchPhrase, phrase, StringComparison.OrdinalIgnoreCase));
			if (toRemove != null)
			{
				backlog.Remove(toRemove);
				await _serializer.SaveToFileAsync("nyaaBacklog.json", backlog);
				await ReplyAsync(string.Format(StringConsts.Removed, phrase));
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, phrase);
		}

		[Command("listNyaaPhrases"), Alias("nyaal"), Summary("List tracked Nyaa.si entries"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task ListNyaaPhrasesAsync()
		{
			var backlog = await _serializer.LoadFromFileAsync<NyaaBacklogEntry>("nyaaBacklog.json") ?? new List<NyaaBacklogEntry>();
			if (backlog.Count == 0)
			{
				await ReplyAsync("No Nyaa entries configured.");
				LoggingService.CustomCommandLog(Context.Message, ModuleName);
				return;
			}

			var lines = backlog
				.Select((e, i) =>
					$"{i + 1}. {e.DisplayName} | `{e.SearchPhrase}` | last updated: {(e.LastUpdated.HasValue ? e.LastUpdated.Value.ToString("u") : "never")}");

			await ReplyAsync("**Tracked Nyaa entries:**\n" + string.Join("\n", lines));
			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("runNyaaWeeklySearch"), Alias("nyaaw"), Summary("Run Nyaa.si weekly search now"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task RunNyaaWeeklySearchAsync()
		{
			await _timerService.NyaaWeeklySearch();
			await ReplyAsync("Nyaa weekly search executed.");
			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		private BacklogEntry FormatStreamMondayEntryText(params string[] text)
		{
			var captions = string.Join(" ", text).Split(';').ToList();

			return captions.Count switch
			{
				0 => new BacklogEntry(string.Empty, string.Empty),
				1 => new BacklogEntry(captions[0], string.Empty),
				_ => new BacklogEntry(captions[0], captions[1].Trim())
			};
		}
	}
}