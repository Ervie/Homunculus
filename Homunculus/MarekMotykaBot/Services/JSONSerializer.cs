using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using MarekMotykaBot.DataTypes;

namespace MarekMotykaBot.Services
{
    public class JSONSerializer
    {
        private readonly string resources = AppContext.BaseDirectory + "/Resources/TextFiles/";
		
        public JSONSerializer()
        {
        }

		public List<OneLinerJoke> LoadOneLiners()
		{
            var text = File.ReadAllText(resources + "oneLiners.json", Encoding.UTF8);
            return JsonConvert.DeserializeObject<List<OneLinerJoke>>(text);
		}
    }
}
