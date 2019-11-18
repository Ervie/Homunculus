using Discord.Commands;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class WaifuModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly Random _rng;

		private readonly List<string> _marekWaifuList;

		public string ModuleName { get => "WaifuModule"; }

		public ILoggingService LoggingService { get; }

		public WaifuModule(
			Random random,
			IJSONSerializerService serializer,
			ILoggingService loggingService
		)
		{
			_rng = random;
			LoggingService = loggingService;

			_marekWaifuList = serializer.LoadFromFile<string>("marekWaifus.json");
		}

		[Command("BetterWaifu"), Alias("bw"), Summary("Waifu selector, multiple waifus separated with spaces")]
		public async Task BetterWaifu(params string[] waifus)
		{
			var waifusList = waifus
				.Distinct(StringComparer.CurrentCultureIgnoreCase)
				.ToList();

			if (waifusList.Contains("Asuna") && waifusList.Contains("Erina"))
			{
				await Context.Channel.SendMessageAsync(StringConsts.WaifuEqual);
				return;
			}

			foreach (string waifu in _marekWaifuList)
			{
				if (waifusList.Contains(waifu.RemoveEmojisAndEmotes(), StringComparer.CurrentCultureIgnoreCase))
				{
					await Context.Channel.SendMessageAsync(string.Format(StringConsts.WaifuShit, waifu));
					return;
				}
			}

			int selectedWaifuIndex = _rng.Next(waifusList.Count);

			string returnedAnswer = waifusList.Count switch
			{
				0 => "...",
				1 => "...",
				2 => string.Format(StringConsts.WaifuBetter, waifus[selectedWaifuIndex]),
				_ => string.Format(StringConsts.WaifuBest, waifusList[selectedWaifuIndex])
			};

			await Context.Channel.SendMessageAsync(returnedAnswer);
			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', waifus));
		}

		[Command("BetterHusbando"), Alias("bh"), Summary("Husbando selector, multiple husbandos separated with spaces")]
		public async Task BetterHusbando(params string[] husbandos)
		{
			var husbandoList = husbandos
				.Distinct(StringComparer.CurrentCultureIgnoreCase)
				.ToList();

			int selectedHusbandoIndex = _rng.Next(husbandoList.Count);

			string returnedAnswer = husbandoList.Count switch
			{
				0 => "...",
				1 => "...",
				2 => string.Format(StringConsts.HusbandoBetter, husbandoList[selectedHusbandoIndex]),
				_ => string.Format(StringConsts.HusbandoBest, husbandoList[selectedHusbandoIndex])
			};

			await Context.Channel.SendMessageAsync(returnedAnswer);
			LoggingService.CustomCommandLog(Context.Message, ModuleName, string.Join(' ', husbandos));
		}
	}
}