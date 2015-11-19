#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Security;
using System.Threading;

namespace HeuristicLab.Services.Hive.DataAccess {
  public class LogFactory {
    public static ILogger GetLogger(string source) {
      //ignore the source parameter for now
      return new Logger(source, Settings.Default.EventLogName);
    }
  }

  public interface ILogger {
    void Log(string message);
    void Error(Exception e);
  }

  internal class Logger : ILogger {
    private EventLog log;

    /// <summary>
    /// Creating an EventSource requires certain permissions, which by default a IIS AppPool user does not have.
    /// In this case just ignore security exceptions.
    /// In order to make this work, the eventsource needs to be created manually
    /// </summary>
    public Logger(string name, string source) {
      try {
        log = new EventLog();
        log.Source = source;
      }
      catch (Exception) { }
    }

    public void Log(string message) {
      try {
        if (log != null) {
          log.WriteEntry(string.Format("{0} (AppDomain: {1}, Thread: {2})", message, AppDomain.CurrentDomain.Id, Thread.CurrentThread.ManagedThreadId), EventLogEntryType.Information);
        }
      }
      catch (Exception) { }
    }

    public void Error(Exception e) {
      try {
        if (log != null) {
          log.WriteEntry(string.Format("{0} (AppDomain: {1}, Thread: {2})", e.Message, AppDomain.CurrentDomain.Id, Thread.CurrentThread.ManagedThreadId), EventLogEntryType.Error);
        }
      }
      catch (Exception) { }
    }
  }
}