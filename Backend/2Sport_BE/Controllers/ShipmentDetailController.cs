using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace _2Shipment_BE.Controllers
{
    public class ShipmentDetailController : Controller
    {
        private readonly IShipmentDetailService _shipmentDetailService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public ShipmentDetailController(IShipmentDetailService shipmentDetailService, IMapper mapper, IUserService userService)
        {
            _shipmentDetailService = shipmentDetailService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        [Route("list-shipment-details")]
        public async Task<IActionResult> GetShipmentDetails()
        {
            var userId = GetCurrentUserIdFromToken();
            if(userId is 0 || userId.ToString().IsNullOrEmpty())
            {
                return Unauthorized("Invalid user");
            }
            try
            {
                var query = await _shipmentDetailService.GetAllShipmentDetails(userId);
                var shipments = query.Select(_ => _mapper.Map<ShipmentDetail, ShipmentDetailVM>(_)).ToList();
                if (shipments.Count > 0)
                {
                return Ok(shipments);
                }
                return Ok(shipments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /*[HttpPost]
        [Route("add-many-shipment-details")]
        public async Task<IActionResult> AddShipments(List<ShipmentDetail> newShipments)
        {
            try
            {
                await _shipmentDetailService.AddShipmentDetails(newShipments);
                return Ok("Add new Shipment Details successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }*/
        [HttpPost]
        [Route("add-shipment-detail")]
        public async Task<IActionResult> AddShipment([FromBody]ShipmentDetailCM shipmentDetailCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }
            var userId = GetCurrentUserIdFromToken();
            if(userId == null)
            {
                return Unauthorized("Invalid user");
            }
            try
            {
                var newShipmentDetail = new ShipmentDetail()
                {
                    PhoneNumber = shipmentDetailCM.PhoneNumber,
                    Address = shipmentDetailCM.Address,
                    FullName = shipmentDetailCM.FullName,
                    UserId = userId,
                    User = await GetUserFromToken()
                };
                await _shipmentDetailService.AddShipmentDetail(newShipmentDetail);

                ShipmentDetailVM detailVM = new ShipmentDetailVM()
                {
                    UserId = userId,
                    FullName = shipmentDetailCM.FullName,
                    Address = shipmentDetailCM.Address,
                    PhoneNumber = shipmentDetailCM.PhoneNumber,
                };

                ResponseModel<ShipmentDetailVM> response = new ResponseModel<ShipmentDetailVM>()
                {
                    IsSuccess = true,
                    Message = "Query successful!",
                    Data = detailVM
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPut]
        [Route("update-shipment-detail/{id}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody]ShipmentDetailUM shipmentDetailUM)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("Invalid user");
                }
                var checkExist = await _shipmentDetailService.GetShipmentDetailById(id);
                if (checkExist != null)
                {
                    checkExist.FullName = shipmentDetailUM.FullName;
                    checkExist.PhoneNumber = shipmentDetailUM.PhoneNumber;
                    checkExist.Address = shipmentDetailUM.Address;

                    await _shipmentDetailService.UpdateShipmentDetail(checkExist);
                    ShipmentDetailVM detailVM = new ShipmentDetailVM()
                    {
                        Id = checkExist.Id,
                        UserId = userId,
                        FullName = shipmentDetailUM.FullName,
                        Address = shipmentDetailUM.Address,
                        PhoneNumber = shipmentDetailUM.PhoneNumber,
                    };

                    ResponseModel<ShipmentDetailVM> response = new ResponseModel<ShipmentDetailVM>()
                    {
                        IsSuccess = true,
                        Message = "Query successful!",
                        Data = detailVM
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound($"Not found with id = ${id}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        [Route("delete-shipment-detail")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            var userId = GetCurrentUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user");
            }
            try
            {
                await _shipmentDetailService.DeleteShipmentDetailById(id);
                return Ok("Removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [NonAction]
        protected int GetCurrentUserIdFromToken()
        {
            int UserId = 0;
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        string strUserId = identity.FindFirst("UserId").Value;
                        int.TryParse(strUserId, out UserId);

                    }
                }
                return UserId;
            }
            catch
            {
                return UserId;
            }
        }
        [NonAction]
        private async Task<User> GetUserFromToken()
        {
            var user = await _userService.GetAsync(_ => _.Id == GetCurrentUserIdFromToken());
            return user.FirstOrDefault();
        }
    }
}
