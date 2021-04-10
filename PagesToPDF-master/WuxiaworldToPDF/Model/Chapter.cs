namespace WuxiaWorldToPDF.Model
{
    public class Chapter
    {
        public Volume Volume { get; set; }
        public string Title { get; set; }
        public string ChapterNumber { get; set; }
        public string ChapEndpoint { get; set; }
        public byte[] ChapterBytes { get; set; }

        public Chapter(Volume volume)
        {
            Volume = volume;
        }
    }
}
