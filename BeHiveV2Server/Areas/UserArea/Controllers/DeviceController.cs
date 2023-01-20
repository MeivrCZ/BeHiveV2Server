using BeHiveV2Server.Areas.AdminArea.Controllers;
using BeHiveV2Server.Models;
using BeHiveV2Server.Services.Database;
using BeHiveV2Server.Services.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeHiveV2Server.Areas.UserArea.Models;
using BeHiveV2Server.Models.Hives.SHB1Device;
using BeHiveV2Server.Models.Hives.SHB1Device.Data;
using Microsoft.Extensions.Logging;

namespace BeHiveV2Server.Areas.UserArea.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly ServerDBContext _dbContext;

        public DeviceController(ILogger<DeviceController> logger, ServerDBContext dbContext)
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

            var DeviceList = from s in _dbContext.Devices where s.users.Where(u => u.user.UserName == User.Identity.Name).Count() == 1 || s.owner.UserName == User.Identity.Name select s;
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
        public IActionResult AddDevice()
        {
            return View(new AddDeviceModel());
        }
        [HttpPost]
        public IActionResult AddDevice(AddDeviceModel model)
        {
            if (ModelState.IsValid)
            {
                if (_dbContext.Devices.Where(d => d.serialNumber == model.serial).Count() > 0)
                {
                    Device device = _dbContext.Devices.Where(d => d.serialNumber == model.serial).FirstOrDefault();

                    var user = _dbContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                    _dbContext.userDevices.Add(new Services.Database.Models.UserDevice { device = device, user = user, DeviceID = device.id, UserID = user.Id });
                    _dbContext.SaveChanges();

                    return Redirect("/devices");
                }
            }
            return View(model);
        }

        public IActionResult manage(int id)
        {
            Device searchedDevice = _dbContext.Devices.Where(d => d.id == id).FirstOrDefault();

            switch (searchedDevice.model)
            {
                case DeviceModels.SmartHiveBox1:
                    SHB1Device device = _dbContext.SHB1Devices.Include(o => o.data).Where(d => d.id == searchedDevice.id).FirstOrDefault();
                    ManageSHB1Model model = new ManageSHB1Model() { id = device.id, serialNumber = device.serialNumber, device = device.device, weight = new Dictionary<long, double>(), humidity = new Dictionary<long, double>(), insideTemperature = new Dictionary<long, double>(), pressure = new Dictionary<long, double>(), outTemperature = new Dictionary<long, double>()};
                    //long currentTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                    //_logger.LogError(device.data.Where(d => (currentTime - 1800) < d.unixTimestamp && currentTime > d.unixTimestamp).Count().ToString());

                    if (device.data.Any())
                    {
                        model.lastUpdate = device.data.Last().unixTimestamp;
                    }
                    else
                    {
                        model.lastUpdate = -1;
                    }

                    for (int i = 1; i <= 48 && i < device.data.Count(); i++)
                    {
                        SHB1Data data = device.data.Where(d => model.lastUpdate - 1800 * i < d.unixTimestamp && model.lastUpdate - 1800 * (i - 1) >= d.unixTimestamp).FirstOrDefault();
                        if(data != null)
                        {
                            model.outTemperature.Add(data.unixTimestamp, data.outTemperature ?? 100);
                            model.insideTemperature.Add(data.unixTimestamp, data.insideTemperature ?? 100);
                        }
                    }
                    for (int i = 1; i <= 24 && i < device.data.Count(); i++)
                    {
                        SHB1Data data = device.data.Where(d => model.lastUpdate - 3600 * i < d.unixTimestamp && model.lastUpdate - 3600 * (i - 1) >= d.unixTimestamp).FirstOrDefault();
                        if (data != null)
                        {
                            model.humidity.Add(data.unixTimestamp, data.humidity ?? 100);
                        }
                    }
                    for (int i = 1; i <= 30 && i < device.data.Count(); i++)
                    {
                        SHB1Data data = device.data.Where(d => model.lastUpdate - (86400 * i) < d.unixTimestamp && model.lastUpdate - (86400 * (i - 1)) >= d.unixTimestamp).FirstOrDefault();
                        if (data != null)
                        {
                            model.weight.Add(data.unixTimestamp, data.weight ?? 100);
                            model.pressure.Add(data.unixTimestamp, data.pressure ?? 100);
                        }
                    }

                    
                    

                    return View("SHB1Manage", model);
                    break;
                case DeviceModels.SmartHivePedestal1:
                    break;
            }

            return BadRequest();
        }
    }
}