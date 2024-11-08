using _2Sport_BE.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryMethodController : ControllerBase
    {
        private readonly IDeliveryMethodService _service;
        public DeliveryMethodController(IDeliveryMethodService service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var response = _service.GetAllMethods();

            return Ok(response);
        }
    }
}
