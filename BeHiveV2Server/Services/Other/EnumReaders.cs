using BeHiveV2Server.Models;

namespace BeHiveV2Server.Services.Other
{
    public class EnumReaders
    {
        public static string ReadDeviceModelEnum(DeviceModels model)
        {
            switch (model)
            {
                case DeviceModels.SmartHiveBox1:
                    return "SHB1";
                    break;
                case DeviceModels.SmartHivePedestal1:
                    return "SHP1";
                    break;
                case DeviceModels.Meteostation1:
                    return "MET1";
                    break;
            }
            return "unknown";
        }

        public static DeviceModels ReverseDeviceModelEnum(string model)
        {
            switch (model)
            {
                case "SHB1":
                    return DeviceModels.SmartHiveBox1;
                    break;
                case "SHP1":
                    return DeviceModels.SmartHivePedestal1;
                    break;
                case "MET1":
                    return DeviceModels.Meteostation1;
                    break;
            }
            return DeviceModels.none;
        }
    }
}
