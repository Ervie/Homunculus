using System;
using System.Collections.Generic;

namespace MarekMotykaBot.DataTypes
{
	internal class TimedTask
	{
		public ICollection<int> Minutes { get; set; }

		public ICollection<int> Hours { get; set; }

		public ICollection<int> DaysOfMonth { get; set; }

		public ICollection<DayOfWeek> DaysOfWeek { get; set; }

		public string Name { get; set; }
	}
}