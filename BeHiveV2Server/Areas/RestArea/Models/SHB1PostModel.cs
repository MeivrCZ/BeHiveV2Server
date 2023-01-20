using System.ComponentModel.DataAnnotations;

namespace BeHiveV2Server.Areas.RestArea.Models
{
    public class SHB1PostModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Serial { get; set; }
        [Required]
        public long unixTimestamp { get; set; }

        public double outsideTemperature { get; set; }
        public double pressure { get; set; }
        public double humidity { get; set; }
        public double weight { get; set; }
        public double insideTemperature { get; set; }
        //public int noise { get; set; }
    }
}
