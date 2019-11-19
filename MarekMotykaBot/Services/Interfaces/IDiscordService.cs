using Microsoft.Extensions.Configuration;

namespace MarekMotykaBot.Services
{
	public interface IDiscordService
	{
		IConfiguration Configuration { get; set; }
	}
}