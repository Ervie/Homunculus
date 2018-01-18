using Discord.Commands;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class WaifuModule : ModuleBase<SocketCommandContext>
    {
        private readonly Random _rng;

        private readonly JSONSerializerService _serializer;

        private readonly List<string> _marekWaifuList;

        public WaifuModule(Random random, JSONSerializerService serializer)
        {
            _rng = random;
            _serializer = serializer;

            _marekWaifuList = serializer.LoadFromFile<string>("marekWaifus.json");
        }

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

            int selectedWaifuIndex = _rng.Next(waifus.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBetter, waifus[selectedWaifuIndex]));
        }

        [Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, multiple waifus separated with spaces")]
        public async Task BetterWaifu(params string[] waifus)
        {
            var waifusList = waifus.ToList();
            waifusList = waifusList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

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

            int selectedWaifuIndex = _rng.Next(waifusList.Count);

            switch (waifusList.Count)
            {
                case 0:
                case 1:
                    await Context.Channel.SendMessageAsync("...");
                    break;
                case 2:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBetter, waifus[selectedWaifuIndex]));
                    break;
                default:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuBest, waifusList[selectedWaifuIndex]));
                    break; 
            }
        }

        [Command("BetterHusbando"), Alias("bh"), Summary("Husbando selector, 2 husbandos")]
        public async Task BetterHusbando(string firstHusbando, string secondHusbando)
        {
            var husbandos = new List<string>();

            husbandos.Add(firstHusbando);
            husbandos.Add(secondHusbando);
            
            int selectedHusbandoIndex = _rng.Next(husbandos.Count);

            await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBetter, husbandos[selectedHusbandoIndex]));
        }

        [Command("BetterHusbando"), Alias("bh"), Summary("Husbando selector, multiple husbandos separated with spaces")]
        public async Task BetterHusbando(params string[] husbandos)
        {
            var husbandoList = husbandos.ToList();
            husbandoList = husbandoList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

            int selectedHusbandoIndex = _rng.Next(husbandoList.Count);

            switch (husbandoList.Count)
            {
                case 0:
                case 1:
                    await Context.Channel.SendMessageAsync("...");
                    break;
                case 2:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBest, husbandoList[selectedHusbandoIndex]));
                    break;
                default:
                    await Context.Channel.SendMessageAsync(string.Format(StringConsts.HusbandoBetter, husbandos[selectedHusbandoIndex]));
                    break;
            }
        }
    }
}