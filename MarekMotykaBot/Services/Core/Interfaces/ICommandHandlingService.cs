using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface ICommandHandlingService
	{
		Task OnMessageReceivedAsync(SocketMessage s);

		Task<bool> BlockCommandAsync(SocketCommandContext context, string commandName);

		Task<bool> DeclineCommandAsync(SocketCommandContext context, string messageContent);

		Task AddDeclineCacheAsync(string discordId, string commandName);
	}
}