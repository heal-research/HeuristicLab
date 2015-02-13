#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Clients.Hive.JobManager {
  public class JobManagerMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Job Manager"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Services", "&Hive" }; }
    }
    public override void Execute() {
      MainFormManager.MainForm.ShowContent(HiveClient.Instance);
    }
    public override int Position {
      get { return 10000; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.H; }
    }
  }
}
