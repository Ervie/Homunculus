using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MarekMotykaBot
{
    public class Program
    {
        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        private CommandHandler _handler;

        public async Task StartAsync()
        {

            _client = new DiscordSocketClient();
            
            await _client.LoginAsync(TokenType.Bot, "MzQ3ODM1MjAyMjIwOTgyMjgz.DHeKuA.gKhpvqPwkPHsr9Jgh5lHBjw5UQ0");

            await _client.StartAsync();

            _handler = new CommandHandler(_client);

            await _handler.InstallCommands();

            await Task.Delay(-1);
        }

    }
}
