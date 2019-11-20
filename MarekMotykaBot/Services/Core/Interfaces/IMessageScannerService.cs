using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IMessageScannerService
	{
		Task ScanMessage(SocketMessage s);

		Task ScanUpdateMessage(Cacheable<IMessage, ulong> oldMessage, SocketMessage s, ISocketMessageChannel channel);

		void ScanDeletedMessage(Cacheable<IMessage, ulong> deletedMessage, ISocketMessageChannel channel);

		Task DetectMentions(SocketCommandContext context, SocketUserMessage message);

		Task DetectWaifus(SocketCommandContext context, SocketUserMessage message);

		void  DetectSwearWord(SocketCommandContext context, SocketUserMessage message);

		Task DetectStreamMonday(SocketCommandContext context, SocketUserMessage message);

		void  DetectMarekMessage(SocketUserMessage message);
	}
}