using RateMyProduction.Core.Interfaces;
using RateMyProduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RateMyProduction.Infrastructure.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly RateMyProductionContext _context;

        public EfRepository(RateMyProductionContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> ListAllAsync()
            => await _context.Set<T>().ToListAsync();

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().FirstOrDefaultAsync(predicate);

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().SingleOrDefaultAsync(predicate);

        public async Task AddAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _context.Set<T>().AddRangeAsync(entities);

        public void Update(T entity)
            => _context.Set<T>().Update(entity);

        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().AnyAsync(predicate);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
