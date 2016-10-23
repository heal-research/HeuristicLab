#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Encodings.RealVectorEncoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.TestFunctions.Tests {
  /// <summary>
  ///This is a test class for SumSquaresEvaluatorTest and is intended
  ///to contain all SumSquaresEvaluatorTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SumSquaresEvaluatorTest {
    /// <summary>
    ///A test for EvaluateFunction
    ///</summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions")]
    [TestProperty("Time", "short")]
    public void SumSquaresEvaluateFunctionTest() {
      var privateObject = new PrivateObject(typeof(SumSquaresEvaluator));
      RealVector point = null;
      double expected = (double)privateObject.GetProperty("BestKnownQuality");
      double actual;
      int minimumProblemSize = (int)privateObject.GetProperty("MinimumProblemSize");
      int maximumProblemSize = (int)privateObject.GetProperty("MaximumProblemSize");
      for (int dimension = minimumProblemSize; dimension <= System.Math.Min(10, maximumProblemSize); dimension++) {
        point = (RealVector)privateObject.Invoke("GetBestKnownSolution", dimension);
        actual = (double)privateObject.Invoke("Evaluate", point);
        Assert.AreEqual(expected, actual);
      }
    }
  }
}
