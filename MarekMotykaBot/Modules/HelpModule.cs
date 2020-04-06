using Discord.Commands;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Services.Core.Interfaces;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class HelpModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly CommandService _commandService;
		private readonly IEmbedBuilderService _embedBuilderService;

		public ILoggingService LoggingService { get; }

		public HelpModule(
			CommandService commandService,
			ILoggingService loggingService,
			IEmbedBuilderService embedBuilderService
		)
		{
			_commandService = commandService;
			_embedBuilderService = embedBuilderService;
			LoggingService = loggingService;
		}

		public string ModuleName { get => "HelpModule"; }

		[Command("Version"), Alias("v"), Summary("Prints version information")]
		public async Task AboutAsync()
		{
			await ReplyAsync("", false, _embedBuilderService.BuildAbout());

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("Help"), Alias("h"), Summary("List all the commands")]
		public async Task HelpAsync()
		{
			await ReplyAsync("", false, await _embedBuilderService.BuildCommandListAsync(_commandService.Modules, Context));

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}

		[Command("StreamMonday"), Alias("sm", "Streamdziałek"), Summary("Prints schedule for next StreamMonday")]
		public async Task StreamMondayAsync()
		{
			await ReplyAsync("", false, await _embedBuilderService.BuildStreamMondayScheduleAsync());

			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}
	}
}