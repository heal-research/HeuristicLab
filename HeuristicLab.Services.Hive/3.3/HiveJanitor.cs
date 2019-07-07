#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Services.Hive.DataAccess.Interfaces;

namespace HeuristicLab.Services.Hive {
  public class HiveJanitor {
    private bool stop;
    private AutoResetEvent cleanupWaitHandle;
    private AutoResetEvent generateStatisticsWaitHandle;

    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }
    private IEventManager EventManager {
      get { return ServiceLocator.Instance.EventManager; }
    }

    private IStatisticsGenerator StatisticsGenerator {
      get { return ServiceLocator.Instance.StatisticsGenerator; }
    }

    public HiveJanitor() {
      stop = false;
      cleanupWaitHandle = new AutoResetEvent(false);
      generateStatisticsWaitHandle = new AutoResetEvent(false);
    }

    public void StopJanitor() {
      stop = true;
      cleanupWaitHandle.Set();
      generateStatisticsWaitHandle.Set();
    }

    public void RunCleanup() {
      var pm = PersistenceManager;
      while (!stop) {
        try {
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: starting cleanup.");
          bool cleanup = false;

          var lifecycleDao = pm.LifecycleDao;
          pm.UseTransaction(() => {
            var lifecycle = lifecycleDao.GetLastLifecycle();
            if (lifecycle == null
                || DateTime.Now - lifecycle.LastCleanup > Properties.Settings.Default.CleanupInterval) {
              lifecycleDao.UpdateLifecycle();
              cleanup = true;
            }
            pm.SubmitChanges();
          }, true);

          if (cleanup) {
            EventManager.Cleanup();
          }
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: cleanup finished.");
        }
        catch (Exception e) {
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log(string.Format("HiveJanitor: The following exception occured: {0}", e.ToString()));
        }
        cleanupWaitHandle.WaitOne(Properties.Settings.Default.CleanupInterval);
      }
      cleanupWaitHandle.Close();
    }

    public void RunGenerateStatistics() {
      while (!stop) {
        try {
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: starting generate statistics.");
          StatisticsGenerator.GenerateStatistics();
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: generate statistics finished.");
        }
        catch (Exception e) {
          LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log(string.Format("HiveJanitor: The following exception occured: {0}", e));
        }

        generateStatisticsWaitHandle.WaitOne(Properties.Settings.Default.GenerateStatisticsInterval);
      }

      generateStatisticsWaitHandle.Close();
    }
  }
}



