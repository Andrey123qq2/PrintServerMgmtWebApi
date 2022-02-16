using API.ApiModels.Printers;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "PrintServer_Admins")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrintersController : Controller
    {
        private readonly PrintersService _service;
        public PrintersController(PrintersService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieve the printer info by name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] GetPrinterRequest request)
        {
            var response = _service.Get(request);
            return Ok(response);
        }


        /// <summary>
        /// Delete the printer by name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(DeletePrinterRequest request)
        {
            var response = _service.Delete(request);
            return Ok(response);
        }

        /// <summary>
        /// Create printer 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(CreatePrinterRequest request)
        {
            var response = _service.Create(request);
            return Ok(response);
        }

        /// <summary>
        /// Update printer properties
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="patchEntity"></param>
        /// <returns></returns>
        [HttpPatch]
        public IActionResult Patch(string printerName, [FromBody] JsonPatchDocument<UpdatePrinterRequest> patchEntity)
        {
            if (patchEntity != null)
            {
                var updateRequest = new UpdatePrinterRequest();
                patchEntity.ApplyTo(updateRequest);
                var response = _service.Update(printerName, updateRequest);
                return Ok(response);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Get printer jobs queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetQueue([FromQuery] GetPrinterQueueRequest request)
        {
            var response = _service.GetQueue(request);
            return Ok(response);
        }

        /// <summary>
        /// Get printer jobs queue count
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetQueueCount([FromQuery] GetPrinterQueueCountRequest request)
        {
            var response = _service.GetQueueCount(request);
            return Ok(response);
        }

        /// <summary>
        /// Get list of drivers
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDrivers()
        {
            var response = _service.GetDrivers();
            return Ok(response);
        }

        /// <summary>
        /// Clear printer jobs queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ClearQueue(ClearPrinterQueueRequest request)
        {
            var response = _service.ClearQueue(request);
            return Ok(response);
        }

        /// <summary>
        /// Remove enitity from printer ACL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RemoveFromACL(RemoveFromPrinterACLRequest request)
        {
            var response = _service.RemoveFromACL(request);
            return Ok(response);
        }

        /// <summary>
        /// Add print permission to ACL for sid entity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddPrintPermission(AddPrintPermissionRequest request)
        {
            var response = _service.AddPrintPermission(request);
            return Ok(response);
        }
    }
}