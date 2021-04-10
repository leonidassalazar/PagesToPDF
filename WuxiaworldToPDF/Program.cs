using Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using WuxiaWorldToPDF.Business;

namespace WuxiaWorldToPDF
{
    public class Program
    {
        private static void Main()
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