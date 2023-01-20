using BeHiveV2Server.Models.Stages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models.Hives.SHP1Device
{
    public class SHP1Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string serialNumber { get; set; }
        [Required]
        public Device device { get; set; }
        public List<Stage> ?stages { get; set; }
        [Required]
        public DateTime hardwareUpdate { get; set; }
        [Required]
        public int version { get; set; }

        //data lists

        //settings - hive

        //settings - pedestal
    }
}
