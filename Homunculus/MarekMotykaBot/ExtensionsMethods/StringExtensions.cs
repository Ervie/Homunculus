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
				if (string.IsNullOrEmpty(newString))
					newString += charArray[i].ToString();
				else if (newString[newString.Length - 1] != charArray[i])
					newString += charArray[i].ToString();
			}

			return newString;
		}

		public static string RemoveEmojis(this string inputString)
		{
			return Regex.Replace(inputString, @"[^a-zA-Z0-9\s]+", "", RegexOptions.Compiled);
		}

		public static string RemoveEmotes(this string inputString)
		{
			return Regex.Replace(inputString, "(<:.+>)+", "", RegexOptions.Compiled);
		}

		public static string RemoveEmojisAndEmotes(this string inputString)
		{
			return inputString.RemoveEmotes().RemoveEmojis();
		}
	}
}