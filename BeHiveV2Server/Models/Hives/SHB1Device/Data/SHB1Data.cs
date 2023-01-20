using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models.Hives.SHB1Device.Data
{
    public class SHB1Data
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public SHB1Device device { get; set; }
        [Required]
        public long unixTimestamp { get; set; }
        public double ?outTemperature { get; set; }
        public double ?pressure { get; set; }
        public double ?humidity { get; set; }
        public double ?insideTemperature { get; set; }
        public double ?weight { get; set; }
        //public int ?noise { get; set; }
    }
}
