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
using System.Threading.Tasks;

namespace MarekMotykaBot
{
    public class MessageScannerService
    {
        private readonly DiscordSocketClient _client;

        private readonly JSONSerializer _serializer;

        private readonly IConfiguration _configuration;

        private readonly List<string> _swearWordList = new List<string>() { "penis" };

        private readonly List<string> _waifuList = new List<string>() { "Asuna", "Rias", "Erina" };

        private readonly List<string> _marekFaceWords = new List<string>() { "czerń", "czarn", "nigga", "nigger", "murzyn", "black", "schartz", "afryk", "africa", "negro", "kuro", "murzyń" };

        public MessageScannerService(DiscordSocketClient client, JSONSerializer serializer, IConfiguration configuration)
        {
            _client = client;
            _serializer = serializer;
            _configuration = configuration;
        }

        public async Task ScanMessage(SocketMessage s)
        {
            var message = s as SocketUserMessage;

            if (message == null)
                return;

            var context = new SocketCommandContext(_client, message);

            if (!message.Author.IsBot && !message.Content.StartsWith(_configuration["prefix"]))
            {
                await DetectWaifus(context, message);
                await DetectMarekFaceTriggerWords(context, message);
                await DetectMentions(context, message);
                await DetectSwearWord(context, message);
            }
        }

        public async Task ScanUpdateMessage(Cacheable<IMessage, ulong> oldMessage, SocketMessage s, ISocketMessageChannel channel)
        {
            var message = s as SocketUserMessage;

            if (message == null)
                return;

            var context = new SocketCommandContext(_client, message);

            if (!message.Author.IsBot)
            {
                await RemoveMarekFaceReaction(context, message, await DetectMarekFaceTriggerWords(context, message));
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
        /// Check for marekface trigger words in each message and add reaction.
        /// </summary>
        private async Task<bool> DetectMarekFaceTriggerWords(SocketCommandContext context, SocketUserMessage message)
        {
            foreach (string word in _marekFaceWords)
            {
                if (message.Content.ToLowerInvariant().Contains(word) && !message.Author.IsBot)
                {
                    var marekFace = context.Guild.Emotes.Where(x => x.Name.ToLower().Equals("marekface")).FirstOrDefault();

                    if (marekFace != null)
                    {
                        await message.AddReactionAsync(marekFace);
                    }
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Detect waidus name in each message
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
        /// Remove marekFace reaction.
        /// </summary>
        private async Task RemoveMarekFaceReaction(SocketCommandContext context, SocketUserMessage message, bool shouldRemove)
        {
            var marekFace = context.Guild.Emotes.Where(x => x.Name.ToLower().Equals("marekface")).FirstOrDefault();

            if (marekFace != null && shouldRemove)
            {
                await message.RemoveReactionAsync(marekFace, context.Client.CurrentUser);
            }
        }

        /// <summary>
        /// Check for swearword and who posted it - increment counter;
        /// </summary>
        private async Task DetectSwearWord(SocketCommandContext context, SocketUserMessage message)
        {
            foreach (string swearWord in _swearWordList)
            {
                if (message.Content.ToLowerInvariant().Contains(swearWord.ToLowerInvariant()) && !message.Author.IsBot)
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
                        messageText =  (firstSubstringIndex < 0)
                                        ? messageText
                                        : messageText.Remove(firstSubstringIndex, swearWord.Length);
                    }

                    _serializer.SaveToFile<WordCounterEntry>("wordCounter.json", counterList);
                }
            }
        }
    }
}