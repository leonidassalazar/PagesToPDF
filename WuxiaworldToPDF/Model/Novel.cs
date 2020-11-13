using System.Collections.Generic;

namespace WuxiaWorldToPDF.Model
{
    public class Novel
    {
        public string Title { get; set; }
        public string NovelCode { get; set; }
        public List<Volume> Volumes { get; set; }
        public byte[] NovelBytes { get; set; }

        public Novel()
        {
            Volumes = new List<Volume>();
        }
    }
}
