using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Views {
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

    protected override async void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        tabControl.TabPages.Clear();
        return;
      }
      // distinct names of results of types IRegressionSolution
      var solutionResultNames = Content.SelectMany(run => run.Results.Where(kvp => kvp.Value is IRegressionSolution).Select(kvp => kvp.Key)).Distinct().ToArray();

      foreach (var resultName in solutionResultNames) {
        var tabPage = new TabPage(resultName);
        tabControl.TabPages.Add(tabPage);

        var solutions = new List<IRegressionSolution>();
        foreach (var run in Content) {
          // in experiments we may mix algorithms and therefore have different solution names in different runs
          // we only combine solutions with the same name
          if (run.Results.TryGetValue(resultName, out var sol) && sol is IRegressionSolution) {
            solutions.Add((IRegressionSolution)sol);
          }
        }
        var plot = new RegressionSolutionPartialDependencePlotView {
          Content = solutions[0],
          Dock = DockStyle.Fill
        };

        tabPage.Controls.Add(plot);
        for (int i = 1; i < solutions.Count; i++)
          await plot.AddSolution(solutions[i]);
      }
    }
  }
}
