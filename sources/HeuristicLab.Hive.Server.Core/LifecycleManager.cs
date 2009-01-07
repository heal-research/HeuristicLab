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
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using System.Timers;

namespace HeuristicLab.Hive.Server.Core {
  class LifecycleManager: ILifecycleManager {
    private static Timer timer =
      new Timer();

    #region ILifecycleManager Members
    public event EventHandler OnServerHeartbeat;

    public LifecycleManager() {
      timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
    }

    public void Init() {
      timer.Interval = new TimeSpan(0, 0, 10).TotalMilliseconds; // TODO: global constant needed
      timer.Start();
    }

    void timer_Elapsed(object sender, ElapsedEventArgs e) {
      if (OnServerHeartbeat != null)
        OnServerHeartbeat(this, null);
    }

    public ITransactionManager GetTransactionManager() {
      return ServiceLocator.GetTransactionManager();
    }

    public void Shutdown() {
      ServiceLocator.GetTransactionManager().UpdateDB();
    }

    #endregion
  }
}
