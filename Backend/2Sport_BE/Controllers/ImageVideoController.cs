using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageVideoController : ControllerBase
    {
        private readonly IImageVideosService _imageVideoService;
        private readonly IMapper _mapper;

        public ImageVideoController(IImageVideosService imageVideoService, IMapper mapper)
        {
            _imageVideoService = imageVideoService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("list-all-image")]
        public async Task<IActionResult> GetAllImages()
        {
            try
            {
                var imageList = (await _imageVideoService.GetAllImages()).Include(_ => _.Product).ToList();
                var result = _mapper.Map<List<ImagesVideoVM>>(imageList);
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("delete-image-video/{productId}")]
        public async Task<IActionResult> DeleteImageVideo(int productId)
        {
            try
            {
                var deletedImageVideos = (await _imageVideoService.GetImageVideosByProductId(productId)).ToList();
                if (deletedImageVideos.Count <= 0)
                {
                    return BadRequest($"Cannot find image video with product id: {productId}");
                }
                await _imageVideoService.DeleteImagesVideos(deletedImageVideos);
                return Ok("Delete image video successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
    }
}
