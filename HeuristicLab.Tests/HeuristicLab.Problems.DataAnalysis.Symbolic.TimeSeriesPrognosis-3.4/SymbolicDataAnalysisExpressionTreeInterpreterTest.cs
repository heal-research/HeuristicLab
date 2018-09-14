#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis.Symbolic.Tests;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Tests {

  [TestClass()]
  public class SymbolicTimeSeriesPrognosisInterpreterTest {
    private const int N = 1000;
    private const int Rows = 100;
    private const int Columns = 50;

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void TimeSeriesPrognosisInterpreterTestTypeCoherentGrammarPerformance() {
      TestTypeCoherentGrammarPerformance(new SymbolicTimeSeriesPrognosisExpressionTreeInterpreter("y"), 12.5e6);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void TimeSeriesPrognosisInterpreterTestFullGrammarPerformance() {
      TestFullGrammarPerformance(new SymbolicTimeSeriesPrognosisExpressionTreeInterpreter("y"), 12.5e6);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "long")]
    public void TimeSeriesPrognosisInterpreterTestArithmeticGrammarPerformance() {
      TestArithmeticGrammarPerformance(new SymbolicTimeSeriesPrognosisExpressionTreeInterpreter("y"), 12.5e6);
    }

    private void TestTypeCoherentGrammarPerformance(ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter interpreter, double nodesPerSecThreshold) {
      var twister = new MersenneTwister(31415);
      var dataset = Util.CreateRandomDataset(twister, Rows, Columns);
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();
      grammar.MaximumFunctionArguments = 0;
      grammar.MaximumFunctionDefinitions = 0;
      grammar.MinimumFunctionArguments = 0;
      grammar.MinimumFunctionDefinitions = 0;
      var randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 100, 0, 0);
      foreach (ISymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }
      double nodesPerSec = Util.CalculateEvaluatedNodesPerSec(randomTrees, interpreter, dataset, 3);
      //mkommend: commented due to performance issues on the builder
      //Assert.IsTrue(nodesPerSec > nodesPerSecThreshold); // evaluated nodes per seconds must be larger than 15mNodes/sec
    }

    private void TestFullGrammarPerformance(ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter interpreter, double nodesPerSecThreshold) {
      var twister = new MersenneTwister(31415);
      var dataset = Util.CreateRandomDataset(twister, Rows, Columns);
      var grammar = new FullFunctionalExpressionGrammar();
      grammar.MaximumFunctionArguments = 0;
      grammar.MaximumFunctionDefinitions = 0;
      grammar.MinimumFunctionArguments = 0;
      grammar.MinimumFunctionDefinitions = 0;
      var randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 100, 0, 0);
      foreach (ISymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }
      double nodesPerSec = Util.CalculateEvaluatedNodesPerSec(randomTrees, interpreter, dataset, 3);
      //mkommend: commented due to performance issues on the builder
      //Assert.IsTrue(nodesPerSec > nodesPerSecThreshold); // evaluated nodes per seconds must be larger than 15mNodes/sec
    }

    private void TestArithmeticGrammarPerformance(ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter interpreter, double nodesPerSecThreshold) {
      var twister = new MersenneTwister(31415);
      var dataset = Util.CreateRandomDataset(twister, Rows, Columns);
      var grammar = new ArithmeticExpressionGrammar();
      grammar.MaximumFunctionArguments = 0;
      grammar.MaximumFunctionDefinitions = 0;
      grammar.MinimumFunctionArguments = 0;
      grammar.MinimumFunctionDefinitions = 0;
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
    public void TimeSeriesPrognosisInterpreterTestEvaluation() {
      Dataset ds = new Dataset(new string[] { "Y", "A", "B" }, new double[,] {
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

      var interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
      EvaluateTerminals(interpreter, ds);
      EvaluateOperations(interpreter, ds);
      EvaluateAdf(interpreter, ds);
    }

    private void EvaluateTerminals(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, Dataset ds) {
      // constants
      Evaluate(interpreter, ds, "(+ 1.5 3.5)", 0, 5.0);

      // variables
      Evaluate(interpreter, ds, "(variable 2.0 a)", 0, 2.0);
      Evaluate(interpreter, ds, "(variable 2.0 a)", 1, 4.0);
    }

    private void EvaluateAdf(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, Dataset ds) {

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

    private void EvaluateOperations(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, Dataset ds) {
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

    private void Evaluate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, Dataset ds, string expr, int index, double expected) {
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
