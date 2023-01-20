using System.ComponentModel.DataAnnotations;

namespace BeHiveV2Server.Areas.UserArea.Models
{
    public class LogInModel
    {
        [Required(ErrorMessage = "nebylo zadáno")]
        public string UserIdentity { get; set; } //email or username
        [Required(ErrorMessage = "nebylo zadáno")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
