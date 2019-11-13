using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface ICommandHandlingService
	{
		Task OnMessageReceivedAsync(SocketMessage s);

		Task<bool> BlockCommand(SocketCommandContext context, string commandName);

		Task<bool> DeclineCommand(SocketCommandContext context, string messageContent);

		void AddDeclineCache(string discordId, string commandName);
	}
}