using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class LoggingService: IDiscordService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public IConfiguration Configuration { get; set; }

        private string _logDirectory { get; }
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

        public LoggingService(IConfiguration _configuration, DiscordSocketClient discord, CommandService commands)
        {
            Configuration = _configuration;

            _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            _discord = discord;
            _commands = commands;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(_logFile))
                File.Create(_logFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            File.AppendAllText(_logFile, logText + "\n");

            return Console.Out.WriteLineAsync(logText);
        }
    }
}