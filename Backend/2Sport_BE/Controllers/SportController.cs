using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Composition;

namespace _2Sport_BE.Controllers
{
    [Route("api/sport")]
    [ApiController]
    public class SportController : ControllerBase
    {
        private readonly ISportService _sportService;
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SportController(ISportService sportService,
                               IProductService productService,
                               IUnitOfWork unitOfWork, IMapper mapper)
        {
            _sportService = sportService;
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("list-sports")]
        public async Task<IActionResult> GetSports()
        {
            try
            {
                var query = await _sportService.GetAllSports();
                var sports = query.Select(_ => _mapper.Map<Sport, SportVM>(_)).ToList();
                return Ok (new { total = sports.Count, data = sports });
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet]
        [Route("get-sport-by-id/{sportId}")]
        public async Task<IActionResult> GetSportById(int sportId)
        {
            try
            {
                var sport = await _sportService.GetSportById(sportId);
                var result = _mapper.Map<SportVM>(sport);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("add-sports")]
        public async Task<IActionResult> AddSports(List<SportCM> newSportCMs)
        {
            try
            {
                var newSports = _mapper.Map<List<Sport>>(newSportCMs);
                foreach (var newSport in newSports)
                {
                    newSport.Status = true;
                    newSport.CreatedAt = DateTime.Now;
                }
                await _sportService.AddSports(newSports);
                return Ok("Add new sports successfully!");
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("update-sport/{sportId}")] 
        public async Task<IActionResult> UpdateSport(int sportId, SportUM sport)
        {
            try
            {
                var updatedSport = await _sportService.GetSportById(sportId);
                updatedSport.Name = sport.Name;
                await _sportService.UpdateSport(updatedSport);
                await _unitOfWork.SaveChanges();
                return Ok(updatedSport);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        [Route("delete-sport/{sportId}")]
        public async Task<IActionResult> DeleteSport(int sportId)
        {
            try
            {
                var deletedSport =  await _sportService.GetSportById(sportId);
                if (deletedSport != null)
                {
                    deletedSport.Status = !deletedSport.Status;
                    await _sportService.UpdateSport(deletedSport);
                    var deletedProducts = await _productService.GetProducts(_ => _.SportId == sportId);
                    if (deletedProducts != null)
                    {
                        foreach (var product in deletedProducts)
                        {
                            await _productService.DeleteProductById(product.Id);
                        }
                    }
                    return Ok($"Delete brand with id: {sportId}!");
                }
                return BadRequest($"Cannot find brand with id {sportId}!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
