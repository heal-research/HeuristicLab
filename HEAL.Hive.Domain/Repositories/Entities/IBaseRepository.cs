using HEAL.Hive.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories.Entities {
  public interface IBaseRepository<T> : ICrudRepository<T, Guid> where T : Resource {

    Task<T> GetByIdAsync(Guid id, bool resolveDependencies);
    Task<IEnumerable<T>> GetAllAsync(bool resolveDependencies);
    Task<IEnumerable<T>> GetAllWhereAsync(Expression<Func<T, bool>> predicate, bool resolveDependencies);
    Task RestoreAsync(Guid id);
    Task RestoreAsync(Guid[] ids);

  }
}
