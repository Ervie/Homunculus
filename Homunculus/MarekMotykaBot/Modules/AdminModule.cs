using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using MarekMotykaBot.Services.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class AdminModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly JSONSerializerService _serializer;
		private readonly TimerService _timerService;

		private readonly List<string> _swearWordList;

		public string ServiceName { get => "AdminModule"; }

		public ILoggingService LoggingService { get; }

		public AdminModule(JSONSerializerService serializer, TimerService timerService, LoggingService loggingService)
		{
			_serializer = serializer;
			_timerService = timerService;
			LoggingService = loggingService;

			_swearWordList = _serializer.LoadFromFile<string>("swearWords.json");
		}

		[Command("Penis"), Summary("This is a Christian server!"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task SwearWordCounterAsync()
		{
			StringBuilder sb = new StringBuilder();

			var counterList = _serializer.LoadFromFile<WordCounterEntry>("wordCounter.json");

			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = StringConsts.SwearWordCounterHeader
			};

			foreach (string swearWord in _swearWordList)
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

			await ReplyAsync("", false, builder.Build());

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