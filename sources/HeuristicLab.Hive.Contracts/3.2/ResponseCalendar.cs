using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Contracts {
  public class ResponseCalendar: Response {
    [DataMember]
    public bool ForceFetch { get; set; }
    [DataMember]
    public IEnumerable<AppointmentDto> Appointments { get; set; }
  }
}
