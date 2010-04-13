using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureAlteringOperators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  [TestClass]
  public class SubroutineCreaterTest {
    private static ISymbolicExpressionGrammar grammar;
    private static List<SymbolicExpressionTree> subroutineTrees;
    private static int failedEvents;

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
    public static void SubroutineCreaterTestInitialize(TestContext testContext) {
      var randomTrees = new List<SymbolicExpressionTree>();
      subroutineTrees = new List<SymbolicExpressionTree>();
      int populationSize = 1000;
      failedEvents = 0;
      grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister();
      for (int i = 0; i < populationSize; i++) {
        var randTree = ProbabilisticTreeCreator.Create(random, grammar, 100, 10);
        // PTC create is tested separately
        randomTrees.Add(randTree);
      }
      var newPopulation = new List<SymbolicExpressionTree>();
      for (int i = 0; i < populationSize; i++) {
        var par0 = (SymbolicExpressionTree)randomTrees[random.Next(populationSize)].Clone();
        bool success = SubroutineCreater.CreateSubroutine(random, par0, grammar, 100, 10, 3, 3);
        if (!success) failedEvents++;
        subroutineTrees.Add(par0);
      }
    }


    [TestMethod()]
    public void SubroutineCreaterCreateTest() {
      foreach (var tree in subroutineTrees)
        Assert.IsTrue(grammar.IsValidExpression(tree));
    }

    [TestMethod()]
    public void SubroutineCreaterSizeDistributionTest() {
      Assert.Inconclusive("SubroutineCreater: " + Util.GetSizeDistributionString(subroutineTrees, 105, 5));
    }

    [TestMethod()]
    public void SubroutineCreaterFunctionDistributionTest() {
      Assert.Inconclusive("SubroutineCreater: " + Util.GetFunctionDistributionString(subroutineTrees));
    }

    [TestMethod()]
    public void SubroutineCreaterNumberOfSubTreesDistributionTest() {
      Assert.Inconclusive("SubroutineCreater: " + Util.GetNumberOfSubTreesDistributionString(subroutineTrees));
    }


    [TestMethod()]
    public void SubroutineCreaterTerminalDistributionTest() {
      Assert.Inconclusive("SubroutineCreater: " + Util.GetTerminalDistributionString(subroutineTrees));
    }
  }
}
