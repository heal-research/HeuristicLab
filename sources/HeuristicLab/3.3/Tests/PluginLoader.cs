
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab_3._3.Tests {
  internal static class PluginLoader {
    public const string AssemblyExtension = ".dll";
    public static List<Assembly> pluginAssemblies;

    static PluginLoader() {
      foreach (string path in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory)
        .Where(s => s.EndsWith(AssemblyExtension)))
        Assembly.LoadFrom(path);

      pluginAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(IsPluginAssembly).ToList();
    }

    private static bool IsPluginAssembly(Assembly assembly) {
      return assembly.GetExportedTypes()
        .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface).Any();
    }


    //public static void LoadPluginsIntoAppDomain() {
    //  new HeuristicLab.Algorithms.DataAnalysis.HeuristicLabAlgorithmsDataAnalysisPlugin();
    //  new HeuristicLab.Algorithms.EvolutionStrategy.HeuristicLabAlgorithmsEvolutionStrategyPlugin();
    //  new HeuristicLab.Algorithms.GeneticAlgorithm.HeuristicLabAlgorithmsGeneticAlgorithmPlugin();
    //  new HeuristicLab.Algorithms.LocalSearch.HeuristicLabAlgorithmsLocalSearchPlugin();
    //  new HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm.HeuristicLabAlgorithmsOffspringSelectionGeneticAlgorithmPlugin();
    //  new HeuristicLab.Algorithms.SimulatedAnnealing.HeuristicLabAlgorithmsSimulatedAnnealingPlugin();
    //  new HeuristicLab.Algorithms.TabuSearch.HeuristicLabAlgorithmsTabuSearchPlugin();

    //  new HeuristicLab.Analysis.HeuristicLabAnalysisPlugin();
    //  new HeuristicLab.Analysis.Views.HeuristicLabAnalysisViewsPlugin();

    //  new HeuristicLab.Clients.Common.HeuristicLabClientsCommonPlugin();
    //  new HeuristicLab.CodeEditor.HeuristicLabCodeEditorPlugin();
    //  new HeuristicLab.Collections.HeuristicLabCollectionsPlugin();

    //  new HeuristicLab.Common.HeuristicLabCommonPlugin();
    //  new HeuristicLab.Common.Resources.HeuristicLabCommonResourcesPlugin();

    //  new HeuristicLab.Core.HeuristicLabCorePlugin();
    //  new HeuristicLab.Core.Views.HeuristicLabCoreViewsPlugin();

    //  new HeuristicLab.Data.HeuristicLabDataPlugin();
    //  new HeuristicLab.Data.Views.HeuristicLabDataViewsPlugin();

    //  new HeuristicLab.Encodings.BinaryVectorEncoding.HeuristicLabEncodingsBinaryVectorEncodingPlugin();
    //  new HeuristicLab.Encodings.IntegerVectorEncoding.HeuristicLabEncodingsIntegerVectorEncodingPlugin();
    //  new HeuristicLab.Encodings.PermutationEncoding.HeuristicLabEncodingsPermutationEncodingPlugin();
    //  new HeuristicLab.Encodings.PermutationEncoding.Views.HeuristicLabEncodingsPermutationEncodingViewsPlugin();
    //  new HeuristicLab.Encodings.RealVectorEncoding.HeuristicLabEncodingsRealVectorEncodingPlugin();
    //  new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.HeuristicLabEncodingsSymbolicExpressionTreeEncodingPlugin();
    //  new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.HeuristicLabEncodingsSymbolicExpressionTreeEncodingViewsPlugin();

    //  new HeuristicLab.MainForm.HeuristicLabMainFormPlugin();
    //  new HeuristicLab.MainForm.WindowsForms.HeuristicLabMainFormPlugin();

    //  new HeuristicLab.Operators.HeuristicLabOperatorsPlugin();
    //  new HeuristicLab.Operators.Programmable.HeuristicLabOperatorsProgrammablePlugin();
    //  new HeuristicLab.Operators.Views.HeuristicLabOperatorsViewsPlugin();
    //  new HeuristicLab.Operators.Views.GraphVisualization.HeuristicLabOperatorsViewsGraphVisualizationPlugin();

    //  new HeuristicLab.Parameters.HeuristicLabParametersPlugin();
    //  new HeuristicLab.Parameters.Views.HeuristicLabParametersViewsPlugin();

    //  new HeuristicLab.Persistence.HeuristicLabPersistencePlugin();
    //  new HeuristicLab.Persistence.GUI.HeuristicLabPersistenceGUIPlugin();

    //  new HeuristicLab.Problems.ArtificialAnt.HeuristicLabProblemsArtificialAntPlugin();
    //  new HeuristicLab.Problems.ArtificialAnt.Views.HeuristicLabProblemsArtificialAntViewsPlugin();
    //  new HeuristicLab.Problems.DataAnalysis.HeuristicLabProblemsDataAnalysisPlugin();
    //  new HeuristicLab.Problems.DataAnalysis.Regression.HeuristicLabProblemsDataAnalysisRegressionPlugin();
    //  new HeuristicLab.Problems.DataAnalysis.Views.HeuristicLabProblemsDataAnalysisViewsPlugin();
    //  new HeuristicLab.Problems.ExternalEvaluation.HeuristicLabProblemsExternalEvaluationPlugin();
    //  new HeuristicLab.Problems.ExternalEvaluation.GP.HeuristicLabProblemsExternalEvaluationGPPlugin();
    //  new HeuristicLab.Problems.ExternalEvaluation.GP.Views.HeuristicLabProblemsExternalEvaluationGPViewsPlugin();
    //  new HeuristicLab.Problems.ExternalEvaluation.Views.HeuristicLabProblemsExternalEvaluationViewsPlugin();
    //  new HeuristicLab.Problems.Knapsack.HeuristicLabProblemsKnapsackPlugin();
    //  new HeuristicLab.Problems.Knapsack.Views.HeuristicLabProblemsKnapsackViewsPlugin();
    //  new HeuristicLab.Problems.OneMax.HeuristicLabProblemsOneMaxPlugin();
    //  new HeuristicLab.Problems.OneMax.Views.HeuristicLabProblemsKnapsackViewsPlugin();
    //  new HeuristicLab.Problems.TestFunctions.HeuristicLabProblemsTestFunctionsPlugin();
    //  new HeuristicLab.Problems.TestFunctions.Views.HeuristicLabProblemsTestFunctionsViewsPlugin();
    //  new HeuristicLab.Problems.TravelingSalesman.HeuristicLabProblemsTravelingSalesmanPlugin();
    //  new HeuristicLab.Problems.TravelingSalesman.Views.HeuristicLabProblemsTravelingSalesmanViewsPlugin();
    //  new HeuristicLab.Problems.VehicleRouting.HeuristicLabProblemsVehicleRoutingPlugin();
    //  new HeuristicLab.Problems.VehicleRouting.Views.HeuristicLabProblemsVehicleRoutingViewsPlugin();

    //  new HeuristicLab.Random.HeuristicLabRandomPlugin();
    //  new HeuristicLab.Selection.HeuristicLabSelectionPlugin();
    //  new HeuristicLab.SequentialEngine.HeuristicLabSequentialEnginePlugin();
    //  new HeuristicLab.Tracing.HeuristicLabPersistencePlugin();
    //}
  }
}
