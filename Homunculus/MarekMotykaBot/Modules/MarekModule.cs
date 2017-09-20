using Discord.Commands;
using ImgFlipAPI.APISource.Core;
using ImgFlipAPI.APISource.Core.Models;
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
    public class MarekModule : ModuleBase<SocketCommandContext>
    {
        private readonly ImgurService _imgur;
        private readonly JSONSerializer _serializer;
        private readonly ImgFlipService _imgFlip;
        private readonly Random _rng;

        private readonly string[] eightBallResponses =
        {
            "Tak",
            "Nie",
            "Być może",
            "Z tym to se możesz... To wiesz co se możesz",
            "Oczywiście, {0} potwierdzi",
            "No pewex",
            "Ale że co? No prośba...",
            "Nie odpowiem Ci teraz, bo piszę magisterkę",
            "To {0} tak mówił",
            "Doubt",
            "TakNieTakNieTakNie :^)",
            "Kek"
        };

        public MarekModule(ImgurService imgur, JSONSerializer serializer, ImgFlipService imgFlip, Random random)
        {
            _imgur = imgur;
            _serializer = serializer;
            _rng = random;
            _imgFlip = imgFlip;
        }

        [Command("NoCoSeMoge"), Alias("no"), Summary("He will tell you what you can do")]
        public async Task CoSeMogeAsync()
        {
            await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
            await Task.Delay(3000);
            await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
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

        //TODO: Meme generator
        [Command("MarekMeme"), Alias("meme"), Summary("Create your own Marek meme image, text split by semicolon")]
        public async Task NewMemeAsync(params string[] text)
        {
            var captions = string.Join(" ", text).Split(';').ToList();

            if (captions.Count < 2)
                return;

            if (string.IsNullOrWhiteSpace(captions.ElementAt(0)) || string.IsNullOrWhiteSpace(captions.ElementAt(1)))
                return;

            string toptext = captions.ElementAt(0);
            string bottomtext = captions.ElementAt(1);

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
            int randomResponseIndex = _rng.Next(0, eightBallResponses.ToList().Count - 1);

            string selectedResponse = eightBallResponses[randomResponseIndex];

            var users = Context.Guild.Users.Where(x => !x.DiscordId().Equals("MarekMotykaBot#2213") && !x.DiscordId().Equals("Erina#5946")).ToList();

            int randomUserIndex = _rng.Next(0, users.Count - 1);

            var selectedUser = users.ElementAt(randomUserIndex).Username;

            await Context.Channel.SendMessageAsync($"{string.Format(selectedResponse, selectedUser)}");
        }
    }
}