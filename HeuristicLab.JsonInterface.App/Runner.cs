using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Optimization;
using HeuristicLab.SequentialEngine;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface.App {
  internal static class Runner {
    internal static void Run(string template, string config, string outputFile) {
      IOptimizer optimizer = JsonTemplateInstantiator.Instantiate(template, config, out IEnumerable<string> allowedResultNames);
      if(optimizer is EngineAlgorithm e)
        e.Engine = new SequentialEngine.SequentialEngine();
      
      Task task = optimizer.StartAsync();
      while(!task.IsCompleted) {
        WriteResultsToFile(outputFile, optimizer, allowedResultNames);
        Thread.Sleep(100);
      }
     
      WriteResultsToFile(outputFile, optimizer, allowedResultNames);
    }

    private static void WriteResultsToFile(string file, IOptimizer optimizer, IEnumerable<string> allowedResultNames) =>
      File.WriteAllText(file, FetchResults(optimizer, allowedResultNames));

    private static string FetchResults(IOptimizer optimizer, IEnumerable<string> allowedResultNames) {
      JArray arr = new JArray();
      
      foreach (var run in optimizer.Runs) {
        JObject obj = new JObject();
        arr.Add(obj);
        obj.Add("Run", JToken.FromObject(run.ToString()));
        foreach (var res in run.Results) {
          if (allowedResultNames.Contains(res.Key))
            obj.Add(res.Key, JToken.FromObject(res.Value.ToString()));
        }
      }
      return SingleLineArrayJsonWriter.Serialize(arr);
    }
  }
}
