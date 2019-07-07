using System;
using System.Diagnostics;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {
  [TestClass]
  public class BottomUpSimilarityCalculatorTest {
    private readonly SymbolicExpressionTreeBottomUpSimilarityCalculator similarityCalculator = new SymbolicExpressionTreeBottomUpSimilarityCalculator() { MatchConstantValues = false, MatchVariableWeights = false };
    private readonly SymbolicExpressionImporter importer = new SymbolicExpressionImporter();

    private const int N = 1000;
    private const int Rows = 1;
    private const int Columns = 10;

    public BottomUpSimilarityCalculatorTest() {
      var parser = new InfixExpressionParser();
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void BottomUpTreeSimilarityCalculatorTestMapping() {
      TestMatchedNodes("(+ 1 1)", "(+ 2 2)", 0, strict: true);
      TestMatchedNodes("(+ 1 1)", "(+ 2 2)", 3, strict: false);
      TestMatchedNodes("(+ 1 1)", "(+ 1 2)", 1, strict: true);
      TestMatchedNodes("(+ 2 1)", "(+ 1 2)", 3, strict: true);

      TestMatchedNodes("(- 1 1)", "(- 2 2)", 0, strict: true);
      TestMatchedNodes("(- 1 1)", "(- 2 2)", 3, strict: false);

      TestMatchedNodes("(- 2 1)", "(- 1 2)", 2, strict: true);
      TestMatchedNodes("(- 2 1)", "(- 1 2)", 3, strict: false);
    }

    private void TestMatchedNodes(string expr1, string expr2, int expected, bool strict) {
      var t1 = importer.Import(expr1);
      var t2 = importer.Import(expr2);

      var map = SymbolicExpressionTreeBottomUpSimilarityCalculator.ComputeBottomUpMapping(t1, t2, strict);

      if (map.Count != expected) {
        throw new Exception($"Match count {map.Count} is different than expected value {expected} for expressions:\n{expr1} and {expr2} (strict = {strict})");
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
  }
}
