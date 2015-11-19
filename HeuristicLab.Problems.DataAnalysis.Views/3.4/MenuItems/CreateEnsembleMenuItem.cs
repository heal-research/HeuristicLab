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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Views;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  internal class CreateEnsembleMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Create &Solution Ensembles"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit", "&Data Analysis" }; }
    }
    public override int Position {
      get { return 5100; }
    }
    public override string ToolTipText {
      get { return "Create ensembles of data analysis solutions from the solutions in the current optimizer."; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      ToolStripItem.Enabled = GetDataAnalysisResults(activeView).Any();
    }
    protected override void OnViewChanged(object sender, EventArgs e) {
      base.OnViewChanged(sender, e);
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      ToolStripItem.Enabled = GetDataAnalysisResults(activeView).Any();
    }

    public override void Execute() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      var solutionGroups = from pair in GetDataAnalysisResults(activeView)
                           group pair.Value by pair.Key into g
                           select g;
      foreach (var group in solutionGroups) {
        // check if all solutions in the group are either only regression or only classification solutions
        if (group.All(s => s is IRegressionSolution)) {
          // show all regression ensembles
          // N.B. this assumes all solutions are based on the same problem data!
          // the problem data is not cloned because the individual solutions were already cloned
          var problemData = group.OfType<IRegressionSolution>().First().ProblemData;
          var ensemble = new RegressionEnsembleSolution(problemData);
          ensemble.Name = group.Key + " ensemble";
          var nestedSolutions = group.OfType<RegressionEnsembleSolution>().SelectMany(e => e.RegressionSolutions);
          var solutions = group.Where(s => !(s is RegressionEnsembleSolution)).OfType<IRegressionSolution>();
          ensemble.AddRegressionSolutions(nestedSolutions.Concat(solutions));
          MainFormManager.MainForm.ShowContent(ensemble);
        } else if (group.All(s => s is IClassificationSolution)) {
          // show all classification ensembles
          // N.B. this assumes all solutions are based on the same problem data!
          // the problem data is not cloned because the individual solutions were already cloned
          var problemData = (ClassificationProblemData)group.OfType<IClassificationSolution>().First().ProblemData;
          var ensemble = new ClassificationEnsembleSolution(Enumerable.Empty<IClassificationModel>(), problemData);
          ensemble.Name = group.Key + " ensemble";
          var nestedSolutions = group.OfType<ClassificationEnsembleSolution>().SelectMany(e => e.ClassificationSolutions);
          var solutions = group.Where(s => !(s is ClassificationEnsembleSolution)).OfType<IClassificationSolution>();
          ensemble.AddClassificationSolutions(nestedSolutions.Concat(solutions));
          MainFormManager.MainForm.ShowContent(ensemble);
        }
      }
    }

    private IEnumerable<KeyValuePair<string, IDataAnalysisSolution>> GetDataAnalysisResults(IContentView view) {
      var empty = Enumerable.Empty<KeyValuePair<string, IDataAnalysisSolution>>();
      if (view == null) return empty;
      if (view.Content == null) return empty;
      if (view.Locked) return empty;

      var optimizer = view.Content as IOptimizer;
      if (optimizer != null) return GetDataAnalysisResults(optimizer.Runs);

      RunCollectionBubbleChartView bubbleChart;
      var viewHost = view as ViewHost;
      if (viewHost != null)
        bubbleChart = viewHost.ActiveView as RunCollectionBubbleChartView;
      else bubbleChart = view as RunCollectionBubbleChartView;
      if (bubbleChart != null && bubbleChart.SelectedRuns.Any()) return GetDataAnalysisResults(bubbleChart.SelectedRuns);

      return empty;
    }

    private IEnumerable<KeyValuePair<string, IDataAnalysisSolution>> GetDataAnalysisResults(IEnumerable<IRun> runs) {
      var cloner = new Cloner();
      var allResults = from r in runs
                       where r.Visible
                       select r.Results;
      return from r in allResults
             from result in r
             let solution = result.Value as IDataAnalysisSolution
             where solution != null
             let s = (IDataAnalysisSolution)cloner.Clone(result.Value)
             select new KeyValuePair<string, IDataAnalysisSolution>(result.Key, s);
    }
  }
}
