using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface.App {
  internal static class Runner {
    internal static void Run(string template, string config, string outputFile, TimeSpan outputInterval) {
      var convertable = JsonTemplateInstantiator.Instantiate(template, config);

      if (convertable is EngineAlgorithm e)
        e.Engine = new ParallelEngine.ParallelEngine();

      if (convertable is IOptimizer optimizer) {
        optimizer.Runs.Clear();
        var modifiers = optimizer.Runs.Modifiers;
        Task task = optimizer.StartAsync();
        while (!task.IsCompleted) {
          WriteResultsToFile(outputFile, optimizer, modifiers);
          Thread.Sleep(outputInterval);
        }
        WriteResultsToFile(outputFile, optimizer, modifiers);
      } else {
        throw new ArgumentException("The convertable is not an IOptimizer.");
      }
    }

    private static void WriteResultsToFile(string file, IOptimizer optimizer, IEnumerable<IRunCollectionModifier> runCollectionModifiers) {
      if (optimizer.Runs.Count > 0)
        File.WriteAllText(file, FetchResults(optimizer, runCollectionModifiers));
    }

    private static string FetchResults(IOptimizer optimizer, IEnumerable<IRunCollectionModifier> runCollectionModifiers) {
      JArray arr = new JArray();

      foreach (var modifier in runCollectionModifiers)
        modifier.Modify(optimizer.Runs.ToList());

      foreach (var run in optimizer.Runs) {
        JObject obj = new JObject();
        arr.Add(obj);
        obj.Add("Run", JToken.FromObject(run.ToString()));
        foreach (var result in run.Results)
          obj.Add(result.Key, result.Value.ToString());
      }
      return SingleLineArrayJsonWriter.Serialize(arr);
    }
  }
}
