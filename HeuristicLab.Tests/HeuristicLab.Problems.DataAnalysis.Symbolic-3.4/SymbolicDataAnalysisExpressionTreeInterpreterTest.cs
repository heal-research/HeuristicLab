#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {


  [TestClass]
  public class SymbolicDataAnalysisExpressionTreeInterpreterTest {
    private const int N = 1000;
    private const int Rows = 1000;
    private const int Columns = 50;

    private static Dataset ds = new Dataset(new string[] { "Y", "A", "B" }, new double[,] {
        { 1.0, 1.0, 1.0 },
        { 2.0, 2.0, 2.0 },
        { 3.0, 1.0, 2.0 },
        { 4.0, 1.0, 1.0 },
        { 5.0, 2.0, 2.0 },
        { 6.0, 1.0, 2.0 },
        { 7.0, 1.0, 1.0 },
        { 8.0, 2.0, 2.0 },
        { 9.0, 1.0, 2.0 },
        { 10.0, 1.0, 1.0 },
        { 11.0, 2.0, 2.0 },
        { 12.0, 1.0, 2.0 }
      });

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void StandardInterpreterTestTypeCoherentGrammarPerformance() {
      TestTypeCoherentGrammarPerformance(new SymbolicDataAnalysisExpressionTreeInterpreter(), 12.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void StandardInterpreterTestFullGrammarPerformance() {
      TestFullGrammarPerformance(new SymbolicDataAnalysisExpressionTreeInterpreter(), 12.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void StandardInterpreterTestArithmeticGrammarPerformance() {
      TestArithmeticGrammarPerformance(new SymbolicDataAnalysisExpressionTreeInterpreter(), 12.5e6);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void CompiledInterpreterTestTypeCoherentGrammarPerformance() {
      TestTypeCoherentGrammarPerformance(new SymbolicDataAnalysisExpressionCompiledTreeInterpreter(), 12.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void CompiledInterpreterTestFullGrammarPerformance() {
      TestFullGrammarPerformance(new SymbolicDataAnalysisExpressionCompiledTreeInterpreter(), 12.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void CompiledInterpreterTestArithmeticGrammarPerformance() {
      TestArithmeticGrammarPerformance(new SymbolicDataAnalysisExpressionCompiledTreeInterpreter(), 12.5e6);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void ILEmittingInterpreterTestTypeCoherentGrammarPerformance() {
      TestTypeCoherentGrammarPerformance(new SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(), 7.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void ILEmittingInterpreterTestArithmeticGrammarPerformance() {
      TestArithmeticGrammarPerformance(new SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(), 7.5e6);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void LinearInterpreterTestTypeCoherentGrammarPerformance() {
      TestTypeCoherentGrammarPerformance(new SymbolicDataAnalysisExpressionTreeLinearInterpreter(), 12.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void LinearInterpreterTestFullGrammarPerformance() {
      TestFullGrammarPerformance(new SymbolicDataAnalysisExpressionTreeLinearInterpreter(), 12.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void LinearInterpreterTestArithmeticGrammarPerformance() {
      TestArithmeticGrammarPerformance(new SymbolicDataAnalysisExpressionTreeLinearInterpreter(), 12.5e6);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void BatchInterpreterTestTypeCoherentGrammarPerformance() {
      TestTypeCoherentGrammarPerformance(new SymbolicDataAnalysisExpressionTreeBatchInterpreter(), 12.5e6);
    }
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void BatchInterpreterTestArithmeticGrammarPerformance() {
      TestArithmeticGrammarPerformance(new SymbolicDataAnalysisExpressionTreeBatchInterpreter(), 12.5e6);
    }

    private void TestTypeCoherentGrammarPerformance(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double nodesPerSecThreshold) {
      var twister = new MersenneTwister(31415);
      var dataset = Util.CreateRandomDataset(twister, Rows, Columns);

      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();

      var randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 100, 0, 0);
      foreach (ISymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }
      double nodesPerSec = Util.CalculateEvaluatedNodesPerSec(randomTrees, interpreter, dataset, 3);
      //mkommend: commented due to performance issues on the builder
      // Assert.IsTrue(nodesPerSec > nodesPerSecThreshold); // evaluated nodes per seconds must be larger than 15mNodes/sec
    }

    private void TestFullGrammarPerformance(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double nodesPerSecThreshold) {
      var twister = new MersenneTwister(31415);
      var dataset = Util.CreateRandomDataset(twister, Rows, Columns);

      var grammar = new FullFunctionalExpressionGrammar();
      var randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 100, 0, 0);
      foreach (ISymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }
      double nodesPerSec = Util.CalculateEvaluatedNodesPerSec(randomTrees, interpreter, dataset, 3);
      //mkommend: commented due to performance issues on the builder
      //Assert.IsTrue(nodesPerSec > nodesPerSecThreshold); // evaluated nodes per seconds must be larger than 15mNodes/sec
    }

    private void TestArithmeticGrammarPerformance(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double nodesPerSecThreshold) {
      var twister = new MersenneTwister(31415);
      var dataset = Util.CreateRandomDataset(twister, Rows, Columns);

      var grammar = new ArithmeticExpressionGrammar();
      var randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 100, 0, 0);
      foreach (SymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }

      double nodesPerSec = Util.CalculateEvaluatedNodesPerSec(randomTrees, interpreter, dataset, 3);
      //mkommend: commented due to performance issues on the builder
      //Assert.IsTrue(nodesPerSec > nodesPerSecThreshold); // evaluated nodes per seconds must be larger than 15mNodes/sec
    }


    /// <summary>
    ///A test for Evaluate
    ///</summary>
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void StandardInterpreterTestEvaluation() {
      var interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
      EvaluateTerminals(interpreter, ds);
      EvaluateOperations(interpreter, ds);
      EvaluateLaggedOperations(interpreter, ds);
      EvaluateSpecialFunctions(interpreter, ds);
      EvaluateAdf(interpreter, ds);
    }

    /// <summary>
    ///A test for Evaluate
    ///</summary>
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void ILEmittingInterpreterTestEvaluation() {
      var interpreter = new SymbolicDataAnalysisExpressionTreeILEmittingInterpreter();
      EvaluateTerminals(interpreter, ds);
      EvaluateOperations(interpreter, ds);
      EvaluateLaggedOperations(interpreter, ds);
      EvaluateSpecialFunctions(interpreter, ds);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void CompiledInterpreterTestEvaluation() {
      var interpreter = new SymbolicDataAnalysisExpressionCompiledTreeInterpreter();
      EvaluateTerminals(interpreter, ds);
      EvaluateOperations(interpreter, ds);
      EvaluateSpecialFunctions(interpreter, ds);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void LinearInterpreterTestEvaluation() {
      var interpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();
      //ADFs are not supported by the linear interpreter
      EvaluateTerminals(interpreter, ds);
      EvaluateOperations(interpreter, ds);
      EvaluateLaggedOperations(interpreter, ds);
      EvaluateSpecialFunctions(interpreter, ds);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void TestInterpretersEstimatedValuesConsistency() {
      var twister = new MersenneTwister();
      int seed = twister.Next(0, int.MaxValue);
      twister.Seed((uint)seed);
      const int numRows = 100;
      var dataset = Util.CreateRandomDataset(twister, numRows, Columns);

      var grammar = new TypeCoherentExpressionGrammar();

      var interpreters = new ISymbolicDataAnalysisExpressionTreeInterpreter[] {
        new SymbolicDataAnalysisExpressionTreeLinearInterpreter(),
        new SymbolicDataAnalysisExpressionTreeInterpreter(),
      };

      var rows = Enumerable.Range(0, numRows).ToList();
      var randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 10, 0, 0);
      foreach (ISymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }

      for (int i = 0; i < randomTrees.Length; ++i) {
        var tree = randomTrees[i];
        var valuesMatrix = interpreters.Select(x => x.GetSymbolicExpressionTreeValues(tree, dataset, rows)).ToList();
        for (int m = 0; m < interpreters.Length - 1; ++m) {
          var sum = valuesMatrix[m].Sum();
          for (int n = m + 1; n < interpreters.Length; ++n) {
            var s = valuesMatrix[n].Sum();
            if (double.IsNaN(sum) && double.IsNaN(s)) continue;

            string errorMessage = string.Format("Interpreters {0} and {1} do not agree on tree {2} (seed = {3}).", interpreters[m].Name, interpreters[n].Name, i, seed);
            Assert.AreEqual(sum, s, 1e-12, errorMessage);
          }
        }
      }
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void TestCompiledInterpreterEstimatedValuesConsistency() {
      const double delta = 1e-12;

      var twister = new MersenneTwister();
      int seed = twister.Next(0, int.MaxValue);
      twister.Seed((uint)seed);

      Console.WriteLine(seed);

      const int numRows = 100;
      var dataset = Util.CreateRandomDataset(twister, numRows, Columns);

      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();
      grammar.Symbols.First(x => x.Name == "Power Functions").Enabled = true;

      var randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 10, 0, 0);
      foreach (ISymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }

      var interpreters = new ISymbolicDataAnalysisExpressionTreeInterpreter[] {
        new SymbolicDataAnalysisExpressionCompiledTreeInterpreter(),
        new SymbolicDataAnalysisExpressionTreeInterpreter(),
        new SymbolicDataAnalysisExpressionTreeLinearInterpreter(),
      };
      var rows = Enumerable.Range(0, numRows).ToList();
      var formatter = new SymbolicExpressionTreeHierarchicalFormatter();

      for (int i = 0; i < randomTrees.Length; ++i) {
        var tree = randomTrees[i];
        var valuesMatrix = interpreters.Select(x => x.GetSymbolicExpressionTreeValues(tree, dataset, rows).ToList()).ToList();
        for (int m = 0; m < interpreters.Length - 1; ++m) {
          for (int n = m + 1; n < interpreters.Length; ++n) {
            for (int row = 0; row < numRows; ++row) {
              var v1 = valuesMatrix[m][row];
              var v2 = valuesMatrix[n][row];
              if (double.IsNaN(v1) && double.IsNaN(v2)) continue;
              if (Math.Abs(v1 - v2) > delta) {
                Console.WriteLine(formatter.Format(tree));
                foreach (var node in tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix().ToList()) {
                  var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
                  if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(twister);
                  rootNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

                  var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
                  if (startNode.HasLocalParameters) startNode.ResetLocalParameters(twister);
                  startNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

                  rootNode.AddSubtree(startNode);
                  var t = new SymbolicExpressionTree(rootNode);
                  var start = t.Root.GetSubtree(0);
                  var p = node.Parent;
                  start.AddSubtree(node);
                  Console.WriteLine(node);

                  var y1 = interpreters[m].GetSymbolicExpressionTreeValues(t, dataset, new[] { row }).First();
                  var y2 = interpreters[n].GetSymbolicExpressionTreeValues(t, dataset, new[] { row }).First();

                  if (double.IsNaN(y1) && double.IsNaN(y2)) continue;
                  string prefix = Math.Abs(y1 - y2) > delta ? "++" : "==";
                  Console.WriteLine("\t{0} Row {1}: {2} {3}, Deviation = {4}", prefix, row, y1, y2, Math.Abs(y1 - y2));
                  node.Parent = p;
                }
              }
              string errorMessage = string.Format("Interpreters {0} and {1} do not agree on tree {2} and row {3} (seed = {4}).", interpreters[m].Name, interpreters[n].Name, i, row, seed);
              Assert.AreEqual(v1, v2, delta, errorMessage);
            }
          }
        }
      }
    }

    private void EvaluateTerminals(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataset ds) {
      // constants
      Evaluate(interpreter, ds, "(+ 1.5 3.5)", 0, 5.0);

      // variables
      Evaluate(interpreter, ds, "(variable 2.0 a)", 0, 2.0);
      Evaluate(interpreter, ds, "(variable 2.0 a)", 1, 4.0);
    }

    private void EvaluateAdf(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataset ds) {

      // ADF      
      Evaluate(interpreter, ds, @"(PROG 
                                    (MAIN 
                                      (CALL ADF0)) 
                                    (defun ADF0 1.0))", 1, 1.0);
      Evaluate(interpreter, ds, @"(PROG 
                                    (MAIN 
                                      (* (CALL ADF0) (CALL ADF0)))
                                    (defun ADF0 2.0))", 1, 4.0);
      Evaluate(interpreter, ds, @"(PROG 
                                    (MAIN 
                                      (CALL ADF0 2.0 3.0))
                                    (defun ADF0 
                                      (+ (ARG 0) (ARG 1))))", 1, 5.0);
      Evaluate(interpreter, ds, @"(PROG 
                                    (MAIN (CALL ADF1 2.0 3.0))
                                    (defun ADF0 
                                      (- (ARG 1) (ARG 0)))
                                    (defun ADF1
                                      (+ (CALL ADF0 (ARG 1) (ARG 0))
                                         (CALL ADF0 (ARG 0) (ARG 1)))))", 1, 0.0);
      Evaluate(interpreter, ds, @"(PROG 
                                    (MAIN (CALL ADF1 (variable 2.0 a) 3.0))
                                    (defun ADF0 
                                      (- (ARG 1) (ARG 0)))
                                    (defun ADF1                                                                              
                                      (CALL ADF0 (ARG 1) (ARG 0))))", 1, 1.0);
      Evaluate(interpreter, ds,
               @"(PROG 
                                    (MAIN (CALL ADF1 (variable 2.0 a) 3.0))
                                    (defun ADF0 
                                      (- (ARG 1) (ARG 0)))
                                    (defun ADF1                                                                              
                                      (+ (CALL ADF0 (ARG 1) (ARG 0))
                                         (CALL ADF0 (ARG 0) (ARG 1)))))", 1, 0.0);
    }

    private void EvaluateSpecialFunctions(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataset ds) {
      // special functions
      Action<double> checkAiry = (x) => {
        double ai, aip, bi, bip;
        alglib.airy(x, out ai, out aip, out bi, out bip);
        Evaluate(interpreter, ds, "(airya " + x + ")", 0, ai);
        Evaluate(interpreter, ds, "(airyb " + x + ")", 0, bi);
      };

      Action<double> checkBessel = (x) => {
        Evaluate(interpreter, ds, "(bessel " + x + ")", 0, alglib.besseli0(x));
      };

      Action<double> checkSinCosIntegrals = (x) => {
        double si, ci;
        alglib.sinecosineintegrals(x, out si, out ci);
        Evaluate(interpreter, ds, "(cosint " + x + ")", 0, ci);
        Evaluate(interpreter, ds, "(sinint " + x + ")", 0, si);
      };
      Action<double> checkHypSinCosIntegrals = (x) => {
        double shi, chi;
        alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
        Evaluate(interpreter, ds, "(hypcosint " + x + ")", 0, chi);
        Evaluate(interpreter, ds, "(hypsinint " + x + ")", 0, shi);
      };
      Action<double> checkFresnelSinCosIntegrals = (x) => {
        double c = 0, s = 0;
        alglib.fresnelintegral(x, ref c, ref s);
        Evaluate(interpreter, ds, "(fresnelcosint " + x + ")", 0, c);
        Evaluate(interpreter, ds, "(fresnelsinint " + x + ")", 0, s);
      };
      Action<double> checkNormErf = (x) => {
        Evaluate(interpreter, ds, "(norm " + x + ")", 0, alglib.normaldistribution(x));
        Evaluate(interpreter, ds, "(erf " + x + ")", 0, alglib.errorfunction(x));
      };

      Action<double> checkGamma = (x) => {
        Evaluate(interpreter, ds, "(gamma " + x + ")", 0, alglib.gammafunction(x));
      };
      Action<double> checkPsi = (x) => {
        try {
          Evaluate(interpreter, ds, "(psi " + x + ")", 0, alglib.psi(x));
        } catch (alglib.alglibexception) { // ignore cases where alglib throws an exception
        }
      };
      Action<double> checkDawson = (x) => {
        Evaluate(interpreter, ds, "(dawson " + x + ")", 0, alglib.dawsonintegral(x));
      };
      Action<double> checkExpInt = (x) => {
        Evaluate(interpreter, ds, "(expint " + x + ")", 0, alglib.exponentialintegralei(x));
      };

      foreach (var e in new[] { -2.0, -1.0, 0.0, 1.0, 2.0 }) {
        checkAiry(e);
        checkBessel(e);
        checkSinCosIntegrals(e);
        checkGamma(e);
        checkExpInt(e);
        checkDawson(e);
        checkPsi(e);
        checkNormErf(e);
        checkFresnelSinCosIntegrals(e);
        checkHypSinCosIntegrals(e);
      }
    }

    private void EvaluateLaggedOperations(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataset ds) {
      // lag
      Evaluate(interpreter, ds, "(lagVariable 1.0 a -1) ", 1, ds.GetDoubleValue("A", 0));
      Evaluate(interpreter, ds, "(lagVariable 1.0 a -1) ", 2, ds.GetDoubleValue("A", 1));
      Evaluate(interpreter, ds, "(lagVariable 1.0 a 0) ", 2, ds.GetDoubleValue("A", 2));
      Evaluate(interpreter, ds, "(lagVariable 1.0 a 1) ", 0, ds.GetDoubleValue("A", 1));

      // integral
      Evaluate(interpreter, ds, "(integral -1.0 (variable 1.0 a)) ", 1, ds.GetDoubleValue("A", 0) + ds.GetDoubleValue("A", 1));
      Evaluate(interpreter, ds, "(integral -1.0 (lagVariable 1.0 a 1)) ", 1, ds.GetDoubleValue("A", 1) + ds.GetDoubleValue("A", 2));
      Evaluate(interpreter, ds, "(integral -2.0 (variable 1.0 a)) ", 2, ds.GetDoubleValue("A", 0) + ds.GetDoubleValue("A", 1) + ds.GetDoubleValue("A", 2));
      Evaluate(interpreter, ds, "(integral -1.0 (* (variable 1.0 a) (variable 1.0 b)))", 1, ds.GetDoubleValue("A", 0) * ds.GetDoubleValue("B", 0) + ds.GetDoubleValue("A", 1) * ds.GetDoubleValue("B", 1));
      Evaluate(interpreter, ds, "(integral -2.0 3.0)", 1, 9.0);

      // derivative
      // (f_0 + 2 * f_1 - 2 * f_3 - f_4) / 8; // h = 1
      Evaluate(interpreter, ds, "(diff (variable 1.0 a)) ", 5, (ds.GetDoubleValue("A", 5) + 2 * ds.GetDoubleValue("A", 4) - 2 * ds.GetDoubleValue("A", 2) - ds.GetDoubleValue("A", 1)) / 8.0);
      Evaluate(interpreter, ds, "(diff (variable 1.0 b)) ", 5, (ds.GetDoubleValue("B", 5) + 2 * ds.GetDoubleValue("B", 4) - 2 * ds.GetDoubleValue("B", 2) - ds.GetDoubleValue("B", 1)) / 8.0);
      Evaluate(interpreter, ds, "(diff (* (variable 1.0 a) (variable 1.0 b)))", 5, +
        (ds.GetDoubleValue("A", 5) * ds.GetDoubleValue("B", 5) +
        2 * ds.GetDoubleValue("A", 4) * ds.GetDoubleValue("B", 4) -
        2 * ds.GetDoubleValue("A", 2) * ds.GetDoubleValue("B", 2) -
        ds.GetDoubleValue("A", 1) * ds.GetDoubleValue("B", 1)) / 8.0);
      Evaluate(interpreter, ds, "(diff -2.0 3.0)", 5, 0.0);

      // timelag
      Evaluate(interpreter, ds, "(lag -1.0 (lagVariable 1.0 a 2)) ", 1, ds.GetDoubleValue("A", 2));
      Evaluate(interpreter, ds, "(lag -2.0 (lagVariable 1.0 a 2)) ", 2, ds.GetDoubleValue("A", 2));
      Evaluate(interpreter, ds, "(lag -1.0 (* (lagVariable 1.0 a 1) (lagVariable 1.0 b 2)))", 1, ds.GetDoubleValue("A", 1) * ds.GetDoubleValue("B", 2));
      Evaluate(interpreter, ds, "(lag -2.0 3.0)", 1, 3.0);
    }

    private void EvaluateOperations(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataset ds) {
      // addition
      Evaluate(interpreter, ds, "(+ (variable 2.0 a ))", 1, 4.0);
      Evaluate(interpreter, ds, "(+ (variable 2.0 a ) (variable 3.0 b ))", 0, 5.0);
      Evaluate(interpreter, ds, "(+ (variable 2.0 a ) (variable 3.0 b ))", 1, 10.0);
      Evaluate(interpreter, ds, "(+ (variable 2.0 a) (variable 3.0 b ))", 2, 8.0);
      Evaluate(interpreter, ds, "(+ 8.0 2.0 2.0)", 0, 12.0);

      // subtraction
      Evaluate(interpreter, ds, "(- (variable 2.0 a ))", 1, -4.0);
      Evaluate(interpreter, ds, "(- (variable 2.0 a ) (variable 3.0 b))", 0, -1.0);
      Evaluate(interpreter, ds, "(- (variable 2.0 a ) (variable 3.0 b ))", 1, -2.0);
      Evaluate(interpreter, ds, "(- (variable 2.0 a ) (variable 3.0 b ))", 2, -4.0);
      Evaluate(interpreter, ds, "(- 8.0 2.0 2.0)", 0, 4.0);

      // multiplication
      Evaluate(interpreter, ds, "(* (variable 2.0 a ))", 0, 2.0);
      Evaluate(interpreter, ds, "(* (variable 2.0 a ) (variable 3.0 b ))", 0, 6.0);
      Evaluate(interpreter, ds, "(* (variable 2.0 a ) (variable 3.0 b ))", 1, 24.0);
      Evaluate(interpreter, ds, "(* (variable 2.0 a ) (variable 3.0 b ))", 2, 12.0);
      Evaluate(interpreter, ds, "(* 8.0 2.0 2.0)", 0, 32.0);

      // division
      Evaluate(interpreter, ds, "(/ (variable 2.0 a ))", 1, 1.0 / 4.0);
      Evaluate(interpreter, ds, "(/ (variable 2.0 a ) 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(/ (variable 2.0 a ) 2.0)", 1, 2.0);
      Evaluate(interpreter, ds, "(/ (variable 3.0 b ) 2.0)", 2, 3.0);
      Evaluate(interpreter, ds, "(/ 8.0 2.0 2.0)", 0, 2.0);

      // gt
      Evaluate(interpreter, ds, "(> (variable 2.0 a) 2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(> 2.0 (variable 2.0 a))", 0, -1.0);
      Evaluate(interpreter, ds, "(> (variable 2.0 a) 1.9)", 0, 1.0);
      Evaluate(interpreter, ds, "(> 1.9 (variable 2.0 a))", 0, -1.0);
      Evaluate(interpreter, ds, "(> (log -1.0) (log -1.0))", 0, -1.0); // (> nan nan) should be false

      // lt
      Evaluate(interpreter, ds, "(< (variable 2.0 a) 2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(< 2.0 (variable 2.0 a))", 0, -1.0);
      Evaluate(interpreter, ds, "(< (variable 2.0 a) 1.9)", 0, -1.0);
      Evaluate(interpreter, ds, "(< 1.9 (variable 2.0 a))", 0, 1.0);
      Evaluate(interpreter, ds, "(< (log -1.0) (log -1.0))", 0, -1.0); // (< nan nan) should be false

      // If
      Evaluate(interpreter, ds, "(if -10.0 2.0 3.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(if -1.0 2.0 3.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(if 0.0 2.0 3.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(if 1.0 2.0 3.0)", 0, 2.0);
      Evaluate(interpreter, ds, "(if 10.0 2.0 3.0)", 0, 2.0);
      Evaluate(interpreter, ds, "(if (log -1.0) 2.0 3.0)", 0, 3.0); // if(nan) should return the else branch

      // NOT
      Evaluate(interpreter, ds, "(not -1.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(not -2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(not 1.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(not 2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(not 0.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(not (log -1.0))", 0, 1.0);

      // AND
      Evaluate(interpreter, ds, "(and -1.0 -2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and -1.0 2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 1.0 -2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 1.0 0.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 0.0 0.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 1.0 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(and 1.0 2.0 3.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(and 1.0 -2.0 3.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and (log -1.0))", 0, -1.0); // (and NaN)
      Evaluate(interpreter, ds, "(and (log -1.0)  1.0)", 0, -1.0); // (and NaN 1.0)

      // OR
      Evaluate(interpreter, ds, "(or -1.0 -2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(or -1.0 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(or 1.0 -2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(or 1.0 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(or 0.0 0.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(or -1.0 -2.0 -3.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(or -1.0 -2.0 3.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(or (log -1.0))", 0, -1.0); // (or NaN)
      Evaluate(interpreter, ds, "(or (log -1.0)  1.0)", 0, -1.0); // (or NaN 1.0)

      // XOR
      Evaluate(interpreter, ds, "(xor -1.0 -2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(xor -1.0 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(xor 1.0 -2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(xor 1.0 2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(xor 0.0 0.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(xor -1.0 -2.0 -3.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(xor -1.0 -2.0 3.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(xor -1.0 2.0 3.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(xor 1.0 2.0 3.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(xor (log -1.0))", 0, -1.0);
      Evaluate(interpreter, ds, "(xor (log -1.0)  1.0)", 0, 1.0);

      // sin, cos, tan
      Evaluate(interpreter, ds, "(sin " + Math.PI.ToString(NumberFormatInfo.InvariantInfo) + ")", 0, 0.0);
      Evaluate(interpreter, ds, "(sin 0.0)", 0, 0.0);
      Evaluate(interpreter, ds, "(cos " + Math.PI.ToString(NumberFormatInfo.InvariantInfo) + ")", 0, -1.0);
      Evaluate(interpreter, ds, "(cos 0.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(tan " + Math.PI.ToString(NumberFormatInfo.InvariantInfo) + ")", 0, Math.Tan(Math.PI));
      Evaluate(interpreter, ds, "(tan 0.0)", 0, Math.Tan(Math.PI));

      // exp, log
      Evaluate(interpreter, ds, "(log (exp 7.0))", 0, Math.Log(Math.Exp(7)));
      Evaluate(interpreter, ds, "(exp (log 7.0))", 0, Math.Exp(Math.Log(7)));
      Evaluate(interpreter, ds, "(log -3.0)", 0, Math.Log(-3));

      // power
      Evaluate(interpreter, ds, "(pow 2.0 3.0)", 0, 8.0);
      Evaluate(interpreter, ds, "(pow 4.0 0.5)", 0, 1.0); // interpreter should round to the nearest integer value value (.5 is rounded to the even number)
      Evaluate(interpreter, ds, "(pow 4.0 2.5)", 0, 16.0); // interpreter should round to the nearest integer value value (.5 is rounded to the even number)
      Evaluate(interpreter, ds, "(pow -2.0 3.0)", 0, -8.0);
      Evaluate(interpreter, ds, "(pow 2.0 -3.0)", 0, 1.0 / 8.0);
      Evaluate(interpreter, ds, "(pow -2.0 -3.0)", 0, -1.0 / 8.0);

      // root
      Evaluate(interpreter, ds, "(root 9.0 2.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(root 27.0 3.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(root 2.0 -3.0)", 0, Math.Pow(2.0, -1.0 / 3.0));

      // mean
      Evaluate(interpreter, ds, "(mean -1.0 1.0 -1.0)", 0, -1.0 / 3.0);
    }

    private void Evaluate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataset ds, string expr, int index, double expected) {
      var importer = new SymbolicExpressionImporter();
      ISymbolicExpressionTree tree = importer.Import(expr);

      double actual = interpreter.GetSymbolicExpressionTreeValues(tree, ds, Enumerable.Range(index, 1)).First();

      Assert.IsFalse(double.IsNaN(actual) && !double.IsNaN(expected));
      Assert.IsFalse(!double.IsNaN(actual) && double.IsNaN(expected));
      if (!double.IsNaN(actual) && !double.IsNaN(expected))
        Assert.AreEqual(expected, actual, 1.0E-12, expr);
    }
  }
}
