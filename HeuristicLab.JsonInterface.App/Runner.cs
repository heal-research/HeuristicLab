using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Optimization;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface.App {
  internal static class Runner {
    internal static void Run(string template, string config, string outputFile) {
      try {
        InstantiatorResult instantiatorResult = JsonTemplateInstantiator.Instantiate(template, config);
        IOptimizer optimizer = instantiatorResult.Optimizer;
        IEnumerable<IResultJsonItem> configuredResultItem = instantiatorResult.ConfiguredResultItems;

        optimizer.Runs.Clear();
        if (optimizer is EngineAlgorithm e)
          e.Engine = new ParallelEngine.ParallelEngine();

        Task task = optimizer.StartAsync();
        while (!task.IsCompleted) {
          WriteResultsToFile(outputFile, optimizer, configuredResultItem);
          Thread.Sleep(100);
        }

        WriteResultsToFile(outputFile, optimizer, configuredResultItem);
      } catch (Exception e) {
        Console.Error.WriteLine($"{e.Message} \n\n\n\n {e.StackTrace}");
        File.WriteAllText(outputFile, e.Message + "\n\n\n\n" + e.StackTrace);
        Environment.Exit(-1);
      }
    }

    private static void WriteResultsToFile(string file, IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItem) =>
      File.WriteAllText(file, FetchResults(optimizer, configuredResultItem));

    private static IEnumerable<IResultFormatter> ResultFormatter { get; } =
      PluginInfrastructure.ApplicationManager.Manager.GetInstances<IResultFormatter>();

    private static IResultFormatter GetResultFormatter(string fullName) =>
      ResultFormatter?.Where(x => x.GetType().FullName == fullName).Last();

    private static string FetchResults(IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItems) {
      JArray arr = new JArray();
      IEnumerable<string> configuredResults = configuredResultItems.Select(x => x.Name);

      foreach (var run in optimizer.Runs) {
        JObject obj = new JObject();
        arr.Add(obj);
        obj.Add("Run", JToken.FromObject(run.ToString()));

        // zip and filter the results with the ResultJsonItems
        var filteredResults = configuredResultItems.Zip(
          run.Results.Where(x => configuredResultItems.Any(y => y.Name == x.Key)), 
          (x, y) => new { Item = x, Value = y.Value });

        // add results to the JObject
        foreach(var result in filteredResults) {
          var formatter = GetResultFormatter(result.Item.ResultFormatterType);
          obj.Add(result.Item.Name, formatter.Format(result.Value));
        }
      }
      return SingleLineArrayJsonWriter.Serialize(arr);
    }
  }
}
