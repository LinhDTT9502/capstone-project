using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface ISportService
    {
        Task<IQueryable<Sport>> GetAllSports();
        Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null);

        Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null,
                                  Func<IQueryable<Sport>, IOrderedQueryable<Sport>> orderBy = null,
                                  string includeProperties = "",
                                  int? pageIndex = null,
                                  int? pageSize = null);

        Task<Sport> GetSportById(int? id);
        Task AddSports(IEnumerable<Sport> sports);
        Task DeleteSportById(int id);
        Task UpdateSport(Sport newSport);
    }
    public class SportService : ISportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddSports(IEnumerable<Sport> sports)
        {
            await _unitOfWork.SportRepository.InsertRangeAsync(sports);
        }

        public async Task DeleteSportById(int id)
        {
            await _unitOfWork.SportRepository.DeleteAsync(id);
        }

        public async Task<IQueryable<Sport>> GetAllSports()
        {
            var query = await _unitOfWork.SportRepository.GetAllAsync();
            return query.AsQueryable();
        }

        public async Task<Sport> GetSportById(int? id)
        {
            return await _unitOfWork.SportRepository.FindAsync(id);
        }

        public async Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null)
        {
            var query = await _unitOfWork.SportRepository.GetAsync(filter);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null, int? pageIndex = null, int? pageSize = null)
        {
            var query = await _unitOfWork.SportRepository.GetAsync(filter, null, null, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null, 
                                                    Func<IQueryable<Sport>, 
                                                    IOrderedQueryable<Sport>> orderBy = null, string includeProperties = "", 
                                                    int? pageIndex = null, int? pageSize = null)
        {
            var query = await _unitOfWork.SportRepository.GetAsync(filter, orderBy, includeProperties, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task UpdateSport(Sport newSport)
        {
            await _unitOfWork.SportRepository.UpdateAsync(newSport);
        }

    }
}
