using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _configuration;

        public StartupService(IConfiguration configuration, DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _configuration = configuration;
        }

        public async Task StartAsync()
        {
            string discordToken = _configuration["tokens:discord"];
            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}