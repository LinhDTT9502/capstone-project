using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IImageVideosService
    {
        Task AddImage(ImagesVideo image);
    }
    public class ImageVideosService : IImageVideosService
    {
        private readonly TwoSportCapstoneDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public ImageVideosService(TwoSportCapstoneDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public async Task AddImage(ImagesVideo image)
        {
            await _unitOfWork.ImagesVideoRepository.InsertAsync(image);

        }
    }
}
