using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core
{
	public class MessageScannerService : IDiscordService, IMessageScannerService
	{
		private readonly DiscordSocketClient _client;

		private readonly IJSONSerializerService _serializer;

		private readonly ILoggingService _logger;

		private readonly List<string> _swearWordList;
		private readonly List<string> _waifuList;
		private readonly List<string> _marekFaceWords;
		private readonly List<string> _skeletorWords;
		private readonly List<string> _takeuchiWords;
		private readonly List<string> _ziewaczWords;
		private readonly List<string> _nosaczWords;

		public IConfiguration Configuration { get; set; }

		public MessageScannerService(
			DiscordSocketClient client,
			IJSONSerializerService serializer,
			IConfiguration configuration,
			ILoggingService logger
			)
		{
			_client = client;
			_serializer = serializer;
			Configuration = configuration;
			_logger = logger;

			_swearWordList = _serializer.LoadFromFile<string>("swearWords.json");
			_marekFaceWords = _serializer.LoadFromFile<string>("marekTrigger.json");
			_skeletorWords = _serializer.LoadFromFile<string>("skeletorTrigger.json");
			_waifuList = _serializer.LoadFromFile<string>("marekWaifus.json");
			_takeuchiWords = _serializer.LoadFromFile<string>("takeuchiTrigger.json");
			_ziewaczWords = _serializer.LoadFromFile<string>("ziewaczTrigger.json");
			_nosaczWords = _serializer.LoadFromFile<string>("nosaczTrigger.json");
		}

		public async Task ScanMessageAsync(SocketMessage s)
		{
			if (!(s is SocketUserMessage message))
				return;

			var context = new SocketCommandContext(_client, message);

			if (!message.Author.IsBot && !message.Content.StartsWith(Configuration["prefix"]))
			{
				await DetectWaifusAsync(context, message);
				await AddReactionAfterTriggerWord(context, message, _marekFaceWords, "marekface");
				await AddReactionAfterTriggerWord(context, message, _skeletorWords, "skeletor");
				await AddReactionAfterTriggerWord(context, message, _takeuchiWords, "takeuchi");
				await AddReactionAfterTriggerWord(context, message, _ziewaczWords, "ziewface");
				await AddReactionAfterTriggerWord(context, message, _nosaczWords, "nosacz");
				await DetectMentionsAsync(context, message);
				await DetectSwearWordAsync(context, message);
				await DetectMarekMessageAsync(message);
			}
		}

		public async Task ScanUpdateMessageAsync(Cacheable<IMessage, ulong> oldMessage, SocketMessage s, ISocketMessageChannel channel)
		{
			if (!(s is SocketUserMessage message))
				return;

			var context = new SocketCommandContext(_client, message);

			if (!message.Author.IsBot)
			{
				await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _marekFaceWords, "marekface"), "marekface");
				await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _skeletorWords, "skeletor"), "skeletor");
				await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _takeuchiWords, "takeuchi"), "takeuchi");
				await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _ziewaczWords, "ziewface"), "ziewface");
				await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _nosaczWords, "nosacz"), "nosacz");
			}

			_logger.CustomEditLog(message, oldMessage.Value);
		}

		public async Task ScanDeletedMessageAsync(Cacheable<IMessage, ulong> deletedMessage, Cacheable<IMessageChannel, ulong> channel)
			=> await Task.Run(() => _logger.CustomDeleteLog(deletedMessage.Value));
		

		/// <summary>
		/// Add reaction if trigger word is detected in message.
		/// </summary>
		/// <returns>True if reaction emote was added.</returns>
		private async Task<bool> AddReactionAfterTriggerWord(SocketCommandContext context, SocketUserMessage message, ICollection<string> triggerWords, string emoteCode)
		{
			foreach (string word in triggerWords)
			{
				if (message.Content.ToLowerInvariant().Contains(word) && !message.Author.IsBot)
				{
					GuildEmote emote = context.Guild.Emotes.Where(x => x.Name.ToLower().Equals(emoteCode)).FirstOrDefault();

					if (emote != null)
					{
						await message.AddReactionAsync(emote);
						_logger.CustomReactionLog(message, emote.Name);
					}
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Remove reaction if trigger word is edited out.
		/// </summary>
		private async Task RemoveReactionAfterTriggerMissing(SocketCommandContext context, SocketUserMessage message, bool shouldRemove, string emoteCode)
		{
			if (shouldRemove)
			{
				GuildEmote emote = context.Guild.Emotes.Where(x => x.Name.ToLower().Equals(emoteCode)).FirstOrDefault();

				if (emote != null)
				{
					await message.RemoveReactionAsync(emote, context.Client.CurrentUser);
				}
			}
		}

		public async Task DetectMentionsAsync(SocketCommandContext context, SocketUserMessage message)
		{
			if (message.MentionedUsers.Where(x => x.DiscordId().Equals("Marek Motyka#3254") || x.DiscordId().Equals("Erina#5946")).FirstOrDefault() != null ||
				message.Tags.Any(x => x.Type.Equals(TagType.EveryoneMention) || x.Type.Equals(TagType.HereMention)))
			{
				var today = DateTime.Now;

				var response = (today.DayOfWeek, today.Hour) switch
				{
					(DayOfWeek day, int hour) when ((day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) && hour < 12) => StringConsts.Snoring,
					(DayOfWeek day, int hour) when ((day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)) => StringConsts.Drunk,
					(DayOfWeek day, int hour) when (hour < 9) => StringConsts.Snoring,
					(DayOfWeek day, int hour) when (hour < 18) => StringConsts.Job,
					_ => StringConsts.LonkStole
				};

				await context.Channel.SendMessageAsync(response);
			}
		}

		/// <summary>
		/// Detect waifus name in each message
		/// </summary>
		public async Task DetectWaifusAsync(SocketCommandContext context, SocketUserMessage message)
		{
			foreach (var waifuName in _waifuList)
			{
				if (message.Content.ToLowerInvariant().Contains(waifuName.ToLowerInvariant()) && !message.Author.IsBot)
				{
					await context.Channel.SendMessageAsync(string.Format(StringConsts.MarekWaifus, waifuName));
					return;
				}
			}
			var containsNakiri = message.Content.ToLowerInvariant().Contains("nakiri") && !message.Author.IsBot;
			if (containsNakiri)
			{
				await context.Channel.SendMessageAsync(StringConsts.OnlyErina);
			}
		}

		/// <summary>
		/// Check for swearword and who posted it - increment counter;
		/// </summary>
		public async Task DetectSwearWordAsync(SocketCommandContext context, SocketUserMessage message)
		{
			string compressedText = message.Content.RemoveRepeatingChars();
			compressedText = new string(compressedText.Where(c => !char.IsWhiteSpace(c)).ToArray());
			compressedText = compressedText.ToLowerInvariant();

			foreach (string swearWord in _swearWordList)
			{
				if (compressedText.Contains(swearWord.ToLowerInvariant()) && !message.Author.IsBot)
				{
					var counterList = await _serializer.LoadFromFileAsync<WordCounterEntry>("wordCounter.json");

					string messageText = message.Content.ToLowerInvariant();
					string username = context.User.DiscordId();

					while (messageText.Contains(swearWord))
					{
						if (counterList.Exists(x => x.DiscordUsername.Equals(username) && x.Word.Equals(swearWord)))
						{
							counterList.Find(x => x.DiscordUsername.Equals(username) && x.Word.Equals(swearWord)).CounterValue++;
						}
						else
						{
							counterList.Add(new WordCounterEntry(username, context.User.Username, swearWord, 1));
						}

						int firstSubstringIndex = messageText.IndexOf(swearWord);
						messageText = (firstSubstringIndex < 0)
										? messageText
										: messageText.Remove(firstSubstringIndex, swearWord.Length);
					}

					await _serializer.SaveToFileAsync("wordCounter.json", counterList);
				}
			}
		}

		public async Task DetectMarekMessageAsync(SocketUserMessage message)
		{
			if (message.Author.DiscordId().Equals("Erina#5946"))
			{
				var lastMessage = await _serializer.LoadSingleFromFileAsync<LastMarekMessage>("marekLastMessage.json");

				bool isImage = string.IsNullOrWhiteSpace(message.Content) && message.Attachments.Any();

				lastMessage.MessageContent = isImage ?
					message.Attachments.First().Url :
					message.Content;
				lastMessage.IsImage = isImage;

				lastMessage.DatePosted = message.Timestamp.DateTime.ToLocalTime();

				await _serializer.SaveSingleToFileAsync("marekLastMessage.json", lastMessage);
			}
		}
	}
}