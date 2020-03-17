using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Optimization;
using HeuristicLab.ParallelEngine;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.SequentialEngine;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface.App {
  internal static class Runner {
    internal static void Run(string template, string config, string outputFile) {
      InstantiatorResult instantiatorResult = JsonTemplateInstantiator.Instantiate(template, config);
      IOptimizer optimizer = instantiatorResult.Optimizer;
      IEnumerable<IResultJsonItem> configuredResultItem = instantiatorResult.ConfiguredResultItems;

      optimizer.Runs.Clear();
      if(optimizer is EngineAlgorithm e)
        e.Engine = new ParallelEngine.ParallelEngine();
      
      Task task = optimizer.StartAsync();
      while(!task.IsCompleted) {
        WriteResultsToFile(outputFile, optimizer, configuredResultItem);
        Thread.Sleep(100);
      }
     
      WriteResultsToFile(outputFile, optimizer, configuredResultItem);
    }

    private static void WriteResultsToFile(string file, IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItem) =>
      File.WriteAllText(file, FetchResults(optimizer, configuredResultItem));

    private static string FetchResults(IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItem) {
      JArray arr = new JArray();
      IEnumerable<string> configuredResults = configuredResultItem.Select(x => x.Name);

      foreach (var run in optimizer.Runs) {
        JObject obj = new JObject();
        arr.Add(obj);
        obj.Add("Run", JToken.FromObject(run.ToString()));
        foreach (var res in run.Results) {
          if (configuredResults.Contains(res.Key)) {
            if (res.Value is ISymbolicRegressionSolution solution) {
              var formatter = new SymbolicDataAnalysisExpressionMATLABFormatter();
              var x = formatter.Format(solution.Model.SymbolicExpressionTree);
              obj.Add(res.Key, JToken.FromObject(x));
            } else
              obj.Add(res.Key, JToken.FromObject(res.Value.ToString()));
          }
        }
      }
      return SingleLineArrayJsonWriter.Serialize(arr);
    }
  }
}
