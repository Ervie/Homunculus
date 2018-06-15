using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarekMotykaBot
{
	public class MessageScannerService : IDiscordService
	{
		private readonly DiscordSocketClient _client;

		private readonly JSONSerializerService _serializer;

		private readonly DropboxService _dropbox;

		private readonly List<string> _swearWordList;

		private readonly List<string> _waifuList;

		private readonly List<string> _marekFaceWords;

		private readonly List<string> _takeuchiWords;

        private readonly List<string> _ziewaczWords;

        public IConfiguration Configuration { get; set; }

		public MessageScannerService(DiscordSocketClient client, JSONSerializerService serializer, DropboxService dropbox, IConfiguration configuration)
		{
			_client = client;
			_serializer = serializer;
			Configuration = configuration;
			_dropbox = dropbox;

			_swearWordList = _serializer.LoadFromFile<string>("swearWords.json");
			_marekFaceWords = _serializer.LoadFromFile<string>("marekTrigger.json");
			_waifuList = serializer.LoadFromFile<string>("marekWaifus.json");
			_takeuchiWords = serializer.LoadFromFile<string>("takeuchiTrigger.json");
            _ziewaczWords = serializer.LoadFromFile<string>("ziewaczTrigger.json");
        }

		public async Task ScanMessage(SocketMessage s)
		{

			if (!(s is SocketUserMessage message))
				return;

			var context = new SocketCommandContext(_client, message);

			if (!message.Author.IsBot && !message.Content.StartsWith(Configuration["prefix"]))
			{
				await DetectWaifus(context, message);
				await AddReactionAfterTriggerWord(context, message, _marekFaceWords, "marekface");
				await AddReactionAfterTriggerWord(context, message, _takeuchiWords, "takeuchi");
                await AddReactionAfterTriggerWord(context, message, _ziewaczWords, "ziewface");
				await DetectMentions(context, message);
				await DetectSwearWord(context, message);
				await DetectStreamMonday(context, message);
			}
		}

		public async Task ScanUpdateMessage(Cacheable<IMessage, ulong> oldMessage, SocketMessage s, ISocketMessageChannel channel)
		{

			if (!(s is SocketUserMessage message))
				return;

			var context = new SocketCommandContext(_client, message);

			if (!message.Author.IsBot)
			{
				await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _marekFaceWords, "marekface"), "marekface");
				await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _takeuchiWords, "takeuchi"), "takeuchi");
                await RemoveReactionAfterTriggerMissing(context, message, await AddReactionAfterTriggerWord(context, message, _ziewaczWords, "ziewface"), "ziewface");
            }
		}

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

		private async Task DetectMentions(SocketCommandContext context, SocketUserMessage message)
		{
			if (message.MentionedUsers.Where(x => x.DiscordId().Equals("MarekMotykaBot#2213") || x.DiscordId().Equals("Erina#5946")).FirstOrDefault() != null ||
				message.Tags.Any(x => x.Type.Equals(TagType.EveryoneMention) || x.Type.Equals(TagType.HereMention)))
			{
				DateTime today = DateTime.Now;

				if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday)
				{
					if (today.Hour < 12)
						await context.Channel.SendMessageAsync(StringConsts.Snoring);
					else
						await context.Channel.SendMessageAsync(StringConsts.Girlfriend);
				}
				else
				{
					if (today.Hour < 7)
						await context.Channel.SendMessageAsync(StringConsts.Snoring);
					else if (today.Hour < 17)
						await context.Channel.SendMessageAsync(StringConsts.Job);
					else
						await context.Channel.SendMessageAsync(StringConsts.Doctor);
				}
			}
		}

		/// <summary>
		/// Detect waifus name in each message
		/// </summary>
		private async Task DetectWaifus(SocketCommandContext context, SocketUserMessage message)
		{
			foreach (string waifuName in _waifuList)
			{
				if (message.Content.ToLowerInvariant().Contains(waifuName.ToLowerInvariant()) && !message.Author.IsBot)
				{
					await context.Channel.SendMessageAsync(string.Format(StringConsts.MarekWaifus, waifuName));
					break;
				}
			}
		}

		/// <summary>
		/// Check for swearword and who posted it - increment counter;
		/// </summary>
		private async Task DetectSwearWord(SocketCommandContext context, SocketUserMessage message)
		{
			string compressedText = message.Content.RemoveRepeatingChars();
			compressedText = new string(compressedText.Where(c => !char.IsWhiteSpace(c)).ToArray());
			compressedText = compressedText.ToLowerInvariant();

			foreach (string swearWord in _swearWordList)
			{
				if (compressedText.Contains(swearWord.ToLowerInvariant()) && !message.Author.IsBot)
				{
					var counterList = _serializer.LoadFromFile<WordCounterEntry>("wordCounter.json");

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

					_serializer.SaveToFile<WordCounterEntry>("wordCounter.json", counterList);

					await _dropbox.UploadFileAsync("wordCounter.json", "wordCounter.json");
				}
			}
		}

		/// <summary>
		/// Check for @Streamdziałek mention, show schedule.
		/// </summary>
		private async Task DetectStreamMonday(SocketCommandContext context, SocketUserMessage message)
		{
			if (message.MentionedRoles.Any(x => x.Name.Equals("Streamdziałek")))
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

				await context.Channel.SendMessageAsync("", false, builder.Build());
			}
			
		}
	}
}