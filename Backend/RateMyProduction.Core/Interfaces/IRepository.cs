using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RateMyProduction.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);
    void Delete(T entity);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> SaveChangesAsync();
}
