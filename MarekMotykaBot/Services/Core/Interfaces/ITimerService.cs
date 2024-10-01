using System;
using System.Timers;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface ITimerService
	{
		void StartTimer();

		void TimerTick(object src, ElapsedEventArgs e);

		void ChangeStreamDay(DayOfWeek dayOfWeek, int? hour = null);
	}
}