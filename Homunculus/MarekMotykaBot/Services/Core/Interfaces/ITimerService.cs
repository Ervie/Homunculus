using System.Timers;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface ITimerService
	{
		void StartTimer();

		void HourlyTimerTick(object src, ElapsedEventArgs e);
	}
}