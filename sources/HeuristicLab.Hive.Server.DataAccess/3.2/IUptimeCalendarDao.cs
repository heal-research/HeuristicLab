using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.DataAccess {
  public interface IUptimeCalendarDao: IGenericDao<AppointmentDto> {
    IEnumerable<AppointmentDto> GetUptimeCalendarForResource(Guid resourceId);
    void SetUptimeCalendarForResource(Guid resourceId, IEnumerable<AppointmentDto> appointments);
    void NotifyClientsOfNewCalendar(Guid groupId, bool forcePush);
    IEnumerable<AppointmentDto> GetCalendarForClient(ClientDto client);
  }
}
