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
using Twilio.Rest.Api.V2010.Account;

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
        Task<Comment> GetCommentById(int commentId);
        Task<IQueryable<Comment>> GetChildComments(int parentCommentId);
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
                var deletedComment = (await _unitOfWork.CommentRepository.GetAsync(_ => _.Id == commentId))
                                                                         .FirstOrDefault();

                var deletedChildComments = (await _unitOfWork.CommentRepository
                                                         .GetAsync(_ => _.ParentCommentId == commentId));
                if (deletedComment != null)
                {
                    var coordinator = await _unitOfWork.UserRepository.GetAsync(_ => _.Id == userId &&
                                                                                    _.RoleId == 4);
                    if (deletedComment.UserId != userId && coordinator is null)
                    {
                        response.Message = "You are not allowed to remove this comment!";
                        response.IsSuccess = false;
                        response.Data = -2;
                        return response;
                    }
                    await _unitOfWork.CommentRepository.DeleteAsync(commentId);
                    await _unitOfWork.CommentRepository.DeleteRangeAsync(deletedChildComments);
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

        public async Task<IQueryable<Comment>> GetChildComments(int parentCommentId)
        {
            return (await _unitOfWork.CommentRepository.GetAndIncludeAsync(_ => !string.IsNullOrEmpty(_.ProductCode) 
                                                                        && _.ParentCommentId == parentCommentId,
                                                                            new string[] { "User" }))
                                           .AsQueryable()
                                           .Include(_ => _.User);
        }

        public async Task<Comment> GetCommentById(int commentId)
        {
            var comment = await _unitOfWork.CommentRepository.FindAsync(commentId);
            return comment;
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
