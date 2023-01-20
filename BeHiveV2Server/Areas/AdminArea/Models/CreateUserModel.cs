using System.ComponentModel.DataAnnotations;

namespace BeHiveV2Server.Areas.AdminArea.Models
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "Required")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Not email address")]
        public string email { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Not same as entered password")]
        public string passwordConformation { get; set; }
    }
}
