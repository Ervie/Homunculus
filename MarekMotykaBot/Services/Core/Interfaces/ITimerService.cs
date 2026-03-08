using System;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface ITimerService
	{
		void StartTimer();

		Task ChangeStreamDayAsync(DayOfWeek dayOfWeek, int? hour = null);

		Task NyaaWeeklySearch();
	}
}
