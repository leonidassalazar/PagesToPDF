using Model.System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.WuxiaWorldModel
{
    public class WuxiaWorldUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WuxiaWorldUserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public bool SaveNovel { get; set; }
        public bool SaveVolume { get; set; }
        public bool SaveChapter { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}
