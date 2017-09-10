using Discord;
using Discord.Commands;
using MarekMotykaBot.Resources;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class HelpModule: ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfiguration _configuration;
        
        public HelpModule(CommandService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

		[Command("Version"), Alias("v"), Summary("Prints version information")]
		public async Task AboutAsync()
		{
			StringBuilder sb = new StringBuilder();

			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = "About informations"
			};
			
			//version
			builder.AddField(x =>
			{
				x.Name = "Version";
				x.Value = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
				x.IsInline = true;
			});

			await ReplyAsync("", false, builder.Build());
		}

		[Command("Help"), Alias("h"), Summary("List all the commands")]
        public async Task HelpAsync()
        {
            StringBuilder sb = new StringBuilder();

            string prefix = _configuration["prefix"];

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = StringConsts.ListCommands
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {

                        foreach (string alias in cmd.Aliases)
                        {
                            sb.Append(prefix);
                            sb.Append(alias);

                            if (alias != cmd.Aliases.Last())
                                sb.Append(", ");
                        }

                        if (cmd.Parameters.Count > 0)
                        {
                            sb.Append(" (");

                            foreach (Discord.Commands.ParameterInfo parameter in cmd.Parameters)
                            {
                                sb.Append(parameter.Name);
                                if (parameter != cmd.Parameters.Last())
                                    sb.Append(", ");
                            }

                            sb.Append(") ");
                        }
                            
                        if (!string.IsNullOrWhiteSpace(cmd.Summary))
                        {
                            sb.Append(" - ");
                            sb.Append(cmd.Summary);
                        }

                        description += $"{sb.ToString()}\n";
                        sb.Clear();
                    }
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
