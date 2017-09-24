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

        public SortedList<string, string> LoadEightBallCache()
        {
            var text = File.ReadAllText(resources + "cache8ball.json", Encoding.UTF8);
            var deserializedObject = JsonConvert.DeserializeObject<SortedList<string, string>>(text);
            return deserializedObject == null ? new SortedList<string, string>() : deserializedObject;
        }

        public void SaveEightBallCache(SortedList<string, string> cache)
        {
            var json = JsonConvert.SerializeObject(cache);
            File.WriteAllText(resources + "cache8ball.json", json, Encoding.UTF8);
        }
    }
}
