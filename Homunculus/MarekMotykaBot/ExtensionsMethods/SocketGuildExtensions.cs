using Discord.WebSocket;
using System;
using System.Linq;

namespace MarekMotykaBot.ExtensionsMethods
{
	public static class SocketGuildExtensions
	{
		private static readonly string MarekDiscordId = "Erina#5946";
		private static readonly string MarekBotDiscordId = "MarekMotykaBot#2213";

		public static string GetRandomUserName(this SocketGuild guild, Random rng)
		{
			var guildUsers = guild.Users
				.Where(x => !x.DiscordId().Equals(MarekBotDiscordId) &&
				!x.DiscordId().Equals(MarekDiscordId))
				.ToList();

			int randomUserIndex = rng.Next(0, guildUsers.Count);

			return guildUsers.Any() ? string.Empty : guildUsers.ElementAt(randomUserIndex).Username;
		}

		public static string GetRandomUserName(this SocketGuild guild, Random rng, string excludedUser)
		{
			var uildUsers = guild.Users.Where(x => !x.DiscordId().Equals(MarekBotDiscordId) &&
				!x.DiscordId().Equals(MarekDiscordId) &&
				!x.DiscordId().Equals(excludedUser))
				.ToList();

			int randomUserIndex = rng.Next(0, uildUsers.Count);

			return uildUsers.Count == 0 ? string.Empty : uildUsers.ElementAt(randomUserIndex).Username;
		}
	}
}