using Microsoft.AspNetCore.Mvc;
using BeHiveV2Server.Areas.RestArea.Models;
using BeHiveV2Server.Services.Database;
using BeHiveV2Server.Services.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BeHiveV2Server.Models.Hives.SHB1Device;
using BeHiveV2Server.Models.Hives.SHB1Device.Data;

namespace BeHiveV2Server.Areas.RestArea.Controllers
{
    public class RestAPIDeviceController : ControllerBase
    {
        private readonly ILogger<RestAPIDeviceController> _logger;
        private readonly ServerDBContext _dBContext;
        public RestAPIDeviceController(ILogger<RestAPIDeviceController> logger, ServerDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SHB1PostDataJson([FromBody] SHB1PostModel model)
        {
            _logger.LogError("data post");
            if (ModelState.IsValid)
            {
                _logger.LogError("model valid");
                if (_dBContext.SHB1Devices.Where(d => d.id == model.Id && d.serialNumber == model.Serial).Any())
                {
                    _logger.LogError("device found");
                    SHB1Device device = _dBContext.SHB1Devices.Where(d => d.id == model.Id && d.serialNumber == model.Serial).FirstOrDefault();
                    SHB1Data data = new SHB1Data()
                    {
                        device = device,
                        humidity = model.humidity,
                        insideTemperature = model.insideTemperature,
                        outTemperature = model.outsideTemperature,
                        pressure = model.pressure,
                        unixTimestamp = model.unixTimestamp,
                        weight = model.weight
                    };
                    _dBContext.SHB1Datas.Add(data);
                    _dBContext.SaveChanges();
                    _logger.LogError("data saved");
                    return Ok("data saved to database");
                }
                _logger.LogError("device not found id:" + model.Id + " serial:" + model.Serial);
                return NotFound("device was not found");
            }
            _logger.LogError("model not valid");
            return BadRequest("model is not valid");
        }

        [AllowAnonymous]
        public ActionResult testAction()
        {
            return Ok();
        }
    }
}
