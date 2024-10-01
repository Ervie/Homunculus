using System;
using System.Collections.Generic;

namespace MarekMotykaBot.DataTypes
{
	internal class StreamMondayBacklog
	{
		public DayOfWeek DayOfTheStream { get; set; }
		
		public int HourOfTheStream { get; set; }

		public ICollection<BacklogEntry> BacklogEntries { get; set; }
	}

	internal class BacklogEntry
	{
		public string Name { get; }

		public string Link { get; }

		public BacklogEntry(string name, string link)
		{
			Name = name;
			Link = link;
		}

		public string FormatForEmbedded()
			=> !string.IsNullOrWhiteSpace(Link) && Uri.IsWellFormedUriString(Link, UriKind.Absolute)
			? $"{Name} [link]({Link})"
			: Name;
	}
}