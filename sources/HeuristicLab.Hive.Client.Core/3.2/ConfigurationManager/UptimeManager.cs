using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendar;
using System.Xml.Serialization;
using System.IO;
using HeuristicLab.Hive.Client.Common;

namespace HeuristicLab.Hive.Client.Core.ConfigurationManager {
  public class UptimeManager {

    private List<Appointment> appointments = null;
    public List<Appointment> Appointments {
      get {
        if (appointments == null)
          RestoreFromHDD();
        return appointments;
      }
      set {
        appointments = value;
        PersistToHDD();
      }
    }
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
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      TextWriter w = null;
      try {
        w = new StreamWriter(path + "calendar.xml");
        s.Serialize(w, Appointments);  
      } catch(Exception e) {
        Logging.Instance.Error(this.ToString(), "Persistance of the Calendar failed!", e);
      } finally {
        if(w!=null)
          w.Close();
      }
    }

    private void RestoreFromHDD() {
      XmlSerializer s = new XmlSerializer(typeof(List<Appointment>));
      if(File.Exists(Path.Combine(path, "calendar.xml"))) {
        TextReader r = null;
        
        try {
          r = new StreamReader(path + "calendar.xml");
          Appointments = (List<Appointment>)s.Deserialize(r);        
        } catch (Exception e) {
          Logging.Instance.Error(this.ToString(), "Deserialization of Calendar failed", e);
          Logging.Instance.Info(this.ToString(), "Starting with a new one");
          appointments = new List<Appointment>();
        } finally {
          if(r!=null)
            r.Close();          
        }
      } else {
        Appointments = new List<Appointment>();
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
