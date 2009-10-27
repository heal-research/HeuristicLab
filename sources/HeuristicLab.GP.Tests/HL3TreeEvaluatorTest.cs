#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.GP.StructureIdentification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.GP.Interfaces;
using System.IO;
using HeuristicLab.DataAnalysis;
using System;
using HeuristicLab.Random;
using System.Collections.Generic;
using HeuristicLab.GP.Operators;
using System.Diagnostics;
namespace HeuristicLab.GP.Test {


  /// <summary>
  ///This is a test class for HL3TreeEvaluatorTest and is intended
  ///to contain all HL3TreeEvaluatorTest Unit Tests
  ///</summary>
  [TestClass()]
  public class HL3TreeEvaluatorTest {
    private const int N = 1000;
    private const int Rows = 1000;
    private const int Columns = 50;
    private static IFunctionTree[] randomTrees;
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
      randomTrees = Util.CreateRandomTrees(twister, dataset, N, 1, 100);
    }


    [TestMethod()]
    public void PerformanceTest() {
      double[] estimation = new double[Rows];
      foreach (IFunctionTree tree in randomTrees) {
        Util.InitTree(tree, twister, new List<string>(dataset.VariableNames));
      }
      HL3TreeEvaluator hl3TreeEvaluator = new HL3TreeEvaluator();
      Util.EvaluateTrees(randomTrees, hl3TreeEvaluator, dataset, 10);
    }


    /// <summary>
    ///A test for Evaluate
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.GP.StructureIdentification-3.3.dll")]
    public void HL3TreeEvaluatorEvaluateTest() {

      Dataset ds = new Dataset(new double[,] {
        { 1.0, 1.0, 1.0 },
        { 2.0, 2.0, 2.0 },
        { 3.0, 1.0, 2.0 }
      });

      ds.SetVariableName(0, "y");
      ds.SetVariableName(1, "a");
      ds.SetVariableName(2, "b");

      HL3TreeEvaluator evaluator = new HL3TreeEvaluator();

      // constants
      Evaluate(evaluator, ds, "(+ 1,5 3,5)", 0, 5.0);

      // variables
      Evaluate(evaluator, ds, "(variable 2,0 a 0)", 0, 2.0);
      Evaluate(evaluator, ds, "(variable 2,0 a 0)", 1, 4.0);
      Evaluate(evaluator, ds, "(variable 2,0 a -1)", 1, 2.0);
      Evaluate(evaluator, ds, "(variable 2,0 a -2)", 2, 2.0);

      // differentials
      Evaluate(evaluator, ds, "(differential 2,0 a 0)", 1, 2.0);
      Evaluate(evaluator, ds, "(differential 2,0 a 0)", 2, -2.0);
      Evaluate(evaluator, ds, "(differential 2,0 a -1)", 2, 2.0);
      Evaluate(evaluator, ds, "(differential 2,0 a -2)", 3, 2.0);

      // addition
      Evaluate(evaluator, ds, "(+ (variable 2,0 a 0))", 1, 4.0);
      Evaluate(evaluator, ds, "(+ (variable 2,0 a 0) (variable 3,0 b 0))", 0, 5.0);
      Evaluate(evaluator, ds, "(+ (variable 2,0 a 0) (variable 3,0 b 0))", 1, 10.0);
      Evaluate(evaluator, ds, "(+ (variable 2,0 a 0) (variable 3,0 b 0))", 2, 8.0);
      Evaluate(evaluator, ds, "(+ 8,0 2,0 2,0)", 0, 12.0);

      // subtraction
      Evaluate(evaluator, ds, "(- (variable 2,0 a 0))", 1, -4.0);
      Evaluate(evaluator, ds, "(- (variable 2,0 a 0) (variable 3,0 b 0))", 0, -1.0);
      Evaluate(evaluator, ds, "(- (variable 2,0 a 0) (variable 3,0 b 0))", 1, -2.0);
      Evaluate(evaluator, ds, "(- (variable 2,0 a 0) (variable 3,0 b 0))", 2, -4.0);
      Evaluate(evaluator, ds, "(- 8,0 2,0 2,0)", 0, 4.0);

      // multiplication
      Evaluate(evaluator, ds, "(* (variable 2,0 a 0))", 0, 2.0);
      Evaluate(evaluator, ds, "(* (variable 2,0 a 0) (variable 3,0 b 0))", 0, 6.0);
      Evaluate(evaluator, ds, "(* (variable 2,0 a 0) (variable 3,0 b 0))", 1, 24.0);
      Evaluate(evaluator, ds, "(* (variable 2,0 a 0) (variable 3,0 b 0))", 2, 12.0);
      Evaluate(evaluator, ds, "(* 8,0 2,0 2,0)", 0, 32.0);

      // division
      Evaluate(evaluator, ds, "(/ (variable 2,0 a 0))", 1, 1.0 / 4.0);
      Evaluate(evaluator, ds, "(/ (variable 2,0 a 0) 2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(/ (variable 2,0 a 0) 2,0)", 1, 2.0);
      Evaluate(evaluator, ds, "(/ (variable 3,0 b 0) 2,0)", 2, 3.0);
      Evaluate(evaluator, ds, "(/ 8,0 2,0 2,0)", 0, 2.0);

      // boolean values semantic: false = x <= 0 , true x > 0
      // equ
      Evaluate(evaluator, ds, "(equ (variable 2,0 a 0) 2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(equ (variable 2,0 a 0) 1,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(equ (sqrt -1,0) (log -1,0))", 0, -1.0); // (equ nan nan) should be false


      // gt
      Evaluate(evaluator, ds, "(> (variable 2,0 a 0) 2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(> 2,0 (variable 2,0 a 0))", 0, -1.0);
      Evaluate(evaluator, ds, "(> (variable 2,0 a 0) 1,9)", 0, 1.0);
      Evaluate(evaluator, ds, "(> 1,9 (variable 2,0 a 0))", 0, -1.0);
      Evaluate(evaluator, ds, "(> (sqrt -1,0) (log -1,0))", 0, -1.0); // (> nan nan) should be false

      // lt
      Evaluate(evaluator, ds, "(< (variable 2,0 a 0) 2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(< 2,0 (variable 2,0 a 0))", 0, -1.0);
      Evaluate(evaluator, ds, "(< (variable 2,0 a 0) 1,9)", 0, -1.0);
      Evaluate(evaluator, ds, "(< 1,9 (variable 2,0 a 0))", 0, 1.0);
      Evaluate(evaluator, ds, "(< (sqrt -1,0) (log -1,0))", 0, -1.0); // (< nan nan) should be false

      // If
      Evaluate(evaluator, ds, "(if -10 2,0 3,0)", 0, 3.0);
      Evaluate(evaluator, ds, "(if -1,0 2,0 3,0)", 0, 3.0);
      Evaluate(evaluator, ds, "(if 0,0 2,0 3,0)", 0, 3.0);
      Evaluate(evaluator, ds, "(if 1,0 2,0 3,0)", 0, 2.0);
      Evaluate(evaluator, ds, "(if 10 2,0 3,0)", 0, 2.0);
      Evaluate(evaluator, ds, "(if (sqrt -1,0) 2,0 3,0)", 0, 3.0); // if(nan) should return the else branch

      // signum
      Evaluate(evaluator, ds, "(sign -1,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(sign -2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(sign 1,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(sign 2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(sign 0,0)", 0, 0.0);

      // NOT
      Evaluate(evaluator, ds, "(not -1,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(not -2,0)", 0, 2.0);
      Evaluate(evaluator, ds, "(not 1,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(not 2,0)", 0, -2.0);
      Evaluate(evaluator, ds, "(not 0,0)", 0, 0.0);

      // AND
      Evaluate(evaluator, ds, "(and -1,0 -2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(and -1,0 2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(and 1,0 -2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(and 1,0 0,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(and 0,0 0,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(and 1,0 2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(and 1,0 2,0 3,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(and 1,0 -2,0 3,0)", 0, -1.0);

      // OR
      Evaluate(evaluator, ds, "(or -1,0 -2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(or -1,0 2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(or 1,0 -2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(or 1,0 2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(or 0,0 0,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(or -1,0 -2,0 -3,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(or -1,0 -2,0 3,0)", 0, 1.0);

      // XOR
      Evaluate(evaluator, ds, "(xor -1,0 -2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(xor -1,0 2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(xor 1,0 -2,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(xor 1,0 2,0)", 0, -1.0);
      Evaluate(evaluator, ds, "(xor 0,0 0,0)", 0, -1.0);

      // sin, cos, tan
      Evaluate(evaluator, ds, "(sin " + Math.PI + ")", 0, 0.0);
      Evaluate(evaluator, ds, "(sin 0,0)", 0, 0.0);
      Evaluate(evaluator, ds, "(cos " + Math.PI + ")", 0, -1.0);
      Evaluate(evaluator, ds, "(cos 0,0)", 0, 1.0);
      Evaluate(evaluator, ds, "(tan " + Math.PI + ")", 0, Math.Tan(Math.PI));
      Evaluate(evaluator, ds, "(tan 0,0)", 0, Math.Tan(Math.PI));


      // expt
      Evaluate(evaluator, ds, "(expt 2,0 2,0)", 0, 4.0);
      Evaluate(evaluator, ds, "(expt 2,0 3,0)", 0, 8.0);
      Evaluate(evaluator, ds, "(expt 3,0 2,0)", 0, 9.0);

      // exp, log
      Evaluate(evaluator, ds, "(log (exp 7,0))", 0, Math.Log(Math.Exp(7)));
      Evaluate(evaluator, ds, "(exp (log 7,0))", 0, Math.Exp(Math.Log(7)));
      Evaluate(evaluator, ds, "(log -3,0)", 0, Math.Log(-3));

      // sqrt
      Evaluate(evaluator, ds, "(sqrt 4,0)", 0, 2.0);
      Evaluate(evaluator, ds, "(sqrt (sqrt 16,0))", 0, 2.0);

    }

    private void Evaluate(HL3TreeEvaluator evaluator, Dataset ds, string expr, int index, double expected) {
      var importer = new SymbolicExpressionImporter();
      IFunctionTree tree = importer.Import(expr);

      evaluator.PrepareForEvaluation(ds, tree);

      double actual = evaluator.Evaluate(index);
      Assert.AreEqual(expected, actual, 1.0E-12, expr);
    }
  }
}
