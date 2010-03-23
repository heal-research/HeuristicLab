using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendar;
using System.Xml.Serialization;
using System.IO;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Client.Core.ConfigurationManager {
  public class UptimeManager {

    private AppointmentContainer _appContainer = null;
    public AppointmentContainer AppContainer {
      get {
        if (_appContainer == null)
          RestoreFromHDD();
        return _appContainer;
      }
    }    
    public bool CalendarAvailable { get; set; }

    private static String path = System.IO.Directory.GetCurrentDirectory() + "\\plugins\\Hive.Client.Jobs\\";

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
      XmlSerializer s = new XmlSerializer(typeof(AppointmentContainer));
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      TextWriter w = null;
      try {
        w = new StreamWriter(path + "calendar.xml");
        s.Serialize(w, AppContainer);
      }
      catch (Exception e) {
        Logging.Instance.Error(this.ToString(), "Persistance of the Calendar failed!", e);
      }
      finally {
        if (w != null)
          w.Close();
      }
    }

    private void RestoreFromHDD() {
      XmlSerializer s = new XmlSerializer(typeof(AppointmentContainer));
      if (File.Exists(Path.Combine(path, "calendar.xml"))) {
        TextReader r = null;

        try {
          r = new StreamReader(path + "calendar.xml");
          _appContainer = (AppointmentContainer)s.Deserialize(r);
          CalendarAvailable = true;
        }
        catch (Exception e) {
          Logging.Instance.Error(this.ToString(), "Deserialization of Calendar failed", e);
          Logging.Instance.Info(this.ToString(), "Starting with a new one");
          _appContainer = new AppointmentContainer();
          CalendarAvailable = false;
        }
        finally {
          if (r != null)
            r.Close();
        }
      } else {
        _appContainer = new AppointmentContainer();
      }
    }

    public bool IsOnline() {
      return AppContainer.Appointments.Any(app => (DateTime.Now >= app.StartDate) && (DateTime.Now <= app.EndDate));
    }

    public bool SetAppointments(bool isLocal, bool isForced, IEnumerable<Appointment> appointments) {
      if (!isForced && !isLocal && AppContainer.IsLocal)
        return false;

      AppContainer.Appointments = new List<Appointment>(appointments);
      AppContainer.IsLocal = isLocal;
      AppContainer.Updated = DateTime.Now;
      CalendarAvailable = true;
     
      PersistToHDD();
      
      return true;
    }

    internal bool SetAppointments(bool isLocal, ResponseCalendar response) {
      IList<Appointment> app = new List<Appointment>();
      foreach (AppointmentDto appointmentDto in response.Appointments) {
        app.Add(new Appointment {
          AllDayEvent = appointmentDto.AllDayEvent,
          EndDate = appointmentDto.EndDate,
          StartDate = appointmentDto.StartDate,
          Recurring = appointmentDto.Recurring,

          RecurringId = appointmentDto.RecurringId,
          Locked = true,
          Subject = "Online",
        });
      }
      return SetAppointments(isLocal, response.ForceFetch, app);
    }
  }
}
