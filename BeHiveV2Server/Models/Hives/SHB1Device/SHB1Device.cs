using BeHiveV2Server.Models.Hives.SHB1Device.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models.Hives.SHB1Device
{
    public class SHB1Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        [Required]
        public string serialNumber { get; set; }
        [Required]
        public Device device { get; set; }

        //data
        public ICollection<SHB1Data> data { get; set; }


    }
}
