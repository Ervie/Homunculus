using Discord.WebSocket;

namespace MarekMotykaBot.ExtensionsMethods
{
    public static class SocketUserExtensions
    {
        public static string DiscordId(this SocketUser user)
        {
            return string.Format("{0}#{1}", user.Username, user.Discriminator);
        }
    }
}