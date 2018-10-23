using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.Resources;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class LoggingService : IDiscordService, ILoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public IConfiguration Configuration { get; set; }

		public Logger Logger { get; set; }

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

            Logger = LogManager.GetCurrentClassLogger();

            Logger.Info("Logger init");    

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            if (msg.Exception != null)
            {
                Logger.Error(msg.Exception.ToString());
            }
            else
            {
                Logger.Info(msg.Message);
            }

            return Task.CompletedTask;
        }


		public void CustomDeleteLog(IMessage message)
		{
			if (!(message is SocketUserMessage))
				return;

			string log = string.Format(StringConsts.CustomDeleteLog, message.Author, message.Channel, message.Content);

			Logger.Log(LogLevel.Trace, log);
		}

		public void CustomEditLog(IMessage message, IMessage oldMessage)
		{
			if (!(message is SocketUserMessage) || message.Content == oldMessage.Content)
				return;

			string log = string.Format(StringConsts.CustomEditLog, message.Author, message.Channel, oldMessage.Content, message.Content);

			Logger.Log(LogLevel.Trace, log);
		}

		public void CustomCommandLog(IMessage message, string moduleName)
		{
			if (!(message is SocketUserMessage))
				return;

			string commandName = message.Content.Split(' ').First();

			string log = string.Format(StringConsts.CustomCommandLog, moduleName, commandName, message.Author, message.Channel);

			Logger.Log(LogLevel.Trace, log);
		}

		public void CustomCommandLog(IMessage message, string moduleName, string parameters)
		{
			if (!(message is SocketUserMessage))
				return;

			string commandName = message.Content.Split(' ').First();

			string log = string.Format(StringConsts.CustomCommandLog, moduleName, commandName, message.Author, message.Channel)
				+ ' ' + string.Format(StringConsts.Parameters, parameters);

			Logger.Log(LogLevel.Trace, log);
		}

		public void CustomReactionLog(IMessage message, string reactionName)
		{
			if (!(message is SocketUserMessage))
				return;

			string log = string.Format(StringConsts.CustomReactionLog, reactionName, message.Author, message.Content, message.Channel);

			Logger.Log(LogLevel.Trace, log);
		}

	}
}