using Discord;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.DataTypes.Enumerations;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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

		private ICollection<TimedTask> TimedTasks { get; set; }

		public TimerService(IConfiguration configuration, JSONSerializerService serializer, DiscordSocketClient client, Random rng)
		{
			Configuration = configuration;
			_serializer = serializer;
			_client = client;
			_rng = rng;

			_destinationChannel = (ulong)Int64.Parse(Configuration["tokens:destinationServerId"]);

			TimedTasks = _serializer.LoadFromFile<TimedTask>("timedTasks.json");

			_timer = new Timer(60 * 1000);

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
				if (task.Hours.Contains(currentDateTime.Hour) &&
					task.Minutes.Contains(currentDateTime.Minute) &&
					(task.DaysOfWeek.Contains(currentDateTime.DayOfWeek) || task.DaysOfMonth.Contains(currentDateTime.Day)))
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
			int daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7;
			DateTime nextWednesday = today.AddDays(daysUntilWednesday);

			if (schedule != null && schedule.Count > 0)
			{
				builder.AddField(x =>
				{
					x.Name = "Rozkładówka (backlog) na " + nextWednesday.ToString("dd.MM");
					x.Value = string.Join(Environment.NewLine, schedule.ToArray());
					x.IsInline = false;
				});
			}

			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

			var guild = _client.Guilds.FirstOrDefault(x => x.Name.Equals(Configuration["tokens:destinationServerName"]));

			if (guild != null)
			{
				var alias = guild.Roles.FirstOrDefault(x => x.Name.Equals(Configuration["configValues:streamAlias"]));

				if (alias != null)
				{
					await channelToPost.SendMessageAsync(alias.Mention);
				}
			}

			await channelToPost.SendMessageAsync("", false, builder.Build());
		}

		public async Task RabbitReminder()
		{
			bool rabbitLinkedFlag = _serializer.LoadSingleFromFile<bool>("hasLonkLinkedRabbit.json");

			if (!rabbitLinkedFlag)
			{
				var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

				var guild = _client.Guilds.FirstOrDefault(x => x.Name.Equals(Configuration["tokens:destinationServerName"]));

				if (guild != null)
				{
					var alias = guild.Users.FirstOrDefault(x => x.DiscordId().Equals(Configuration["configValues:streamOwner"]));

					if (alias != null)
					{
						await channelToPost.SendMessageAsync(alias.Mention);
					}
				}

				await channelToPost.SendMessageAsync(StringConsts.RabbitMissing);
			}
		}

		public async Task RabbitReminderReset()
		{
			bool rabbitLinkedFlag = false;

			_serializer.SaveSingleToFile<bool>("hasLonkLinkedRabbit.json", rabbitLinkedFlag);
		}

		public async Task QuoteOfTheDay()
		{
			List<Quote> quotes = _serializer.LoadFromFile<Quote>("quotes.json");

			quotes = quotes.Where(x => x.Categories.Contains(QuoteCategory.Wisdom)).ToList();

			int randomQuoteIndex = _rng.Next(0, quotes.Count);

			Quote selectedQuote = quotes[randomQuoteIndex];

			_serializer.SaveToFile<Quote>("quoteOfTheDay.json", new List<Quote> { selectedQuote });

			var builder = new EmbedBuilder();

			builder.WithFooter(selectedQuote.Author);
			builder.WithTitle(selectedQuote.QuoteBody);

			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

			await channelToPost.SendMessageAsync(StringConsts.QuoteForToday);
			await Task.Delay(1000);
			await channelToPost.SendMessageAsync("", false, builder.Build());
		}

		public async Task SwearWordCount()
		{
			StringBuilder sb = new StringBuilder();

			var counterList = _serializer.LoadFromFile<WordCounterEntry>("wordCounter.json");

			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = StringConsts.SwearWordCounterHeader
			};

			foreach (string swearWord in counterList.Select(x => x.Word).Distinct())
			{
				var specificSwearWordEntries = counterList.Where(x => x.Word.Equals(swearWord)).OrderByDescending(x => x.CounterValue);

				foreach (var entry in specificSwearWordEntries)
				{
					sb.AppendLine(string.Format(StringConsts.SwearWordCounterEntry, entry.DiscordNickname, entry.Word, entry.CounterValue));
				}

				if (!string.IsNullOrEmpty(sb.ToString()))
					builder.AddField(x =>
					{
						x.Name = swearWord;
						x.Value = sb.ToString();
						x.IsInline = true;
					});

				sb.Clear();
			}

			var channelToPost = _client.GetChannel(_destinationChannel) as IMessageChannel;

			await channelToPost.SendMessageAsync("", false, builder.Build());
		}
	}
}