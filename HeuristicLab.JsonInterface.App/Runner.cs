using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Optimization;
using HeuristicLab.SequentialEngine;

namespace HeuristicLab.JsonInterface.App {
  internal static class Runner {
    internal static void Run(string template, string config, string outputFile = @"C:\Workspace\test.txt") {
      IOptimizer optimizer = JsonTemplateInstantiator.Instantiate(template, config);
      if(optimizer is EngineAlgorithm e)
        e.Engine = new SequentialEngine.SequentialEngine();
      
      Task task = optimizer.StartAsync();
      while(!task.IsCompleted) {
        WriteResultsToFile(outputFile, optimizer);
        Thread.Sleep(100);
      }
      WriteResultsToFile(outputFile, optimizer);
    }

    private static void WriteResultsToFile(string file, IOptimizer optimizer) =>
      File.WriteAllText(file, FetchResults(optimizer));

    private static string FetchResults(IOptimizer optimizer) {
      StringBuilder sb = new StringBuilder();
      foreach (var run in optimizer.Runs) {
        sb.AppendLine($"--- {run.ToString()} ---");
        foreach (var res in run.Results) {
          sb.AppendLine($"{res.Key}: {res.Value}");
        }
      }
      return sb.ToString();
    }
  }
}
