using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using MarekMotykaBot.Services.Core;
using MarekMotykaBot.Services.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class AdminModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly IJSONSerializerService _serializer;
		private readonly ITimerService _timerService;
		private readonly IEmbedBuilderService _embedBuilderService;

		private readonly List<string> _swearWordList;

		public string ServiceName { get => "AdminModule"; }

		public ILoggingService LoggingService { get; }

		public AdminModule(
			IJSONSerializerService serializer,
			ITimerService timerService,
			ILoggingService loggingService,
			IEmbedBuilderService embedBuilderService
			)
		{
			_serializer = serializer;
			_timerService = timerService;
			_embedBuilderService = embedBuilderService;
			LoggingService = loggingService;

			_swearWordList = _serializer.LoadFromFile<string>("swearWords.json");
		}

		[Command("Penis"), Summary("This is a Christian server!"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task SwearWordCounterAsync()
		{
			await ReplyAsync("", false, _embedBuilderService.BuildSwearWordCountRanking());

			LoggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("timer"), Alias("t"), Summary("Timer for special tasks"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task StartTimer()
		{
			_timerService.StartTimer();

			LoggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("addSMEntry"), Alias("sma"), Summary("Add entry to StreamMonday schedule"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task AddEntryToStreamMonday(params string[] text)
		{
			string entry = string.Join(" ", text);

			List<string> schedule = _serializer.LoadFromFile<string>("streamMonday.json");

			if (!schedule.Contains(entry))
			{
				schedule.Add(entry);
			}

			_serializer.SaveToFile<string>("streamMonday.json", schedule);

			LoggingService.CustomCommandLog(Context.Message, ServiceName, entry);
		}

		[Command("removeSMEntry"), Alias("smr"), Summary("Remove entry from StreamMonday schedule"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task RemoveEntryFromStreamMonday(params string[] text)
		{
			string entry = string.Join(" ", text);

			List<string> schedule = _serializer.LoadFromFile<string>("streamMonday.json");

			if (schedule.Contains(entry))
			{
				schedule.Remove(entry);
			}

			_serializer.SaveToFile<string>("streamMonday.json", schedule);

			LoggingService.CustomCommandLog(Context.Message, ServiceName, entry);
		}
	}
}