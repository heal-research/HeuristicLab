using System;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {
  [TestClass]
  public class BottomUpSimilarityCalculatorTest {
    private readonly SymbolicExpressionImporter importer = new SymbolicExpressionImporter();
    private readonly InfixExpressionParser parser = new InfixExpressionParser();

    private const int N = 200;
    private const int Rows = 1;
    private const int Columns = 10;

    public BottomUpSimilarityCalculatorTest() {
      var parser = new InfixExpressionParser();
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void BottomUpTreeSimilarityCalculatorTestMapping() {
      TestMatchedNodes("1 + 1", "2 + 2", 0, strict: true);
      TestMatchedNodes("1 + 1", "2 + 2", 3, strict: false);
      TestMatchedNodes("1 + 1", "1 + 2", 1, strict: true);
      TestMatchedNodes("1 + 2", "2 + 1", 3, strict: true);

      TestMatchedNodes("1 - 1", "2 - 2", 0, strict: true);
      TestMatchedNodes("1 - 1", "2 - 2", 4, strict: false); // 4, because of the way strings are parsed into trees by the infix parser

      TestMatchedNodes("2 - 1", "1 - 2", 2, strict: true);
      TestMatchedNodes("2 - 1", "1 - 2", 4, strict: false);

      TestMatchedNodes("(X1 * X2) + (X3 * X4)", "(X1 * X2) + (X3 * X4)", 7, strict: true);
      TestMatchedNodes("(X1 * X2) + (X3 * X4)", "(X1 * X2) + (X3 * X4)", 7, strict: false);

      TestMatchedNodes("(X1 * X2) + (X3 * X4)", "(X1 * X2) + (X5 * X6)", 3, strict: true);
      TestMatchedNodes("(X1 * X2) + (X3 * X4)", "(X1 * X2) + (X5 * X6)", 3, strict: false);

      TestMatchedNodes("(X1 * X2) + (X3 * X4)", "(X1 * X2) - (X5 * X6)", 3, strict: true);
      TestMatchedNodes("(X1 * X2) + (X3 * X4)", "(X1 * X2) - (X5 * X6)", 3, strict: false);

      TestMatchedNodes("SIN(SIN(SIN(X1)))", "SIN(SIN(SIN(X1)))", 4, strict: true);
      TestMatchedNodes("SIN(SIN(SIN(X1)))", "COS(SIN(SIN(X1)))", 3, strict: true);
      TestMatchedNodes("SIN(SIN(SIN(X1)))", "COS(COS(SIN(X1)))", 2, strict: true);
      TestMatchedNodes("SIN(SIN(SIN(X1)))", "COS(COS(COS(X1)))", 1, strict: true);

      const string lhs = "(0.006153 + (X9 * X7 * X2 * 0.229506) + (X6 * X10 * X3 * 0.924598) + (X2 * X1 * 0.951272) + (X4 * X3 * 0.992570) + (X6 * X5 * 1.027299))";
      const string rhs = "(0.006153 + (X10 * X7 * X2 * 0.229506) + (X6 * X10 * X3 * 0.924598) + (X2 * X1 * 0.951272) + (X4 * X3 * 0.992570) + (X6 * X5 * 1.027299))";

      TestMatchedNodes(lhs, lhs, 24, strict: true);
      TestMatchedNodes(lhs, lhs, 24, strict: false);

      TestMatchedNodes(lhs, rhs, 21, strict: true);
      TestMatchedNodes(lhs, rhs, 21, strict: false);
    }

    private void TestMatchedNodes(string expr1, string expr2, int expected, bool strict) {
      var t1 = parser.Parse(expr1);
      var t2 = parser.Parse(expr2);

      Console.WriteLine(SymbolicExpressionTreeHierarchicalFormatter.FormatTree(t1));
      Console.WriteLine(SymbolicExpressionTreeHierarchicalFormatter.FormatTree(t2));

      var map = SymbolicExpressionTreeBottomUpSimilarityCalculator.ComputeBottomUpMapping(t1, t2, strict);
      Console.WriteLine($"Count: {map.Count}");

      if (map.Count != expected) {
        throw new Exception($"Match count {map.Count} is different than expected value {expected} for expressions:\n{expr1} and {expr2} (strict = {strict})\n");
      }
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void BottomUpTreeSimilarityCalculatorTestPerformance() {
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();
      var twister = new MersenneTwister(31415);
      var ds = Util.CreateRandomDataset(twister, Rows, Columns);
      var trees = Util.CreateRandomTrees(twister, ds, grammar, N, 1, 100, 0, 0);

      double s = 0;
      var sw = new Stopwatch();

      var similarityCalculator = new SymbolicExpressionTreeBottomUpSimilarityCalculator { MatchVariableWeights = false, MatchConstantValues = false };

      sw.Start();
      for (int i = 0; i < trees.Length - 1; ++i) {
        for (int j = i + 1; j < trees.Length; ++j) {
          s += similarityCalculator.CalculateSimilarity(trees[i], trees[j]);
        }
      }

      sw.Stop();
      Console.WriteLine("Elapsed time: " + sw.ElapsedMilliseconds / 1000.0 + ", Avg. similarity: " + s / (N * (N - 1) / 2));
      Console.WriteLine(N * (N + 1) / (2 * sw.ElapsedMilliseconds / 1000.0) + " similarity calculations per second.");
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void BottomUpTreeSimilarityCalculatorStrictMatchingConsistency() {
      TestMatchingConsistency(strict: true);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void BottomUpTreeSimilarityCalculatorRelaxedMatchingConsistency() {
      TestMatchingConsistency(strict: false);
    }

    private static void TestMatchingConsistency(bool strict = false) {
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();
      var twister = new MersenneTwister(31415);
      var ds = Util.CreateRandomDataset(twister, Rows, Columns);
      var trees = Util.CreateRandomTrees(twister, ds, grammar, N, 1, 100, 0, 0);

      var similarityCalculator = new SymbolicExpressionTreeBottomUpSimilarityCalculator { MatchConstantValues = strict, MatchVariableWeights = strict };
      var bottomUpSimilarity = 0d;
      for (int i = 0; i < trees.Length - 1; ++i) {
        for (int j = i + 1; j < trees.Length; ++j) {
          bottomUpSimilarity += similarityCalculator.CalculateSimilarity(trees[i], trees[j]);
        }
      }
      bottomUpSimilarity /= N * (N - 1) / 2;

      var hashBasedSimilarity = SymbolicExpressionTreeHash.ComputeAverageSimilarity(trees, false, strict);

      Assert.AreEqual(bottomUpSimilarity, hashBasedSimilarity, 1e-6);

      Console.WriteLine($"Bottom-up similarity: {bottomUpSimilarity}, hash-based similarity: {hashBasedSimilarity}");
    }
  }
}
