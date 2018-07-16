using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

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
				.AddSingleton<CommandHandlingService>()
				.AddSingleton<LoggingService>()
				.AddSingleton<StartupService>()
				.AddSingleton<MessageScannerService>()
				.AddSingleton<YTService>()
				.AddSingleton<ImgurService>()
                .AddSingleton<ImgFlipService>()
				.AddSingleton<JSONSerializerService>()
				.AddSingleton<DropboxService>()
				.AddSingleton<TimerService>()
                .AddSingleton(_config)
                .AddSingleton<Random>();

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandlingService>();

            await Task.Delay(-1);
        }
    }
}