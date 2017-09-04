using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MarekMotykaBot.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MarekMotykaBot
{
    public class Startup
    {
        public async Task<IServiceProvider> ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<CommandHandler>();

            // Discord
            await LoadDiscordAsync(services);

            // Google
            await LoadGoogleAsync(services);

            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            return provider;
        }

        private async Task LoadDiscordAsync(IServiceCollection services)
        {
            var discord = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            });

            var commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
                ThrowOnError = false
            });

            discord.Log += OnLogAsync;
            commands.Log += OnLogAsync;

            await discord.LoginAsync(TokenType.Bot, "MzQ3ODM1MjAyMjIwOTgyMjgz.DHeKuA.gKhpvqPwkPHsr9Jgh5lHBjw5UQ0");
            await discord.StartAsync();

            services.AddSingleton(discord);
            services.AddSingleton(commands);
        }

        private Task LoadGoogleAsync(IServiceCollection services)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "ToBeReplaced",
                MaxUrlLength = 256
            });

            services.AddSingleton(youtube);
            return Task.CompletedTask;
        }

        private Task OnLogAsync(LogMessage msg)
            => PrettyConsole.LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}