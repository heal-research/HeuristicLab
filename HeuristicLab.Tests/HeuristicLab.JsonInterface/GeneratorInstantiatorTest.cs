using System;
using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.JsonInterface;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.TravelingSalesman;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.JsonInterface.Tests {
  [TestClass]
  public class GeneratorInstantiatorTest {
    private string templateFilePath = Directory.GetCurrentDirectory()+"\\Template.json";
    private string configFilePath = Directory.GetCurrentDirectory() + "\\Config.json";

    [TestInitialize()]
    public void CreateTempFiles() {
      GeneticAlgorithm alg = new GeneticAlgorithm();
      TravelingSalesmanProblem tsp = new TravelingSalesmanProblem();
      JCGenerator gen = new JCGenerator();
      //File.WriteAllText(@"C:\Workspace\Template.json", gen.GenerateTemplate(alg, tsp));
      File.WriteAllText(templateFilePath, gen.GenerateTemplate(alg, tsp));
      File.WriteAllText(configFilePath, "["+
        "{\"Name\": \"Seed\",\"Default\": 55555,\"Path\": \"Genetic Algorithm (GA).Seed\"},"+
        "{\"Name\": \"Crossover\", \"Path\": \"Genetic Algorithm (GA).Crossover\", \"Default\": \"MultiPermutationCrossover\"}," +
        "{\"Name\": \"Elites\", \"Path\": \"Genetic Algorithm (GA).Elites\", \"Default\": 5,\"Range\":[-2147483648,2147483647]}" +
        "]");
    }
    
    [TestCleanup()]
    public void ClearTempFiles() {
      File.Delete(templateFilePath);
      File.Delete(configFilePath);
    }

    [TestMethod]
    public void TestInstantiator() {
      JCInstantiator configurator = new JCInstantiator();
      GeneticAlgorithm alg = (GeneticAlgorithm)configurator.Instantiate(templateFilePath, configFilePath);

      Assert.AreEqual(55555, alg.Seed.Value);
      Assert.IsTrue(alg.Crossover is MultiPermutationCrossover);
      Assert.AreEqual(5, alg.Elites.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestRangeChangeWithConfig() {
      File.WriteAllText(configFilePath, "[{\"Name\": \"MutationProbability\", \"Path\": \"Genetic Algorithm (GA).MutationProbability\", \"Default\": 2.0,\"Range\":[0.0,2.0]}]");
      JCInstantiator configurator = new JCInstantiator();
      GeneticAlgorithm alg = (GeneticAlgorithm)configurator.Instantiate(templateFilePath, configFilePath);
    }
  }
}
