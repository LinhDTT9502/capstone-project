using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : Controller
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public PaymentMethodController(IPaymentMethodService paymentMethodService, IMapper mapper, IUserService userService)
        {
            _paymentMethodService = paymentMethodService;
            _mapper = mapper;
            _userService = userService;
        }
        /*[HttpGet("{id}")]
       public async Task<ActionResult<PaymentMethod>> GetPaymentMethodByUser(int id)
       {
           var paymentMethod = await _paymentMethodService.GetPaymentMethodAsync(id);

           if (paymentMethod == null)
           {
               return NotFound("Payment method is not valid");
           }

           return paymentMethod;
       }*/
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var paymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();
            if(paymentMethods.Count() == 0)
            {
                return NotFound("Cannot find payment method!");
            }
            return Ok(paymentMethods);
        }
        [HttpPost]
        public async Task<IActionResult> PostPaymentMethod([FromBody] PaymentMethodCM paymentMethodCM)
        {
            var paymentMethod = new PaymentMethod()
            {
                PaymentMethodName = paymentMethodCM.Name
            };
            var createdPaymentMethod = await _paymentMethodService.AddPaymentMethodAsync(paymentMethod);
            if(createdPaymentMethod == null)
            {
                return BadRequest("Cannot insert!");
            }
            return Ok(createdPaymentMethod);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentMethod(int id, PaymentMethod paymentMethod)
        {
            if (id != paymentMethod.Id)
            {
                return BadRequest();
            }

            var result = await _paymentMethodService.UpdatePaymentMethodAsync(paymentMethod);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var result = await _paymentMethodService.DeletePaymentMethodAsync(id);
            if (!result)
            {
                return NotFound($"Payment method is not valid with ${id}");
            }

            return Ok("Deleted successfully!");
        }
    }
}
