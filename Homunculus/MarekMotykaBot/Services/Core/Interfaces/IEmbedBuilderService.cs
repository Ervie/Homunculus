﻿using Discord;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IEmbedBuilderService
	{
		Embed BuildStreamMondaySchedule();

		Embed BuildSwearWordCountRanking();
	}
}