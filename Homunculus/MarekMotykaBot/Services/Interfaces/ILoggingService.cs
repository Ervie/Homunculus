using Discord;

namespace MarekMotykaBot.Services
{
	public interface ILoggingService
	{
		NLog.Logger Logger { get; set; }

		void CustomDeleteLog(IMessage message);

		void CustomCommandLog(IMessage message);

		void CustomReactionLog(IMessage message, string reactionName);
	}
}