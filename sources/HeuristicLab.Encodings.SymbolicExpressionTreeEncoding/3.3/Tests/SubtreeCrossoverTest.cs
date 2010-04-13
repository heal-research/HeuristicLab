using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  [TestClass]
  public class SubtreeCrossoverTest {
    private static ISymbolicExpressionGrammar grammar;
    private static List<SymbolicExpressionTree> crossoverTrees;
    private static double msPerCrossoverEvent;

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    [ClassInitialize()]
    public static void SubtreeCrossoverTestInitialize(TestContext testContext) {
      crossoverTrees = new List<SymbolicExpressionTree>();
      int populationSize = 1000;
      int generations = 5;
      int failedEvents = 0;
      grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister();
      for (int i = 0; i < populationSize; i++) {
        crossoverTrees.Add(ProbabilisticTreeCreator.Create(random, grammar, 100, 10, 3, 3));
      }
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int gCount = 0; gCount < generations; gCount++) {
        var newPopulation = new List<SymbolicExpressionTree>();
        for (int i = 0; i < populationSize; i++) {
          var par0 = (SymbolicExpressionTree)crossoverTrees[random.Next(populationSize)].Clone();
          var par1 = (SymbolicExpressionTree)crossoverTrees[random.Next(populationSize)].Clone();
          bool success;
          newPopulation.Add(SubtreeCrossover.Cross(random, par0, par1, 0.9, 100, 10, out success));
          if (!success) failedEvents++;
        }
        crossoverTrees = newPopulation;
      }
      stopwatch.Stop();
      foreach (var tree in crossoverTrees)
        Assert.IsTrue(tree.IsValidExpression());
      msPerCrossoverEvent = stopwatch.ElapsedMilliseconds / (double)populationSize / (double)generations;      
    }



    [TestMethod()]
    public void SubtreeCrossoverSpeed() {

      Assert.Inconclusive(msPerCrossoverEvent + " ms per crossover event (~" +
        Math.Round(1000.0 / (msPerCrossoverEvent)) + "crossovers / s)");
    }

    [TestMethod()]
    public void SubtreeCrossoverSizeDistributions() {
      Assert.Inconclusive("SubtreeCrossover: " + Util.GetSizeDistributionString(crossoverTrees, 105, 5));
    }

    [TestMethod()]
    public void SubtreeCrossoverFunctionDistributionTest() {
      Assert.Inconclusive("SubtreeCrossover: " + Util.GetFunctionDistributionString(crossoverTrees));
    }

    [TestMethod()]
    public void SubtreeCrossoverNumberOfSubTreesDistributionTest() {
      Assert.Inconclusive("SubtreeCrossover: " + Util.GetNumberOfSubTreesDistributionString(crossoverTrees));
    }


    [TestMethod()]
    public void SubtreeCrossoverTerminalDistributionTest() {
      Assert.Inconclusive("SubtreeCrossover: " + Util.GetTerminalDistributionString(crossoverTrees));
    }
  }
}
