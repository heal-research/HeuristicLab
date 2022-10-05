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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective.Tests {
  [TestClass]
  public class SpacingTest {
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void SpacingTestEmptyFront() {
      double[][] front = { };

      Spacing.Calculate(front);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void SpacingTestSamePoint() {

      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;

      double[] point1 = new double[2];
      point1[0] = 0.5;
      point1[1] = 0.5;
      double[][] front = { point, point1 };
      double dist = Spacing.Calculate(front);
      Assert.AreEqual(0, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void SpacingTestSinglePoint() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[][] front = { point };
      double dist = Spacing.Calculate(front);
      Assert.AreEqual(0, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void SpacingTestQuadratic() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 0;
      point1[1] = 1;

      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 0;
      double[] point3 = new double[2];
      point3[0] = 1;
      point3[1] = 1;
      double[][] front = { point, point1, point2, point3 };
      double dist = Spacing.Calculate(front);
      Assert.AreEqual(0, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void SpacingTestRectangular() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 0;
      point1[1] = 1;

      double[] point2 = new double[2];
      point2[0] = 2;
      point2[1] = 0;
      double[] point3 = new double[2];
      point3[0] = 2;
      point3[1] = 1;
      double[][] front = { point, point1, point2, point3 };
      double dist = Spacing.Calculate(front);
      Assert.AreEqual(0, dist);
    }

    /// <summary> 
    /// deltoid with 3 points of the 1-unit-square and the northeastern point at 4,4
    /// which gives d=(1,1,1,5) for minimal distances. Mean of d is 2 and variance of d is 3
    /// Spacing is defined as the standarddeviation of d
    /// </summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void SpacingTestDeltoid() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 0;
      point1[1] = 1;

      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 0;
      double[] point3 = new double[2];
      point3[0] = 4;
      point3[1] = 4;
      double[][] front = { point, point1, point2, point3 };
      double dist = Spacing.Calculate(front);
      Assert.AreEqual(Math.Sqrt(3), dist);
    }
  }
}
