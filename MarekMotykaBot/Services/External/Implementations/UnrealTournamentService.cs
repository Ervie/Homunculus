using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;

namespace MarekMotykaBot.Services.External.Implementations
{
	public class UnrealTournamentService : IDiscordService, IUnrealTournamentService
	{
		public IConfiguration Configuration { get; set; }

		public UnrealTournamentService(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ChangeRotation()
		{
			throw new NotImplementedException();
		}
	}
}