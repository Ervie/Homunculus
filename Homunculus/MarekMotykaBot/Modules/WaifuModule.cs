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
		private List<string> _marekWaifuList = new List<string>() { "Erina", "Asuna", "Rias"};

		[Command("Erina"), Summary("Only true waifu")]
        public async Task ErinaHeart()
        {
            await Context.Channel.SendMessageAsync("<3");
        }

		[Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, 2 waifus")]
		public async Task BetterWaifu(string firstWaifu, string secondWaifu)
		{
			var waifus = new List<string>();

			waifus.Add(firstWaifu);
			waifus.Add(secondWaifu);

			if (waifus.Contains("Asuna") && waifus.Contains("Erina"))
			{
				await Context.Channel.SendMessageAsync("Obie świetne <3!");
				return;
			}

			foreach (string waifu in _marekWaifuList)
			{
				if (waifus.Contains(waifu))
				{
					await Context.Channel.SendMessageAsync($"{waifu} najlepsza, reszta to shit!");
					return;
				}
			}

			Random rng = new Random();

			int selectedWaifuIndex = rng.Next(waifus.Count);

			await Context.Channel.SendMessageAsync($"{waifus[selectedWaifuIndex]} lepsza.");
		}

		[Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, multiple waifus")]
		public async Task BetterWaifu(params string[] args)
		{
			var waifus = args.ToList();

			foreach (string waifu in _marekWaifuList)
			{
				if (waifus.Contains(waifu))
				{
					await Context.Channel.SendMessageAsync($"{waifu} najlepsza, reszta to shit!");
					return;
				}
			}

			Random rng = new Random();

			int selectedWaifuIndex = rng.Next(waifus.Count);

			await Context.Channel.SendMessageAsync($"{waifus[selectedWaifuIndex]} najlepsza.");
		}

		

	}
}
