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
  public class ProbabilisticTreeCreaterTest {
    private const int POPULATION_SIZE = 1000;
    private const int MAX_TREE_SIZE = 100;
    private const int MAX_TREE_HEIGHT = 10;
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

    [TestMethod()]
    public void ProbabilisticTreeCreaterDistributionsTest() {
      var randomTrees = new List<SymbolicExpressionTree>();
      var grammar = Grammars.CreateSimpleArithmeticGrammar();
      var random = new MersenneTwister();
      for (int i = 0; i < POPULATION_SIZE; i++) {
        randomTrees.Add(ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_SIZE, MAX_TREE_HEIGHT, 0, 0));
      }

      foreach (var tree in randomTrees)
        Assert.IsTrue(tree.IsValidExpression());
      Assert.Inconclusive("ProbabilisticTreeCreator: " + Environment.NewLine +
        Util.GetSizeDistributionString(randomTrees, 105, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(randomTrees) + Environment.NewLine +
        Util.GetNumberOfSubTreesDistributionString(randomTrees) + Environment.NewLine +
        Util.GetTerminalDistributionString(randomTrees) + Environment.NewLine
        );
    }


    [TestMethod()]
    public void ProbabilisticTreeCreaterWithAdfDistributionsTest() {
      var randomTrees = new List<SymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister();
      for (int i = 0; i < POPULATION_SIZE; i++) {
        randomTrees.Add(ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_SIZE, MAX_TREE_HEIGHT, 3, 3));
      }
      foreach (var tree in randomTrees)
        Assert.IsTrue(tree.IsValidExpression());
      Assert.Inconclusive("ProbabilisticTreeCreator: " + Environment.NewLine +
        Util.GetSizeDistributionString(randomTrees, 105, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(randomTrees) + Environment.NewLine +
        Util.GetNumberOfSubTreesDistributionString(randomTrees) + Environment.NewLine +
        Util.GetTerminalDistributionString(randomTrees) + Environment.NewLine
        );
    }
  }
}
