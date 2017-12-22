using Discord.Commands;
using Discord;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MarekMotykaBot.Modules
{
    public class MarekModule : ModuleBase<SocketCommandContext>
    {
        private readonly ImgurService _imgur;
        private readonly JSONSerializer _serializer;
        private readonly ImgFlipService _imgFlip;
        private readonly Random _rng;
        private readonly IConfiguration _configuration;

        private readonly string[] eightBallResponses =
        {
            "Tak",
            "Nie",
            "Być może",
            "Z tym to se możesz... To wiesz co se możesz",
            "Oczywiście, {0} potwierdzi",
            "No pewex",
            "Ale że co? No prośba...",
            "To {0} tak mówił",
            ">Doubt",
            "TakNieTakNieTakNie :^)",
            "Kek",
            "Prędzej schlam się piwem bezalkoholowym!",
            "Wiadomix",
            "No wiadomix, że nie",
            "Wydaje mi się, że tak",
            "Zdaje Ci się!",
            "Chyba w twoich snach!"
        };

        private SortedList<string, string> cache = new SortedList<string, string>();
        private const byte cacheSize = 10;

        public MarekModule(IConfiguration configuration, ImgurService imgur, JSONSerializer serializer, ImgFlipService imgFlip, Random random)
        {
            _configuration = configuration;
            _imgur = imgur;
            _serializer = serializer;
            _rng = random;
            _imgFlip = imgFlip;
        }

        [Command("NoCoSeMoge"), Alias("no"), Summary("He will tell you what you can do")]
        public async Task CoSeMogeAsync()
        {
            int randomNumer = _rng.Next(1, 10);

            switch (randomNumer)
            {
                case (1):
                    await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
                    await Task.Delay(3000);
                    await Context.Channel.SendMessageAsync($"...");
                    await Task.Delay(1000);
                    await Context.Channel.SendMessageAsync($"...");
                    await Task.Delay(1000);
                    await Context.Channel.SendMessageAsync($"**{StringConsts.RunAway}**");
                    break;
                case (2):
                case (3):
                    await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
                    await Task.Delay(3000);
                    await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
                    await Task.Delay(1500);
                    await Context.Channel.SendMessageAsync($"**{StringConsts.InTheJar}**");
                    break;

                default:
                    await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
                    await Task.Delay(3000);
                    await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
                    break;
            }

            
        }

        [Command("Sowa"), Alias("owl"), Summary("Post random owl image")]
        public async Task SowaAsync()
        {
            string gifUrl;
            gifUrl = await _imgur.GetRandomImageFromGallery("CbtU3");

            await ReplyAsync(gifUrl);
        }

        [Command("MarekMeme"), Alias("meme"), Summary("Post random old Marek meme image")]
        public async Task OldMemeAsync()
        {
            string gifUrl;
            gifUrl = await _imgur.GetRandomImageFromAlbum("V5CPd");

            await ReplyAsync(gifUrl);
        }

        [Command("MarekMeme"), Alias("meme"), Summary("Create your own Marek meme image, text split by semicolon")]
        public async Task NewMemeAsync(params string[] text)
        {
            var captions = string.Join(" ", text).Split(';').ToList();

            if (captions.Count < 2)
                return;

            if (string.IsNullOrWhiteSpace(captions.ElementAt(0)) || string.IsNullOrWhiteSpace(captions.ElementAt(1)))
                return;

            string toptext = captions.ElementAt(0).ToUpper();
            string bottomtext = captions.ElementAt(1).ToUpper();

            string resultUrl = await _imgFlip.CreateMarekMeme(toptext, bottomtext);

            await ReplyAsync(resultUrl);
        }

        [Command("Joke"), Summary("Marek's joke - you know the drill")]
        public async Task JokeAsync()
        {
            List<OneLinerJoke> jokes = _serializer.LoadOneLiners();

            int randomJokeIndex = _rng.Next(1, jokes.Count);

            OneLinerJoke selectedJoke = jokes[randomJokeIndex];

            await Context.Channel.SendMessageAsync($"{selectedJoke.Question}");
            await Task.Delay(3000);
            await Context.Channel.SendMessageAsync($"{selectedJoke.Punchline}");
        }

        [Command("8ball"), Summary("Binary answer for all your questions")]
        public async Task EightBallAsync(params string[] text)
        {
            cache = _serializer.LoadEightBallCache();

            string messageKey = Context.User.DiscordId() + string.Join(" ", text);

            // Check if message was not received earlier; If yes, send same answer
            if (cache.ContainsKey(messageKey))
            {
                await Context.Channel.SendMessageAsync(cache[messageKey]);
            }
            else
            {
                int randomResponseIndex = _rng.Next(0, eightBallResponses.ToList().Count - 1);

                string selectedResponse = eightBallResponses.ElementAt(randomResponseIndex);

                var users = Context.Guild.Users.Where(x => !x.DiscordId().Equals("MarekMotykaBot#2213") && !x.DiscordId().Equals("Erina#5946")).ToList();

                int randomUserIndex = _rng.Next(0, users.Count - 1);

                string selectedUser = users.ElementAt(randomUserIndex).Username;

                if (cache.Count > cacheSize)
                    cache.RemoveAt(0);

                cache.Add(messageKey, string.Format(selectedResponse, selectedUser));

                _serializer.SaveEightBallCache(cache);

                await Context.Channel.SendMessageAsync($"{string.Format(selectedResponse, selectedUser)}");
            }
        }

        [Command("blueribbon"), Summary("Passes for hidden gift")]
        public async Task UnityAsync()
        {
            if (!Context.User.DiscordId().Equals("Tarlfgar#9358"))
            {
                await Context.Channel.SendMessageAsync(String.Format(StringConsts.SecretGiftDeny, "Lonka!"));
            }
            else
            {
                await Context.Channel.SendMessageAsync("Helion user e-mail: " + _configuration["credentials:helionUser"]);
                await Context.Channel.SendMessageAsync("Helion password: " + _configuration["credentials:helionPassword"]);
            }
        }
    }
}