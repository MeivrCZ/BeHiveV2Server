using BeHiveV2Server.Services.Database.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeHiveV2Server.Models
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string serialNumber { get; set; }
        [Required]
        public DeviceModels model { get; set; }
        public bool isVirtual { get; set; }
        public UserIdentity ?owner { get; set; }
        public ICollection<UserDevice> ?users { get; set; }
        //List<UserIdentity> superUsers { get; set; }
        public string ?addPassword { get; set; }
        //List<Area> areas;
        public string ?prewiewPitcture { get; set; }
    }
}
