using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HeuristicLab.Hive.Client.Common {
  public class Logging {      
    private static Logging instance = null;
    private EventLog eventLogger = null;

    public static Logging getInstance() {
      if (instance == null)
        instance = new Logging();
      return instance;
    }

    private Logging() {
      eventLogger = new EventLog("Hive Client Core");    
    }

    public void Info(String source, String message) {
      eventLogger.Source = source;
      eventLogger.WriteEntry(message);      
      eventLogger.Close();
    }

    public void Error(String source, String message) {
      eventLogger.Source = source;
      eventLogger.WriteEntry(message, EventLogEntryType.Error);
      eventLogger.Close();
    }

    public void Error(String source, String message, Exception e) {
      eventLogger.Source = source;
      eventLogger.WriteEntry(message +"\n" + e.ToString(), EventLogEntryType.Error);
      eventLogger.Close();
    }
  }
}
