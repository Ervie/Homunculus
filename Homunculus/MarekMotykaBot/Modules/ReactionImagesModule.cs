using Discord.Commands;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class ReactionImagesModule : ModuleBase<SocketCommandContext>
    {
        private readonly string resources = System.AppContext.BaseDirectory + "/Resources/Images/Reactions/";

        [Command("Jameson"), Alias("jjj"), Summary("JJJ laughs at your foolishness")]
        public async Task JJJAsync()
        {
            await Context.Channel.SendFileAsync(resources + "jjj.gif");
        }
    }
}