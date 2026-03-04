using System;
using Newtonsoft.Json;

namespace MarekMotykaBot.DataTypes
{
	internal class NyaaBacklogEntry
	{
		/// <summary>
		/// Phrase used to search on nyaa.si, e.g. "[ASW] jigokuraku".
		/// </summary>
		public string SearchPhrase { get; set; }

		/// <summary>
		/// Base display name used when creating StreamMonday entries (without episode/season).
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// When this entry was last processed by NyaaWeeklySearch.
		/// </summary>
		[JsonProperty("AddedAt")]
		public DateTime? LastUpdated { get; set; }

		/// <summary>
		/// Title of the last processed Nyaa result (used to detect new episodes).
		/// </summary>
		[JsonProperty("Title")]
		public string LastKnownTitle { get; set; }
	}
}
