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
    public SubtreeCrossoverTest() {
    }

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
      var grammar = new TestGrammar();
      var random = new MersenneTwister();
      for (int i = 0; i < populationSize; i++) {
        crossoverTrees.Add(ProbabilisticTreeCreator.Create(random, grammar, 100, 10));
      }
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int gCount = 0; gCount < generations; gCount++) {
        var newPopulation = new List<SymbolicExpressionTree>();
        for (int i = 0; i < populationSize; i++) {
          var par0 = (SymbolicExpressionTree)crossoverTrees[random.Next(populationSize)].Clone();
          var par1 = (SymbolicExpressionTree)crossoverTrees[random.Next(populationSize)].Clone();
          bool success;
          newPopulation.Add(SubtreeCrossover.Cross(random, grammar, par0, par1, 0.9, 100, 10, out success));
        }
        crossoverTrees = newPopulation;
      }
      stopwatch.Stop();
      foreach (var tree in crossoverTrees)
        Assert.IsTrue(grammar.IsValidExpression(tree));
      msPerCrossoverEvent = stopwatch.ElapsedMilliseconds / (double)populationSize / (double)generations;
    }



    private static List<SymbolicExpressionTree> crossoverTrees;
    private static double msPerCrossoverEvent;

    private class Addition : Symbol { }
    private class Subtraction : Symbol { }
    private class Multiplication : Symbol { }
    private class Division : Symbol { }
    private class Terminal : Symbol { }

    private class TestGrammar : DefaultSymbolicExpressionGrammar {
      public TestGrammar()
        : base(0, 0, 0, 0) {
        Initialize();
      }

      private void Initialize() {
        var add = new Addition();
        var sub = new Subtraction();
        var mul = new Multiplication();
        var div = new Division();
        var terminal = new Terminal();

        var allSymbols = new List<Symbol>() { add, sub, mul, div, terminal };
        var functionSymbols = new List<Symbol>() { add, sub, mul, div };
        allSymbols.ForEach(s => AddAllowedSymbols(StartSymbol, 0, s));

        SetMinSubTreeCount(terminal, 0);
        SetMaxSubTreeCount(terminal, 0);
        int maxSubTrees = 3;
        foreach (var functionSymbol in functionSymbols) {
          SetMinSubTreeCount(functionSymbol, 1);
          SetMaxSubTreeCount(functionSymbol, maxSubTrees);
          foreach (var childSymbol in allSymbols) {
            for (int argumentIndex = 0; argumentIndex < maxSubTrees; argumentIndex++) {
              AddAllowedSymbols(functionSymbol, argumentIndex, childSymbol);
            }
          }
        }
      }
    }

    [TestMethod()]
    public void SubtreeCrossoverSpeed() {
      Assert.Inconclusive(msPerCrossoverEvent + " ms per crossover event (~" +
        Math.Round(1000.0 / (msPerCrossoverEvent)) + "crossovers / s)");
    }

    [TestMethod()]
    public void SubtreeCrossoverSizeDistributionTest() {
      int[] histogram = new int[105 / 5];
      for (int i = 0; i < crossoverTrees.Count; i++) {
        histogram[crossoverTrees[i].Size / 5]++;
      }
      StringBuilder strBuilder = new StringBuilder();
      for (int i = 0; i < histogram.Length; i++) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append("< "); strBuilder.Append((i + 1) * 5);
        strBuilder.Append(": "); strBuilder.AppendFormat("{0:#0.00%}", histogram[i] / (double)crossoverTrees.Count);
      }
      Assert.Inconclusive("Size distribution of SubtreeCrossover: " + strBuilder);
    }

    [TestMethod()]
    public void SubtreeCrossoverFunctionDistributionTest() {
      Dictionary<Symbol, int> occurances = new Dictionary<Symbol, int>();
      double n = 0.0;
      for (int i = 0; i < crossoverTrees.Count; i++) {
        foreach (var node in crossoverTrees[i].IterateNodesPrefix()) {
          if (node.SubTrees.Count > 0) {
            if (!occurances.ContainsKey(node.Symbol))
              occurances[node.Symbol] = 0;
            occurances[node.Symbol]++;
            n++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function.Name); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[function] / n);
      }
      Assert.Inconclusive("Function distribution of SubtreeCrossover: " + strBuilder);
    }

    [TestMethod()]
    public void SubtreeCrossoverNumberOfSubTreesDistributionTest() {
      Dictionary<int, int> occurances = new Dictionary<int, int>();
      double n = 0.0;
      for (int i = 0; i < crossoverTrees.Count; i++) {
        foreach (var node in crossoverTrees[i].IterateNodesPrefix()) {
          if (!occurances.ContainsKey(node.SubTrees.Count))
            occurances[node.SubTrees.Count] = 0;
          occurances[node.SubTrees.Count]++;
          n++;
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var arity in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(arity); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[arity] / n);
      }
      Assert.Inconclusive("Distribution of function arities of SubtreeCrossover: " + strBuilder);
    }


    [TestMethod()]
    public void SubtreeCrossoverTerminalDistributionTest() {
      Dictionary<Symbol, int> occurances = new Dictionary<Symbol, int>();
      double n = 0.0;
      for (int i = 0; i < crossoverTrees.Count; i++) {
        foreach (var node in crossoverTrees[i].IterateNodesPrefix()) {
          if (node.SubTrees.Count == 0) {
            if (!occurances.ContainsKey(node.Symbol))
              occurances[node.Symbol] = 0;
            occurances[node.Symbol]++;
            n++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function.Name); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[function] / n);
      }
      Assert.Inconclusive("Terminal distribution of SubtreeCrossover: " + strBuilder);
    }
  }
}
