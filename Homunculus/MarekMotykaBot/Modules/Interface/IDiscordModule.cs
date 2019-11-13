using MarekMotykaBot.Services;

namespace MarekMotykaBot.Modules.Interface
{
	public interface IDiscordModule
	{
		string ServiceName { get; }

		ILoggingService LoggingService { get; }
	}
}