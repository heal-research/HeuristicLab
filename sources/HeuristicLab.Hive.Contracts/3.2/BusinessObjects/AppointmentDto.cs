using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HeuristicLab.DataAccess;

namespace HeuristicLab.Hive.Contracts.BusinessObjects {
  
  [DataContract]
  public class AppointmentDto: PersistableObject {
    [DataMember]
    public DateTime StartDate { get; set; }
    [DataMember]
    public DateTime EndDate { get; set; }
    [DataMember]
    public bool AllDayEvent { get; set; }
    [DataMember]
    public bool Recurring { get; set; }
    [DataMember]
    public Guid RecurringId { get; set; }
    [DataMember]
    public Guid ResourceId { get; set; }
  }
}
