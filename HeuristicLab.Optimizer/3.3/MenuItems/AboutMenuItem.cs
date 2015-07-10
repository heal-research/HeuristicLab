#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure.Starter;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class AboutMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    private AboutDialog aboutDialog;

    public override string Name {
      get { return "&About..."; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Help" }; }
    }
    public override int Position {
      get { return 9100; }
    }

    public override void Execute() {
      if (aboutDialog == null)
        aboutDialog = new AboutDialog();
      aboutDialog.ShowDialog((IWin32Window)MainFormManager.MainForm);
    }
  }
}
