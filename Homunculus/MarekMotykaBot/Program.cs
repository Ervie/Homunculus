using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using MarekMotykaBot.Services.Core;
using MarekMotykaBot.Services.External;
using MarekMotykaBot.Services.Core.Interfaces;
using MarekMotykaBot.Services.External.Interfaces;

namespace MarekMotykaBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();
        
        private IConfiguration _config;

        public async Task StartAsync()
        {
            var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("configuration.json");

            _config = builder.Build();

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
				.AddSingleton<ICommandHandlingService, CommandHandlingService>()
				.AddSingleton<ILoggingService, LoggingService>()
				.AddSingleton<IStartupService, StartupService>()
				.AddSingleton<IMessageScannerService, MessageScannerService>()
				.AddSingleton<IYTService, YTService>()
				.AddSingleton<IImgurService, ImgurService>()
                .AddSingleton<IImgFlipService, ImgFlipService>()
				.AddSingleton<IJSONSerializerService, JSONSerializerService>()
				.AddSingleton<ITimerService, TimerService>()
                .AddSingleton(_config)
                .AddSingleton<Random>();

            var provider = services.BuildServiceProvider();
			
            provider.GetRequiredService<ILoggingService>();
            await provider.GetRequiredService<IStartupService>().StartAsync();
            provider.GetRequiredService<ICommandHandlingService>();

            await Task.Delay(-1);
        }
    }
}