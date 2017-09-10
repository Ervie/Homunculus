using Discord.Commands;
using MarekMotykaBot.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class WaifuModule : ModuleBase<SocketCommandContext>
    {
        private List<string> _marekWaifuList = new List<string>() { "Erina", "Asuna", "Rias" };

        [Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, 2 waifus")]
        public async Task BetterWaifu(string firstWaifu, string secondWaifu)
        {
            var waifus = new List<string>();

            waifus.Add(firstWaifu);
            waifus.Add(secondWaifu);

            if (waifus.Contains("Legia"))
            {
                await Context.Channel.SendMessageAsync(StringConsts.LegiaWarszawa);
                return;
            }

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

        [Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, multiple waifus separated with spaces")]
        public async Task BetterWaifu(params string[] waifus)
        {
            var waifusList = waifus.ToList();

            if (waifusList.Contains("Asuna") && waifusList.Contains("Erina"))
            {
                await Context.Channel.SendMessageAsync(StringConsts.WaifuEqual);
                return;
            }

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

        [Command("BetterHusbando"), Alias("bh"), Summary("Husbando selector, 2 husbandos")]
        public async Task BetterHusbando(string firstHusbando, string secondHusbando)
        {
            var husbandos = new List<string>();

            husbandos.Add(firstHusbando);
            husbandos.Add(secondHusbando);

            Random rng = new Random();

            int selectedHusbandoIndex = rng.Next(husbandos.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBetter, husbandos[selectedHusbandoIndex]));
        }

        [Command("BetterHusbando"), Alias("bh"), Summary("Husbando selector, multiple husbandos separated with spaces")]
        public async Task BetterHusbando(params string[] husbandos)
        {
            var husbandoList = husbandos.ToList();

            Random rng = new Random();

            int selectedHusbandoIndex = rng.Next(husbandoList.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBest, husbandoList[selectedHusbandoIndex]));
        }
    }
}