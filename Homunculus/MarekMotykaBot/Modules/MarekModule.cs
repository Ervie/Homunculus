using Discord.Commands;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class MarekModule : ModuleBase<SocketCommandContext>
    {
        private readonly ImgurService _imgur;

        public MarekModule(ImgurService imgur)
        {
            _imgur = imgur;
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

        //[Command("Joke"), Alias("joke"), Summary("Marek's joke - you know the drill")]
        //public async Task JokeAsync()
        //{
        //    await ReplyAsync("a");
        //}
    }
}