using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarekMotykaBot.Resources;

namespace MarekMotykaBot.Modules
{
    public class WaifuModule : ModuleBase<SocketCommandContext>
    {
        private List<string> _marekWaifuList = new List<string>() { "Erina", "Asuna", "Rias" };

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
                await Context.Channel.SendMessageAsync(StringConsts.WaifuEqual);
                return;
            }

            foreach (string waifu in _marekWaifuList)
            {
                if (waifus.Contains(waifu))
                {
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuShit, waifu));
                    return;
                }
            }

            Random rng = new Random();

            int selectedWaifuIndex = rng.Next(waifus.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBetter, waifus[selectedWaifuIndex]));
        }

        [Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, multiple waifus separated with coma")]
        public async Task BetterWaifu(params string[] waifus)
        {
            var waifusList = waifus.ToList();

            foreach (string waifu in _marekWaifuList)
            {
                if (waifusList.Contains(waifu))
                {
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuShit, waifu));
                    return;
                }
            }

            Random rng = new Random();

            int selectedWaifuIndex = rng.Next(waifusList.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBest, waifusList[selectedWaifuIndex]));
        }
    }
}