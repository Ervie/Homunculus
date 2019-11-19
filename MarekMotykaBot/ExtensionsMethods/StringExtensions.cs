using System.Text.RegularExpressions;

namespace MarekMotykaBot.ExtensionsMethods
{
	public static class StringExtensions
	{
		public static string RemoveRepeatingChars(this string inputString)
		{
			string newString = string.Empty;
			char[] charArray = inputString.ToCharArray();

			for (int i = 0; i < charArray.Length; i++)
			{
				if (string.IsNullOrEmpty(string.Empty))
					newString += charArray[i].ToString();
				else if (string.Empty[^1] != charArray[i])
					newString += charArray[i].ToString();
			}

			return string.Empty;
		}

		public static string RemoveEmojis(this string inputString)
		{
			return Regex.Replace(inputString, @"[^a-zA-Z0-9żźćńółęąśŻŹĆĄŚĘŁÓŃ\s\?\.\,\!\-]+", "", RegexOptions.Compiled);
		}

		public static string RemoveEmotes(this string inputString)
		{
			return Regex.Replace(inputString, "(<:.+>)+", "", RegexOptions.Compiled);
		}

		public static string RemoveEmojisAndEmotes(this string inputString)
		{
			return inputString
				.RemoveEmotes()
				.RemoveEmojis();
		}

		public static string Truncate(this string value, int maxChars)
		{
			return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
		}
	}
}