using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface IYTService
	{
		Task<Youtube.SearchResult> SearchYoutubeAsync(string query, string dataType);
	}
}