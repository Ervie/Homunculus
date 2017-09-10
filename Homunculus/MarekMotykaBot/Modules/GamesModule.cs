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

        [Command("Flip_coin"), Alias("flip"), Summary("Flip the coin.")]
        public async Task FlipCoinAsync()
        {
            int result = _rng.Next(1, 2);

            string resultString = result == 1 ? "Orzeł" : "Reszka";

            await ReplyAsync(Context.User.Username + ": " + resultString);
        }
    }
}