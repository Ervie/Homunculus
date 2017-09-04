using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarekMotykaBot.Resources;

namespace MarekMotykaBot.Modules
{
	public class MarekModule : ModuleBase<SocketCommandContext>
	{
		[Command("NoCoSeMoge"), Alias("no"), Summary("He will tell you what you can do")]
		public async Task CoSeMogeAsync()
		{
			await Context.Channel.SendMessageAsync($"*{StringConsts.WaitForIt}*");
			await Task.Delay(3000);
			await Context.Channel.SendMessageAsync($"**{StringConsts.ShitString}**");
		}
	}
}
