using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YamYamBusanApp.Helpers
{
    public static class ConfigLoader
    {
        public static string LoadApiKey(string path = "config.json")
        {
            if (!File.Exists(path)) return null;

            var json = File.ReadAllText(path);
            var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return obj.TryGetValue("ApiKey", out var key) ? key : null;
        }
    }
}
