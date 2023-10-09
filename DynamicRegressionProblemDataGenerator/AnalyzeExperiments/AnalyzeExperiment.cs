using System;
using System.Reflection;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure.Manager;
using Xunit;


namespace DynamicRegressionProblemDataGenerator.AnalyzeExperiments {
  public class AnalyzeExperiment {
    static AnalyzeExperiment() {
      var pm = new PluginManager(AppDomain.CurrentDomain.BaseDirectory);
      pm.DiscoverAndCheckPlugins();
      foreach (var plugin in pm.Plugins) {
        foreach (string path in plugin.AssemblyLocations) {
          Assembly.LoadFile(path);
        }
      }
      ContentManager.Initialize(new PersistenceContentManager());
    }
    
    [Fact]
    void AnalyzeExperiment_GA_F1() {
      var runCollection = (RunCollection)ContentManager.Load(@"C:\Users\P41107\Desktop\F1_GA_runs.hl");

      foreach (var run in runCollection) {
        string algorithmName = ((StringValue)run.Parameters["Algorithm Name"]).Value;
        string algorithmType = ((StringValue)run.Parameters["Algorithm Type"]).Value;
        DataTable qualities = (DataTable)run.Results["Qualities"];
        DataTable symbolFrequencies = (DataTable)run.Results["Symbol frequencies"];
        DataTable variableFrequencies = (DataTable)run.Results["Variable frequencies"];
        DataTable bestSolutionVariableImpacts = (DataTable)run.Results["BestSolutionVariableImpacts"];
        DataTable populationVariableImpacts = (DataTable)run.Results["PopulationVariableImpacts"];
      }

    }
  }
}
