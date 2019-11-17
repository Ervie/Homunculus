using MarekMotykaBot.Services.Core.Interfaces;

namespace MarekMotykaBot.Modules.Interface
{
	public interface IDiscordModule
	{
		string ModuleName { get; }

		ILoggingService LoggingService { get; }
	}
}