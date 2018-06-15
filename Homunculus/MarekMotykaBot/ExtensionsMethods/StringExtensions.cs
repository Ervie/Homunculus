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
	}
}