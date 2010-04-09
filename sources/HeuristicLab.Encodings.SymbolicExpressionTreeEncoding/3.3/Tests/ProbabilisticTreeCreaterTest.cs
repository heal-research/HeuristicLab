using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  [TestClass]
  public class ProbabilisticTreeCreaterTest {
    public ProbabilisticTreeCreaterTest() {
      int populationSize = 1000;
      randomTrees = new List<SymbolicExpressionTree>();
      var grammar = new TestGrammar();
      var random = new MersenneTwister();
      for (int i = 0; i < populationSize; i++) {
        randomTrees.Add(ProbabilisticTreeCreator.Create(random, grammar, 100, 10));
      }
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

    private List<SymbolicExpressionTree> randomTrees;


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
    public void SizeDistributionTest() {
      int[] histogram = new int[105 / 5];
      for (int i = 0; i < randomTrees.Count; i++) {
        histogram[randomTrees[i].Size / 5]++;
      }
      StringBuilder strBuilder = new StringBuilder();
      for (int i = 0; i < histogram.Length; i++) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append("< "); strBuilder.Append((i + 1) * 5);
        strBuilder.Append(": "); strBuilder.AppendFormat("{0:#0.00%}", histogram[i] / (double)randomTrees.Count);
      }
      Assert.Inconclusive("Size distribution of ProbabilisticTreeCreator: " + strBuilder);
    }

    [TestMethod()]
    public void FunctionDistributionTest() {
      Dictionary<Symbol, int> occurances = new Dictionary<Symbol, int>();
      double n = 0.0;
      for (int i = 0; i < randomTrees.Count; i++) {
        foreach (var node in randomTrees[i].IterateNodesPrefix()) {
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
      Assert.Inconclusive("Function distribution of ProbabilisticTreeCreator: " + strBuilder);
    }

    [TestMethod()]
    public void NumberOfSubTreesDistributionTest() {
      Dictionary<int, int> occurances = new Dictionary<int, int>();
      double n = 0.0;
      for (int i = 0; i < randomTrees.Count; i++) {
        foreach (var node in randomTrees[i].IterateNodesPrefix()) {
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
      Assert.Inconclusive("Distribution of function arities of ProbabilisticTreeCreator: " + strBuilder);
    }


    [TestMethod()]
    public void TerminalDistributionTest() {
      Dictionary<Symbol, int> occurances = new Dictionary<Symbol, int>();
      double n = 0.0;
      for (int i = 0; i < randomTrees.Count; i++) {
        foreach (var node in randomTrees[i].IterateNodesPrefix()) {
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
      Assert.Inconclusive("Terminal distribution of ProbabilisticTreeCreator: " + strBuilder);
    }
  }
}
