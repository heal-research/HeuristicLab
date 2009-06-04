using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendar;

namespace HeuristicLab.Hive.Client.Core.ConfigurationManager {
  public class UptimeManager {

    public List<Appointment> Appointments { get; set; }

    private static UptimeManager instance = null;
    public static UptimeManager Instance {
      get {
        if (instance == null) {
          instance = new UptimeManager();
        }
        return instance;
      }
    }

    
  }
}
