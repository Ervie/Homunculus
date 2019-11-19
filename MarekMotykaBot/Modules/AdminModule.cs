using Discord;
using Discord.Commands;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class AdminModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly IJSONSerializerService _serializer;
		private readonly IEmbedBuilderService _embedBuilderService;

		public string ModuleName { get => "AdminModule"; }

		public ILoggingService LoggingService { get; }

		public AdminModule(
			IJSONSerializerService serializer,
			ILoggingService loggingService,
			IEmbedBuilderService embedBuilderService
			)
		{
			_serializer = serializer;
			_embedBuilderService = embedBuilderService;
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

			List<string> schedule = _serializer.LoadFromFile<string>("streamMonday.json");

			if (!schedule.Contains(entry))
			{
				schedule.Add(entry);
			}

			_serializer.SaveToFile("streamMonday.json", schedule);

			await ReplyAsync(string.Format(StringConsts.Added, entry));

			LoggingService.CustomCommandLog(Context.Message, ModuleName, entry);
		}

		[Command("removeSMEntry"), Alias("smr"), Summary("Remove entry from StreamMonday schedule"), RequireUserPermission(GuildPermission.Administrator)]
		public async Task RemoveEntryFromStreamBacklog(params string[] text)
		{
			string entry = string.Join(" ", text);

			List<string> schedule = _serializer.LoadFromFile<string>("streamMonday.json");

			if (schedule.Contains(entry))
			{
				schedule.Remove(entry);
			}

			_serializer.SaveToFile("streamMonday.json", schedule);

			await ReplyAsync(string.Format(StringConsts.Removed, entry));

			LoggingService.CustomCommandLog(Context.Message, ModuleName, entry);
		}
	}
}