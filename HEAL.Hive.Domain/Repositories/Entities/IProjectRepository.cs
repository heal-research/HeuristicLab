using HEAL.Hive.Domain.Entities;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories.Entities {
  public interface IProjectRepository : IBaseRepository<Project> {

    Task UnassignComputingResourcesAsync(Project project);

  }
}
