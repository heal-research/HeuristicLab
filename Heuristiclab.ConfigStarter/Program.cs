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

      IJsonItem root = JsonItemConverter.Extract(alg);
      ActivateJsonItems(root);

      JCGenerator.GenerateTemplate(@"C:\Workspace", "Template", alg, root);
      
      List<ICommandLineArgument> arguments = new List<ICommandLineArgument>();
      arguments.Add(new StartArgument("JsonInterface"));
      arguments.Add(new OpenArgument(@"C:\Workspace\Template.json"));
      arguments.Add(new OpenArgument(@"C:\Workspace\Config.json"));
      arguments.Add(new StringArgument(@"C:\Workspace\Output.json"));

      app.Run(arguments.ToArray());
      
    }

    private static void ActivateJsonItems(IJsonItem item) {
      foreach (var x in item) {
        x.Active = true;
        if (x is ValueLookupJsonItem i) {
          i.Active = true;
        }
      }
    }
  }
}
