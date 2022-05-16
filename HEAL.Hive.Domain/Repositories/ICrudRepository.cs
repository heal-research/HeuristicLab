using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories {
  public interface ICrudRepository<T, TKey> {

    Task<T> GetByIdAsync(TKey key);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllWhereAsync(Expression<Func<T, bool>> predicate);
    Task InsertAsync(T entity);
    Task InsertAsync(T[] entities);
    Task UpdateAsync(T entity);
    Task UpdateAsync(T[] entities);
    Task DeleteAsync(T entity);
    Task DeleteAsync(T[] entities);

  }
}
