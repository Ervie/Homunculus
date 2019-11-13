using Discord;
using Discord.Commands;
using MarekMotykaBot.Modules.Interface;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using MarekMotykaBot.Services.Core;
using MarekMotykaBot.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
	public class HelpModule : ModuleBase<SocketCommandContext>, IDiscordModule
	{
		private readonly CommandService _service;
		private readonly IConfiguration _configuration;
		private readonly IJSONSerializerService _serializer;

		public ILoggingService LoggingService { get; }

		public HelpModule(
			CommandService service,
			IConfiguration configuration,
			IJSONSerializerService serializer,
			ILoggingService loggingService
			)
		{
			_service = service;
			_configuration = configuration;
			_serializer = serializer;
			LoggingService = loggingService;
		}

		public string ServiceName { get => "HelpModule"; }
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

			LoggingService.CustomCommandLog(Context.Message, ServiceName);
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

			LoggingService.CustomCommandLog(Context.Message, ServiceName);
		}

		[Command("StreamMonday"), Alias("sm", "Streamdziałek"), Summary("Prints schedule for next StreamMonday")]
		public async Task StreamMondayAsync()
		{
			List<string> schedule = _serializer.LoadFromFile<string>("streamMonday.json");

			var builder = new EmbedBuilder();

			DateTime today = DateTime.Today;
			int daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7;
			DateTime nextWednesday = today.AddDays(daysUntilWednesday);

			if (schedule != null && schedule.Count > 0)
			{
				builder.AddField(x =>
				{
					x.Name = "Rozkładówka (backlog) na " + nextWednesday.ToString("dd.MM");
					x.Value = string.Join(Environment.NewLine, schedule.ToArray());
					x.IsInline = false;
				});
			}

			var role = Context.Guild.Roles.First(x => x.Name.StartsWith(_configuration["configValues:streamAlias"]));

			if (role != null)
			{
				await ReplyAsync(role.Mention);
			}
			await ReplyAsync("", false, builder.Build());

			LoggingService.CustomCommandLog(Context.Message, ServiceName);
		}
	}
}