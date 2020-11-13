using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WuxiaWorldToPDF
{
    public class Configuration
    {
        private static JObject _configuration;
        private Configuration()
        {
            var configurationContent = File.ReadAllText("appsettings.json", Encoding.UTF8);
            _configuration = JsonConvert.DeserializeObject<JObject>(configurationContent);
        }

        private static Configuration _instance;

        public static Configuration GetInstance()
        {
            return _instance ??= new Configuration();
        }

        public string GetNode(string node)
        {
            return _configuration[node]?.ToString();
        }
    }
}
