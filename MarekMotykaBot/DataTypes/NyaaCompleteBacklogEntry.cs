using System;

namespace MarekMotykaBot.DataTypes
{
	internal class NyaaCompleteBacklogEntry
	{
		/// <summary>
		/// Base display name used when generating episode entries (without episode number suffix).
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Static torrent URL added unchanged for every episode entry.
		/// </summary>
		public string TorrentUrl { get; set; }

		/// <summary>
		/// Total number of episodes; processing stops once all episodes have been added.
		/// </summary>
		public int TotalEpisodes { get; set; }

		/// <summary>
		/// The episode number that will be added on the next weekly run.
		/// </summary>
		public int NextEpisodeNumber { get; set; } = 1;

		/// <summary>
		/// When this entry was last processed by NyaaWeeklySearch.
		/// </summary>
		public DateTime? LastUpdated { get; set; }
	}
}
