using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IBaseService<T> : ICrudService<T, Guid> {

    Task<T> GetByIdAsync(Guid id, bool resolveDependencies);
    Task<IEnumerable<T>> GetAllAsync(bool resolveDependencies);
    Task RestoreAsync(Guid id);
    Task RestoreAsync(Guid[] ids);

  }
}
