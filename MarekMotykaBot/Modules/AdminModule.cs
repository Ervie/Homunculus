using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using System;
using System.Collections.Generic;
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
			await ReplyAsync("", false, _embedBuilderService.BuildSwearWordCountRanking());

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("addSMEntry"), Alias("sma"), Summary("Add entry to StreamMonday schedule"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task AddEntryToStreamBacklog(params string[] text)
		{
			string entry = string.Join(" ", text);

			var schedule = _serializer.LoadSingleFromFile<StreamMondayBacklog>("streamMonday.json");

			if (!schedule.BacklogEntries.Contains(entry))
			{
				schedule.BacklogEntries.Add(entry);

				_serializer.SaveSingleToFile("streamMonday.json", schedule);

				await ReplyAsync(string.Format(StringConsts.Added, entry));
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, entry);
		}

		[Command("removeSMEntry"), Alias("smr"), Summary("Remove entry from StreamMonday schedule"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task RemoveEntryFromStreamBacklog(params string[] text)
		{
			string entry = string.Join(" ", text);

			var schedule = _serializer.LoadSingleFromFile<StreamMondayBacklog>("streamMonday.json");

			if (schedule.BacklogEntries.Contains(entry))
			{
				schedule.BacklogEntries.Remove(entry);

				_serializer.SaveSingleToFile("streamMonday.json", schedule);

				await ReplyAsync(string.Format(StringConsts.Removed, entry));
			}

			LoggingService.CustomCommandLog(Context.Message, ModuleName, entry);
		}

		[Command("changeSMDay"), Alias("smd"), Summary("Change day of the SM (1-7)"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task ChangeStreamDay(params string[] text)
		{
			Enum.TryParse(text.FirstOrDefault(), out DayOfWeek newDayOfWeek);

			if (newDayOfWeek is { })
			{
				_timerService.ChangeStreamDay(newDayOfWeek);
				var schedule = _serializer.LoadSingleFromFile<StreamMondayBacklog>("streamMonday.json");
				schedule.DayOfTheStream = newDayOfWeek;
				_serializer.SaveSingleToFile("streamMonday.json", schedule);

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
	}
}