using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class MarekModule : ModuleBase<SocketCommandContext>
    {
        private readonly ImgurService _imgur;
		private readonly JSONSerializer _serializer;
		private readonly Random _rng;

		public MarekModule(ImgurService imgur, JSONSerializer serializer, Random random)
        {
            _imgur = imgur;
			_serializer = serializer;
			_rng = random;
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
	}
}