using BeHiveV2Server.Models;
using Microsoft.AspNetCore.Identity;

namespace BeHiveV2Server.Services.Database.Models
{
    public class UserIdentity : IdentityUser
    {
        [PersonalData]
        public ICollection<Device> ?OwnedDevices { get; set; }
        [PersonalData]
        public ICollection<UserDevice> ?MemberDevices { get; set; }
    }
}
