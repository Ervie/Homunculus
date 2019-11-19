namespace MarekMotykaBot.DataTypes.Caches
{
    internal class EightBallCache
    {
        public string DiscordUsername { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public EightBallCache(string discordUserName, string question, string answer)
        {
            DiscordUsername = discordUserName;
            Question = question;
            Answer = answer;
        }
    }
}