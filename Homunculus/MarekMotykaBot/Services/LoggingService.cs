using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class LoggingService : IDiscordService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public IConfiguration Configuration { get; set; }

        private Logger _logger;

        private string _logDirectory;
        
        public LoggingService(IConfiguration _configuration, DiscordSocketClient discord, CommandService commands)
        {
            Configuration = _configuration;

            _discord = discord;
            _commands = commands;

            Initialize();
        }

        private void Initialize()
        {
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            _logger = LogManager.GetCurrentClassLogger();

            _logger.Info("Logger init");    

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            if (msg.Exception != null)
            {
                _logger.Error(msg.Exception.ToString());
            }
            else
            {
                _logger.Info(msg.Message);
            }

            return Task.CompletedTask;
        }
    }
}