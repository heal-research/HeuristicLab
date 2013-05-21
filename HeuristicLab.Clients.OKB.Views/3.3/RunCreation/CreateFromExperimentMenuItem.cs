#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Clients.OKB.RunCreation {
  internal class CreateFromExperimentMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Upload Runs from Experiment"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Services", "&OKB" }; }
    }
    public override int Position {
      get { return 4220; }
    }

    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      ToolStripItem.Enabled = (activeView != null) && (activeView.Content != null) && ((activeView.Content is Experiment) || (activeView.Content is RunCollection) || (activeView.Content is IOptimizer)) && !activeView.Locked && OKBRoles.CheckUserPermissions();
    }

    public override void Execute() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      MainFormManager.MainForm.ShowContent(activeView.Content, typeof(OKBExperimentUploadView));
    }
  }
}
