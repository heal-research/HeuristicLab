﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface.App {
  internal static class Runner {
    internal static void Run(string template, string config, string outputFile = @"C:\Workspace\test.txt") {
      IAlgorithm alg = JCInstantiator.Instantiate(template, config);
  
      Task task = alg.StartAsync();
      while(!task.IsCompleted) {
        WriteResultsToFile(outputFile, alg);
        Thread.Sleep(100);
      }
      WriteResultsToFile(outputFile, alg);
    }

    private static void WriteResultsToFile(string file, IAlgorithm optimizer) =>
      File.WriteAllText(file, FetchResults(optimizer));

    private static string FetchResults(IAlgorithm optimizer) {
      StringBuilder sb = new StringBuilder();
      //foreach (var run in optimizer.Runs) {
        //sb.AppendLine($"--- {run.ToString()} ---");
        foreach (var res in optimizer.Results) {
          sb.AppendLine($"{res.Name}: {res.Value}");
        }
      //}
      return sb.ToString();
    }
  }
}
