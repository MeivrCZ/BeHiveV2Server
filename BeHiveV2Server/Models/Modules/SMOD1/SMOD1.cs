using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models.Modules.SMOD1
{
    public class SMOD1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int serialNumber { get; set; }
        [Required]
        public Module module { get; set; }
        //data
    }
}
