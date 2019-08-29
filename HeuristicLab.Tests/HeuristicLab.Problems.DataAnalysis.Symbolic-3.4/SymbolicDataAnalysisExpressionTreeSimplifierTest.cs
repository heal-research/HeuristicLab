#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {

  [TestClass()]
  public class SymbolicDataAnalysisExpressionTreeSimplifierTest {

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SimplifierAxiomsTest() {
      SymbolicExpressionImporter importer = new SymbolicExpressionImporter();
      SymbolicExpressionTreeStringFormatter formatter = new SymbolicExpressionTreeStringFormatter();
      #region single argument arithmetics

      AssertEqualAfterSimplification("(+ 1.0)", "1.0");
      AssertEqualAfterSimplification("(- 1.0)", "-1.0");
      AssertEqualAfterSimplification("(- (variable 2.0 a))", "(variable -2.0 a)");
      AssertEqualAfterSimplification("(* 2.0)", "2.0");
      AssertEqualAfterSimplification("(* (variable 2.0 a))", "(variable 2.0 a)");
      AssertEqualAfterSimplification("(/ 2.0)", "0.5");
      AssertEqualAfterSimplification("(/ (variable 2.0 a))", "(/ 1.0 (variable 2.0 a))");
      #endregion

      #region aggregation of constants into factors
      AssertEqualAfterSimplification("(* 2.0 (variable 2.0 a))", "(variable 4.0 a)");
      AssertEqualAfterSimplification("(/ (variable 2.0 a) 2.0)", "(variable 1.0 a)");
      AssertEqualAfterSimplification("(/ (variable 2.0 a) (* 2.0 2.0))", "(variable 0.5 a)");
      #endregion

      #region constant and variable folding
      AssertEqualAfterSimplification("(+ 1.0 2.0)", "3.0");
      AssertEqualAfterSimplification("(+ (variable 2.0 a) (variable 2.0 a))", "(variable 4.0 a)");
      AssertEqualAfterSimplification("(- (variable 2.0 a) (variable 1.0 a))", "(variable 1.0 a)");
      AssertEqualAfterSimplification("(* (variable 2.0 a) (variable 2.0 a))", "(* (* (variable 1.0 a) (variable 1.0 a)) 4.0)");
      AssertEqualAfterSimplification("(/ (variable 1.0 a) (variable 2.0 a))", "0.5");
      #endregion

      #region logarithm rules

      // cancellation
      AssertEqualAfterSimplification("(log (exp (variable 2.0 a)))", "(variable 2.0 a)");
      // must not transform logs in this way as we do not know wether both variables are positive
      AssertEqualAfterSimplification("(log (* (variable 1.0 a) (variable 1.0 b)))", "(log (* (variable 1.0 a) (variable 1.0 b)))");
      // must not transform logs in this way as we do not know wether both variables are positive
      AssertEqualAfterSimplification("(log (/ (variable 1.0 a) (variable 1.0 b)))", "(log (/ (variable 1.0 a) (variable 1.0 b)))");
      #endregion

      #region exponentiation rules
      // cancellation
      AssertEqualAfterSimplification("(exp (log (variable 2.0 a)))", "(variable 2.0 a)");
      // exp transformation
      AssertEqualAfterSimplification("(exp (+ (variable 2.0 a) (variable 3.0 b)))", "(* (exp (variable 2.0 a)) (exp (variable 3.0 b)))");
      // exp transformation
      AssertEqualAfterSimplification("(exp (- (variable 2.0 a) (variable 3.0 b)))", "(* (exp (variable 2.0 a)) (exp (variable -3.0 b)))");
      // exp transformation
      AssertEqualAfterSimplification("(exp (- (variable 2.0 a) (* (variable 3.0 b) (variable 4.0 c))))", "(* (exp (variable 2.0 a)) (exp (* (variable 1.0 b) (variable 1.0 c) -12.0)))");
      // exp transformation
      AssertEqualAfterSimplification("(exp (- (variable 2.0 a) (* (variable 3.0 b) (cos (variable 4.0 c)))))", "(* (exp (variable 2.0 a)) (exp (* (variable 1.0 b) (cos (variable 4.0 c)) -3.0)))");
      #endregion

      #region power rules

      // cancellation
      AssertEqualAfterSimplification("(pow (variable 2.0 a) 0.0)", "1.0");
      // fixed point
      AssertEqualAfterSimplification("(pow (variable 2.0 a) 1.0)", "(variable 2.0 a)");
      // inversion fixed point
      AssertEqualAfterSimplification("(pow (variable 2.0 a) -1.0)", "(/ 1.0 (variable 2.0 a))");
      // inversion 
      AssertEqualAfterSimplification("(pow (variable 2.0 a) -2.0)", "(/ 1.0 (pow (variable 2.0 a) 2.0))");
      // constant folding
      AssertEqualAfterSimplification("(pow 3.0 2.0)", "9.0");
      #endregion

      #region root rules
      // cancellation
      AssertEqualAfterSimplification("(root (variable 2.0 a) 0.0)", "1.0");
      // fixed point
      AssertEqualAfterSimplification("(root (variable 2.0 a) 1.0)", "(variable 2.0 a)");
      // inversion fixed point
      AssertEqualAfterSimplification("(root (variable 2.0 a) -1.0)", "(/ 1.0 (variable 2.0 a))");
      // inversion
      AssertEqualAfterSimplification("(root (variable 2.0 a) -2.0)", "(/ 1.0 (root (variable 2.0 a) 2.0))");
      // constant folding
      AssertEqualAfterSimplification("(root 9.0 2.0)", "3.0");
      #endregion

      #region boolean operations
      // always true and
      AssertEqualAfterSimplification("(and 1.0 2.0)", "1.0");
      // always false and
      AssertEqualAfterSimplification("(and 1.0 -2.0)", "-1.0");
      // always true or
      AssertEqualAfterSimplification("(or -1.0 2.0)", "1.0");
      // always false or
      AssertEqualAfterSimplification("(or -1.0 -2.0)", "-1.0");
      // constant not 
      AssertEqualAfterSimplification("(not -2.0)", "1.0");
      // constant not 
      AssertEqualAfterSimplification("(not 2.0)", "-1.0");
      // constant not 
      AssertEqualAfterSimplification("(not 0.0)", "1.0");
      // nested nots
      AssertEqualAfterSimplification("(not (not 1.0))", "1.0");
      // not of non-Boolean argument
      AssertEqualAfterSimplification("(not (variable 1.0 a))", "(not (> (variable 1.0 a) 0.0))");
      // not Boolean argument
      AssertEqualAfterSimplification("(not (and (> (variable 1.0 a) 0.0) (> (variable 1.0 a) 0.0)))", "(not (and (> (variable 1.0 a) 0.0) (> (variable 1.0 a) 0.0)))");
      #endregion

      #region conditionals
      // always false
      AssertEqualAfterSimplification("(if -1.0 (variable 2.0 a) (variable 3.0 a))", "(variable 3.0 a)");
      // always true
      AssertEqualAfterSimplification("(if 1.0 (variable 2.0 a) (variable 3.0 a))", "(variable 2.0 a)");
      // always false (0.0)
      AssertEqualAfterSimplification("(if 0.0 (variable 2.0 a) (variable 3.0 a))", "(variable 3.0 a)");
      // complex constant condition (always false)
      AssertEqualAfterSimplification("(if (* 1.0 -2.0) (variable 2.0 a) (variable 3.0 a))", "(variable 3.0 a)");
      // complex constant condition (always false)
      AssertEqualAfterSimplification("(if (/ (variable 1.0 a) (variable -2.0 a)) (variable 2.0 a) (variable 3.0 a))", "(variable 3.0 a)");
      // insertion of relational operator
      AssertEqualAfterSimplification("(if (variable 1.0 a) (variable 2.0 a) (variable 3.0 a))", "(if (> (variable 1.0 a) 0.0) (variable 2.0 a) (variable 3.0 a))");
      #endregion

      #region factor variables
      AssertEqualAfterSimplification("(factor a 1.0)", "(factor a 1.0)");
      // factor folding
      AssertEqualAfterSimplification("(+ (factor a 1.0 1.0) (factor a 2.0 3.0))", "(factor a 3.0 4.0)");
      AssertEqualAfterSimplification("(- (factor a 1.0 1.0) (factor a 2.0 3.0))", "(factor a -1.0 -2.0)");
      AssertEqualAfterSimplification("(* (factor a 2.0 2.0) (factor a 2.0 3.0))", "(factor a 4.0 6.0)");
      AssertEqualAfterSimplification("(/ (factor a 2.0 5.0))", "(factor a 0.5 0.2)");
      AssertEqualAfterSimplification("(/ (factor a 4.0 6.0) (factor a 2.0 3.0))", "(factor a 2.0 2.0)");
      AssertEqualAfterSimplification("(+ 3.0 (factor a 4.0 6.0))", "(factor a 7.0 9.0)");
      AssertEqualAfterSimplification("(+ (factor a 4.0 6.0) 3.0)", "(factor a 7.0 9.0)");
      AssertEqualAfterSimplification("(- 3.0 (factor a 4.0 6.0))", "(factor a -1.0 -3.0)");
      AssertEqualAfterSimplification("(- (factor a 4.0 6.0) 3.0)", "(factor a 1.0 3.0)");
      AssertEqualAfterSimplification("(* 2.0 (factor a 4.0 6.0))", "(factor a 8.0 12.0)");
      AssertEqualAfterSimplification("(* (factor a 4.0 6.0) 2.0)", "(factor a 8.0 12.0)");
      AssertEqualAfterSimplification("(* (factor a 4.0 6.0) (variable 2.0 a))", "(* (factor a 8.0 12.0) (variable 1.0 a))"); // not possible (a is used as factor and double variable) interpreter will fail
      AssertEqualAfterSimplification(
        "(log (factor a 10.0 100.0))",
        string.Format(CultureInfo.InvariantCulture, "(factor a {0} {1})", Math.Log(10.0), Math.Log(100.0)));
      AssertEqualAfterSimplification(
        "(exp (factor a 2.0 3.0))",
        string.Format(CultureInfo.InvariantCulture, "(factor a {0} {1})", Math.Exp(2.0), Math.Exp(3.0)));
      AssertEqualAfterSimplification("(sqrt (factor a 9.0 16.0))", "(factor a 3.0 4.0))");
      AssertEqualAfterSimplification("(sqr (factor a 2.0 3.0))", "(factor a 4.0 9.0))");
      AssertEqualAfterSimplification("(root (factor a 8.0 27.0) 3)", "(factor a 2.0 3.0))");
      AssertEqualAfterSimplification("(pow (factor a 2.0 3.0) 3)", "(factor a 8.0 27.0))");

      AssertEqualAfterSimplification("(sin (factor a 1.0 2.0) )",
        string.Format(CultureInfo.InvariantCulture, "(factor a {0} {1}))", Math.Sin(1.0), Math.Sin(2.0)));
      AssertEqualAfterSimplification("(cos (factor a 1.0 2.0) )",
        string.Format(CultureInfo.InvariantCulture, "(factor a {0} {1}))", Math.Cos(1.0), Math.Cos(2.0)));
      AssertEqualAfterSimplification("(tan (factor a 1.0 2.0) )",
        string.Format(CultureInfo.InvariantCulture, "(factor a {0} {1}))", Math.Tan(1.0), Math.Tan(2.0)));


      AssertEqualAfterSimplification("(binfactor a val 1.0)", "(binfactor a val 1.0)");
      // binfactor folding
      AssertEqualAfterSimplification("(+ (binfactor a val 1.0) (binfactor a val 2.0))", "(binfactor a val 3.0)");
      AssertEqualAfterSimplification("(+ (binfactor a val0 1.0) (binfactor a val1 2.0))", "(+ (binfactor a val0 1.0) (binfactor a val1 2.0))"); // cannot be simplified (different vals)
      AssertEqualAfterSimplification("(+ (binfactor a val 1.0) (binfactor b val 2.0))", "(+ (binfactor a val 1.0) (binfactor b val 2.0))"); // cannot be simplified (different vars)
      AssertEqualAfterSimplification("(- (binfactor a val 1.0) (binfactor a val 2.0))", "(binfactor a val -1.0)");
      AssertEqualAfterSimplification("(* (binfactor a val 2.0) (binfactor a val 3.0))", "(binfactor a val 6.0)");
      AssertEqualAfterSimplification("(/ (binfactor a val 6.0) (binfactor a val 3.0))", "(/ (binfactor a val 6.0) (binfactor a val 3.0))"); // not allowed! 0/0 for other values than 'val'
      AssertEqualAfterSimplification("(/ (binfactor a val 4.0))", "(/ 1.0 (binfactor a val 4.0))"); // not allowed!

      AssertEqualAfterSimplification("(+ 3.0 (binfactor a val 4.0 ))", "(+ (binfactor a val 4.0 ) 3.0))"); // not allowed
      AssertEqualAfterSimplification("(- 3.0 (binfactor a val 4.0 ))", "(+ (binfactor a val -4.0 ) 3.0)"); 
      AssertEqualAfterSimplification("(+ (binfactor a val 4.0 ) 3.0)", "(+ (binfactor a val 4.0 ) 3.0)");  // not allowed
      AssertEqualAfterSimplification("(- (binfactor a val 4.0 ) 3.0)", "(+ (binfactor a val 4.0 ) -3.0)"); 
      AssertEqualAfterSimplification("(* 2.0 (binfactor a val 4.0))", "(binfactor a val 8.0 )");
      AssertEqualAfterSimplification("(* (binfactor a val 4.0) 2.0)", "(binfactor a val 8.0 )");
      AssertEqualAfterSimplification("(* (binfactor a val 4.0) (variable 2.0 a))", "(* (binfactor a val 1.0) (variable 1.0 a) 8.0)");   // not possible (a is used as factor and double variable) interpreter will fail
      AssertEqualAfterSimplification("(log (binfactor a val 10.0))", "(log (binfactor a val 10.0))"); // not allowed (log(0))

      // exp( binfactor w val=a) = if(val=a) exp(w) else exp(0) = binfactor( (exp(w) - 1) val a) + 1
      AssertEqualAfterSimplification("(exp (binfactor a val 3.0))", 
        string.Format(CultureInfo.InvariantCulture, "(+ (binfactor a val {0}) 1.0)", Math.Exp(3.0) - 1)
        ); 
      AssertEqualAfterSimplification("(sqrt (binfactor a val 16.0))", "(binfactor a val 4.0))"); // sqrt(0) = 0
      AssertEqualAfterSimplification("(sqr (binfactor a val 3.0))", "(binfactor a val 9.0))"); // 0*0 = 0
      AssertEqualAfterSimplification("(root (binfactor a val 27.0) 3)", "(binfactor a val 3.0))");
      AssertEqualAfterSimplification("(pow (binfactor a val 3.0) 3)", "(binfactor a val 27.0))");

      AssertEqualAfterSimplification("(sin (binfactor a val 2.0) )",
        string.Format(CultureInfo.InvariantCulture, "(binfactor a val {0}))", Math.Sin(2.0))); // sin(0) = 0
      AssertEqualAfterSimplification("(cos (binfactor a val 2.0) )", 
        string.Format(CultureInfo.InvariantCulture, "(+ (binfactor a val {0}) 1.0)", Math.Cos(2.0) - 1)); // cos(0) = 1
      AssertEqualAfterSimplification("(tan (binfactor a val 2.0) )",
        string.Format(CultureInfo.InvariantCulture, "(binfactor a val {0}))", Math.Tan(2.0))); // tan(0) = 0

      // combination of factor and binfactor
      AssertEqualAfterSimplification("(+ (binfactor a x0 2.0) (factor a 2.0 3.0))", "(factor a 4.0 3.0)");
      AssertEqualAfterSimplification("(+ (factor a 2.0 3.0) (binfactor a x0 2.0))", "(factor a 4.0 3.0)");
      AssertEqualAfterSimplification("(* (binfactor a x1 2.0) (factor a 2.0 3.0))", "(binfactor a x1 6.0)"); // all other values have weight zero in binfactor
      AssertEqualAfterSimplification("(* (factor a 2.0 3.0) (binfactor a x1 2.0))", "(binfactor a x1 6.0)"); // all other values have weight zero in binfactor
      AssertEqualAfterSimplification("(/ (binfactor a x0 2.0) (factor a 2.0 3.0))", "(binfactor a x0 1.0)");
      AssertEqualAfterSimplification("(/ (factor a 2.0 3.0) (binfactor a x0 2.0))",
        string.Format(CultureInfo.InvariantCulture, "(factor a 1.0 {0})", 3.0 / 0.0));
      AssertEqualAfterSimplification("(- (binfactor a x0 2.0) (factor a 2.0 3.0))", "(factor a 0.0 -3.0)");
      AssertEqualAfterSimplification("(- (factor a 2.0 3.0) (binfactor a x0 2.0))", "(factor a 0.0 3.0)");
      #endregion

      #region abs
      AssertEqualAfterSimplification("(abs 2.0)", "2.0");
      AssertEqualAfterSimplification("(abs -2.0)", "2.0"); // constant folding
      AssertEqualAfterSimplification("(abs (exp (variable 2.0 x)))", "(exp (variable 2.0 x)))"); // exp is always positive
      AssertEqualAfterSimplification("(abs (exp (variable 2.0 x)))", "(exp (variable 2.0 x)))"); // exp is always positive
      AssertEqualAfterSimplification("(abs (sqr (variable 2.0 a)))", "(sqr (variable 2.0 a))"); // sqr is always positive
      AssertEqualAfterSimplification("(abs (sqrt (variable 2.0 a)))", "(sqrt (variable 2.0 a))"); // sqrt is always positive (for our cases)
      AssertEqualAfterSimplification("(abs (cuberoot (variable 2.0 a)))", "(cuberoot (variable 2.0 a))"); // cuberoot is always positive (for our cases)

      AssertEqualAfterSimplification("(* (abs (variable 2.0 x)) 2.0)", "(abs (variable 4.0 x))");  // can multiply positive constants into abs
      AssertEqualAfterSimplification("(* (abs (variable 2.0 x)) -2.0)", "(* (abs (variable 4.0 x)) -1.0)"); // for negative constants keep the sign

      AssertEqualAfterSimplification("(abs (* (variable 1.0 a) (variable 2.0 b)))", "(* (abs (variable 1.0 a)) (abs (variable 1.0 b)) 2.0))");
      AssertEqualAfterSimplification("(abs (/ (variable 1.0 a) (variable 2.0 b)))", "(/ (abs (variable 1.0 a)) (abs (variable 2.0 b))))");
      #endregion

      #region square and sqrt
      AssertEqualAfterSimplification("(sqr (sqrt (variable 2.0 x)))", "(variable 2.0 x)");
      AssertEqualAfterSimplification("(sqrt (sqr (variable 2.0 x)))", "(variable 2.0 x)");
      AssertEqualAfterSimplification("(sqr (abs (variable 2.0 a)))", "(sqr (variable 2.0 a))");
      AssertEqualAfterSimplification("(sqr (exp (variable 2.0 x)))", "(exp (variable 4.0 x))");

      AssertEqualAfterSimplification("(sqr (* (variable 2.0 a) (variable 3.0 b) (variable 4.0 c)))",
        "(* (sqr (variable 1.0 a)) (sqr (variable 1.0 b)) (sqr (variable 1.0 c)) 576)"); // 2²*3²*4²
      #endregion

      #region cube and cuberoot
      AssertEqualAfterSimplification("(cube (cuberoot (variable 2.0 x)))", "(variable 2.0 x)");
      AssertEqualAfterSimplification("(cuberoot (cube (variable 2.0 x)))", "(variable 2.0 x)");
      AssertEqualAfterSimplification("(cube (exp (variable 2.0 x)))", "(exp (variable 6.0 x))");

      AssertEqualAfterSimplification("(sqr (cube (variable 2.0 x)))", "(pow (variable 2.0 x) 6)");
      AssertEqualAfterSimplification("(cube (sqr (variable 2.0 x)))", "(pow (variable 2.0 x) 6)");
      #endregion

      #region AQ
      AssertEqualAfterSimplification("(* (aq (variable 1.0 x) (variable 1.0 y)) 2.0)", "(aq (variable 2.0 x) (variable 1.0 y))");
      AssertEqualAfterSimplification("(/ (aq (variable 1.0 x) (variable 1.0 y)) 2.0)", "(aq (variable 0.5 x) (variable 1.0 y))");

      #endregion
    }


    private void AssertEqualAfterSimplification(string original, string expected) {
      var formatter = new SymbolicExpressionTreeStringFormatter();
      var importer = new SymbolicExpressionImporter();
      var actualTree = TreeSimplifier.Simplify(importer.Import(original));
      var expectedTree = importer.Import(expected);
      Assert.AreEqual(formatter.Format(expectedTree), formatter.Format(actualTree));

    }
  }
}

