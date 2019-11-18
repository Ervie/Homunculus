using System.Text;

namespace MarekMotykaBot.DataTypes
{
	public class CharadeEntry
	{
		public Series Series { get; set; }

		public string[] KnownBy { get; set; }
	}

	public class Series
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public CharadeTranslations Translation { get; set; }
	}

	public class CharadeTranslations
	{
		public string Polish { get; set; }

		public string EnglishLiteral { get; set; }

		public string EnglishOfficial { get; set; }

		public string Japansese { get; set; }

		public string ListTranslationsWithNewLine(string mainTitle)
		{
			StringBuilder stringBuilder = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(Polish) && !Polish.Equals(mainTitle))
				stringBuilder.AppendLine(Polish);

			if (!string.IsNullOrWhiteSpace(EnglishLiteral) && !EnglishLiteral.Equals(mainTitle))
				stringBuilder.AppendLine(EnglishLiteral);

			if (!string.IsNullOrWhiteSpace(EnglishOfficial) && !EnglishOfficial.Equals(mainTitle))
				stringBuilder.AppendLine(EnglishOfficial);

			if (!string.IsNullOrWhiteSpace(Japansese) && !Japansese.Equals(mainTitle))
				stringBuilder.AppendLine(Japansese);

			return stringBuilder.ToString();
		}
	}
}