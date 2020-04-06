using Discord;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.DataTypes.Enumerations;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace MarekMotykaBot.Services.Core
{
	public class TimerService : IDiscordService, ITimerService
	{
		private readonly IJSONSerializerService _serializer;
		private readonly IEmbedBuilderService _embedBuilderService;
		private readonly IUnrealTournamentService _unrealTournamentService;
		private readonly DiscordSocketClient _client;
		private readonly Random _rng;
		private readonly Timer _timer;
		private readonly ulong _destinationChannel;

		public IConfiguration Configuration { get; set; }

		private ICollection<TimedTask> TimedTasks { get; set; }

		public TimerService(
			IConfiguration configuration,
			IJSONSerializerService serializer,
			IEmbedBuilderService statisticsService,
			IUnrealTournamentService unrealTournamentService,
			DiscordSocketClient client,
			Random rng
		)
		{
			Configuration = configuration;
			_serializer = serializer;
			_client = client;
			_embedBuilderService = statisticsService;
			_unrealTournamentService = unrealTournamentService;
			_rng = rng;

			_destinationChannel = (ulong)long.Parse(Configuration["tokens:destinationServerId"]);

			TimedTasks = _serializer.LoadFromFile<TimedTask>("timedTasks.json");

			_timer = new Timer(60 * 1000);

			_timer.Elapsed += new ElapsedEventHandler(TimerTick);
		}

		public void StartTimer()
		{
			if (!_timer.Enabled)
				_timer.Start();
		}

		public void TimerTick(object src, ElapsedEventArgs e)
		{
			DateTime currentDateTime = DateTime.Now;

			foreach (TimedTask task in TimedTasks)
			{
				if (task.Hours.Contains(currentDateTime.Hour) &&
					task.Minutes.Contains(currentDateTime.Minute) &&
					(task.DaysOfWeek.Contains(currentDateTime.DayOfWeek) || task.DaysOfMonth.Contains(currentDateTime.Day)))
				{
					GetType().GetMethod(task.Name).Invoke(this, null);
				}
			}
		}

		public async Task StreamMondaySchedule()
		{
			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

			Embed streamBacklog = await _embedBuilderService.BuildStreamMondayScheduleAsync();

			await channelToPost.SendMessageAsync("", false, streamBacklog);
		}

		public async Task QuoteOfTheDay()
		{
			var quotes = (await _serializer.LoadFromFileAsync<Quote>("quotes.json"))
				.Where(x => x.Categories.Contains(QuoteCategory.Wisdom))
				.ToList();

			int randomQuoteIndex = _rng.Next(0, quotes.Count);
			var selectedQuote = quotes[randomQuoteIndex];

			await _serializer.SaveToFileAsync("quoteOfTheDay.json", new List<Quote> { selectedQuote });

			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

			await channelToPost.SendMessageAsync(StringConsts.QuoteForToday);
			await Task.Delay(1000);
			await channelToPost.SendMessageAsync("", false, _embedBuilderService.BuildQuote(selectedQuote));
		}

		public async Task SwearWordCount()
		{
			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;
			var swearWordCountRanking = await _embedBuilderService.BuildSwearWordCountRankingAsync();

			await channelToPost.SendMessageAsync("", false, swearWordCountRanking);
		}

		public void ResetUTMapRotation() 
			=> _unrealTournamentService.ChangeRotation(new UTRotationConfiguration()
				{
					Repeat = false,
					ExcludeMaps = true
				});

		public void ChangeStreamDay(DayOfWeek dayOfWeek)
		{
			var streamMondayTask = TimedTasks.First(x => x.Name.Equals("StreamMondaySchedule"));
			streamMondayTask.DaysOfWeek.Clear();
			streamMondayTask.DaysOfWeek.Add(dayOfWeek);
		}
	}
}