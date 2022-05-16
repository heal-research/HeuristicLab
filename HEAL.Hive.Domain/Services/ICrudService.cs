using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services {
  public interface ICrudService<T, TKey> {

    Task<T> GetByIdAsync(TKey key);
    Task<IEnumerable<T>> GetAllAsync();
    Task InsertAsync(T entity, bool overrideKey = true);
    Task InsertAsync(T[] entities, bool overrideKey = true);
    Task UpdateAsync(T entity);
    Task UpdateAsync(T[] entities);
    Task DeleteAsync(T entity);
    Task DeleteAsync(T[] entities);
    Task DeleteAsync(TKey id);
    Task DeleteAsync(TKey[] ids);

  }
}
