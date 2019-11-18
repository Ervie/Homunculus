using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IEmbedBuilderService
	{
		Embed BuildStreamMondaySchedule();

		Embed BuildSwearWordCountRanking();

		Embed BuildQuote(Quote quote);

		Embed BuildCharadeEntry(CharadeEntry charadeEntry);

		Embed BuildAbout();

		Task<Embed> BuildCommandListAsync(IEnumerable<ModuleInfo> moduleInfos, SocketCommandContext commandContext);
	}
}