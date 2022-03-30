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
      //InstantiatorResult instantiatorResult = JsonTemplateInstantiator.Instantiate(template, config);
      //IJsonConvertable convertable = instantiatorResult.Convertable;

      //convertable.Runs.Clear();
      //if (convertable is EngineAlgorithm e)
      //  e.Engine = new ParallelEngine.ParallelEngine();

      //Task task = convertable.StartAsync();
      //while (!task.IsCompleted) {
      //  WriteResultsToFile(outputFile, convertable, instantiatorResult.RunCollectionModifiers);
      //  Thread.Sleep(outputInterval);
      //}

      //WriteResultsToFile(outputFile, convertable, instantiatorResult.RunCollectionModifiers);
    }

    //private static void WriteResultsToFile(string file, IOptimizer optimizer, IEnumerable<IRunCollectionModifier> runCollectionModifiers) {
    //  if (optimizer.Runs.Count > 0) 
    //    File.WriteAllText(file, FetchResults(optimizer, runCollectionModifiers));
    //}

    //private static string FetchResults(IOptimizer optimizer, IEnumerable<IRunCollectionModifier> runCollectionModifiers) {
    //  JArray arr = new JArray();
      
    //  foreach (var modifier in runCollectionModifiers)
    //    modifier.Modify(optimizer.Runs.ToList());
      
    //  foreach (var run in optimizer.Runs) {
    //    JObject obj = new JObject();
    //    arr.Add(obj);
    //    obj.Add("Run", JToken.FromObject(run.ToString()));
    //    foreach (var result in run.Results)
    //      obj.Add(result.Key, result.Value.ToString());
    //  }
    //  return SingleLineArrayJsonWriter.Serialize(arr);
    //}
  }
}
