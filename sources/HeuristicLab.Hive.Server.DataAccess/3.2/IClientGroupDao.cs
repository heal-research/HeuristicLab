using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.DataAccess {
  public interface IClientGroupDao: IGenericDao<ClientGroupDto> {
    void AddRessourceToClientGroup(Guid ressource, Guid clientGroupId);
    void RemoveRessourceFromClientGroup(Guid ressource, Guid clientGroupId);

    IEnumerable<ClientGroupDto> MemberOf(ClientDto client);

    IEnumerable<ClientGroupDto> FindAllWithSubGroupsAndClients();
    IEnumerable<Guid> FindAllGroupAndParentGroupIdsForClient(Guid clientId);
  }
}
