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
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Clients.Hive.JobManager {
  public class CreateHiveJobMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Create Hive Job"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Services", "&Hive" }; }
    }

    public override int Position {
      get { return 10001; }
    }

    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;

      if (activeView != null && activeView.Content != null && !activeView.Locked) {
        var content = activeView.Content as IItem;
        if (content != null) {
          Type contentType = content.GetType();
          ToolStripItem.Enabled = ItemTask.IsTypeSupported(contentType);
          return;
        }
      }
      ToolStripItem.Enabled = false;
    }

    public override void Execute() {
      IContentView activeView = (IContentView)MainFormManager.MainForm.ActiveView;
      var content = (IItem)activeView.Content;
      var clonedConent = (IItem)content.Clone();

      //IOptimizer and IExecutables need some special care
      if (clonedConent is IOptimizer) {
        ((IOptimizer)clonedConent).Runs.Clear();
      }
      if (clonedConent is IExecutable) {
        IExecutable exec = clonedConent as IExecutable;
        if (exec.ExecutionState != ExecutionState.Prepared) {
          exec.Prepare();
        }
      }

      HiveClient.Instance.Refresh();

      ItemTask hiveTask = ItemTask.GetItemTaskForItem(clonedConent);
      HiveTask task = hiveTask.CreateHiveTask();
      RefreshableJob rJob = new RefreshableJob();
      rJob.Job.Name = clonedConent.ToString();
      rJob.HiveTasks.Add(task);
      task.ItemTask.ComputeInParallel = clonedConent is Experiment || clonedConent is BatchRun;

      MainFormManager.MainForm.ShowContent(rJob);
    }
  }
}
