#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  ///This is a test class for BoothEvaluatorTest and is intended
  ///to contain all BoothEvaluatorTest Unit Tests
  ///</summary>
  [TestClass()]
  public class BoothEvaluatorTest {
    /// <summary>
    ///A test for EvaluateFunction
    ///</summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions")]
    [TestProperty("Time", "short")]
    public void BoothEvaluateFunctionTest() {
      BoothEvaluator target = new BoothEvaluator();
      RealVector point = null;
      double expected = target.BestKnownQuality;
      double actual;
      for (int dimension = target.MinimumProblemSize; dimension <= System.Math.Min(10, target.MaximumProblemSize); dimension++) {
        point = target.GetBestKnownSolution(dimension);
        actual = target.Evaluate(point);
        Assert.AreEqual(expected, actual);
      }
    }
  }
}
