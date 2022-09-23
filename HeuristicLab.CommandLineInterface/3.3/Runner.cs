using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CommandLineInterface
{
  public class Runner
  {
    public static void Run(ICommandLineArgument[] args) {
      Console.WriteLine("Hello World");
      Console.WriteLine(args.Count());
      foreach(var arg in args)
        Console.WriteLine(arg.Value);
    }
  }
}
