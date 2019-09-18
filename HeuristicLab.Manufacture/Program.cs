using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Algorithms.ALPS;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Algorithms.TabuSearch;
using HeuristicLab.Analysis;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Knapsack;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Problems.VehicleRouting;
using HeuristicLab.SequentialEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace ParameterTest {
  
  public class Program {
    static void Main(string[] args) {
      //GeneticAlgorithm ga = new GeneticAlgorithm();
      GeneticAlgorithm alg = new GeneticAlgorithm();
      TravelingSalesmanProblem tsp = new TravelingSalesmanProblem();

      JCGenerator gen = new JCGenerator();
      File.WriteAllText(@"C:\Workspace\TemplateProto9.json", gen.GenerateTemplate(alg, tsp));
      
      JCInstantiator configurator = new JCInstantiator();
      //configurator.Instantiate(@"C:\Workspace\TemplateProto9.json");
      //Configure(@"C:\Workspace\TemplateProto9.json");
      //Console.WriteLine(Optimizer);

      Console.ReadLine();
    }
  }
}
