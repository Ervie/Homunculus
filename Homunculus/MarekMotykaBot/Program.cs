using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using MarekMotykaBot.Utils;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MarekMotykaBot
{
    public class Program
    {
        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        //private DiscordSocketClient _client;

        //private CommandHandler _handler;

        private Startup _startup = new Startup();

        public async Task StartAsync()
        {
            PrettyConsole.NewLine($"MarekMotykaBot v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
            PrettyConsole.NewLine();

            var services = await _startup.ConfigureServices();

            var manager = services.GetService<CommandHandler>();

            await manager.InstallCommands();

            //_client = new DiscordSocketClient();

            //await _client.LoginAsync(TokenType.Bot, "MzQ3ODM1MjAyMjIwOTgyMjgz.DHeKuA.gKhpvqPwkPHsr9Jgh5lHBjw5UQ0");

            //await _client.StartAsync();

            //_handler = new CommandHandler(_client);

            //await _handler.InstallCommands();

            await Task.Delay(-1);
        }

    }
}
