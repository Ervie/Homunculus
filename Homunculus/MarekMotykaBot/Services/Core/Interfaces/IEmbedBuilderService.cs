using Discord;
using MarekMotykaBot.DataTypes;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IEmbedBuilderService
	{
		Embed BuildStreamMondaySchedule();

		Embed BuildSwearWordCountRanking();

		Embed BuildQuote(Quote quote);
	}
}