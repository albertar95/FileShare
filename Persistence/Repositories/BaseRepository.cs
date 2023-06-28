using Application.Persistence.Common;
using Domain;
using Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private readonly FileShareDbContext _context;

        public BaseRepository(FileShareDbContext context)
        {
            _context = context;
            _context.Configuration.AutoDetectChangesEnabled = false;
        }
        public async Task<bool> Add<T>(T entity) where T : class
        {
            if (entity == null) return false;
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Added;
                if (await _context.SaveChangesAsync() == 1)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> Delete<T>(T entity) where T : class
        {
            if (entity == null) return false;
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                if (await _context.SaveChangesAsync() == 1)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> Update<T>(T entity) where T : class
        {
            if (entity == null) return false;
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                if (await _context.SaveChangesAsync() == 1)
                    return true;
                else
                    return false;
            }
        }
    }
}
