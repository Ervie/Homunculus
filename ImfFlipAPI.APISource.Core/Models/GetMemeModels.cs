using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImgFlipAPI.APISource.Core.Models
{
    public class Meme
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
		public TextBox[] boxes { get; set; }
	}

    public class MemeList
    {
        public List<Meme> memes { get; set; }
    }

    public class GetMemeRoot
    {
        public bool success { get; set; }
        public MemeList data { get; set; }
    }

	public class TextBox
	{
		public string text { get; set; }
		public int x { get; set; }
		public int y { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public string color { get; set; }
		public string outline_color { get; set; }
	}
}
