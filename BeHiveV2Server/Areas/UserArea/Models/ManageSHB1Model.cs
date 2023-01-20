using BeHiveV2Server.Models;

namespace BeHiveV2Server.Areas.UserArea.Models
{
    public class ManageSHB1Model
    {
        public int id { get; set; }
        public string serialNumber { get; set; }
        public Device device { get; set; }

        public Dictionary<long, double> outTemperature { get; set; }
        public Dictionary<long, double> pressure { get; set; }
        public Dictionary<long, double> humidity { get; set; }
        public Dictionary<long, double> insideTemperature { get; set; }
        public Dictionary<long, double> weight { get; set; }

        public long lastUpdate { get; set; }
    }
}
