using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IComputingResourceService {

    Task<int> GetNewHeartbeatIntervalAsync(Guid computingResourceId);
    Task<IDictionary<Guid, ISet<Guid>>> GetComputingResourceGenealogyAsync();
    Task<IDictionary<Guid, string>> GetComputingResourceNamesAsync();
    Task AddComputingResourceToGroupAsync(Guid droneGroupId, Guid computingResourceId);
    Task RemoveComputingResourceFromGroupAsync(Guid droneGroupId, Guid computingResourceId);

  }
}
