namespace MarekMotykaBot.DataTypes.Caches
{
	internal class DeclineCache
	{
		public string DiscordUsername { get; set; }

		public string CommandName { get; set; }

		public DeclineCache(string discordUserName, string commandName)
		{
			DiscordUsername = discordUserName;
			CommandName = commandName;
		}
	}
}