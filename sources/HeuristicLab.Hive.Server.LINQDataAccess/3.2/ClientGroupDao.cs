using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class ClientGroupDao : BaseDao<ClientGroupDto, ClientGroup>, IClientGroupDao {


    #region IGenericDao<ClientGroupDto,ClientGroup> Members

    public ClientGroupDto FindById(Guid id) {
      return (from cc in Context.ClientGroups
              where cc.ResourceId.Equals(id)
              select EntityToDto(cc, null)).SingleOrDefault();
    }


    public IEnumerable<ClientGroupDto> FindAll() {
      return (from cc in Context.ClientGroups
              select EntityToDto(cc, null)).ToList();
    }

    public ClientGroupDto Insert(ClientGroupDto bObj) {
      //Auto GUID is disabled for Ressource... thx to the clients... grml      
      if(bObj.Id == Guid.Empty)
        bObj.Id = Guid.NewGuid();

      ClientGroup cc = DtoToEntity(bObj, null);
      Context.ClientGroups.InsertOnSubmit(cc);
      CommitChanges();
      bObj.Id = cc.ResourceId;
      return bObj;
    }

    public void Delete(ClientGroupDto bObj) {
      
      //Deleting all references
      Context.ClientGroup_Resources.DeleteAllOnSubmit(Context.ClientGroup_Resources.Where(cg => cg.ClientGroupId.Equals(bObj.Id)));

      Resource res = Context.Resources.SingleOrDefault(c => c.ResourceId.Equals(bObj.Id));

      Context.Resources.DeleteOnSubmit(res);
      CommitChanges();
    }

    public void Update(ClientGroupDto bObj) {
      ClientGroup client = Context.ClientGroups.SingleOrDefault(c => c.ResourceId.Equals(bObj.Id));
      DtoToEntity(bObj, client);
      CommitChanges();
    }

    public void AddRessourceToClientGroup(Guid resource, Guid clientGroupId) {
      ClientGroup cg = Context.ClientGroups.SingleOrDefault(c => c.ResourceId.Equals(clientGroupId));
      Resource res = Context.Resources.SingleOrDefault(r => r.ResourceId.Equals(resource));
      cg.ClientGroup_Resources.Add(new ClientGroup_Resource { ClientGroup = cg, Resource = res });      
      CommitChanges();
    }

    public void RemoveRessourceFromClientGroup(Guid resource, Guid clientGroupId) {
      ClientGroup_Resource cgr =
        Context.ClientGroup_Resources.SingleOrDefault(
          cg => cg.ResourceId.Equals(resource) && cg.ClientGroupId.Equals(clientGroupId));
      Context.ClientGroup_Resources.DeleteOnSubmit(cgr);
      CommitChanges();
    }

    public IEnumerable<ClientGroupDto> MemberOf(ClientDto client) {
      return (from cgr in Context.ClientGroup_Resources
              where cgr.ResourceId.Equals(client.Id)
              select EntityToDto(cgr.ClientGroup, null)).ToList();
    }

    public IEnumerable<ClientGroupDto> FindAllWithSubGroupsAndClients() {
      List<ClientGroupDto> groupList = new List<ClientGroupDto>();

      var q = (from cg in Context.ClientGroups
               where !Context.ClientGroup_Resources.Any(cgr => cgr.ResourceId.Equals(cg.ResourceId))
               select cg);

      foreach (ClientGroup cg in q) {
        ClientGroupDto cgd = EntityToDto(cg, null);
        groupList.Add(cgd);
        FillSubGroupsAndClientsRecursivly(cgd, cg.ClientGroup_Resources);        
      }
      return groupList;
    }

    private void FillSubGroupsAndClientsRecursivly(ClientGroupDto parentClientGroup, System.Data.Linq.EntitySet<ClientGroup_Resource> parentResourceSet) {
      ClientDao cd = new ClientDao();      
      //Get all the Groups
      
      var qGroups = (from cg in Context.ClientGroups
               where cg.Resource.ClientGroup_Resources.Any(cgr => cgr.ClientGroupId.Equals(parentClientGroup.Id))
               select cg);

      foreach (ClientGroup cg in qGroups) {
        ClientGroupDto cgd = EntityToDto(cg, null);
        parentClientGroup.Resources.Add(cgd);
        FillSubGroupsAndClientsRecursivly(cgd, cg.ClientGroup_Resources);
      }

      //get the clients
      var qClients = (from cl in Context.Clients
               where cl.Resource.ClientGroup_Resources.Any(cgr => cgr.ClientGroupId.Equals(parentClientGroup.Id))
               select cl);
      foreach (Client client in qClients) {
        parentClientGroup.Resources.Add(cd.EntityToDto(client, null));                        
      }     
    }

    public IEnumerable<Guid> FindAllGroupAndParentGroupIdsForClient(Guid clientId) {
      List<Guid> guids = new List<Guid>();
      Client c = Context.Clients.SingleOrDefault(client => client.ResourceId.Equals(clientId));
      FindAllGroupAndParentGroupIdsForClientRecursive(c.Resource, guids);
      return guids;
    }

    private void FindAllGroupAndParentGroupIdsForClientRecursive(Resource resource, List<Guid> guids) {
      foreach (ClientGroup_Resource cgr in resource.ClientGroup_Resources) {
        guids.Add(cgr.ClientGroupId);
        FindAllGroupAndParentGroupIdsForClientRecursive(cgr.ClientGroup.Resource, guids);        
      }
    }

    #endregion

    public override ClientGroup DtoToEntity(ClientGroupDto source, ClientGroup target) {
      if (source == null)
        return null;
      if (target == null)
        target = new ClientGroup();
      if (target.Resource == null)
        target.Resource = new Resource();

      target.Resource.Name = source.Name;
      target.Resource.ResourceId = source.Id;

      return target;
    }

    public override ClientGroupDto EntityToDto(ClientGroup source, ClientGroupDto target) {
      if (source == null)
        return null;
      if (target == null)
        target = new ClientGroupDto();

      target.Id = source.ResourceId;
      target.Name = source.Resource.Name;

      return target;
    }

    public IEnumerable<ClientGroupDto> FindByName(string res) {
      return (from cq in Context.ClientGroups
              where cq.Resource.Name == res
              select EntityToDto(cq, null)).ToList();      
    }


  }
}
