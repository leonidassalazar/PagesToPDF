using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.WuxiaWorldModel
{
    public class Chapter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ChapterId { get; set; }
        [ForeignKey("VolumeId")]
        public virtual Volume Volume { get; set; }
        public string Title { get; set; }
        public string ChapterNumber { get; set; }
        public string ChapEndpoint { get; set; }
        [NotMapped]
        public byte[] ChapterBytes { get; set; }
    }
}
