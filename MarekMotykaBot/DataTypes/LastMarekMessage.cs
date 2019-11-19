using System;

namespace MarekMotykaBot.DataTypes
{
	public class LastMarekMessage
	{
		public DateTime DatePosted { get; set; }

		public bool IsImage { get; set; }

		public string MessageContent { get; set; }
	}
}