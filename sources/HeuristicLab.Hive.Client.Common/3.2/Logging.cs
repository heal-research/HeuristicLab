#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HeuristicLab.Hive.Client.Common {
  /// <summary>
  /// The Logging class uses the Windows Event logging mechanism. It writes the logs to a custom log 
  /// called "Hive Client". For the creation of the Hive Client Log the program must be executed with
  /// Administrator privileges   
  /// </summary>
  public class Logging {      
    private static Logging instance = null;
    private EventLog eventLogger = null;

    /// <summary>
    /// This is an implementation of the singleton design pattern.
    /// </summary>
    /// <returns>the instance of the logger</returns>
    public static Logging Instance { 
      get {
        if (instance == null)
         instance = new Logging();
        return instance;
      } 
      set {}
    } 
    
    static Logging GetInstance() {
      if (instance == null)
        instance = new Logging();
      return instance;
    }

    private Logging() {      
      eventLogger = new EventLog("Hive Client");          
    }

    /// <summary>
    /// Writes an Info Log-Entry
    /// </summary>
    /// <param name="source">string representation of the caller</param>
    /// <param name="message">the message</param>
    public void Info(String source, String message) {
      eventLogger.Source = source;
      eventLogger.WriteEntry(message);      
      eventLogger.Close();
    }

    /// <summary>
    /// Writes an Error Log-Entry
    /// </summary>
    /// <param name="source">string representation of the caller</param>
    /// <param name="message">the message</param>
    public void Error(String source, String message) {
      eventLogger.Source = source;
      eventLogger.WriteEntry(message, EventLogEntryType.Error);
      eventLogger.Close();
    }

    /// <summary>
    /// Writes an Error Log-Entry
    /// </summary>
    /// <param name="source">string representation of the caller</param>
    /// <param name="message">the message</param>
    /// <param name="e">the exception</param>
    public void Error(String source, String message, Exception e) {
      eventLogger.Source = source;
      eventLogger.WriteEntry(message +"\n" + e.ToString(), EventLogEntryType.Error);
      eventLogger.Close();
    }
  }
}
