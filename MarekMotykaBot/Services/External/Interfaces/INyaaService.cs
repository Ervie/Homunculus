using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface INyaaService
	{
		/// <summary>
		/// Searches nyaa.si (Anime - English-translated) and returns all results that are newer than the provided last known title
		/// (or only the newest result if there is no last known title). For each new result the title and download-torrent URL are returned.
		/// The list is ordered from newest to oldest.
		/// </summary>
		Task<IReadOnlyList<(string Title, string TorrentUrl)>> GetNewTorrentDownloadsSinceAsync(
			string searchPhrase,
			string lastKnownTitle,
			int maxResults = 20);
	}
}
