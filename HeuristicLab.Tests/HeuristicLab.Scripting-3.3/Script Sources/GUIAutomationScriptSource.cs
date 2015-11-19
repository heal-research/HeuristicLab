using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Views;
using HeuristicLab.Problems.TravelingSalesman;

public class GUIAutomationScript : HeuristicLab.Scripting.CSharpScriptBase {
  readonly ManualResetEvent mutex = new ManualResetEvent(false);

  public override void Main() {
    var ga = new GeneticAlgorithm {
      MaximumGenerations = { Value = 50 },
      PopulationSize = { Value = 10 },
      Problem = new TravelingSalesmanProblem()
    };

    var experiment = new Experiment();
    for (int i = 0; i < 5; i++) {
      experiment.Optimizers.Add(new BatchRun() { Optimizer = (IOptimizer)ga.Clone(), Repetitions = 10 });
      ga.PopulationSize.Value *= 2;
    }

    experiment.ExecutionStateChanged += OnExecutionStateChanged;
    experiment.Start();
    mutex.WaitOne();

    vars.experiment = experiment;
    MainFormManager.MainForm.ShowContent(experiment);
    var viewHost = (ViewHost)MainFormManager.MainForm.ShowContent(experiment.Runs, typeof(RunCollectionBubbleChartView));
    var bubbleChart = (UserControl)(viewHost.ActiveView);
    bubbleChart.Controls.OfType<ComboBox>().Single(x => x.Name == "yAxisComboBox").SelectedItem = "BestQuality";
    bubbleChart.Controls.OfType<ComboBox>().Single(x => x.Name == "xAxisComboBox").SelectedItem = "PopulationSize";
  }

  private void OnExecutionStateChanged(object sender, EventArgs e) {
    if (((IExecutable)sender).ExecutionState == ExecutionState.Stopped)
      mutex.Set();
  }
}