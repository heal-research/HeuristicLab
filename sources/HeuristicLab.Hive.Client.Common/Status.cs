using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Common {
  public static class Status {
    public static int CurrentJobs { get; set; }
    public static int CurrentUsedCores { get; set; }
    public static DateTime LoginTime { get; set; }
    public static bool LoggedIn { get; set; }

    static Status() {
      CurrentJobs = 0;
      CurrentUsedCores = 0;
      LoggedIn = false;
    }
  }
}
