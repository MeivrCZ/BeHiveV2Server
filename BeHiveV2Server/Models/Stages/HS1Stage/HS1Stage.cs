using BeHiveV2Server.Models.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models.Stages.HS1Stage
{
    public class HS1Stage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int serialNumber { get; set; }
        [Required]
        public Stage stage { get; set; }
        public List<Module> ?modules { get; set; }
        //public List<HS1Data> data { get; set; }
    }
}
