using BeHiveV2Server.Models.Stages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models.Modules
{
    public class Module
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int serialNumber { get; set; }
        [Required]
        public ModuleModel model { get; set; }
        [Required]
        public int position { get; set; }
        [Required]
        public Device device { get; set; }
        [Required]
        public Stage stage { get; set; }
    }
}
