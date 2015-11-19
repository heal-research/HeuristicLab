using System;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Tests;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  /// <summary>
  /// Summary description for MaxCommonSubtreeSimilarityCalculatorTest
  /// </summary>
  [TestClass]
  public class SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculatorTest {
    private readonly ISymbolicExpressionTreeNodeSimilarityComparer comparer;

    private const int N = 100;
    private const int Rows = 1;
    private const int Columns = 10;

    public SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculatorTest() {
      comparer = new SymbolicExpressionTreeNodeEqualityComparer();
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void MaxCommonSubtreeSimilarityCalculatorTestPerformance() {
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();
      var twister = new MersenneTwister(31415);
      var ds = Util.CreateRandomDataset(twister, Rows, Columns);
      var trees = Util.CreateRandomTrees(twister, ds, grammar, N, 1, 100, 0, 0);

      double s = 0;
      var sw = new Stopwatch();

      sw.Start();
      for (int i = 0; i < trees.Length - 1; ++i) {
        for (int j = i + 1; j < trees.Length; ++j) {
          s += SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator.MaxCommonSubtreeSimilarity(trees[i], trees[j], comparer);
        }
      }
      sw.Stop();
      Console.WriteLine("Elapsed time: " + sw.ElapsedMilliseconds / 1000.0 + ", Avg. similarity: " + s / (N * (N - 1) / 2));
      Console.WriteLine(N * (N + 1) / (2 * sw.ElapsedMilliseconds / 1000.0) + " similarity calculations per second.");
    }
  }
}
