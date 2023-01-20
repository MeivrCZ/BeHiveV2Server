using BeHiveV2Server.Models;
using BeHiveV2Server.Models.Hives.SHB1Device;
using BeHiveV2Server.Models.Hives.SHB1Device.Data;
using BeHiveV2Server.Services.Database;
using BeHiveV2Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace BeHiveV2Server.Services.Creators
{
    public class TestingDeviceCreator
    {
        private readonly ILogger<TestingDeviceCreator> _logger;
        private readonly ServerDBContext _dbContext;

        public TestingDeviceCreator(ILogger<TestingDeviceCreator> logger, ServerDBContext dBContext)
        {
            _logger = logger;
            _dbContext = dBContext;
        }

        public async Task CreateSHB1WithData()
        {
            Device device = new Device()//device
            {
                isVirtual = true,
                model = DeviceModels.SmartHiveBox1,
                name = "testDevice",
                serialNumber = "11111"
            };

            _dbContext.Devices.Add(device);//model
            _dbContext.SaveChanges();

            SHB1Device shb1Device = new SHB1Device()
            {
                device = device,
                id = device.id,
                serialNumber = device.serialNumber
            };

            _dbContext.SHB1Devices.Add(shb1Device);
            _dbContext.SaveChanges();

            //data
            Random random = new Random();
            SHB1Data data;
            int i = 43200;
            do
            {
                data = new SHB1Data() { device = shb1Device, unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - (i*1800), humidity = random.Next(1, 99), insideTemperature = random.Next(-10, 50), pressure = random.Next(4800, 5200), outTemperature = random.Next(-10, 40), weight = random.Next(1, 12)/*, noise = 0*/ };
                _dbContext.SHB1Datas.Add(data);
                i--;
            }
            while (i > 0);

            _dbContext.SaveChanges();
        }
    }
}
