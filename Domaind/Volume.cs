using System.Collections.Generic;

namespace Models
{
    public class Volume
    {
        public Volume(Novel novel)
        {
            Novel = novel;
            Chapters = new List<Chapter>();
        }

        public Novel Novel { get; set; }
        public string Title { get; set; }
        public string VolumeNumber { get; set; }
        public List<Chapter> Chapters { get; set; }
        public byte[] VolumeBytes { get; set; }
    }
}