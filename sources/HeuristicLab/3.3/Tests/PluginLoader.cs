
namespace HeuristicLab_3._3.Tests {
  internal static class PluginLoader {
    public static void LoadPluginsIntoAppDomain() {
      new HeuristicLab.Analysis.Views.HeuristicLabAnalysisViewsPlugin();
      new HeuristicLab.Core.Views.HeuristicLabCoreViewsPlugin();
      new HeuristicLab.Data.Views.HeuristicLabDataViewsPlugin();
      new HeuristicLab.Encodings.PermutationEncoding.Views.HeuristicLabEncodingsPermutationEncodingViewsPlugin();
      new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.HeuristicLabEncodingsSymbolicExpressionTreeEncodingViewsPlugin();
      new HeuristicLab.MainForm.HeuristicLabMainFormPlugin();
      new HeuristicLab.MainForm.WindowsForms.HeuristicLabMainFormPlugin();
      new HeuristicLab.Operators.Views.HeuristicLabOperatorsViewsPlugin();
      new HeuristicLab.Operators.Views.GraphVisualization.HeuristicLabOperatorsViewsGraphVisualizationPlugin();
      new HeuristicLab.Parameters.Views.HeuristicLabParametersViewsPlugin();
      new HeuristicLab.Problems.ArtificialAnt.Views.HeuristicLabProblemsArtificialAntViewsPlugin();
      new HeuristicLab.Problems.DataAnalysis.Views.HeuristicLabProblemsDataAnalysisViewsPlugin();
      new HeuristicLab.Problems.ExternalEvaluation.Views.HeuristicLabProblemsExternalEvaluationViewsPlugin();
      new HeuristicLab.Problems.Knapsack.Views.HeuristicLabProblemsKnapsackViewsPlugin();
      new HeuristicLab.Problems.OneMax.Views.HeuristicLabProblemsKnapsackViewsPlugin();
      new HeuristicLab.Problems.TestFunctions.Views.HeuristicLabProblemsTestFunctionsViewsPlugin();
      new HeuristicLab.Problems.TravelingSalesman.Views.HeuristicLabProblemsTravelingSalesmanViewsPlugin();
      new HeuristicLab.Problems.VehicleRouting.Views.HeuristicLabProblemsVehicleRoutingViewsPlugin();
    }
  }
}
