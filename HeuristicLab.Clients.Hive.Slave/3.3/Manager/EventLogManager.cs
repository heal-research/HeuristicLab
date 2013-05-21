#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Diagnostics;
namespace HeuristicLab.Clients.Hive.SlaveCore {
  public static class EventLogManager {
    public static EventLog ServiceEventLog { get; set; }

    public static void LogMessage(string message) {
      if (ServiceEventLog != null) {
        try {
          ServiceEventLog.WriteEntry(string.Format("Hive Slave: {0} ", message), EventLogEntryType.Error);
        }
        catch { }
      }
    }

    public static void LogException(Exception ex) {
      if (ServiceEventLog != null) {
        try {
          ServiceEventLog.WriteEntry(string.Format("Hive Slave threw exception: {0} with stack trace: {1}", ex.ToString(), ex.StackTrace), EventLogEntryType.Error);
        }
        catch { }
      }
    }
  }
}
