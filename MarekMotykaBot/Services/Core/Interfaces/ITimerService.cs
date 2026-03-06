using System;
using System.Threading.Tasks;
using System.Timers;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface ITimerService
	{
		void StartTimer();

		void TimerTick(object src, ElapsedEventArgs e);

		Task ChangeStreamDayAsync(DayOfWeek dayOfWeek, int? hour = null);

		Task NyaaWeeklySearch();
	}
}