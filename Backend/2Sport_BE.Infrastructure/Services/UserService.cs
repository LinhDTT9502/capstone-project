using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IUserService
    {
        Task<User> FindAsync(int id);
        Task<IQueryable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>> where, string? includes = "");
        Task AddAsync(User user);
        Task AddRangeAsync(IEnumerable<User> users);
        Task UpdateAsync(User user);
        Task RemoveAsync(int id);
        Task<User> FindOrInsert(Expression<Func<User, bool>> where);
        Task<bool> CheckExistAsync(int id);
        void Save();

    }
    public class UserService : IUserService
    {
        private IUnitOfWork unitOfWork;
        private TwoSportDBContext _context;
        public UserService(
            IUnitOfWork unitOfWork,
            TwoSportDBContext dBContext)
        {
            this.unitOfWork = unitOfWork;
            _context = dBContext;
        }


        public async Task AddAsync(User user)
        {
           await unitOfWork.UserRepository.InsertAsync(user);
        }

        public Task AddRangeAsync(IEnumerable<User> users)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckExistAsync(int id)
        {
            IEnumerable<User> users = await unitOfWork.UserRepository.GetAsync(_ => _.Id == id);
            if (users.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<User> FindAsync(int id)
        {
            var context = _context;
            var user = await _context.Users.FirstOrDefaultAsync(_ => _.Id == id);
            return user;
        }
        //HAM NAY NUA
        public async Task<User> FindOrInsert(Expression<Func<User, bool>> where)
        {
            var user = await _context.Users.Where(where).FirstOrDefaultAsync();
            if(user != null)
            {
                
            }
            else
            {

            }
            throw new NotImplementedException();
        }

        public async Task<IQueryable<User>> GetAllAsync()
        {
            IEnumerable<User> users = await unitOfWork.UserRepository.GetAllAsync();
            return users.AsQueryable();
        }

        public async Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>> where, string? includes = "")
        {
            IEnumerable<User> users = await unitOfWork.UserRepository.GetAsync(where, null, includes);
            return users;
        }

        public async Task RemoveAsync(int id)
        {
           await unitOfWork.UserRepository.DeleteAsync(id);
        }

        public void Save()
        {
            unitOfWork.Save();
        }

        public async Task UpdateAsync(User user)
        {
            await unitOfWork.UserRepository.UpdateAsync(user);
        }
    }
}
