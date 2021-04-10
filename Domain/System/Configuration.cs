using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.System
{
    public class Configuration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ConfigurationId { get; set; }
        public string LocalSave { get; set; }
    }
}
