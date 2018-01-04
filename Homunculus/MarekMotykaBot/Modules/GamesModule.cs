using Discord.Commands;
using MarekMotykaBot.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class GamesModule : ModuleBase<SocketCommandContext>
    {
        private readonly Random _rng;

        public GamesModule(Random random)
        {
            _rng = random;
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
                    if (maxNumber == -1)
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
    }
}