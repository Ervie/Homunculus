using Discord.Commands;
using System;
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
            int rolledNumber = _rng.Next(1, 6);

            await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
        }

        [Command("Roll_k100"), Alias("k100"), Summary("Roll the k100 dice.")]
        public async Task RollK100Async()
        {
            int rolledNumber = _rng.Next(1, 100);

            await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
        }

        [Command("Roll"), Alias("k"), Summary("Roll customizable dice.")]
        public async Task RollDiceAsync(string diceSize)
        {
            try
            {
                int maxNumber = Int32.Parse(diceSize);

                int rolledNumber = _rng.Next(1, maxNumber);

                await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
            }
            catch (FormatException)
            {
                await ReplyAsync(Context.User.Username + ", wiesz co to liczba naturalna?");
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync("Panieee, tak to się nie da.");
            }
            catch (OverflowException)
            {
                await ReplyAsync("Za dużo :/");
            }
            catch (Exception)
            {
            }
        }

        [Command("Roll"), Alias("k"), Summary("Roll customizable dice.")]
        public async Task RollDiceAsync()
        {
            await ReplyAsync("Ni ma liczby, rzucam zwykłą.");

            int rolledNumber = _rng.Next(1, 6);

            await ReplyAsync(Context.User.Username + ": " + rolledNumber.ToString());
        }

        [Command("Flip_coin"), Alias("flip"), Summary("Flip the coin.")]
        public async Task FlipCoinAsync()
        {
            int result = _rng.Next(1, 2);

            string resultString = result == 1 ? "Orzeł" : "Reszka";

            await ReplyAsync(Context.User.Username + ": " + resultString);
        }
    }
}