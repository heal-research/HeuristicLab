using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;
using System.Threading;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class ClientDao: BaseDao<ClientDto, Client>, IClientDao {

    public ClientDao() {      
    }

    public ClientDto FindById(Guid id) {
      return (from client in Context.Clients
              where client.ResourceId.Equals(id)
              select EntityToDto(client, null)
            ).SingleOrDefault();      
    }

    public IEnumerable<ClientDto> FindAll() {
      return (from client in Context.Clients
              select
                EntityToDto(client, null)
             ).ToList();
    }

    public IEnumerable<ClientDto> FindAllClientsWithoutGroup() {
      return (from client in Context.Clients
              where client.Resource.ClientGroup_Resources.Count == 0
              select EntityToDto(client, null)).ToList();
    }

    public ClientDto GetClientForJob(Guid jobId) {
      return (from job in Context.Jobs
              where job.JobId.Equals(jobId)
              select EntityToDto(job.Client, null)).SingleOrDefault();
    }

    public void SetServerSideCalendar(ClientDto client, Guid clientGroupId) {
      Client dbclient = Context.Clients.SingleOrDefault(c => c.ResourceId.Equals(client.Id));
      dbclient.UseCalendarFromResourceId = clientGroupId;
      dbclient.CalendarSyncStatus = Enum.GetName(typeof(CalendarState), CalendarState.Fetch);
      CommitChanges();
    }

    public ClientDto Insert(ClientDto info) {
      Client c = DtoToEntity(info, null);      
      Context.Clients.InsertOnSubmit(c);
      CommitChanges();
      info.Id = c.ResourceId;
      return info;
    }

    //Cascading delete takes care of the rest
    public void Delete(ClientDto info) {
      Resource res = Context.Resources.SingleOrDefault(c => c.ResourceId.Equals(info.Id));            
      Context.Resources.DeleteOnSubmit(res);
      CommitChanges();
    }

    public void Update(ClientDto info) {
      Client client = Context.Clients.SingleOrDefault(c => c.ResourceId.Equals(info.Id));
      DtoToEntity(info, client);
      CommitChanges();      
    }

    public override Client DtoToEntity(ClientDto source, Client target) {
      if (source == null)
        return null;
      if (target == null) 
        target = new Client();
      
      target.CPUSpeed = source.CpuSpeedPerCore;
      
      if(target.Resource == null)
        target.Resource = new Resource();

      target.FreeMemory = source.FreeMemory;
      target.Resource.Name = source.Name;
      target.Resource.ResourceId = source.Id;
      target.CalendarSyncStatus = Enum.GetName(typeof(CalendarState), source.CalendarSyncStatus);
      target.Login = source.Login;
      target.Memory = source.Memory;
      target.NumberOfCores = source.NrOfCores;
      target.NumberOfFreeCores = source.NrOfFreeCores;
      target.Status = Enum.GetName(typeof(State), source.State);
      return target;
    }

    public override ClientDto EntityToDto(Client source, ClientDto target) {
      if (source == null)
        return null;
      if(target == null) 
        target = new ClientDto();
      target.CpuSpeedPerCore = source.CPUSpeed;
      target.FreeMemory = source.FreeMemory;
      target.Id = source.ResourceId;
      target.CalendarSyncStatus = (CalendarState) Enum.Parse(typeof (CalendarState), source.CalendarSyncStatus);
      target.Login = source.Login;
      target.Memory = source.Memory;
      target.Name = source.Resource.Name;
      target.NrOfCores = source.NumberOfCores;
      target.NrOfFreeCores = source.NumberOfFreeCores;
      target.State = (State) Enum.Parse(typeof (State), source.Status);
      return target;
    } 
  }
}
