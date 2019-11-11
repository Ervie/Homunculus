﻿namespace MarekMotykaBot.DataTypes
{
	internal class OneLinerJoke
	{
		public string Question { get; set; }

		public string Punchline { get; set; }

		public OneLinerJoke(string question, string punchline)
		{
			Question = question;
			Punchline = punchline;
		}
	}
}