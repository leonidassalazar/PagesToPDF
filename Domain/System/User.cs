using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.System
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [ForeignKey("ConfigurationId")]
        public Configuration Configuration { get; set; }
    }
}
