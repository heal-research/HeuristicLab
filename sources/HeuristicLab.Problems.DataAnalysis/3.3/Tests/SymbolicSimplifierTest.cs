#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class SymbolicSimplifierTest {
    [DeploymentItem(@"RegressionSolution01.hl")]
    [DeploymentItem(@"RegressionSolution02.hl")]
    [DeploymentItem(@"RegressionSolution03.hl")]
    [DeploymentItem(@"RegressionSolution04.hl")]
    [DeploymentItem(@"RegressionSolution05.hl")]
    [DeploymentItem(@"RegressionSolution06.hl")]
    [TestMethod]
    public void SimplifyRegressionSolutionsTest() {
      ContentManager.Initialize(new PersistenceContentManager());

      {
        SymbolicRegressionSolution solution = LoadSolution("RegressionSolution01.hl");
        SymbolicRegressionSolution simplifiedSolution = SimplifySolution(solution);
        AssertEqualEnumerations(solution.EstimatedValues, simplifiedSolution.EstimatedValues);
      }
      {
        SymbolicRegressionSolution solution = LoadSolution("RegressionSolution02.hl");
        SymbolicRegressionSolution simplifiedSolution = SimplifySolution(solution);
        AssertEqualEnumerations(solution.EstimatedValues, simplifiedSolution.EstimatedValues);
      }
      {
        SymbolicRegressionSolution solution = LoadSolution("RegressionSolution03.hl");
        SymbolicRegressionSolution simplifiedSolution = SimplifySolution(solution);
        AssertEqualEnumerations(solution.EstimatedValues, simplifiedSolution.EstimatedValues);
      }
      {
        SymbolicRegressionSolution solution = LoadSolution("RegressionSolution04.hl");
        SymbolicRegressionSolution simplifiedSolution = SimplifySolution(solution);
        AssertEqualEnumerations(solution.EstimatedValues, simplifiedSolution.EstimatedValues);
      }
      {
        SymbolicRegressionSolution solution = LoadSolution("RegressionSolution05.hl");
        SymbolicRegressionSolution simplifiedSolution = SimplifySolution(solution);
        AssertEqualEnumerations(solution.EstimatedValues, simplifiedSolution.EstimatedValues);
      }
      {
        SymbolicRegressionSolution solution = LoadSolution("RegressionSolution06.hl");
        SymbolicRegressionSolution simplifiedSolution = SimplifySolution(solution);
        AssertEqualEnumerations(solution.EstimatedValues, simplifiedSolution.EstimatedValues);
      }
    }

    private SymbolicRegressionSolution LoadSolution(string fileName) {
      var doc = ContentManager.Load(fileName);
      Result result = doc as Result;
      if (result != null) {
        return (SymbolicRegressionSolution)result.Value;
      }
      SymbolicRegressionSolution solution = doc as SymbolicRegressionSolution;
      if (solution != null) {
        return solution;
      }
      Assert.Fail("Cannot load file " + fileName);
      throw new AssertFailedException();
    }

    private SymbolicRegressionSolution SimplifySolution(SymbolicRegressionSolution original) {
      SymbolicSimplifier simplifier = new SymbolicSimplifier();
      SymbolicExpressionTree simplifiedTree = simplifier.Simplify(original.Model.SymbolicExpressionTree);
      SymbolicRegressionModel simplifiedModel = new SymbolicRegressionModel(original.Model.Interpreter, simplifiedTree);
      return new SymbolicRegressionSolution(original.ProblemData, simplifiedModel, original.LowerEstimationLimit, original.UpperEstimationLimit);
    }

    private void AssertEqualEnumerations(IEnumerable<double> expected, IEnumerable<double> actual) {
      var expectedEnumerator = expected.GetEnumerator();
      var actualEnumerator = actual.GetEnumerator();
      while (expectedEnumerator.MoveNext() & actualEnumerator.MoveNext()) {
        Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current, Math.Abs(1E-6 * expectedEnumerator.Current));
      }
      if (expectedEnumerator.MoveNext() | actualEnumerator.MoveNext())
        Assert.Fail("Number of elements in enumerations do not match");
    }
  }
}
