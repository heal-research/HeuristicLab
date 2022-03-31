using System;
using System.IO;
using System.Reflection;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.JsonInterface;
using Newtonsoft.Json;

namespace ConsoleApp1
{
  internal class Program
  {
    static void Main(string[] args)
    {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      ga.Engine = new SequentialEngine();
      ga.Problem = new SymbolicRegressionSingleObjectiveProblem();

      var converter = new JsonItemConverter();
      JsonItem item = converter.ConvertToJson(ga);

      JsonTemplateGenerator.GenerateTemplate(@"D:\test", ga);
    }
  }
}
