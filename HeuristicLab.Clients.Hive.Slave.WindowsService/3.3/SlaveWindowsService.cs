#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ServiceProcess;
using System.Threading;

namespace HeuristicLab.Clients.Hive.SlaveCore.WindowsService {
  public partial class SlaveWindowsService : ServiceBase {
    private Core core;
    private Thread coreThread;

    public SlaveWindowsService() {
      InitializeComponent();

      try {
        if (!System.Diagnostics.EventLog.SourceExists("HLHive")) {
          System.Diagnostics.EventLog.CreateEventSource("HLHive", "HiveSlave");
        }
        eventLog.Source = "HLHive";
        eventLog.Log = "HiveSlave";
      }
      catch (Exception) { }
    }

    protected override void OnStart(string[] args) {
      core = new Core();
      core.ServiceEventLog = eventLog;
      coreThread = new Thread(core.Start);
      coreThread.IsBackground = true; //dont keep app alive
      coreThread.Start();

      try {
        eventLog.WriteEntry("HeuristicLab Hive Slave started!");
      }
      catch (Exception) { }
    }

    protected override void OnStop() {
      try {
        eventLog.WriteEntry("Shutting down HeuristicLab Hive Slave...");
      }
      catch (Exception) { }

      core.Shutdown();

      try {
        eventLog.WriteEntry("HeuristicLab Hive Slave stopped!");
      }
      catch (Exception) { }
    }
  }
}
