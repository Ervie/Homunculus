using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarekMotykaBot.ExtensionsMethods
{
    public static class SocketGuildExtensions
    {
		public static string GetRandomUserName(this SocketGuild guild, Random rng)
		{
			var users = guild.Users.Where(x => !x.DiscordId().Equals("MarekMotykaBot#2213") &&
				!x.DiscordId().Equals("Erina#5946")).ToList();

			int randomUserIndex = rng.Next(0, users.Count);

			return users.Count == 0 ? string.Empty : users.ElementAt(randomUserIndex).Username;
		}
		public static string GetRandomUserName(this SocketGuild guild, Random rng, string excludedUser)
		{
			var users = guild.Users.Where(x => !x.DiscordId().Equals("MarekMotykaBot#2213") &&
				!x.DiscordId().Equals("Erina#5946") && 
				!x.DiscordId().Equals(excludedUser)).ToList();

			int randomUserIndex = rng.Next(0, users.Count);

			return users.Count == 0 ? string.Empty : users.ElementAt(randomUserIndex).Username;
		}
	}
}
