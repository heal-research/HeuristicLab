using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class UptimeCalendarDao: BaseDao<AppointmentDto, UptimeCalendar>, IUptimeCalendarDao {
    public override UptimeCalendar DtoToEntity(AppointmentDto source, UptimeCalendar target) {
      if(source == null)
        return null;
      if (target == null)
        target = new UptimeCalendar();

      target.AllDayEvent = source.AllDayEvent;
      target.EndDate = source.EndDate;
      target.StartDate = source.StartDate;
      target.Recurring = source.Recurring;
      target.RecurringId = source.RecurringId;

      target.ResourceId = source.ResourceId;

      return target;
      
    }

    public override AppointmentDto EntityToDto(UptimeCalendar source, AppointmentDto target) {
      if (source == null)
        return null;
      if (target == null)
        target = new AppointmentDto();
      
      target.AllDayEvent = source.AllDayEvent;
      target.EndDate = source.EndDate;
      target.StartDate = source.StartDate;
      target.Recurring = source.Recurring;
      target.RecurringId = source.RecurringId;

      target.ResourceId = source.ResourceId;

      return target;

    }

    #region IGenericDao<AppointmentDto> Members

    public AppointmentDto FindById(Guid id) {
      return (from app in Context.UptimeCalendars
              where app.UptimeCalendarId.Equals(id)
              select EntityToDto(app, null)).SingleOrDefault();
    }

    public IEnumerable<AppointmentDto> FindAll() {
      return (from app in Context.UptimeCalendars              
              select EntityToDto(app, null)).ToList();
    }

    public AppointmentDto Insert(AppointmentDto bObj) {
      UptimeCalendar uc = DtoToEntity(bObj, null);
      Context.UptimeCalendars.InsertOnSubmit(uc);
      Context.SubmitChanges();
      bObj.Id = uc.UptimeCalendarId;
      return bObj;
    }

    public void Delete(AppointmentDto bObj) {
      Context.UptimeCalendars.DeleteOnSubmit(Context.UptimeCalendars.SingleOrDefault(uc => uc.UptimeCalendarId.Equals(bObj.Id)));
      Context.SubmitChanges();
    }

    public void Update(AppointmentDto bObj) {
      UptimeCalendar cc = Context.UptimeCalendars.SingleOrDefault(c => c.UptimeCalendarId.Equals(bObj.Id));
      DtoToEntity(bObj, cc);
      Context.SubmitChanges();
    }

    public IEnumerable<AppointmentDto> GetUptimeCalendarForResource(Guid resourceId) {
      return (from uc in Context.UptimeCalendars
              where uc.ResourceId.Equals(resourceId)
              select EntityToDto(uc, null)).ToList();
    }

    public void SetUptimeCalendarForResource(Guid resourceId, IEnumerable<AppointmentDto> appointments) {
      var q = (from uc in Context.UptimeCalendars
               where uc.ResourceId.Equals(resourceId)
               select uc);
      
      Context.UptimeCalendars.DeleteAllOnSubmit(q);

      foreach (AppointmentDto appdto in appointments) {      
        UptimeCalendar uc = DtoToEntity(appdto, null);
        uc.ResourceId = resourceId;
        Context.UptimeCalendars.InsertOnSubmit(uc);
      }

      Context.SubmitChanges();            
    }    

    public IEnumerable<AppointmentDto> GetCalendarForClient(ClientDto client) {
      Client dbc = Context.Clients.SingleOrDefault(c => c.ResourceId.Equals(client.Id));
      IList<AppointmentDto> appointments = new List<AppointmentDto>();
      if (dbc != null) {
        ClientGroup cg =
          Context.ClientGroups.SingleOrDefault(cgroup => cgroup.ResourceId.Equals(dbc.UseCalendarFromResourceId));
        //in case no plan has been set
        if (cg == null)
          if (dbc.Resource.ClientGroup_Resources.FirstOrDefault() != null)
            cg = dbc.Resource.ClientGroup_Resources.FirstOrDefault().ClientGroup;

        if (cg == null)
          return appointments;

        while (cg.Resource.UptimeCalendars.Count == 0) {
          if (cg.Resource.ClientGroup_Resources.FirstOrDefault() != null)
            cg = cg.Resource.ClientGroup_Resources.FirstOrDefault().ClientGroup;
          else {
            break;
          }
        }

        foreach (UptimeCalendar appointment in cg.Resource.UptimeCalendars) {
          appointments.Add(EntityToDto(appointment,null));  
        }

      }
      return appointments;
    }

    public void NotifyClientsOfNewCalendar(Guid groupId, bool forcePush) {
      
      //Get the current ClientGroup
      ClientGroup cg = Context.ClientGroups.SingleOrDefault(cgroup => cgroup.ResourceId.Equals(groupId));
      if(cg == null)
        return;

      //Get all the affected clients
      List<Client> clients = Context.Clients.Where(c => c.UseCalendarFromResourceId.Equals(cg.ResourceId)).ToList();
      
      //Set new state
      foreach (Client client in clients) {
        client.CalendarSyncStatus = (forcePush ? Enum.GetName(typeof(CalendarState), CalendarState.ForceFetch) : Enum.GetName(typeof(CalendarState), CalendarState.Fetch));      
      }
      
      Context.SubmitChanges();

      //Get all Subgroups
      List<ClientGroup> groups = (from cg1 in Context.ClientGroups
                                  where cg1.Resource.ClientGroup_Resources.Any(
                                cgr => cgr.ClientGroupId.Equals(groupId))
                              select cg1).ToList();

      //If they have their own calendar - stop propagation
      //otherweise - propagate
      foreach (ClientGroup cgroup in groups) {
        if(cgroup.Resource.UptimeCalendars.Count == 0)
          NotifyClientsOfNewCalendar(groupId, forcePush);        
      }
    }

    #endregion
  }
}
