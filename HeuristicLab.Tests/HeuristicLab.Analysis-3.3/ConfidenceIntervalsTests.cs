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
using HeuristicLab.Analysis.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Analysis.Tests {
  [TestClass]
  public class ConfidenceIntervalsTests {
    private readonly double[] x = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 1.5, 0.3 };
    private readonly double[] y = new double[] { -0.4071000,  1.3207543,  0.8364860, -0.6574363,  0.6215807,  
      1.1562429, -0.7299977, -0.1013315, -1.5221018, -0.9375169, -0.6257027,  1.1698417, -0.9868505,  0.6890087, 
      -0.1643505, -0.4920212, -0.6958091,  1.8849663,  0.8117723,  0.9806149};
    private readonly double[] z = new double[] { 45, 55, 67, 45, 68, 79, 98, 87, 84, 82 };

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void ConfidenceIntervalsTest1() {
      var result = z.ConfidenceIntervals(0.98);
      Assert.AreEqual(54.8, Math.Round(result.Item1, 1));
      Assert.AreEqual(87.2, Math.Round(result.Item2, 1));
    }

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void ConfidenceIntervalsTest2() {
      //comparison with R's t.test(..)
      var result = x.ConfidenceIntervals(0.95);
      Assert.AreEqual(1.880506, Math.Round(result.Item1, 6));
      Assert.AreEqual(5.679494, Math.Round(result.Item2, 6));
    }

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void ConfidenceIntervalsTest3() {
      //comparison with R's t.test(..)
      var result = y.ConfidenceIntervals(0.1);
      Assert.AreEqual(0.0802945, Math.Round(result.Item1, 7));
      Assert.AreEqual(0.1348105, Math.Round(result.Item2, 7));
    }

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void ConfidenceIntervalsTest4() {
      //comparison with R's t.test(..)
      var result = y.ConfidenceIntervals(0.99);
      Assert.AreEqual(-0.5047921, Math.Round(result.Item1, 7));
      Assert.AreEqual(0.7198970, Math.Round(result.Item2, 7));
    }
  }
}
