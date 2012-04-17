#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Problems.DataAnalysis.MenuItems {
  internal class CreateEnsembleMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Create &Solution Ensembles"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit" }; }
    }
    public override int Position {
      get { return 2500; }
    }
    public override string ToolTipText {
      get { return "Create ensembles of data analysis solutions from the solutions in the current optimizer."; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if ((activeView != null) && (activeView.Content != null) && (activeView.Content is IOptimizer) && !activeView.Locked) {
        var optimizer = activeView.Content as IOptimizer;
        ToolStripItem.Enabled = GetDataAnalysisResults(optimizer).Any();
      } else {
        ToolStripItem.Enabled = false;
      }
    }

    public override void Execute() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if ((activeView != null) && (activeView.Content != null) && (activeView.Content is IOptimizer) && !activeView.Locked) {
        var optimizer = activeView.Content as IOptimizer;
        var solutionGroups = from pair in GetDataAnalysisResults(optimizer)
                             group pair.Value by pair.Key into g
                             select g;
        foreach (var group in solutionGroups) {
          // check if all solutions in the group are either only regression or only classification solutions
          if (group.All(s => s is IRegressionSolution)) {
            // show all regression ensembles
            // clone problemdata (N.B. this assumes all solutions are based on the same problem data!)
            var problemData = (RegressionProblemData)group
              .OfType<IRegressionSolution>()
              .First()
              .ProblemData.Clone();
            var ensemble = new RegressionEnsembleSolution(problemData);
            ensemble.Name = group.Key + " ensemble";
            var nestedSolutions = group.OfType<RegressionEnsembleSolution>().SelectMany(e => e.RegressionSolutions);
            var solutions = group.Where(s => !(s is RegressionEnsembleSolution)).OfType<IRegressionSolution>();
            ensemble.AddRegressionSolutions(nestedSolutions.Concat(solutions));
            MainFormManager.MainForm.ShowContent(ensemble);
          } else if (group.All(s => s is IClassificationSolution)) {
            // show all classification ensembles
            var problemData = (ClassificationProblemData)group
              .OfType<IClassificationSolution>()
              .First()
              .ProblemData.Clone();
            var ensemble = new ClassificationEnsembleSolution(Enumerable.Empty<IClassificationModel>(), problemData);
            ensemble.Name = group.Key + " ensemble";
            var nestedSolutions = group.OfType<ClassificationEnsembleSolution>().SelectMany(e => e.ClassificationSolutions);
            var solutions = group.Where(s => !(s is ClassificationEnsembleSolution)).OfType<IClassificationSolution>();
            ensemble.AddClassificationSolutions(nestedSolutions.Concat(solutions));
            MainFormManager.MainForm.ShowContent(ensemble);
          }
        }
      }
    }

    private IEnumerable<KeyValuePair<string, IItem>> GetDataAnalysisResults(IOptimizer optimizer) {
      var allResults = from r in optimizer.Runs
                       select r.Results;
      return from r in allResults
             from result in r
             let s = result.Value as IDataAnalysisSolution
             where s != null
             select result;
    }
  }
}
