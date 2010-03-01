using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class ClientDao: BaseDao, IClientDao {

    public ClientDao() {      
    }

    public ClientInfo FindById(Guid id) {
      return (from client in Context.Clients
              where client.ResourceId.Equals(id)
              select
                new ClientInfo {
                  CpuSpeedPerCore = client.CPUSpeed,
                  FreeMemory = client.FreeMemory,
                  Id = client.ResourceId,
                  Login = client.Login,
                  Memory = client.Memory,
                  Name = client.Resource.Name,
                  NrOfCores = client.NumberOfCores,
                  NrOfFreeCores = client.NumberOfFreeCores,
                  State = (State)Enum.Parse(typeof(State), client.Status)
                }
            ).SingleOrDefault();      
    }

    public IEnumerable<ClientInfo> FindAll() {
      return (from client in Context.Clients
              select
                new ClientInfo {
                                 CpuSpeedPerCore = client.CPUSpeed,
                                 FreeMemory = client.FreeMemory,
                                 Id = client.ResourceId,
                                 Login = client.Login,
                                 Memory = client.Memory,
                                 Name = client.Resource.Name,
                                 NrOfCores = client.NumberOfCores,
                                 NrOfFreeCores = client.NumberOfFreeCores,
                                 State = (State) Enum.Parse(typeof (State), client.Status)
                               }
             ).ToList();
    }

 
    public ClientInfo Insert(ClientInfo info) {
      Client c = new Client {
                              CPUSpeed = info.CpuSpeedPerCore,
                              FreeMemory = info.FreeMemory,
                              Resource = new Resource {Name = info.Name, ResourceId = info.Id},
                              Login = info.Login,
                              Memory = info.Memory,
                              NumberOfCores = info.NrOfCores,
                              NumberOfFreeCores = info.NrOfFreeCores,
                              Status = Enum.GetName(typeof (State), info.State)
                            };

      Context.Clients.InsertOnSubmit(c);
      Context.SubmitChanges();
      info.Id = c.ResourceId;
      return info;
    }

    public void Delete(ClientInfo info) {
      Client client = Context.Clients.SingleOrDefault(c => c.ResourceId.Equals(info.Id));
      Context.Clients.DeleteOnSubmit(client);
    }

    public void Update(ClientInfo info) {
      Client client = Context.Clients.SingleOrDefault(c => c.ResourceId.Equals(info.Id));
      client.CPUSpeed = info.CpuSpeedPerCore;
      client.FreeMemory = info.FreeMemory;
      client.Resource.Name = info.Name;
      client.Login = info.Login;
      client.Memory = info.Memory;
      client.NumberOfCores = info.NrOfCores;
      client.NumberOfFreeCores = info.NrOfFreeCores;
      client.Status = Enum.GetName(typeof (State), info.State);
      Context.SubmitChanges();
    }



    #region IGenericDao<ClientInfo,Client> Members



    #endregion
  }
}
