#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Services.Hive.DataAccess;

namespace HeuristicLab.Services.Hive {
  public class HiveJanitor {
    private bool stop;
    private AutoResetEvent waitHandle;

    private DataAccess.ITransactionManager trans {
      get { return ServiceLocator.Instance.TransactionManager; }
    }

    private IEventManager eventManager {
      get { return ServiceLocator.Instance.EventManager; }
    }

    private IHiveDao dao {
      get { return ServiceLocator.Instance.HiveDao; }
    }

    public HiveJanitor() {
      stop = false;
      waitHandle = new AutoResetEvent(true);
    }

    public void StopJanitor() {
      stop = true;
      waitHandle.Set();
    }

    public void Run() {
      while (!stop) {
        try {
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: starting cleanup");
          bool cleanup = false;
          trans.UseTransaction(() => {
            DateTime lastCleanup = dao.GetLastCleanup();
            if (DateTime.Now - lastCleanup > HeuristicLab.Services.Hive.Properties.Settings.Default.CleanupInterval) {
              dao.SetLastCleanup(DateTime.Now);
              cleanup = true;
            }
          }, true);

          if (cleanup) {
            eventManager.Cleanup();
          }
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: cleanup finished");
        }
        catch (Exception e) {
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log(string.Format("HiveJanitor: The following exception occured: {0}", e.ToString()));
        }
        waitHandle.WaitOne(HeuristicLab.Services.Hive.Properties.Settings.Default.CleanupInterval);
      }
      waitHandle.Close();
    }
  }
}



