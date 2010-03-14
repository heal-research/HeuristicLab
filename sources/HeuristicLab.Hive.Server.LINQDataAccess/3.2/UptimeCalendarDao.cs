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

    #endregion
  }
}
