using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GuideMeServerMVC.Services
{
    public abstract class ServiceBase<T> where T : BaseViewModel
    {
        protected GuidemeDbContext _context;
        public ServiceBase(GuidemeDbContext context)
        {
            _context = context;
        }

        public async virtual Task<bool> SaveAsync(T model)
        {
            if (_context != null)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;         
        }

        public async virtual Task<T> GetById(int id)
        {
            if (_context != null)
                return await _context.Set<T>().AsNoTracking().SingleAsync<T>(x => x.Id == id);

            return null;
        }

        public async virtual Task<List<T>> GetAll()
        {
            if (_context != null)
                return await _context.Set<T>().AsNoTracking().ToListAsync<T>();

            return null;
        }

        
        public async virtual Task<bool> UpdateAsync(T model)
        {
            if (_context != null)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public abstract Dictionary<string, string> ValidarDados(T model);


        
    }
}
