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

using System.Linq;
using HeuristicLab.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class EnumerableStatisticExtensionsTest {
    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "short")]
    public void QuantileTest() {
      var xs = new double[] { 1, 1, 1, 3, 4, 7, 9, 11, 13, 13 };
      {
        var q = xs.Quantile(0.3);
        Assert.AreEqual(q, 2.0, 1E-6);
      }
      {
        var q = xs.Quantile(0.75);
        Assert.AreEqual(q, 11.0, 1E-6);
      }
      // quantile = 0.5 is equivalent to median
      {
        // even number of elements
        Assert.AreEqual(xs.Quantile(0.5), xs.Median(), 1E-6);
      }
      {
        // odd number of elements
        Assert.AreEqual(xs.Take(9).Quantile(0.5), xs.Take(9).Median(), 1E-6);
      }
    }
  }
}