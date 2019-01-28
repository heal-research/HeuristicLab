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

using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class ChangeNestingLevelMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Change &Nesting Level..."; }
    }
    public override IEnumerable<string> Structure {
      get { return new[] { "&View" }; }
    }
    public override int Position {
      get { return 4000; }
    }
    public override string ToolTipText {
      get { return "Change the maximum nesting level of controls."; }
    }
    public override void Execute() {
      using (var dialog = new ChangeNestingLevelDialog()) {
        var mainForm = MainFormManager.MainForm as IWin32Window;
        if (mainForm != null)
          dialog.ShowDialog(mainForm);
        else
          dialog.ShowDialog();
      }
    }
  }
}
