using System;
using Newtonsoft.Json;

namespace MarekMotykaBot.DataTypes
{
	internal class NyaaBacklogEntry
	{
		/// <summary>
		/// Base display name used when creating StreamMonday entries (without episode/season).
		/// </summary>
		public string DisplayName { get; set; }
		
		/// <summary>
		/// Phrase used to search on nyaa.si, e.g. "[ASW] jigokuraku".
		/// </summary>
		public string SearchPhrase { get; set; }

		/// <summary>
		/// When this entry was last processed by NyaaWeeklySearch.
		/// </summary>
		public DateTime? LastUpdated { get; set; }

		/// <summary>
		/// Title of the last processed Nyaa result (used to detect new episodes).
		/// </summary>
		public string LastKnownTitle { get; set; }
	}
}
