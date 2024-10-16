﻿#region License Information

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  internal sealed class ShrinkDataAnalysisRunsMenuItem : MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Remove Duplicate Datasets"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit", "&Data Analysis" }; }
    }
    public override int Position {
      get { return 5300; }
    }
    public override string ToolTipText {
      get { return "This command shrinks the memory usage of data analysis optimizers by checking and unifying duplicate datasets."; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }

    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView == null) {
        ToolStripItem.Enabled = false;
        return;
      }

      var content = activeView.Content;
      RunCollection runCollection = content as RunCollection;
      if (runCollection == null && content is IOptimizer)
        runCollection = ((IOptimizer)content).Runs;

      if (runCollection == null) {
        ToolStripItem.Enabled = false;
        return;
      }

      ToolStripItem.Enabled = runCollection.Any(run => run.Parameters.Any(p => p.Value is IDataAnalysisProblemData));
    }

    public override async void Execute() {
      IContentView activeView = (IContentView)MainFormManager.MainForm.ActiveView;
      var content = activeView.Content;
      
      Progress.Show(content, "Removing duplicate datasets.", ProgressMode.Indeterminate);
      try {
        await Task.Run(() => DatasetUtil.RemoveDuplicateDatasets(activeView.Content));
      } finally {
        Progress.Hide(content);
      }
    }
  }
}
