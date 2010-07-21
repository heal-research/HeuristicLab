#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Tests {


  /// <summary>
  ///This is a test class for SimpleArithmeticExpressionInterpreter and is intended
  ///to contain all SimpleArithmeticExpressionInterpreter Unit Tests
  ///</summary>
  [TestClass()]
  public class SimpleArithmeticExpressionInterpreterTest {
    private const int N = 1000;
    private const int Rows = 1000;
    private const int Columns = 50;
    private static SymbolicExpressionTree[] randomTrees;
    private static Dataset dataset;
    private static MersenneTwister twister;
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
    public static void CreateRandomTrees(TestContext testContext) {
      twister = new MersenneTwister();
      dataset = Util.CreateRandomDataset(twister, Rows, Columns);
      var grammar = new GlobalSymbolicExpressionGrammar(new FullFunctionalExpressionGrammar());
      grammar.MaxFunctionArguments = 0;
      grammar.MaxFunctionDefinitions = 0;
      grammar.MinFunctionArguments = 0;
      grammar.MinFunctionDefinitions = 0;
      randomTrees = Util.CreateRandomTrees(twister, dataset, grammar, N, 1, 100, 0, 0);
    }


    [TestMethod()]
    public void SimpleArithmeticExpressionInterpreterPerformanceTest() {
      double[] estimation = new double[Rows];
      foreach (SymbolicExpressionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }
      SimpleArithmeticExpressionInterpreter interpreter = new SimpleArithmeticExpressionInterpreter();
      Util.EvaluateTrees(randomTrees, interpreter, dataset, 10);
    }


    /// <summary>
    ///A test for Evaluate
    ///</summary>
    [TestMethod()]
    public void SimpleArithmeticExpressionInterpreterEvaluateTest() {

      Dataset ds = new Dataset(new string[] { "Y", "A", "B" }, new double[,] {
        { 1.0, 1.0, 1.0 },
        { 2.0, 2.0, 2.0 },
        { 3.0, 1.0, 2.0 }
      });

      SimpleArithmeticExpressionInterpreter interpreter = new SimpleArithmeticExpressionInterpreter();

      // constants
      Evaluate(interpreter, ds, "(+ 1.5 3.5)", 0, 5.0);

      // variables
      Evaluate(interpreter, ds, "(variable 2.0 a)", 0, 2.0);
      Evaluate(interpreter, ds, "(variable 2.0 a)", 1, 4.0);


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
      //Evaluate(interpreter, ds, "(> (sqrt -1.0) (log -1.0))", 0, -1.0); // (> nan nan) should be false

      // lt
      Evaluate(interpreter, ds, "(< (variable 2.0 a) 2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(< 2.0 (variable 2.0 a))", 0, -1.0);
      Evaluate(interpreter, ds, "(< (variable 2.0 a) 1.9)", 0, -1.0);
      Evaluate(interpreter, ds, "(< 1.9 (variable 2.0 a))", 0, 1.0);
      //Evaluate(interpreter, ds, "(< (sqrt -1,0) (log -1,0))", 0, -1.0); // (< nan nan) should be false

      // If
      Evaluate(interpreter, ds, "(if -10.0 2.0 3.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(if -1.0 2.0 3.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(if 0.0 2.0 3.0)", 0, 3.0);
      Evaluate(interpreter, ds, "(if 1.0 2.0 3.0)", 0, 2.0);
      Evaluate(interpreter, ds, "(if 10.0 2.0 3.0)", 0, 2.0);
      // Evaluate(interpreter, ds, "(if (sqrt -1.0) 2.0 3.0)", 0, 3.0); // if(nan) should return the else branch

      // NOT
      Evaluate(interpreter, ds, "(not -1.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(not -2.0)", 0, 2.0);
      Evaluate(interpreter, ds, "(not 1.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(not 2.0)", 0, -2.0);
      Evaluate(interpreter, ds, "(not 0.0)", 0, 0.0);

      // AND
      Evaluate(interpreter, ds, "(and -1.0 -2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and -1.0 2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 1.0 -2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 1.0 0.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 0.0 0.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(and 1.0 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(and 1.0 2.0 3.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(and 1.0 -2.0 3.0)", 0, -1.0);

      // OR
      Evaluate(interpreter, ds, "(or -1.0 -2.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(or -1.0 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(or 1.0 -2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(or 1.0 2.0)", 0, 1.0);
      Evaluate(interpreter, ds, "(or 0.0 0.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(or -1.0 -2.0 -3.0)", 0, -1.0);
      Evaluate(interpreter, ds, "(or -1.0 -2.0 3.0)", 0, 1.0);

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

      // mean
      Evaluate(interpreter, ds, "(mean -1.0 1.0 -1.0)", 0, -1.0 / 3.0);

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
      Evaluate(interpreter, ds, @"(PROG 
                                    (MAIN (CALL ADF1 (variable 2.0 a) 3.0))
                                    (defun ADF0 
                                      (- (ARG 1) (ARG 0)))
                                    (defun ADF1                                                                              
                                      (+ (CALL ADF0 (ARG 1) (ARG 0))
                                         (CALL ADF0 (ARG 0) (ARG 1)))))", 1, 0.0);
    }

    private void Evaluate(SimpleArithmeticExpressionInterpreter interpreter, Dataset ds, string expr, int index, double expected) {
      var importer = new SymbolicExpressionImporter();
      SymbolicExpressionTree tree = importer.Import(expr);

      double actual = interpreter.GetSymbolicExpressionTreeValues(tree, ds, Enumerable.Range(index, 1)).First();

      Assert.AreEqual(expected, actual, 1.0E-12, expr);
    }
  }
}
