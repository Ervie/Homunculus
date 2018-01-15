using Discord;
using Discord.Commands;
using MarekMotykaBot.DataTypes;
using MarekMotykaBot.Resources;
using MarekMotykaBot.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarekMotykaBot.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly JSONSerializerService _serializer;

        private readonly List<string> _swearWordList = new List<string>() { "penis", "kurwa", "dupa" };

        public AdminModule(JSONSerializerService serializer)
        {
            _serializer = serializer;
        }

        [Command("Penis"), Summary("This is a Christian server!"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task SwearWordCounterAsync()
        {
            StringBuilder sb = new StringBuilder();

            var counterList = _serializer.LoadFromFile<WordCounterEntry>("wordCounter.json");

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = StringConsts.SwearWordCounterHeader
            };

            foreach (string swearWord in _swearWordList)
            {
                var specificSwearWordEntries = counterList.Where(x => x.Word.Equals(swearWord)).OrderBy(x => x.CounterValue);

                foreach (var entry in specificSwearWordEntries.OrderByDescending(x => x.CounterValue))
                {
                    sb.AppendLine(string.Format(StringConsts.SwearWordCounterEntry, entry.DiscordNickname, entry.Word, entry.CounterValue));
                }

                builder.AddField(x =>
                {
                    x.Name = swearWord;
                    x.Value = sb.ToString();
                    x.IsInline = true;
                });

                sb.Clear();
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}