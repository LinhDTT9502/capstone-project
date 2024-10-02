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
        Task<IQueryable<ImagesVideo>> GetAllImages();
        Task<IQueryable<ImagesVideo>> GetAllVideos();
        Task DeleteImagesVideos(List<ImagesVideo> imagesVideos);
        Task<IQueryable<ImagesVideo>> GetImageVideosByProductId(int productId);
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

        public async Task DeleteImagesVideos(List<ImagesVideo> imagesVideos)
        {
            await _unitOfWork.ImagesVideoRepository.DeleteRangeAsync(imagesVideos);
        }

        public async Task<IQueryable<ImagesVideo>> GetAllImages()
        {
            var imageList = await _unitOfWork.ImagesVideoRepository.GetAsync(_ => _.VideoUrl == null);
            return imageList.AsQueryable();
        }

        public async Task<IQueryable<ImagesVideo>> GetAllVideos()
        {
            var videoList = await _unitOfWork.ImagesVideoRepository.GetAsync(_ => _.ImageUrl == null);
            return videoList.AsQueryable();
        }

        public async Task<IQueryable<ImagesVideo>> GetImageVideosByProductId(int productId)
        {
            var videoList = await _unitOfWork.ImagesVideoRepository.GetAsync(_ => _.ProductId == productId);
            return videoList.AsQueryable();
        }
    }
}
