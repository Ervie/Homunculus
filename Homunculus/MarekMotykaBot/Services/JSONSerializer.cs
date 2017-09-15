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
			//List<OneLinerJoke> test = new List<OneLinerJoke>();
			//test.Add(new OneLinerJoke("A", "b"));
			//File.WriteAllText(resources + "oneLiners.json", JsonConvert.SerializeObject(test));
			return JsonConvert.DeserializeObject<List<OneLinerJoke>>(File.ReadAllText(resources + "oneLiners.json"));
		}
    }
}
