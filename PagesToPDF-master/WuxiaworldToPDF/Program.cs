using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace WuxiaWorldToPDF
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configuration = Configuration.GetInstance();
            var novels = JsonConvert.DeserializeObject<JArray>(configuration.GetNode("novels"));
            var novelsCode = (novels ?? new JArray())
                .ToDictionary(novel => novel["Code"]?.ToString() ?? Guid.NewGuid().ToString(),
                                                    novel => novel["Title"]?.ToString());

            var wuxiaWorldNovelHtmlToPdf = new WuxiaWorldNovelHtmlToPdf(novelsCode);
            wuxiaWorldNovelHtmlToPdf.CreateAndSaveBookAllNovels();
        }
    }
}
