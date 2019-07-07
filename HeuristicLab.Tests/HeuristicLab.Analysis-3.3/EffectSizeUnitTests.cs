#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class EffectSizeUnitTests {
    private readonly double[] x = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 1.5, 0.3 };
    private readonly double[] y = new double[] { 1.0, 3.0, 10.0, 4.0, 2.5, 6.0, 7.5, 8.0, 1.5, 0.5 };
    private readonly double[] z = new double[] { 45.0, 0.3, 12.0, 45.0, 68.0, 79.0, 10.0, 87.0, 84.0, 1.0 };

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void CohensdTest1() {
      //compared to R lsr package/cohenD(..)
      var result = SampleSizeDetermination.CalculateCohensD(x, y);
      Assert.AreEqual(0.2074003, Math.Round(result, 7));
    }

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void CohensdTest2() {
      //compared to R lsr package/cohenD(..)
      var result = SampleSizeDetermination.CalculateCohensD(x, z);
      Assert.AreEqual(1.57424, Math.Round(result, 5));
    }

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void HedgesGTest1() {
      //compared to R effsize package/cohen.d(..,hedges.correction=TRUE)
      var result = SampleSizeDetermination.CalculateHedgesG(x, y);
      Assert.AreEqual(0.1986369, Math.Round(result, 7));
    }

    [TestMethod]
    [TestCategory("Analysis.Statistics")]
    [TestProperty("Time", "short")]
    public void HedgesGTest2() {
      //compared to R effsize package/cohen.d(..,hedges.correction=TRUE)
      var result = SampleSizeDetermination.CalculateHedgesG(x, z);
      Assert.AreEqual(1.507722, Math.Round(result, 6));
    }
  }
}
