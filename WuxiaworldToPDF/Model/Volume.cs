using System.Collections.Generic;

namespace WuxiaWorldToPDF.Model
{
    public class Volume
    {
        public Novel Novel { get; set; }
        public string Title { get; set; }
        public string VolumeNumber { get; set; }
        public List<Chapter> Chapters { get; set; }
        public byte[] VolumeBytes { get; set; }

        public Volume(Novel novel)
        {
            Novel = novel;
            Chapters = new List<Chapter>();
        }
    }
}
