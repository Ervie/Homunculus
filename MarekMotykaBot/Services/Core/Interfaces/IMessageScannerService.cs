using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IMessageScannerService
	{
		Task ScanMessageAsync(SocketMessage s);

		Task ScanUpdateMessageAsync(Cacheable<IMessage, ulong> oldMessage, SocketMessage s, ISocketMessageChannel channel);

		Task ScanDeletedMessageAsync(Cacheable<IMessage, ulong> deletedMessage, Cacheable<IMessageChannel, ulong> channel);
		
		Task DetectMentionsAsync(SocketCommandContext context, SocketUserMessage message);

		Task DetectWaifusAsync(SocketCommandContext context, SocketUserMessage message);

		Task DetectSwearWordAsync(SocketCommandContext context, SocketUserMessage message);

		Task DetectMarekMessageAsync(SocketUserMessage message);
	}
}