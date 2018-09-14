#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class CreateExperimentMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Create &Experiment"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit" }; }
    }
    public override int Position {
      get { return 2300; }
    }
    public override string ToolTipText {
      get { return "Create an experiment for the shown optimizer"; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      ToolStripItem.Enabled = (activeView != null) && (activeView.Content != null) && (activeView.Content is IOptimizer) && !activeView.Locked;
    }

    public override void Execute() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if ((activeView != null) && (activeView.Content != null) && (activeView.Content is IOptimizer) && !activeView.Locked) {
        using (CreateExperimentDialog dialog = new CreateExperimentDialog((IOptimizer)activeView.Content)) {
          if (dialog.ShowDialog() == DialogResult.OK) MainFormManager.MainForm.ShowContent(dialog.Experiment);
        }
      }
    }
  }
}
