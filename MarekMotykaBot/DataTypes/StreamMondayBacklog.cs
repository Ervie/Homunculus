using System;
using System.Collections.Generic;

namespace MarekMotykaBot.DataTypes
{
	internal class StreamMondayBacklog
	{
		public DayOfWeek DayOfTheStream { get; set; }

		public ICollection<string> BacklogEntries { get; set; }
	}
}