using HtmlAgilityPack;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External
{
	public class NyaaService : IDiscordService, INyaaService
	{
		private const string NyaaBaseUrl = "https://nyaa.si";
		private const string SearchPath = "/?f=0&c=1_2&q=";
		private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);

		private readonly HttpClient _httpClient;

		public IConfiguration Configuration { get; set; }

		public NyaaService(IConfiguration configuration, HttpClient httpClient)
		{
			Configuration = configuration;
			_httpClient = httpClient ?? new HttpClient();
			if (_httpClient.Timeout != RequestTimeout)
			{
				_httpClient.Timeout = RequestTimeout;
			}
			if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
			{
				_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "MarekMotykaBot/1.0 (Discord bot)");
			}
		}

		public async Task<IReadOnlyList<(string Title, string TorrentUrl)>> GetNewTorrentDownloadsSinceAsync(
			string searchPhrase,
			string lastKnownTitle,
			int maxResults = 20)
		{
			if (string.IsNullOrWhiteSpace(searchPhrase))
			{
				return Array.Empty<(string Title, string TorrentUrl)>();
			}

			try
			{
				var allResults = await GetSearchResultsAsync(searchPhrase, lastKnownTitle, maxResults).ConfigureAwait(false);
				var list = new List<(string Title, string TorrentUrl)>();

				foreach (var (viewPath, title) in allResults)
				{
					string torrentUrl = await GetTorrentDownloadUrlFromViewPageAsync(viewPath).ConfigureAwait(false);
					if (string.IsNullOrEmpty(torrentUrl))
					{
						continue;
					}

					list.Add((title, torrentUrl));
				}

				return list;
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
			{
				return Array.Empty<(string Title, string TorrentUrl)>();
			}
		}

		private async Task<(string ViewPath, string Title)> GetFirstSearchResultAsync(string searchPhrase)
		{
			string query = Uri.EscapeDataString(searchPhrase);
			string url = NyaaBaseUrl + SearchPath + query;

			using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();
			string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			// Search page: first result link is in a table; look for <a href="https://nyaa.si/view/123">Title</a>
			var links = doc.DocumentNode.SelectNodes("//a[@href]");
			if (links == null)
			{
				return (null, null);
			}

			foreach (var a in links)
			{
				string href = a.GetAttributeValue("href", null);
				if (string.IsNullOrEmpty(href))
					continue;
				// Match /view/ID or full https://nyaa.si/view/ID
				int viewIndex = href.IndexOf("/view/", StringComparison.OrdinalIgnoreCase);
				if (viewIndex < 0)
				{
					continue;
				}
				string path = href.Substring(href.IndexOf("/view/", StringComparison.Ordinal));
				// Path should be like /view/2076773 (no query string for torrent page)
				int queryStart = path.IndexOf('?');
				if (queryStart > 0)
				{
					path = path.Substring(0, queryStart);
				}
				string title = a.InnerText?.Trim();
				if (!string.IsNullOrEmpty(title))
				{
					return (path, title);
				}
			}

			return (null, null);
		}

		private async Task<IReadOnlyList<(string ViewPath, string Title)>> GetSearchResultsAsync(
			string searchPhrase,
			string lastKnownTitle,
			int maxResults)
		{
			string query = Uri.EscapeDataString(searchPhrase);
			string url = NyaaBaseUrl + SearchPath + query;

			using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();
			string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var links = doc.DocumentNode.SelectNodes("//a[@href]");
			if (links == null)
			{
				return Array.Empty<(string ViewPath, string Title)>();
			}

			var results = new List<(string ViewPath, string Title)>();
			var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			bool hasLastTitle = !string.IsNullOrWhiteSpace(lastKnownTitle);

			foreach (var a in links)
			{
				string href = a.GetAttributeValue("href", null);
				if (string.IsNullOrEmpty(href))
				{
					continue;
				}

				int viewIndex = href.IndexOf("/view/", StringComparison.OrdinalIgnoreCase);
				if (viewIndex < 0)
				{
					continue;
				}

				string path = href.Substring(href.IndexOf("/view/", StringComparison.Ordinal));
				int queryStart = path.IndexOf('?');
				if (queryStart > 0)
				{
					path = path.Substring(0, queryStart);
				}

				if (!seenPaths.Add(path))
				{
					continue;
				}

				string title = a.InnerText?.Trim();
				if (string.IsNullOrEmpty(title))
				{
					continue;
				}

				if (hasLastTitle && string.Equals(title, lastKnownTitle, StringComparison.Ordinal))
				{
					break;
				}

				results.Add((path, title));

				if (!hasLastTitle && results.Count >= 1)
				{
					break;
				}

				if (results.Count >= maxResults)
				{
					break;
				}
			}

			return results;
		}

		private async Task<string> GetTorrentDownloadUrlFromViewPageAsync(string viewPath)
		{
			string url = viewPath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
				? viewPath
				: NyaaBaseUrl + viewPath;

			using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();
			string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var links = doc.DocumentNode.SelectNodes("//a[@href]");
			if (links == null)
			{
				return null;
			}

			foreach (var a in links)
			{
				string href = a.GetAttributeValue("href", null);
				if (string.IsNullOrEmpty(href))
				{
					continue;
				}

				if (href.IndexOf("/download/", StringComparison.OrdinalIgnoreCase) < 0)
				{
					continue;
				}

				if (!href.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				if (!href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
				{
					href = NyaaBaseUrl + href;
				}

				return href;
			}

			return null;
		}
	}
}
