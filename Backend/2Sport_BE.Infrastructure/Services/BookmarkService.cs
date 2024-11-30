using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Google.Apis.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IBookmarkService
    {
        Task<IQueryable<Bookmark>> GetAllByUserId(int userId);
        Task<int> BookmarkUnBookmarkBlog(Bookmark bookmark);
    }
    public class BookmarkService : IBookmarkService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookmarkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> BookmarkUnBookmarkBlog(Bookmark bookmark)
        {
            try
            {
                var deletedBookmark = (await _unitOfWork.BookmarkRepository
                                                .GetAsync(_ => _.UserId == bookmark.UserId
                                                            && _.BlogId == bookmark.BlogId)).FirstOrDefault();
                if (deletedBookmark is not null)
                {
                    await _unitOfWork.BookmarkRepository.DeleteAsync(bookmark);
                    return 2;
                }
                else
                {
                    await _unitOfWork.BookmarkRepository.InsertAsync(bookmark);
                    return 1;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public async Task<IQueryable<Bookmark>> GetAllByUserId(int userId)
        {
            try
            {
                var bookmarks = await _unitOfWork.BookmarkRepository.GetAsync(_ => _.UserId == userId);
                return bookmarks.AsQueryable();

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
                return null;
            }
        }
    }
}
