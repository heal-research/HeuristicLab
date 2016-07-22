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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class EnumerableStatisticExtensionsTest {
    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "short")]
    public void QuantileTest() {
      {
        Assert.AreEqual(2.0, new double[] { 2.0 }.Quantile(0.5));
        Assert.AreEqual(2.0, new double[] { 2.0 }.Quantile(0.01));
        Assert.AreEqual(2.0, new double[] { 2.0 }.Quantile(0.99));
      }

      {
        Assert.AreEqual(1.5, new double[] { 1.0, 2.0 }.Median());
        Assert.AreEqual(2.0, new double[] { 1.0, 2.0 }.Quantile(0.99));
        Assert.AreEqual(1.0, new double[] { 1.0, 2.0 }.Quantile(0.01));
      }
      {
        Assert.AreEqual(2.0, new double[] { 3.0, 1.0, 2.0 }.Median());
        Assert.AreEqual(3.0, new double[] { 3.0, 1.0, 2.0 }.Quantile(0.99));
        Assert.AreEqual(1.0, new double[] { 3.0, 1.0, 2.0 }.Quantile(0.01));
      }


      var xs = new double[] { 1, 1, 1, 3, 4, 7, 9, 11, 13, 13 };
      {
        var q0 = Quantile(xs, 0.3); // naive implementation using sorting
        Assert.AreEqual(q0, 2.0, 1E-6);

        var q1 = xs.Quantile(0.3); // using select
        Assert.AreEqual(q1, 2.0, 1E-6);
      }
      {
        var q0 = Quantile(xs, 0.75); // naive implementation using sorting
        Assert.AreEqual(q0, 11.0, 1E-6);

        var q1 = xs.Quantile(0.75); // using select
        Assert.AreEqual(q1, 11.0, 1E-6);
      }
      // quantile = 0.5 is equivalent to median
      {
        // even number of elements
        var expected = Median(xs);
        Assert.AreEqual(expected, Quantile(xs, 0.5), 1E-6); // using sorting
        Assert.AreEqual(expected, xs.Quantile(0.5), 1E-6); // using select
      }
      {
        // odd number of elements
        var expected = Median(xs.Take(9));
        Assert.AreEqual(expected, Quantile(xs.Take(9), 0.5), 1E-6); // using sorting
        Assert.AreEqual(expected, xs.Take(9).Quantile(0.5), 1E-6); // using select
      }

      // edge cases
      {
        try {
          new double[] { }.Quantile(0.5); // empty
          Assert.Fail("expected exception");
        }
        catch (Exception) {
        }
      }
      {
        try {
          Enumerable.Repeat(0.0, 10).Quantile(1.0); // alpha < 1 
          Assert.Fail("expected exception");
        }
        catch (Exception) {
        }
      }
    }

    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "medium")]
    public void QuantilePerformanceTest() {
      int n = 10;
      var sw0 = new Stopwatch();
      var sw1 = new Stopwatch();
      const int reps = 1000;
      while (n <= 1000000) {
        for (int i = 0; i < reps; i++) {
          var xs = RandomEnumerable.SampleRandomNumbers(0, 10000, n + 1).Select(x => (double)x).ToArray();
          sw0.Start();
          var q0 = Median(xs); // sorting
          sw0.Stop();


          sw1.Start();
          var q1 = xs.Median(); // selection
          sw1.Stop();
          Assert.AreEqual(q0, q1, 1E-9);
        }
        Console.WriteLine("{0,-10} {1,-10} {2,-10}", n, sw0.ElapsedMilliseconds, sw1.ElapsedMilliseconds);

        n = n * 10;
      }
    }


    // straight forward implementation of median function (using sorting)
    private static double Median(IEnumerable<double> values) {
      // iterate only once 
      double[] valuesArr = values.ToArray();
      int n = valuesArr.Length;
      if (n == 0) throw new InvalidOperationException("Enumeration contains no elements.");

      Array.Sort(valuesArr);

      // return the middle element (if n is uneven) or the average of the two middle elements if n is even.
      if (n % 2 == 1) {
        return valuesArr[n / 2];
      } else {
        return (valuesArr[(n / 2) - 1] + valuesArr[n / 2]) / 2.0;
      }
    }

    // straight forward implementation of quantile function (using sorting)
    private static double Quantile(IEnumerable<double> values, double alpha) {
      Contract.Assert(alpha > 0 && alpha < 1);
      // iterate only once 
      double[] valuesArr = values.ToArray();
      int n = valuesArr.Length;
      if (n == 0) throw new InvalidOperationException("Enumeration contains no elements.");

      Array.Sort(valuesArr);
      //  starts at 0

      // return the element at Math.Ceiling (if n*alpha is fractional) or the average of two elements if n*alpha is integer.
      var pos = n * alpha;
      Contract.Assert(pos >= 0);
      Contract.Assert(pos < n);
      bool isInteger = Math.Round(pos).IsAlmost(pos);
      if (isInteger) {
        return 0.5 * (valuesArr[(int)pos - 1] + valuesArr[(int)pos]);
      } else {
        return valuesArr[(int)Math.Ceiling(pos) - 1];
      }
    }
  }
}