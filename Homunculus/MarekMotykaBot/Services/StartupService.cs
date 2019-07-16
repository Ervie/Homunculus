using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class StartupService: IDiscordService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
		private readonly IServiceProvider _provider;

		public IConfiguration Configuration { get; set; }

        public StartupService(IConfiguration configuration, DiscordSocketClient discord, CommandService commands, IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            Configuration = configuration;
			_provider = provider;
        }

        public async Task StartAsync()
        {
            string discordToken = Configuration["tokens:discord"];
            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}