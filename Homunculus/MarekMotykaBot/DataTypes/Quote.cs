using MarekMotykaBot.DataTypes.Enumerations;
using System.Collections.Generic;

namespace MarekMotykaBot.DataTypes
{
	public class Quote
	{
		public string QuoteBody { get; set; }

		public string Author { get; set; }

		public ICollection<QuoteCategory> Categories { get; set; }

		public Quote(string quoteBody, string author, ICollection<QuoteCategory> categories)
		{
			QuoteBody = quoteBody;
			Author = author;
			Categories = categories;
		}
	}
}