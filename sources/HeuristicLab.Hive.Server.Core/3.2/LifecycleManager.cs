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
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Server.DataAccess;
using System.Timers;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.Hive.Server.Core {
  class LifecycleManager: ILifecycleManager {
    private static Timer timer =
      new Timer();

    private static event EventHandler OnServerHeartbeat;
    private static event EventHandler OnStartup;
    private static event EventHandler OnShutdown;
    #region ILifecycleManager Members

    public void RegisterHeartbeat(EventHandler handler) {
      OnServerHeartbeat += handler;
    }

    public void RegisterStartup(EventHandler handler) {
      OnStartup += handler;
    }

    public void RegisterShutdown(EventHandler handler) {
      OnShutdown += handler;
    }

    public void Init() {
      timer.Interval = new TimeSpan(0, 0, 10).TotalMilliseconds; // TODO: global constant needed
      timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
      timer.Start();

      if (OnStartup != null)
        OnStartup(this, null);
    }

    void timer_Elapsed(object sender, ElapsedEventArgs e) {
      if (OnServerHeartbeat != null)
        OnServerHeartbeat(this, null);
    }

    public void Shutdown() {
      if (OnShutdown != null)
        OnShutdown(this, null);
    }

    #endregion
  }
}
