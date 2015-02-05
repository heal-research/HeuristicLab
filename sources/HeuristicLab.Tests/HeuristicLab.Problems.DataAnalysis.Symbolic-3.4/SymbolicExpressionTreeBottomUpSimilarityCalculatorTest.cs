using System;
using System.Diagnostics;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {
  [TestClass]
  public class BottomUpSimilarityCalculatorTest {
    private readonly SymbolicExpressionTreeBottomUpSimilarityCalculator busCalculator;
    private readonly SymbolicExpressionImporter importer;

    private const int N = 150;
    private const int Rows = 1;
    private const int Columns = 10;

    public BottomUpSimilarityCalculatorTest() {
      busCalculator = new SymbolicExpressionTreeBottomUpSimilarityCalculator { MatchConstantValues = true, MatchVariableWeights = true };
      importer = new SymbolicExpressionImporter();
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void BottomUpTreeSimilarityCalculatorTestMapping() {
      TestMatchedNodes("(+ 1 2)", "(+ 2 1)", 5);
      TestMatchedNodes("(- 2 1)", "(- 1 2)", 2);
      TestMatchedNodes("(* (variable 1 X1) (variable 1 X2))", "(* (+ (variable 1 X1) 1) (+ (variable 1 X2) 1))", 2);

      TestMatchedNodes("(* (variable 1 X1) (variable 1 X2))", "(* (+ (variable 1 X1) 1) (variable 1 X2))", 2);

      TestMatchedNodes("(+ (variable 1 a) (variable 1 b))", "(+ (variable 1 a) (variable 1 a))", 1);
      TestMatchedNodes("(+ (+ (variable 1 a) (variable 1 b)) (variable 1 b))", "(+ (* (+ (variable 1 a) (variable 1 b)) (variable 1 b)) (+ (+ (variable 1 a) (variable 1 b)) (variable 1 b)))", 5);

      TestMatchedNodes(
        "(* (+ 2.84 (exp (+ (log (/ (variable 2.0539 X5) (variable -9.2452e-1 X6))) (/ (variable 2.0539 X5) (variable -9.2452e-1 X6))))) 2.9081)",
        "(* (- (variable 9.581e-1 X6) (+ (- (variable 5.1491e-1 X5) 1.614e+1) (+ (/ (variable 2.0539 X5) (variable -9.2452e-1 X6)) (log (/ (variable 2.0539 X5) (variable -9.2452e-1 X6)))))) 2.9081)",
        9);

      TestMatchedNodes("(+ (exp 2.1033) (/ -4.3072 (variable 2.4691 X7)))", "(/ 1 (+ (/ -4.3072 (variable 2.4691 X7)) (exp 2.1033)))", 6);
      TestMatchedNodes("(+ (exp 2.1033) (/ -4.3072 (variable 2.4691 X7)))", "(/ 1 (+ (/ (variable 2.4691 X7) -4.3072) (exp 2.1033)))", 4);

      const string expr1 = "(* (- 1.2175e+1 (+ (/ (exp -1.4134e+1) (exp 9.2013)) (exp (log (exp (/ (exp (- (* -4.2461 (variable 2.2634 X5)) (- -9.6267e-1 3.3243))) (- (/ (/ (variable 1.0883 X1) (variable 6.9620e-1 X2)) (log 1.3011e+1)) (variable -4.3098e-1 X7)))))))) (log 1.3011e+1))";
      const string expr2 = "(* (- 1.2175e+1 (+ (/ (/ (+ (variable 3.0140 X9) (variable 1.3430 X8)) -1.0864e+1) (exp 9.2013)) (exp (log (exp (/ (exp (- (* -4.2461 (variable 2.2634 X5)) (- -9.6267e-1 3.3243))) (- (/ (/ (variable 1.0883 X1) (variable 6.9620e-1 X2)) (log 1.3011e+1)) (variable -4.3098e-1 X7)))))))) (exp (variable 4.0899e-1 X7)))";

      TestMatchedNodes(expr1, expr2, 23);

    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    private void TestMatchedNodes(string expr1, string expr2, int expected) {
      var t1 = importer.Import(expr1);
      var t2 = importer.Import(expr2);

      var mapping = busCalculator.ComputeBottomUpMapping(t1.Root, t2.Root);
      var c = mapping.Count;

      if (c != expected) {
        throw new Exception("Match count " + c + " is different than expected value " + expected);
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
          s += busCalculator.CalculateSimilarity(trees[i], trees[j]);
        }
      }

      sw.Stop();
      Console.WriteLine("Elapsed time: " + sw.ElapsedMilliseconds / 1000.0 + ", Avg. similarity: " + s / (N * (N - 1) / 2));
      Console.WriteLine(N * (N + 1) / (2 * sw.ElapsedMilliseconds / 1000.0) + " similarity calculations per second.");
    }
  }
}
