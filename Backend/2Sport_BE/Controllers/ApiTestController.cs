using _2Sport_BE.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPayOsService _payOsService;
        private readonly IVnPayService _vnPayService;
        public ApiTestController(IPaymentService paymentService, IPayOsService payOsService, IVnPayService vnPayService)
        {
            _paymentService = paymentService;
            _payOsService = payOsService;
            _vnPayService = vnPayService;
        }
        [HttpGet]
        [Route("GetPaymentInformationPayOs")]
        public async Task<IActionResult> GetPaymentInformationPayOs(string orderCode)
        {
            var response = await _payOsService.GetPaymentDetails(orderCode);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("GetPaymentInformationVnPay")]

        public IActionResult GetPaymentInformation(string orderId, string date)
        {
            DateTime dateUtc = DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc);
            var response = _vnPayService.QueryTransaction(orderId, dateUtc, HttpContext);
           
            return Ok(response);
        }
    }
}
