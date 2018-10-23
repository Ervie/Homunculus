using Discord;

namespace MarekMotykaBot.Services
{
	public interface ILoggingService
	{
		NLog.Logger Logger { get; set; }

		void CustomDeleteLog(IMessage message);

		void CustomEditLog(IMessage message, IMessage oldMessage);

		void CustomCommandLog(IMessage message, string moduleName);

		void CustomCommandLog(IMessage message, string moduleName, string parameters);

		void CustomReactionLog(IMessage message, string reactionName);
	}
}