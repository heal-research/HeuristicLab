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
using System.Text;
using System.Timers;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.DataAccess {
  class DBSynchronizer: IDBSynchronizer {
    private Timer timer =
      new Timer();

    void timer_Elapsed(object sender, ElapsedEventArgs e) {
      UpdateDB();
    }

    #region IDBSynchronizer Members
    public event EventHandler OnUpdate;

    public void EnableAutoUpdate(TimeSpan interval) {
      timer.Interval = interval.TotalMilliseconds;
      timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
      timer.Start();
    }

    public void DisableAutoUpdate() {
      timer.Stop();
    }

    public void UpdateDB() {
      if (OnUpdate != null)
        OnUpdate(this, null);
    }

    #endregion
  }
}
