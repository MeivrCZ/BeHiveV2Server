using BeHiveV2Server.Models.Hives.SHP1Device;

namespace BeHiveV2Server.Areas.AdminArea.Models.DeviceManageModels
{
    public class SHP1ManageModel
    {
        public SHP1Device device { get; set; }
        public List<int> versions { get; set; }
    }
}
