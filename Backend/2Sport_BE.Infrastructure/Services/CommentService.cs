using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface ICommentService
    {
        Task<int> AddComment (int userId, string productCode, Comment comment);
        Task<int> ReplyComment (int userId, string productCode, int parentCommentId, Comment comment);
        Task<IQueryable<Comment>> GetAllComments();
        Task<IQueryable<Comment>> GetAllComment (string productCode);
        Task<ResponseDTO<int>> UpdateComment (int currUserId, int commentId, Comment newComment);
        Task<ResponseDTO<int>> DeleteComment (int userId, int commentId);
    }
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddComment(int userId, string productCode, Comment comment)
        {
            try
            {
                comment.UserId = userId;
                comment.ProductCode = productCode;
                comment.CreatedAt = DateTime.Now;
                await _unitOfWork.CommentRepository.InsertAsync(comment);
                await _unitOfWork.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

        }

        public async Task<ResponseDTO<int>> DeleteComment(int userId, int commentId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var deletedComment = (await _unitOfWork.CommentRepository.GetAsync(_ => _.Id == commentId)).FirstOrDefault();
                if (deletedComment != null)
                {

                    if (deletedComment.UserId != userId)
                    {
                        response.Message = "You are not allowed to remove this comment!";
                        response.IsSuccess = false;
                        response.Data = -2;
                        return response;
                    }
                    await _unitOfWork.CommentRepository.DeleteAsync(commentId);
                    await _unitOfWork.SaveChanges();
                    response.Message = "Remove comment successfully!";
                    response.IsSuccess = true;
                    response.Data = 1;
                    return response;
                }
                response.Message = "Cannot find comment!";
                response.IsSuccess = false;
                response.Data = 0;
                return response;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.Message = "Something wrong!";
                response.IsSuccess = false;
                response.Data = -1;
                return response;
            }

        }

        public async Task<IQueryable<Comment>> GetAllComment(string productCode)
        {
            return (await _unitOfWork.CommentRepository.GetAsync(_ => _.ProductCode.ToLower()
                                                                        .Equals(productCode.ToLower())))
                                                       .AsQueryable()
                                                       .Include(_ => _.User);
        }

        public async Task<IQueryable<Comment>> GetAllComments()
        {
            return (await _unitOfWork.CommentRepository.GetAsync(_ => !string.IsNullOrEmpty(_.ProductCode)))
                                                       .AsQueryable()
                                                       .Include(_ => _.User);
        }

        public async Task<int> ReplyComment(int userId, string productCode, int parentCommentId, Comment comment)
        {
            try
            {
                comment.UserId = userId;
                comment.ProductCode = productCode;
                comment.ParentCommentId = parentCommentId;
                comment.CreatedAt = DateTime.Now;
                await _unitOfWork.CommentRepository.InsertAsync(comment);
                await _unitOfWork.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public async Task<ResponseDTO<int>> UpdateComment(int currUserId, int commentId, Comment newComment)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var editedComment = (await _unitOfWork.CommentRepository.GetAsync(_ => _.Id == commentId)).FirstOrDefault();
                if (editedComment != null)
                {

                    if (editedComment.UserId != currUserId)
                    {
                        response.Message = "You are not allowed to remove this comment!";
                        response.IsSuccess = false;
                        response.Data = -2;
                        return response;
                    }
                    editedComment.Content = newComment.Content;
                    await _unitOfWork.CommentRepository.UpdateAsync(editedComment);
                    await _unitOfWork.SaveChanges();
                    response.Message = "Update comment successfully!";
                    response.IsSuccess = true;
                    response.Data = 1;
                    return response;
                }
                response.Message = "Cannot find comment!";
                response.IsSuccess = false;
                response.Data = 0;
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.Message = "Something wrong!";
                response.IsSuccess = false;
                response.Data = -1;
                return response;
            }
        }
    }
}
