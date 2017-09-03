using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MarekMotykaBot.Resources;
using System;
using System.Linq;

namespace MarekMotykaBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;


		public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
			MessageScanner.Client = client;
            _service = new CommandService();
        }

        public async Task InstallCommands()
        {
			_client.MessageReceived += MessageScanner.ScanMessage;
            _client.MessageReceived += HandleCommandAsync;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
        }

		private async Task HandleCommandAsync(SocketMessage s)
		{
			var message = s as SocketUserMessage;

			if (message == null)
				return;

			var context = new SocketCommandContext(_client, message);

			int argPos = 0;

			if (message.HasStringPrefix("!", ref argPos) ||
				message.HasMentionPrefix(_client.CurrentUser, ref argPos))
			{
                if (!DeclineCommand(context, message.Content).Result)
                {
                    var result = await _service.ExecuteAsync(context, argPos);

                    if (result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
        }

        private async Task<bool> DeclineCommand(SocketCommandContext context, string messageContent)
        {
            string commandName = messageContent.Split(' ').FirstOrDefault();
            bool commandDeclined = false;

            if (!string.IsNullOrWhiteSpace(commandName))
            {
                string meanResponse = string.Format(StringConsts.DeclineCommand, commandName);

                Random rng = new Random();

                int randomInt = rng.Next(1, 10);

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