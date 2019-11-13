using MarekMotykaBot.Services;
using MarekMotykaBot.Services.Core.Interfaces;

namespace MarekMotykaBot.Modules.Interface
{
	public interface IDiscordModule
	{
		string ServiceName { get; }

		ILoggingService LoggingService { get; }
	}
}