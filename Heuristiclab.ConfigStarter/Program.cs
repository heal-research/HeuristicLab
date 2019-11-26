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
      TravelingSalesmanProblem tsp = new TravelingSalesmanProblem();
      tsp.Coordinates[0, 0] = 123;


      SymbolicRegressionSingleObjectiveProblem prop = new SymbolicRegressionSingleObjectiveProblem();
      
      alg.Problem = prop;

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
