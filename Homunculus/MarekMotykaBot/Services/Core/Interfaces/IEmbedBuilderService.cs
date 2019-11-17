using Discord;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IEmbedBuilderService
	{
		Embed BuildStreamMondaySchedule();

		Embed BuildSwearWordCountRanking();
	}
}