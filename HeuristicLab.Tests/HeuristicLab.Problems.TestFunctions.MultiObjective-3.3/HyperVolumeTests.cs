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
  public class HypervolumeTest {

    /// <summary>
    ///  /*
    /// +-----+
    /// |     |
    /// |  x  |
    /// |     |
    /// +-----+
    /// 
    /// box between(0,0) and(1,1) with singular point pareto front at(0.5,0.5)
    /// Hypervolume to each of the corners should be 0.25;  
    /// 
    /// </summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void HypervolumeTestSinglePoint() {

      //Front with a single Point
      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { point };

      double[] referencePoint = new double[2];
      bool[] maximization;

      //Northeast
      maximization = new bool[] { false, false };
      referencePoint[0] = 1;
      referencePoint[1] = 1;
      double ne = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, ne);

      //NorthWest
      maximization = new bool[] { true, false };
      referencePoint[0] = 0;
      referencePoint[1] = 1;
      double nw = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, nw);

      //SouthWest
      maximization = new bool[] { true, true };
      referencePoint[0] = 0;
      referencePoint[1] = 0;
      double sw = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, sw);

      //SouthEast
      maximization = new bool[] { false, true };
      referencePoint[0] = 1;
      referencePoint[1] = 0;
      double se = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, se);


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
    public void HypervolumeTestRandomSinglePoint() {
      //Front with a single Point
      double[] point = new double[2];
      var r = new System.Random();

      point[0] = r.NextDouble();
      point[1] = r.NextDouble();
      double[][] front = { point };

      double[] referencePoint = new double[2];
      bool[] maximization;

      //Northeast
      maximization = new bool[] { false, false };
      referencePoint[0] = 1;
      referencePoint[1] = 1;
      double ne = Hypervolume.Calculate(front, referencePoint, maximization);

      //NorthWest
      maximization = new bool[] { true, false };
      referencePoint[0] = 0;
      referencePoint[1] = 1;
      double nw = Hypervolume.Calculate(front, referencePoint, maximization);

      //SouthWest
      maximization = new bool[] { true, true };
      referencePoint[0] = 0;
      referencePoint[1] = 0;
      double sw = Hypervolume.Calculate(front, referencePoint, maximization);

      //SouthEast
      maximization = new bool[] { false, true };
      referencePoint[0] = 1;
      referencePoint[1] = 0;
      double se = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(1.0, ne + se + nw + sw, 1e8);
    }

    /// <summary>
    ///  /*
    /// x-----+
    /// |     |
    /// |  X  |
    /// |     |
    /// +-----x
    /// 
    /// box between(0,0) and(1,1) with three point (pareto) front at (1,0), (0.5,0.5)  and (0,1) 
    /// Hypervolume to (1,0) and (0,1) of the corners should be 1 (dominated Points need to be reemoved beforehand and   
    /// Hypervolume to (0,0) and (1,1) of the corners should be 0.25
    /// </summary>
    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void HypervolumeTestDiagonalPoint() {
      //Front with three points
      double[] point1 = new double[2];
      point1[0] = 0;
      point1[1] = 1;
      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 0;
      double[] point3 = new double[2];
      point3[0] = 0.5;
      point3[1] = 0.5;
      double[][] front = { point1, point2, point3 };

      double[] referencePoint = new double[2];
      bool[] maximization;

      //Northeast
      maximization = new bool[] { false, false };
      referencePoint[0] = 1;
      referencePoint[1] = 1;
      double ne = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, ne);

      //NorthWest
      maximization = new bool[] { true, false };
      referencePoint[0] = 0;
      referencePoint[1] = 1;
      double nw = Hypervolume.Calculate(NonDominatedSelect.SelectNonDominatedVectors(front, maximization, true), referencePoint, maximization);
      Assert.AreEqual(1, nw);

      //SouthWest
      maximization = new bool[] { true, true };
      referencePoint[0] = 0;
      referencePoint[1] = 0;
      double sw = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, sw);

      //SouthEast
      maximization = new bool[] { false, true };
      referencePoint[0] = 1;
      referencePoint[1] = 0;
      double se = Hypervolume.Calculate(NonDominatedSelect.SelectNonDominatedVectors(front, maximization, true), referencePoint, maximization);
      Assert.AreEqual(1, se);

    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void HypervolumeTestReferencePointViolationNE() {
      //Front with a single Point
      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { point };

      double[] referencePoint = new double[2];
      bool[] maximization;

      //Northeast
      maximization = new bool[] { true, true };
      referencePoint[0] = 1;
      referencePoint[1] = 1;
      double ne = Hypervolume.Calculate(front, referencePoint, maximization);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void HypervolumeTestReferencePointViolationNW() {
      //Front with a single Point
      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { point };

      double[] referencePoint = new double[2];
      bool[] maximization;

      //NorthWest
      maximization = new bool[] { false, true };
      referencePoint[0] = 0;
      referencePoint[1] = 1;
      double nw = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, nw);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void HypervolumeTestReferencePointViolationSW() {
      //Front with a single Point
      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { point };

      double[] referencePoint = new double[2];
      bool[] maximization;

      //SouthWest
      maximization = new bool[] { false, false };
      referencePoint[0] = 0;
      referencePoint[1] = 0;
      double sw = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, sw);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void HypervolumeTestReferencePointViolationSE() {
      //Front with a single Point
      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { point };

      double[] referencePoint = new double[2];
      bool[] maximization;

      //SouthEast
      maximization = new bool[] { true, false };
      referencePoint[0] = 1;
      referencePoint[1] = 0;
      double se = Hypervolume.Calculate(front, referencePoint, maximization);
      Assert.AreEqual(0.25, se);
    }
  }
}
