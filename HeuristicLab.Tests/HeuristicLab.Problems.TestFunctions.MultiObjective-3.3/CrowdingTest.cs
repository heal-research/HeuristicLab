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
  public class CrowdingTest {

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void CrowdingTestEmptyFront() {
      double[][] front = { };

      Crowding.Calculate(front, null);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void CrowdingTestSamePoint() {

      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;

      double[] point1 = new double[2];
      point1[0] = 0.5;
      point1[1] = 0.5;
      double[][] front = { point, point1 };
      double dist = Crowding.Calculate(front, new double[,] { { 0, 1 }, { 0, 1 } });
      Assert.AreEqual(double.PositiveInfinity, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void CrowdingTestSinglePoint() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[][] front = { point };
      double dist = Crowding.Calculate(front, new double[,] { { 0, 1 }, { 0, 1 } });
      Assert.AreEqual(double.PositiveInfinity, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void CrowdingTestDiagonal() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 0.5;
      point1[1] = 0.5;

      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 1;
      double[][] front = { point, point1, point2 };
      double dist = Crowding.Calculate(front, new double[,] { { 0, 1 }, { 0, 1 } });
      Assert.AreEqual(2, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void CrowdingTestDiamond() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 1;
      point1[1] = 1.5;

      double[] point2 = new double[2];
      point2[0] = 3;
      point2[1] = 0.5;
      double[] point3 = new double[2];
      point3[0] = 4;
      point3[1] = 2;
      double[][] front = { point, point1, point2, point3 };
      double dist = Crowding.Calculate(front, new double[,] { { 0, 4 }, { 0, 2 } });
      Assert.AreEqual(1.5, dist);
    }

    /// <summary> 
    /// deltoid with 4 points of the 1-unit-square and the northeastern point at 4,4
    /// 
    /// </summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void CrowdingTestDeltoid() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 0.00000001;         //points should not be exactly equal because sorting behaviour could change result
      point1[1] = 1.00000001;

      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 0;
      double[] point3 = new double[2];
      point3[0] = 4;
      point3[1] = 4;


      double[][] front = { point, point1, point2, point3, };
      double dist = Crowding.Calculate(front, new double[,] { { 0, 4 }, { 0, 4 } });
      Assert.AreEqual(1.25, dist);
    }


  }


}
