﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.DataTypes.Enumerations;
using MarekMotykaBot.Resources;
using Microsoft.Extensions.Configuration;

namespace MarekMotykaBot.Services
{
	public class TimerService : IDiscordService
	{
		private readonly Timer _timer;
		private readonly JSONSerializerService _serializer;
		private readonly DiscordSocketClient _client;
		private readonly Random _rng;
		private readonly ulong _destinationChannel;

		public IConfiguration Configuration { get; set; }

		public ICollection<TimedTask> TimedTasks { get; set; }

		public TimerService(IConfiguration configuration, JSONSerializerService serializer, DiscordSocketClient client, Random rng)
		{
			Configuration = configuration;
			_serializer = serializer;
			_client = client;
			_rng = rng;

			_destinationChannel = (ulong)Int64.Parse(Configuration["tokens:destinationServerId"]);

			TimedTasks = _serializer.LoadFromFile<TimedTask>("timedTasks.json");

			_timer = new Timer(60 * 60 * 1000);

			_timer.Elapsed += new ElapsedEventHandler(HourlyTimerTick);
		}

		public void StartTimer()
		{
			if (!_timer.Enabled)
				_timer.Start();
		}

		private void HourlyTimerTick(object src, ElapsedEventArgs e)
		{
			DateTime currentDateTime = DateTime.Now;

			foreach (var task in TimedTasks)
			{
				if (task.Hours.Contains(currentDateTime.Hour) && task.DaysOfWeek.Contains(currentDateTime.DayOfWeek))
				{
					this.GetType().GetMethod(task.Name).Invoke(this, null);
				}
			}
		}

		public async Task StreamMondaySchedule()
		{
			List<string> schedule = _serializer.LoadFromFile<string>("streamMonday.json");

			var builder = new EmbedBuilder();

			DateTime today = DateTime.Today;
			int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
			DateTime nextMonday = today.AddDays(daysUntilMonday);

			builder.AddField(x =>
			{
				x.Name = "Rozkładówka na " + nextMonday.ToString("dd.MM");
				x.Value = string.Join(Environment.NewLine, schedule.ToArray());
				x.IsInline = false;
			});

			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

			await channelToPost.SendMessageAsync("", false, builder.Build());
		}

		public async Task QuoteOfTheDay()
		{
			List<Quote> quotes = _serializer.LoadFromFile<Quote>("quotes.json");

			quotes = quotes.Where(x => x.Categories.Contains(QuoteCategory.Wisdom)).ToList();

			int randomQuoteIndex = _rng.Next(0, quotes.Count);

			Quote selectedQuote = quotes[randomQuoteIndex];

			var builder = new EmbedBuilder();

			builder.WithFooter(selectedQuote.Author);
			builder.WithTitle(selectedQuote.QuoteBody);

			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

			await channelToPost.SendMessageAsync(StringConsts.QuoteForToday);
			await Task.Delay(1000);
			await channelToPost.SendMessageAsync("", false, builder.Build());
		}
	}
}