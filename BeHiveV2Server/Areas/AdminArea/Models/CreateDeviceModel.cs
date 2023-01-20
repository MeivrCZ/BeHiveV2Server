using System.ComponentModel.DataAnnotations;

namespace BeHiveV2Server.Areas.AdminArea.Models
{
    public class CreateDeviceModel
    {
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        public string model { get; set; }
    }
}
