using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using MarekMotykaBot.Services;

namespace MarekMotykaBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();
        

        public async Task StartAsync()
        {
            var services = new ServiceCollection()      
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,   
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton<CommandHandlingService>()    
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<MessageScannerService>()
                .AddSingleton<YTService>()
                .AddSingleton<Random>();

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandlingService>();

            await Task.Delay(-1); 
        }
    }
}
