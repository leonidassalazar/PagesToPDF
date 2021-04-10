namespace Models
{
    public class Chapter
    {
        public Chapter(Volume volume)
        {
            Volume = volume;
        }

        public Volume Volume { get; set; }
        public string Title { get; set; }
        public string ChapterNumber { get; set; }
        public string ChapEndpoint { get; set; }
        public byte[] ChapterBytes { get; set; }
    }
}