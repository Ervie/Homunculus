using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using MarekMotykaBot.DataTypes;

namespace MarekMotykaBot.Services
{
    class JSONSerializer
    {
        private readonly string resources = AppContext.BaseDirectory + "/Resources/TextFiles/";

        public JSONSerializer()
        {
            
            var result = JsonConvert.DeserializeObject<List<OneLinerJoke>>(File.ReadAllText("oneLiner.json"));
            
        }
    }
}
