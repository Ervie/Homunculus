using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.ExtensionsMethods;
using MarekMotykaBot.Resources;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class CommandHandlingService : IDiscordService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly MessageScannerService _scanner;
        private readonly Random _rng;
        private readonly IServiceProvider _provider;

        public IConfiguration Configuration { get; set; }

        // DiscordSocketClient, CommandService, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandlingService(IConfiguration configuration, DiscordSocketClient discord, CommandService commands, MessageScannerService scanner, Random random, IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _scanner = scanner;
            _provider = provider;
            Configuration = configuration;
            _rng = random;

            SetStartingState();
        }

        public CommandService Commands => _commands;
        

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
			if (!(s is SocketUserMessage msg)) return;
			if (msg.Author == _discord.CurrentUser) return;

            var context = new SocketCommandContext(_discord, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(Configuration["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                if (!DeclineCommand(context, msg.Content).Result)
                {
                    var result = await Commands.ExecuteAsync(context, argPos, _provider);

                    if (!result.IsSuccess &&
						result.Error != CommandError.UnknownCommand &&
						result.Error != CommandError.BadArgCount)
                        await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }
        
        private void SetStartingState()
        {
            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageReceived += _scanner.ScanMessage;
            _discord.MessageUpdated += _scanner.ScanUpdateMessage;
        }

        private async Task<bool> DeclineCommand(SocketCommandContext context, string messageContent)
        {
            string commandName = messageContent.Split(' ').FirstOrDefault();
            bool commandDeclined = false;

            if (!string.IsNullOrWhiteSpace(commandName))
            {
                string meanResponse = string.Format(StringConsts.DeclineCommand, commandName);
                
                int randomInt = _rng.Next(1, 20);

                // bad luck, you suck
                if (randomInt == 1)
                {
                    commandDeclined = true;
                    await context.Channel.SendMessageAsync(meanResponse);
                }
            }
            return commandDeclined;
        }
    }
}