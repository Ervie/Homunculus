using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class GamesModule : ModuleBase<SocketCommandContext>
    {
        private readonly Random _rng;

        private readonly JSONSerializerService _serializer;

        public GamesModule(Random random, JSONSerializerService serializer)
        {
            _rng = random;
            _serializer = serializer;
        }
		[Command("Someone"), Alias("s"), Summary("Get random username from server")]
		public async Task RandomUserAsync()
		{
			await ReplyAsync(Context.Guild.GetRandomUserName(_rng));
		}

        [Command("Roll_k6"), Alias("k6"), Summary("Roll the k6 dice.")]
        public async Task RollK6Async()
        {
            int rolledNumber = _rng.Next(0, 6) + 1;

            await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
        }

        [Command("Roll_k100"), Alias("k100"), Summary("Roll the k100 dice.")]
        public async Task RollK100Async()
        {
            int rolledNumber = _rng.Next(0, 100) + 1;

            await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
        }

        [Command("Roll"), Alias("k"), Summary("Roll customizable dice.")]
        public async Task RollDiceAsync(params string[] diceSize)
        {
            try
            {
                var diceSizes = diceSize.ToList();
                int i = 0;
                int maxNumber = -1;

                if (diceSizes.Count > 0)
                {
                    while (i < diceSizes.Count && !Int32.TryParse(diceSizes[i], out maxNumber))
                    {
                        i++;
                    }
                    if (maxNumber == -1 || maxNumber == 0)
                    {
                        await ReplyAsync(StringConsts.NoNumber);

                        int rolledNumber = _rng.Next(0, 6) + 1;

                        await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
                    }
                    else
                    {
                        int rolledNumber = _rng.Next(0, maxNumber) + 1;

                        await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
                    }
                }
                else
                {
                    await ReplyAsync(StringConsts.NoNumber);

                    int rolledNumber = _rng.Next(0, 6) + 1;

                    await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
                }
            }
            catch (NullReferenceException)
            {
                await ReplyAsync(StringConsts.NoNumber);

                int rolledNumber = _rng.Next(0, 6) + 1;

                await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
            }
            catch (IndexOutOfRangeException)
            {
                await ReplyAsync(StringConsts.NoNumber);

                int rolledNumber = _rng.Next(0, 6) + 1;

                await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
            }
            catch (FormatException)
            {
                await ReplyAsync(StringConsts.NoNumber);

                int rolledNumber = _rng.Next(0, 6) + 1;

                await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync(StringConsts.Impossible);
            }
            catch (OverflowException)
            {
                await ReplyAsync(StringConsts.TooMuch);
            }
            catch (Exception)
            {
            }
        }

        [Command("Flip_coin"), Alias("flip"), Summary("Flip the coin.")]
        public async Task FlipCoinAsync()
        {
            int result = _rng.Next(0, 2);

            string resultString = result == 1 ? "Orzeł" : "Reszka";

            await ReplyAsync(Context.User.Username + ": " + resultString);
        }

        [Command("Charade"), Alias("kalambury", "c"), Summary("Draw a random entry for charade game.")]
        public async Task CharadeAsync()
        {
            List<CharadeEntry> charadeCollection = _serializer.LoadFromFile<CharadeEntry>("Animes.json");
            List<int> charadeCache = _serializer.LoadFromFile<int>("charadeCache.json");

            if (charadeCache.Count() == charadeCollection.Count())
            {
                await Context.Channel.SendMessageAsync($"{StringConsts.CharadeEnd}");
                return;
            }

            int randomCharadeEntryIndex = -1;

            while (true)
            {
                randomCharadeEntryIndex = _rng.Next(0, charadeCollection.Count);

                if (charadeCache.Contains(charadeCollection[randomCharadeEntryIndex].Id))
                    continue;

                CharadeEntry selectedEntry = charadeCollection[randomCharadeEntryIndex];
				
                var builder = new EmbedBuilder();

                builder.WithImageUrl(selectedEntry.PicUrl);

                builder.AddField(x =>
                {
                    x.Name = "Tytuł";
                    x.Value = selectedEntry.Title;
                    x.IsInline = true;
                });

                if (selectedEntry.Translations.Count() > 0)
                {
                    builder.AddField(x =>
                    {
                        x.Name = "Tłumaczenia";
                        x.Value = string.Join(Environment.NewLine, selectedEntry.Translations);
                        x.IsInline = false;
                    });
                }

                await ReplyAsync("", false, builder.Build());

                charadeCache.Add(selectedEntry.Id);

                _serializer.SaveToFile<int>("charadeCache.json", charadeCache);

                break;
            }
        }

        [Command("ResetCharade"), Alias("reset", "r"), Summary("Reset charade cache")]
        public async Task CharadeResetAsync()
        {
            List<int> emptyList = new List<int>();

            _serializer.SaveToFile<int>("charadeCache.json", emptyList);

            await ReplyAsync(StringConsts.CharadeReset);
        }
    }
}