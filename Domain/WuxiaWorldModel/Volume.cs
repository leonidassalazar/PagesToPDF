using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.WuxiaWorldModel
{
    public class Volume
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VolumeId { get; set; }
        [ForeignKey("NovelId")]
        public virtual Novel Novel { get; set; }
        public string Title { get; set; }
        public string VolumeNumber { get; set; }
        public virtual List<Chapter> Chapters { get; set; }
        [NotMapped]
        public byte[] VolumeBytes { get; set; }
    }
}
