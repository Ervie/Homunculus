using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MarekMotykaBot
{
    public static class MessageScanner
	{

		public static DiscordSocketClient Client { get; set; }

		private static List<string> _waifuList = new List<string>() { "Asuna", "Rias", "Erina" };
		private static List<string> _marekFaceWords = new List<string>() { "czarn", "nigga", "nigger", "ciemn", "murzyn" };

		public static async Task ScanMessage(SocketMessage s)
		{
			var message = s as SocketUserMessage;

			if (message == null)
				return;

			var context = new SocketCommandContext(Client, message);

			// Detect waifus

			foreach (string waifuName in _waifuList)
			{
				if (message.Content.ToLowerInvariant().Contains(waifuName.ToLowerInvariant()) && !message.Author.IsBot)
				{
					await context.Channel.SendMessageAsync(string.Format($"{waifuName} jest najlepsza! <3"));
					break;
				}
			}

			// Detect marekface trigger words

			foreach (string word in _marekFaceWords)
			{
				if (message.Content.ToLowerInvariant().Contains(word) && !message.Author.IsBot)
				{
					var marekFace = context.Guild.Emotes.Where(x => x.Name.ToLower().Equals("marekface")).FirstOrDefault();

					if (marekFace != null)
						await message.AddReactionAsync(marekFace);

					break;
				}
			}

		}
	}
}
