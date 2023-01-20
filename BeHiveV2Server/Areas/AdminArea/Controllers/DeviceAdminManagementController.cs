using BeHiveV2Server.Areas.AdminArea.Models;
using BeHiveV2Server.Models;
using BeHiveV2Server.Services.Database;
using BeHiveV2Server.Services.Database.Models;
using BeHiveV2Server.Services.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using BeHiveV2Server.Models.Hives.SHP1Device;
using BeHiveV2Server.Areas.AdminArea.Models.DeviceManageModels;
using BeHiveV2Server.Models.Hives.SHB1Device;

namespace BeHiveV2Server.Areas.AdminArea.Controllers
{
    public class DeviceAdminManagementController : Controller
    {
        private readonly ILogger<DeviceAdminManagementController> _logger;
        private readonly ServerDBContext _dbContext;

        public DeviceAdminManagementController(ILogger<DeviceAdminManagementController> logger, ServerDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Devices(string searchString, string currentSearch, string searchOption, int? pageNumber)
        {
            ViewData["CurrentSearchOption"] = searchOption ?? "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentSearch;
            }

            ViewData["CurrentSearchString"] = searchString;

            var DeviceList = from s in _dbContext.Devices select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "id":
                        DeviceList = DeviceList.Where(x => x.id.ToString().Contains(searchString));
                        break;
                    case "name":
                        DeviceList = DeviceList.Where(x => x.name.Contains(searchString));
                        break;
                    case "serialNumber":
                        DeviceList = DeviceList.Where(x => x.serialNumber.Contains(searchString));
                        break;
                    case "model":
                        DeviceList = DeviceList.Where(x => x.model == EnumReaders.ReverseDeviceModelEnum(searchString.ToUpper()));
                        break;
                }
            }

            DeviceList = DeviceList.OrderBy(u => u.id);

            int pageSize = 10;
            return View(await PaginatedList<Device>.CreateAsync(DeviceList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult CreateVirtualDevice()
        {
            
            return View(new CreateDeviceModel());
        }

        [HttpPost]
        public IActionResult CreateVirtualDevice(CreateDeviceModel deviceModel)
        {
            if (ModelState.IsValid)
            {
                Random random = new Random();
                string serialNumber = "";

                do
                {
                    serialNumber = random.Next(100000, 999999).ToString();
                }
                while (_dbContext.Devices.Where(d => d.serialNumber.Equals(serialNumber)).Count() > 0);

                DeviceModels model = EnumReaders.ReverseDeviceModelEnum(deviceModel.model);

                if(model == DeviceModels.none)
                {
                    return View(deviceModel);
                }

                Device device = new Device() { name = deviceModel.name, model = model, serialNumber = serialNumber };

                _dbContext.Devices.Add(device);
                _dbContext.SaveChanges();

                switch (model)
                {
                    case DeviceModels.SmartHivePedestal1:

                        SHP1Device shp1 = new SHP1Device() { device = device, serialNumber = serialNumber , hardwareUpdate = DateTime.Now, version = 1 };
                        _dbContext.SHP1Devices.Add(shp1);
                        break;
                    case DeviceModels.SmartHiveBox1:

                        SHB1Device shb1 = new SHB1Device() { id = device.id, device = device, serialNumber = serialNumber };
                        _dbContext.SHB1Devices.Add(shb1);
                        break;
                    case DeviceModels.Meteostation1:
                        break;
                }

                _dbContext.SaveChanges();

                return Redirect("/admin/devices");
            }

            return View(deviceModel);
        }

        public IActionResult manage(int deviceID)
        {
            Device device = _dbContext.Devices.FirstOrDefault(d => d.id == deviceID, null);

            if(device != null)
            {
                return View(device);
            }

            return NotFound();
        }
    }
}
