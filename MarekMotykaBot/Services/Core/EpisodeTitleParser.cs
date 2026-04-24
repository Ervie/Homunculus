using System.Text.RegularExpressions;

namespace MarekMotykaBot.Services.Core
{
	/// <summary>
	/// Parses anime release titles (e.g. from nyaa.si) into a canonical
	/// show name, season and episode. Handles the most common release formats:
	///   "[Group] Name - 03 (1080p) [hash].mkv"
	///   "[Group] Name Sx - 03"
	///   "[Group] Name - SxxExx [tags]"
	///   "[Group] Name SxxExx [tags]"
	///   "[Group] Name (Alt Title) - SxxExx"
	///   "[Group] Name (YYYY) - 03"
	/// Batch releases like "Name - 1-12" or "Name 001-024" are intentionally skipped.
	/// </summary>
	public static class EpisodeTitleParser
	{
		public sealed class ParsedEpisode
		{
			public string Name { get; init; }

			public int? Season { get; init; }

			public int Episode { get; init; }
		}

		private static readonly Regex LeadingGroupTagRegex =
			new Regex(@"^(?:\[[^\]]+\]\s*)+", RegexOptions.Compiled);

		private static readonly Regex FileExtensionRegex =
			new Regex(@"\.(?:mkv|mp4|avi|webm|m4v|ts|mov|flac|mka)$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly Regex TrailingTagRegex =
			new Regex(@"\s*(?:\([^()]*\)|\[[^\[\]]*\])\s*$", RegexOptions.Compiled);

		// "Name [- ] S01E03 ..."  (dash is optional, whitespace required)
		private static readonly Regex SeasonEpisodePattern = new Regex(
			@"^(?<name>.+?)\s+(?:-\s+)?S(?<s>\d{1,2})E(?<e>\d{1,4})(?:v\d+)?(?!\d)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		// "Name Sx - 03 ..."
		private static readonly Regex SeasonThenEpisodePattern = new Regex(
			@"^(?<name>.+?)\s+S(?<s>\d{1,2})\s+-\s+(?<e>\d{1,4})(?:v\d+)?(?!\s*-\s*\d)(?!\d)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		// "Name - 03 ..."  (excludes batches like "- 1-12" or "- 001-024")
		private static readonly Regex EpisodePattern = new Regex(
			@"^(?<name>.+?)\s+-\s+(?<e>\d{1,4})(?:v\d+)?(?!\s*-\s*\d)(?!\d)",
			RegexOptions.Compiled);

		/// <summary>
		/// Attempts to parse a release title. Returns <c>null</c> when the title
		/// does not match any known episode format (batch packs, unparseable, etc).
		/// </summary>
		public static ParsedEpisode Parse(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				return null;
			}

			string cleaned = LeadingGroupTagRegex.Replace(title, string.Empty).Trim();
			cleaned = FileExtensionRegex.Replace(cleaned, string.Empty).Trim();

			Match match = SeasonEpisodePattern.Match(cleaned);
			if (match.Success)
			{
				return BuildResult(match, hasSeason: true);
			}

			match = SeasonThenEpisodePattern.Match(cleaned);
			if (match.Success)
			{
				return BuildResult(match, hasSeason: true);
			}

			match = EpisodePattern.Match(cleaned);
			if (match.Success)
			{
				return BuildResult(match, hasSeason: false);
			}

			return null;
		}

		/// <summary>
		/// Produces the full canonical name for the episode, e.g. "Sousou no Frieren s2ep9".
		/// Returns <c>null</c> when the title cannot be parsed.
		/// </summary>
		public static string FormatEpisodeName(string title)
		{
			var parsed = Parse(title);
			if (parsed == null)
			{
				return null;
			}

			string suffix = FormatSuffix(parsed);
			return string.IsNullOrEmpty(parsed.Name) ? suffix : $"{parsed.Name} {suffix}";
		}

		/// <summary>
		/// Produces just the season/episode suffix, e.g. "s2ep9" or "ep3".
		/// Returns <c>null</c> when the title cannot be parsed.
		/// </summary>
		public static string FormatEpisodeSuffix(string title)
		{
			var parsed = Parse(title);
			return parsed == null ? null : FormatSuffix(parsed);
		}

		private static ParsedEpisode BuildResult(Match match, bool hasSeason)
		{
			string name = SanitizeName(match.Groups["name"].Value);
			int episode = int.Parse(match.Groups["e"].Value);
			int? season = hasSeason && match.Groups["s"].Success
				? int.Parse(match.Groups["s"].Value)
				: (int?)null;

			return new ParsedEpisode
			{
				Name = name,
				Season = season,
				Episode = episode
			};
		}

		private static string FormatSuffix(ParsedEpisode parsed)
		{
			if (parsed.Season.HasValue)
			{
				return $"s{parsed.Season.Value}ep{parsed.Episode}";
			}

			return $"ep{parsed.Episode}";
		}

		private static string SanitizeName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return string.Empty;
			}

			string sanitized = name.Trim();

			// Strip trailing "(alt title)" / "(YYYY)" / "[note]" groups repeatedly,
			// e.g. "Name (2026) [x264]" -> "Name".
			while (true)
			{
				string stripped = TrailingTagRegex.Replace(sanitized, string.Empty);
				if (string.Equals(stripped, sanitized))
				{
					break;
				}

				sanitized = stripped;
			}

			return sanitized.TrimEnd(' ', '-', ',').Trim();
		}
	}
}
