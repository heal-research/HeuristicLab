using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureAlteringOperators;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  [TestClass]
  public class ArgumentCreaterTest {
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
    public void ArgumentCreaterDistributionsTest() {
      var trees = new List<SymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister();
      for (int i = 0; i < POPULATION_SIZE; i++) {
        var tree = ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_SIZE, MAX_TREE_HEIGHT, 3, 3);
        ArgumentCreater.CreateNewArgument(random, tree, grammar, MAX_TREE_SIZE, MAX_TREE_HEIGHT, 3, 3);
        Assert.IsTrue(tree.IsValidExpression());
        trees.Add(tree);
      }
      Assert.Inconclusive("ArgumentCreator: " + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 105, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubTreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );
    }
  }
}
