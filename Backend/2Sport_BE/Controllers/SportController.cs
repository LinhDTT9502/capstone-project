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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SportController(ISportService sportService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _sportService = sportService;
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

        [HttpPost]
        [Route("add-sports")]
        public async Task<IActionResult> AddSports(List<Sport> newSports)
        {
            try
            {
                await _sportService.AddSports(newSports);
                return Ok("Add new sports successfully!");
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("update-sport")] 
        public async Task<IActionResult> UpdateSport(SportUM sport)
        {
            try
            {
                var updatedSport = _mapper.Map<SportUM, Sport>(sport);
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
        [Route("delete-sport")]
        public async Task<IActionResult> DeleteSport(int id)
        {
            try
            {
                await _sportService.DeleteSportById(id);
                return Ok("Removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
