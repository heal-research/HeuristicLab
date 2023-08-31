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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Optimizer;
using MenuItem = HeuristicLab.MainForm.WindowsForms.MenuItem;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  internal sealed class CopyEvaluationMetricsMenuItem : MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Copy evaluation metrics"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit", "&Data Analysis" }; }
    }
    public override int Position {
      get { return 5500; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.Alt | Keys.C; }
    }
    public override string ToolTipText {
      get { return "This copies important training and test metrics in a tabular format into the clipboard."; }
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

      ToolStripItem.Enabled = activeView.Content is IRegressionSolution;
    }

    public override void Execute() {
      var activeView = (IContentView)MainFormManager.MainForm.ActiveView;
      var solution = (IRegressionSolution)activeView.Content;

      var numFeatures = solution.Model.VariablesUsedForPrediction.Count();

      var output = $"{numFeatures} Features \t Absoluter Error \t Relative Error \t Pearson R² {Environment.NewLine}";
      output += $" Training \t {solution.TrainingMeanAbsoluteError:0.000} \t  {solution.TrainingRelativeError * 100:0.00} % \t {solution.TrainingRSquared:0.000} {Environment.NewLine}";
      output += $" Test \t {solution.TestMeanAbsoluteError:0.000} \t {solution.TestRelativeError * 100:0.00} % \t {solution.TestRSquared:0.000} {Environment.NewLine}";
      Clipboard.SetText(output);
    }
  }
}
