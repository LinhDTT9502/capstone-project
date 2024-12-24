using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        public BranchController(IBranchService branchService,
                                IImageService imageService,
                                IWarehouseService warehouseService,
                                IMapper mapper)
        {
            _branchService = branchService;
            _imageService = imageService;
            _mapper = mapper;

        }

        [HttpGet]
        [Route("list-all")]
        public async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var branches = await _branchService.ListAllAsync();
                var result = _mapper.Map<List<BranchVM>>(branches.ToList());
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("get-branch-by-id/{branchId}")]
        public async Task<IActionResult> GetBranchById(int branchId)
        {
            try
            {
                var branch = await _branchService.GetBranchById(branchId);
                var result = _mapper.Map<BranchVM>(branch);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("add-branch")]
        public async Task<IActionResult> AddBranch(BranchCM branchCM)
        {
            var addedBranch = _mapper.Map<Branch>(branchCM);
            try
            {
                if (branchCM.ImageURL != null)
                {
                    var uploadResult = await _imageService.UploadImageToCloudinaryAsync(branchCM.ImageURL);
                    if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        addedBranch.ImgAvatarPath = uploadResult.SecureUrl.AbsoluteUri;
                    }
                    else
                    {
                        return BadRequest("Something wrong!");
                    }
                }
                await _branchService.CreateANewBranchAsync(addedBranch);
                return Ok("Add branch successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update-branch/{branchId}")]
        public async Task<IActionResult> UpdateBranch(int branchId, BranchUM branchUM)
        {
            var updatedBranch = await _branchService.GetBranchById(branchId);
            if (updatedBranch != null)
            {
                updatedBranch.BranchName = branchUM.BranchName;
                updatedBranch.Location = branchUM.Location;
                updatedBranch.Hotline = branchUM.Hotline;
                try
                {
                    if (branchUM.ImageURL != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(branchUM.ImageURL);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            updatedBranch.ImgAvatarPath = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }
                    await _branchService.UpdateBranchAsync(updatedBranch);
                    return Ok("Update branch successfully!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest($"Cannot find branch with id: {branchId}");

        }

        [HttpPut]
        [Route("edit-status/{branchId}")]
        public async Task<IActionResult> EditStatus(int branchId)
        {
            try
            {
                var updatedBranch = await _branchService.GetBranchById(branchId);
                updatedBranch.Status = !updatedBranch.Status;
                await _branchService.UpdateBranchAsync(updatedBranch);
                return Ok("Update status successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete]
        [Route("delete-branch/{branchId}")]
        public async Task<IActionResult> DeleteBranch(int branchId)
        {
            var deletedBranch = await _branchService.GetBranchById(branchId);
            if (deletedBranch != null)
            {
                if (deletedBranch.Status == true)
                {
                    deletedBranch.Status = false;
                }
                else
                {
                    deletedBranch.Status = true;
                }
                await _branchService.UpdateBranchAsync(deletedBranch);
                return Ok($"Delete branch with id: {branchId}!");
            }
            return BadRequest($"Cannot find branch with id {branchId}!");
        }
    }
}
