using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.JsonInterface.App;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.JsonInterface;
using System.IO;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.PluginInfrastructure.Manager;
using HeuristicLab.SequentialEngine;
using System.Threading;

namespace Heuristiclab.ConfigStarter {
  public class Program {

    private static string Reduce(string str) => 
      str
      .Replace(" ", "")
      .Replace("-", "")
      .Replace("`", "")
      .Replace(".", "")
      .Replace("<", "")
      .Replace(">", "")
      .Replace("(", "_")
      .Replace(")", "_");

    private static void Visualize(JsonItem item, StringBuilder sb) {
      sb.Append($"  {item.GetHashCode()} [label=\"{item.Name}\"];\n");
      foreach (var i in item.Parameters) {
        sb.Append($"  {item.GetHashCode()} -> {i.GetHashCode()};\n");
      }
      foreach(var i in item.Parameters) {
        Visualize(i, sb);
      }
    }

    public static void Main(string[] args) {

      try {
        string pluginPath = Path.GetFullPath(Directory.GetCurrentDirectory());
        var pluginManager = new PluginManager(pluginPath);
        pluginManager.DiscoverAndCheckPlugins();
        
      } catch(Exception e) {
        Console.WriteLine(e);
      }

      HEAL.Attic.Mapper.StaticCache.UpdateRegisteredTypes();


      HeuristicLabJsonInterfaceAppApplication app = new HeuristicLabJsonInterfaceAppApplication();

      GeneticAlgorithm alg = new GeneticAlgorithm();
      alg.MaximumGenerations.Value = 10000;
      TravelingSalesmanProblem tsp = new TravelingSalesmanProblem();
      tsp.Coordinates[0, 0] = 123;



      SymbolicRegressionSingleObjectiveProblem prop = new SymbolicRegressionSingleObjectiveProblem();
      
      alg.Problem = prop;

      alg.Engine = new SequentialEngine();
      Task t = alg.StartAsync();
      Thread.Sleep(1000);
      alg.Stop();

      StorableConverter storableConverter = new StorableConverter();
      JsonItem item = storableConverter.Extract(alg);

      StringBuilder sb = new StringBuilder();

      //Visualize(item, sb);

      //File.WriteAllText(@"C:\Workspace\item.gv", $"digraph G {{\n{sb.ToString()}}}");


      //Console.WriteLine(alg);
      File.WriteAllText(@"C:\Workspace\Template.json", JCGenerator.GenerateTemplate(alg));

      /*
      List<ICommandLineArgument> arguments = new List<ICommandLineArgument>();
      arguments.Add(new StartArgument("JsonInterface"));
      arguments.Add(new OpenArgument(@"C:\Workspace\Template.json"));
      arguments.Add(new OpenArgument(@"C:\Workspace\ConfigProto1.json"));

      app.Run(arguments.ToArray());
      */
    }
  }
}
