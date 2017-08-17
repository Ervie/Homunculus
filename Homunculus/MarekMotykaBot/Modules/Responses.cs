using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class Responses: ModuleBase<SocketCommandContext>
    {
        [Command("Erina")]
        public async Task ErinaHeart()
        {
            for (int i = 0; i < 3; i++)
            {
                await Context.Channel.SendMessageAsync("<3");
                await Task.Delay(2000);
            }
        }
    }
}
