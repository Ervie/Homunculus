using MarekMotykaBot.DataTypes.Enumerations;

namespace MarekMotykaBot.DataTypes
{
	internal class Quote
	{
		public string QuoteBody { get; set; }

		public string Author { get; set; }

		public QuoteCategory Category { get; set; }

		public Quote(string quoteBody, string author, QuoteCategory category)
		{
			this.QuoteBody = quoteBody;
			this.Author = author;
			this.Category = category;
		}
	}
}