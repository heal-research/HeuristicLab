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
  public class FastHypervolumeTest {
    /// <summary>
    ///  /*
    /// +-----+
    /// |     |
    /// |  x  |
    /// |     |
    /// +-----+
    /// 
    /// box between(0,0,0) and(1,1,1) with singular point pareto front at(0.5,0.5,0.5)
    /// Hypervolume should be 0.125;  
    /// 
    /// </summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void FastHypervolumeTestSinglePoint() {
      double[] point = new double[] { 0.5, 0.5, 0.5 };
      double[][] front = { point };
      double[] referencePoint = new double[] { 1, 1, 1 };
      double hv = Hypervolume.Calculate(front, referencePoint, new bool[3]);
      Assert.AreEqual(0.125, hv);
    }

    /// <summary>
    ///  /*
    /// +-----+
    /// | x   |
    /// |     |
    /// |     |
    /// +-----+
    /// 
    /// box between(0,0) and(1,1) with singular point pareto front at a random Location
    /// Sum of the Hypervolume to each of the corners should be 1;  
    /// 
    /// </summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void FastHypervolumeTestRandomSinglePoint() {
      //Front with a single Point
      double[] point = new double[3];
      var r = new System.Random();

      point[0] = r.NextDouble();
      point[1] = r.NextDouble();
      point[2] = r.NextDouble();
      double[][] front = { point };

      double[] referencePoint = new double[3];

      //Northeast
      referencePoint[0] = 1;
      referencePoint[1] = 1;
      referencePoint[2] = 1;
      double hv = Hypervolume.Calculate(front, referencePoint, new bool[3]);
      double hv2 = 1;
      foreach (double d in point) {
        hv2 *= Math.Abs(d - 1);
      }
      Assert.AreEqual(hv2, hv);
    }

    /// <summary>
    ///  /*
    /// x-----+
    /// |     |
    /// |  X  |
    /// |     |
    /// +-----x
    /// 
    /// box between(0,0,0) and(1,1,1) with three point (pareto) front at (1,0,0), (0.5,0.5,0)  and (0,1,0) 
    /// Hypervolume should be 0.25
    /// </summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void FastHypervolumeTestDiagonalPoint() {
      //Front with three points
      double[] point1 = new double[] { 1, 0, 0 };
      double[] point2 = new double[] { 0, 1, 0 };
      double[] point3 = new double[] { 0.5, 0.5, 0 };
      double[][] front = { point1, point2, point3 };

      double[] referencePoint = new double[] { 1, 1, 1 };
      double hv = Hypervolume.Calculate(front, referencePoint, new bool[3]);
      Assert.AreEqual(0.5, hv);
    }
  }
}
