using System.Threading;

namespace MarekMotykaBot.DataTypes
{
    internal class CharadeEntry
    {
        public int Id { get; set; }

        public int[] AnimeId { get; set; }

        public int[] MangaId { get; set; }

        public string Title { get; set; }

        public string PicUrl { get; set; }

        public string[] Translations { get; set; }

		public string[] Users { get; set; }

		public CharadeEntry(int id, string title, string[] translations, int[] animeId, int[] mangaId, string picUrl)
        {
            Title = title;
            Translations = translations;
            AnimeId = animeId;
            MangaId = mangaId;
            PicUrl = picUrl;
            Id = id;
        }
    }
}