using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models.Stages
{
    public class Stage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int serialNumber { get; set; }
        [Required]
        public StageModels model { get; set; }
        [Required]
        public int position { get; set; }
        [Required]
        public Device device { get; set; }
    }
}
