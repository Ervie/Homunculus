using MarekMotykaBot.DataTypes.Enumerations;
using System.Collections.Generic;

namespace MarekMotykaBot.DataTypes
{
	internal class Quote
	{
		public string QuoteBody { get; set; }

		public string Author { get; set; }

		public ICollection<QuoteCategory> Categories { get; set; }

		public Quote(string quoteBody, string author, ICollection<QuoteCategory> categories)
		{
			this.QuoteBody = quoteBody;
			this.Author = author;
			this.Categories = categories;
		}
	}
}