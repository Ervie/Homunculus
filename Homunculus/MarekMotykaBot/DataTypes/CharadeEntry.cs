namespace MarekMotykaBot.DataTypes
{
    internal class CharadeEntry
    {
        public int[] AnimeId { get; set; }

        public int[] MangaId { get; set; }

        public string Title { get; set; }

        public string[] Translations { get; set; }

        public CharadeEntry(string title, string[] translations, int[] animeId, int[] mangaId)
        {
            Title = title;
            Translations = translations;
            AnimeId = animeId;
            MangaId = mangaId;
        }
    }
}