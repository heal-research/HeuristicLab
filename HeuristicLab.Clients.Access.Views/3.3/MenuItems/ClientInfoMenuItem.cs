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

using System.Collections.Generic;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Clients.Access.Views {
  public class ClientInfoMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Client Information"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Services", "&Access" }; }
    }
    public override void Execute() {
      using (ClientInformationDialog dialog = new ClientInformationDialog()) {
        dialog.ShowDialog();
      }
    }
    public override int Position {
      get { return 10000; }
    }
  }
}
