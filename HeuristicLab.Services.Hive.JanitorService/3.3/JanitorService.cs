#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.ServiceProcess;
using System.Threading;

namespace HeuristicLab.Services.Hive.JanitorService {
  public partial class JanitorService : ServiceBase {
    private HiveJanitor janitor;
    private Thread janitorCleanupThread;
    private Thread janitorGenerateStatisticsThread;

    public JanitorService() {
      InitializeComponent();
    }

    protected override void OnStart(string[] args) {
      janitor = new HiveJanitor();

      janitorCleanupThread = new Thread(janitor.RunCleanup) {
        IsBackground = true
      };
      janitorGenerateStatisticsThread = new Thread(janitor.RunGenerateStatistics) {
        IsBackground = true
      };

      janitorCleanupThread.Start();
      janitorGenerateStatisticsThread.Start();
    }

    protected override void OnStop() {
      janitor.StopJanitor();
      janitorCleanupThread.Join();
      janitorGenerateStatisticsThread.Join();
    }
  }
}
