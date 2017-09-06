using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarekMotykaBot.Resources;

namespace MarekMotykaBot.Modules
{
    public class ReactionImagesModule : ModuleBase<SocketCommandContext>
    {

        private readonly string resources = System.AppContext.BaseDirectory + "/Resources/Images/Reactions/";


        [Command("Jameson"), Alias("jjj"), Summary("JJJ laught at your foolishness")]
        public async Task JJJAsync()
        {
            await Context.Channel.SendFileAsync(resources + "jjj.gif");
        }
    }
}
