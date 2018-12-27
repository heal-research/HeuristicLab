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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimizer;
using MenuItem = HeuristicLab.MainForm.WindowsForms.MenuItem;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  internal sealed class ChangeDataOfOptimizersMenuItem : MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Change problem data"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit", "&Data Analysis" }; }
    }
    public override int Position {
      get { return 5400; }
    }
    public override string ToolTipText {
      get { return "This command changes the problem data of all optimizers in the active view."; }
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

      var optimizer = activeView.Content as IOptimizer;
      if (optimizer == null) {
        ToolStripItem.Enabled = false;
        return;
      }

      ToolStripItem.Enabled = true;
    }

    public override void Execute() {
      using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
        openFileDialog.Title = "Open Problem file";
        openFileDialog.FileName = "Item";
        openFileDialog.Multiselect = false;
        openFileDialog.DefaultExt = "hl";
        openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";

        if (openFileDialog.ShowDialog() != DialogResult.OK) return;

        var content = ContentManager.Load(openFileDialog.FileName);
        var problem = content as IDataAnalysisProblem;
        IDataAnalysisProblemData problemData = null;
        if (problem != null) {
          problemData = problem.ProblemData;
        } else {
          problemData = content as IDataAnalysisProblemData;
        }
        if (problemData == null) throw new ArgumentException("The specified file does not contain a HeuristicLab problem.");

        var activeView = (IContentView)MainFormManager.MainForm.ActiveView;
        var optimizer = (IOptimizer)activeView.Content;
        var newOptimizer = (IOptimizer)optimizer.Clone();

        var algorithm = newOptimizer as IAlgorithm;
        if (algorithm != null) {
          ChangeProblemData(algorithm, problemData);
        }
        foreach (var alg in newOptimizer.NestedOptimizers.OfType<IAlgorithm>()) {
          ChangeProblemData(alg, (IDataAnalysisProblemData)problemData.Clone());
          if (problem != null) alg.Name += " " + problem.Name;
        }

        MainFormManager.MainForm.ShowContent(newOptimizer);
      }

    }

    private void ChangeProblemData(IAlgorithm algorithm, IDataAnalysisProblemData problemData) {
      if (algorithm == null) throw new ArgumentNullException("algorithm");
      if (problemData == null) throw new ArgumentNullException("problemData");

      var problem = algorithm.Problem as IDataAnalysisProblem;
      if (problem != null) problem.ProblemDataParameter.ActualValue = problemData;
    }
  }
}
