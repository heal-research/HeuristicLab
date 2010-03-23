using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendar;

namespace HeuristicLab.Hive.Client.Core.ConfigurationManager {
  public class AppointmentContainer {
    public DateTime Updated { get; set; }
    public List<Appointment> Appointments { get; set; }
    public bool IsLocal { get; set; }

    public AppointmentContainer() {
      Appointments = new List<Appointment>();
      IsLocal = false;
    }
  }
}
