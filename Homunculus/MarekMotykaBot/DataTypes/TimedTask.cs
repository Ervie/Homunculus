using System;
using System.Collections.Generic;
using System.Text;

namespace MarekMotykaBot.DataTypes
{
    public class TimedTask
    {
		public ICollection<int> Hours { get; set; }

		public ICollection<DayOfWeek> DaysOfWeek { get; set; }

		public string Name { get; set; }
	}
}
