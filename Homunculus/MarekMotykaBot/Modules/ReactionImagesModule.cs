using Discord.Commands;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Services.Core.Interfaces;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class ReactionImagesModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly string resources = $"{System.AppContext.BaseDirectory}/Resources/Images/Reactions/";

		public string ModuleName { get => "ReactionImagesModule"; }

		public ILoggingService LoggingService { get; }

		public ReactionImagesModule(ILoggingService loggingService)
		{
			LoggingService = loggingService;
		}

		[Command("Jameson"), Alias("jjj"), Summary("JJJ laughs at your foolishness")]
		public async Task JJJAsync()
		{
			await Context.Channel.SendFileAsync(resources + "jjj.gif");
			LoggingService.CustomCommandLog(Context.Message, ModuleName);
		}
	}
}