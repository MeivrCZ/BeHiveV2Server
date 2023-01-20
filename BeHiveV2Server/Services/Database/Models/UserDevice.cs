using BeHiveV2Server.Models;

namespace BeHiveV2Server.Services.Database.Models
{
    public class UserDevice
    {
        public int DeviceID { get; set; }
        public Device device { get; set; }
        public string UserID { get; set; }
        public UserIdentity user { get; set; }
    }
}
