using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Console {
  public class HiveEventEntry {
    public string Message { get; private set; }
    public string EventDate { get; private set; }
    public string EventTime { get; private set; }
    public string ID { get; private set; }

    public HiveEventEntry(string message, string eventDate, string eventTime, string id) {
      Message = message;
      EventDate = eventDate;
      EventTime = eventTime;
      ID = id;
    }
  }
}
