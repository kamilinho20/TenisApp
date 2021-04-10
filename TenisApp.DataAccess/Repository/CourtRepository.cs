using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TenisApp.Core.Model;
using TenisApp.DataAccess.DbContext;

namespace TenisApp.DataAccess.Repository
{
    public interface ICourtRepository
    {
        Task<Court> AddCourt(Court court);
        Task UpdateCourt(Court modelCourt);
        Task DeleteCourt(Court modelCourt);
        Task<List<Court>> GetCourts();
        ValueTask<Court> GetCourt(int id);
    }

    public class CourtRepository : ICourtRepository
    {
        private readonly AppDbContext _dbContext;

        public CourtRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Court> AddCourt(Court court)
        {
            var added = await _dbContext.AddAsync(court);
            await _dbContext.SaveChangesAsync();
            return added.Entity;
        }

        public Task UpdateCourt(Court modelCourt)
        {
            _dbContext.Update(modelCourt);
            return _dbContext.SaveChangesAsync();
        }

        public Task DeleteCourt(Court modelCourt)
        {
            _dbContext.Remove(modelCourt);
            return _dbContext.SaveChangesAsync();
        }

        public Task<List<Court>> GetCourts()
        {
            return _dbContext.Courts.ToListAsync();
        }

        public ValueTask<Court> GetCourt(int id)
        {
            return _dbContext.Courts.FindAsync(id);
        }
    }
}