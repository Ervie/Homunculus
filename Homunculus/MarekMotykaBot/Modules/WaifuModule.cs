using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MarekMotykaBot.Modules
{
    public class ResponseModule: ModuleBase<SocketCommandContext>
    {
        
        [Command("Erina")]
        public async Task ErinaHeart(IMessage message)
        {
            await Context.Channel.SendMessageAsync("<3");
        } 

        
    }
}
