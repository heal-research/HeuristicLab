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
using System.Threading.Tasks;
using HeuristicLab.Core;
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

      //The ToolStripItem is Disabled by default.
      //If any of the following conditions apply, a Check for the user privilege can be omitted. 
      ToolStripItem.Enabled = false;
      if (activeView == null) { return; }
      if (activeView.Content == null) { return; }
      if (!((activeView.Content is Experiment)
        || (activeView.Content is RunCollection)
        || (activeView.Content is IOptimizer))) { return; }
      if (activeView.Locked) { return; }

      //Check if the user has the required OKB permissions.
      //In case of an server outage, a timeout may occur and the call takes a long time.
      //To prevent a possible UI-freeze, the permission-check is implemented as async.
      CheckPrivilege();
    }

    private async void CheckPrivilege() {
      await Task.Run(() => {
        IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
        ToolStripItem.Enabled = true;//OKBRoles.CheckUserPermissions();
      });
    }

    public override void Execute() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      var view = new OKBExperimentUploadView();
      view.AddRuns((IItem)activeView.Content);
      view.Show();
    }
  }
}
