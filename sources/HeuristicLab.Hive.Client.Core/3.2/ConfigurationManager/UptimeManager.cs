using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendar;
using System.Xml.Serialization;
using System.IO;

namespace HeuristicLab.Hive.Client.Core.ConfigurationManager {
  public class UptimeManager {

    public List<Appointment> Appointments { get; set; }
    private static String path = System.IO.Directory.GetCurrentDirectory()+"\\plugins\\Hive.Client.Jobs\\";

    private static UptimeManager instance = null;
    public static UptimeManager Instance {
      get {
        if (instance == null) {
          instance = new UptimeManager();
        }
        return instance;
      }
    }

    private void PersistToHDD() {
      XmlSerializer s = new XmlSerializer(typeof(List<Appointment>));
      using (TextWriter w = new StreamWriter(path + "calendar.xml")) {
        s.Serialize(w, Appointments);  
      }
    }

    private void RestoreFromHDD() {
      XmlSerializer s = new XmlSerializer(typeof(List<Appointment>));
      using (TextReader r = new StreamReader(path + "calendar.xml")) {
        Appointments = (List<Appointment>)s.Deserialize(r);        
      }           
    }

    public bool isOnline(DateTime time) {
      foreach (Appointment app in Appointments)
        if ((time >= app.StartDate) &&
            (time <= app.EndDate))
          return true;
      return false;
    }
  }
}
