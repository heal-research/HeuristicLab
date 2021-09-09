using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [Content(typeof(RunCollection), false)]
  [View("RunCollection Partial Dependence Plots")]
  public partial class RunCollectionPartialDependencePlotView : AsynchronousContentView {
    public new RunCollection Content {
      get => (RunCollection)base.Content;
      set => base.Content = value;
    }

    public RunCollectionPartialDependencePlotView() {
      InitializeComponent();
    }

    private static async void AddSolution(RegressionSolutionPartialDependencePlotView plot, IEnumerable<IRegressionSolution> solutions) {
      foreach(var sol in solutions) {
        await plot.AddSolution(sol);
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) return;

      var solutions = Content.Select(run => (IRegressionSolution)run.Results["Best training solution"]).ToList();
      var plot = new RegressionSolutionPartialDependencePlotView {
        Content = solutions[0],
        Dock = DockStyle.Fill
      };

      mainPanel.Controls.Add(plot);
      AddSolution(plot, solutions);
    }
  }
}
