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
using System.Threading;

namespace HeuristicLab.Clients.Hive.SlaveCore.ConsoleClient {
  public class Program {
    static void Main(string[] args) {
      EventLog eventLog = null;
      try {
        if (!System.Diagnostics.EventLog.SourceExists("HLHive")) {
          System.Diagnostics.EventLog.CreateEventSource("HLHive", "HiveSlave");
        }
        eventLog = new EventLog();
        eventLog.Source = "HLHive";
        eventLog.Log = "HiveSlave";
      }
      catch (Exception) { }

      //slave part
      Core core = new Core();
      core.ServiceEventLog = eventLog;
      Console.WriteLine("Starting core ...");
      Thread coreThread = new Thread(core.Start);
      coreThread.IsBackground = true; //dont keep app alive
      coreThread.Start();
      Thread.Sleep(1000);

      //mock a slave client
      SlaveCommListener listener = new SlaveCommListener();
      listener.Open();

      Console.WriteLine("Press a key to quit");
      Console.ReadLine();
      listener.Close();
      core.Shutdown();
      Console.ReadLine();
    }
  }
}
