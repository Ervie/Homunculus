namespace MarekMotykaBot.DataTypes
{
	internal class Quote
	{
		public string QuoteBody { get; set; }

		public string Author { get; set; }

		public Quote(string quoteBody, string author)
		{
			this.QuoteBody = quoteBody;
			this.Author = author;
		}
	}
}