using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IFeedbackService
    {
        Task<IQueryable<Feedback>> GetFeedbacks();
        Task<IQueryable<Feedback>> GetFeedbackById(int feedbackId);
        Task<int> SendFeedback(int? userId, Feedback addedFeedback);
        Task<int> RemoveFeedback(int feedbackId);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IQueryable<Feedback>> GetFeedbackById(int feedbackId)
        {
            try
            {
                var query = await _unitOfWork.FeedbackRepository.GetAsync(_ => _.Id == feedbackId);
                return query.AsQueryable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IQueryable<Feedback>> GetFeedbacks()
        {
            try
            {
                var query = await _unitOfWork.FeedbackRepository.GetAllAsync();
                return query.AsQueryable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<int> RemoveFeedback(int feedbackId)
        {
            try
            {
                await _unitOfWork.FeedbackRepository.DeleteAsync(feedbackId);
                return 1;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<int> SendFeedback(int? userId, Feedback addedFeedback)
        {
            try
            {
                addedFeedback.CreatedAt = DateTime.Now;
                if (userId > 0)
                {
                    var user = await _unitOfWork.UserRepository.FindAsync(userId);
                    addedFeedback.UserId = user.Id;
                    addedFeedback.PhoneNumber = user.PhoneNumber;
                    addedFeedback.UserName = user.UserName;
                    addedFeedback.FullName = user.FullName;
                }
                await _unitOfWork.FeedbackRepository.InsertAsync(addedFeedback);
                return 1;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
