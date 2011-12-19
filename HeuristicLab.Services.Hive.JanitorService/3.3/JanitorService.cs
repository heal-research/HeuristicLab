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

using System.ServiceProcess;
using System.Threading;

namespace HeuristicLab.Services.Hive.JanitorService {
  public partial class JanitorService : ServiceBase {
    private HiveJanitor janitor;
    private Thread janitorThread;

    public JanitorService() {
      InitializeComponent();
    }

    protected override void OnStart(string[] args) {
      janitor = new HiveJanitor();

      janitorThread = new Thread(janitor.Run);
      janitorThread.IsBackground = true; //dont keep app alive
      janitorThread.Start();
    }

    protected override void OnStop() {
      janitor.StopJanitor();
      janitorThread.Join();
    }
  }
}
