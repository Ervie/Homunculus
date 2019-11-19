namespace MarekMotykaBot.DataTypes
{
	internal class WordCounterEntry
	{
		public string DiscordUsername { get; set; }

		public string DiscordNickname { get; set; }

		public string Word { get; set; }

		public int CounterValue { get; set; }

		public WordCounterEntry(string discordUsername, string discordNickname, string word, int counterValue)
		{
			DiscordNickname = discordNickname;
			DiscordUsername = discordUsername;
			Word = word;
			CounterValue = counterValue;
		}
	}
}