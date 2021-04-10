using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.WuxiaWorldModel
{
    public class Novel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NovelId { get; set; }
        public string Title { get; set; }
        public string NovelCode { get; set; }
        public virtual List<Volume> Volumes { get; set; }
        [NotMapped]
        public byte[] NovelBytes { get; set; }
    }
}
