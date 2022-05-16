using HEAL.Hive.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IProjectService : IBaseService<Project> {

    Task<IDictionary<Guid, ISet<Guid>>> GetProjectGenealogyAsync();
    Task<IDictionary<Guid, string>> GetProjectNamesAsync();
    Task<Project> GetProjectByNameAsync(string name);

  }
}
