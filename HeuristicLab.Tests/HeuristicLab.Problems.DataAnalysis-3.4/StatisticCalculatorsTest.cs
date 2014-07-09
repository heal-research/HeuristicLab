#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class StatisticCalculatorsTest {
    private double[,] testData = new double[,] {
     {5,1,1,1,2,1,3,1,1,2},
     {5,4,4,5,7,10,3,2,1,2},
     {3,1,1,1,2,2,3,1,1,2},
     {6,8,8,1,3,4,3,7,1,2},
     {4,1,1,3,2,1,3,1,1,2},
     {8,10,10,8,7,10,9,7,1,4},            
     {1,1,1,1,2,10,3,1,1,2},              
     {2,1,2,1,2,1,3,1,1,2},                 
     {2,1,1,1,2,1,1,1,5,2},                 
     {4,2,1,1,2,1,2,1,1,2},                   
     {1,1,1,1,1,1,3,1,1,2},    
     {2,1,1,1,2,1,2,1,1,2},                   
     {5,3,3,3,2,3,4,4,1,4},                          
     {8,7,5,10,7,9,5,5,4,4},          
     {7,4,6,4,6,1,4,3,1,4},                          
     {4,1,1,1,2,1,2,1,1,2},     
     {4,1,1,1,2,1,3,1,1,2},      
     {10,7,7,6,4,10,4,1,2,4},  
     {6,1,1,1,2,1,3,1,1,2},     
     {7,3,2,10,5,10,5,4,4,4},   
     {10,5,5,3,6,7,7,10,1,4} 
      };

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CalculateMeanAndVarianceTest() {
      System.Random random = new System.Random(31415);

      int n = testData.GetLength(0);
      int cols = testData.GetLength(1);
      {
        for (int col = 0; col < cols; col++) {
          double scale = random.NextDouble();
          IEnumerable<double> x = from rows in Enumerable.Range(0, n)
                                  select testData[rows, col] * scale;
          double[] xs = x.ToArray();
          double mean_alglib, variance_alglib;
          mean_alglib = variance_alglib = 0.0;
          double tmp = 0;

          alglib.samplemoments(xs, n, out  mean_alglib, out variance_alglib, out tmp, out tmp);

          var calculator = new OnlineMeanAndVarianceCalculator();
          for (int i = 0; i < n; i++) {
            calculator.Add(xs[i]);
          }
          double mean = calculator.Mean;
          double variance = calculator.Variance;

          Assert.IsTrue(mean_alglib.IsAlmost(mean));
          Assert.IsTrue(variance_alglib.IsAlmost(variance));
        }
      }
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CalculatePearsonsRSquaredTest() {
      System.Random random = new System.Random(31415);
      int n = testData.GetLength(0);
      int cols = testData.GetLength(1);
      for (int c1 = 0; c1 < cols; c1++) {
        for (int c2 = c1 + 1; c2 < cols; c2++) {
          {
            double c1Scale = random.NextDouble() * 1E7;
            double c2Scale = random.NextDouble() * 1E7;
            IEnumerable<double> x = from rows in Enumerable.Range(0, n)
                                    select testData[rows, c1] * c1Scale;
            IEnumerable<double> y = from rows in Enumerable.Range(0, n)
                                    select testData[rows, c2] * c2Scale;
            double[] xs = x.ToArray();
            double[] ys = y.ToArray();
            double r2_alglib = alglib.pearsoncorrelation(xs, ys, n);
            r2_alglib *= r2_alglib;

            var r2Calculator = new OnlinePearsonsRSquaredCalculator();
            for (int i = 0; i < n; i++) {
              r2Calculator.Add(xs[i], ys[i]);
            }
            double r2 = r2Calculator.RSquared;

            Assert.IsTrue(r2_alglib.IsAlmost(r2));
          }
        }
      }
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CalculatePearsonsRSquaredOfConstantTest() {
      System.Random random = new System.Random(31415);
      int n = 12;
      int cols = testData.GetLength(1);
      for (int c1 = 0; c1 < cols; c1++) {
        double c1Scale = random.NextDouble() * 1E7;
        IEnumerable<double> x = from rows in Enumerable.Range(0, n)
                                select testData[rows, c1] * c1Scale;
        IEnumerable<double> y = (new List<double>() { 150494407424305.47 })
          .Concat(Enumerable.Repeat(150494407424305.47, n - 1));
        double[] xs = x.ToArray();
        double[] ys = y.ToArray();
        double r2_alglib = alglib.pearsoncorrelation(xs, ys, n);
        r2_alglib *= r2_alglib;

        var r2Calculator = new OnlinePearsonsRSquaredCalculator();
        for (int i = 0; i < n; i++) {
          r2Calculator.Add(xs[i], ys[i]);
        }
        double r2 = r2Calculator.RSquared;

        Assert.AreEqual(r2_alglib.ToString(), r2.ToString());
      }
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CalculateHoeffdingsDTest() {
      OnlineCalculatorError error;
      // direct perfect dependency
      var xs = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 };
      var ys = new double[] { 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0 };
      var d = HoeffdingsDependenceCalculator.CalculateHoeffdings(xs, ys, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, 1.0, 1E-5);

      // perfect negative dependency
      ys = xs.Select(x => -x).ToArray();
      d = HoeffdingsDependenceCalculator.CalculateHoeffdings(xs, ys, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, 1.0, 1E-5);

      // ties
      xs = new double[] { 1.0, 1.0, 2.0, 2.0, 3.0, 3.0, 4.0, 4.0, 5.0, 5.0, 5.0 };
      ys = new double[] { 2.0, 2.0, 3.0, 3.0, 4.0, 4.0, 5.0, 5.0, 6.0, 6.0, 6.0 };
      d = HoeffdingsDependenceCalculator.CalculateHoeffdings(xs, ys, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, 0.6783, 1E-5);

      // ties
      xs = new double[] { 1.0, 1.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 6.0, 6.0 };
      ys = xs.Select(x => x * x).ToArray();
      d = HoeffdingsDependenceCalculator.CalculateHoeffdings(xs, ys, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, 0.75, 1E-5);

      // degenerate
      xs = new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
      ys = new double[] { 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0 };
      d = HoeffdingsDependenceCalculator.CalculateHoeffdings(xs, ys, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, -0.3516, 1E-4);


      var normal = new HeuristicLab.Random.NormalDistributedRandom(new HeuristicLab.Random.MersenneTwister(31415), 0, 1);

      xs = Enumerable.Range(0, 1000).Select(i => normal.NextDouble()).ToArray();
      ys = Enumerable.Range(0, 1000).Select(i => normal.NextDouble()).ToArray();

      // independent
      d = HoeffdingsDependenceCalculator.CalculateHoeffdings(xs, ys, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, -0.00023, 1E-5);


      xs = Enumerable.Range(0, 1000).Select(i => normal.NextDouble()).ToArray();
      ys = xs.Select(x => x * x).ToArray();

      d = HoeffdingsDependenceCalculator.CalculateHoeffdings(xs, ys, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, 0.25071, 1E-5);

      // symmetric?
      d = HoeffdingsDependenceCalculator.CalculateHoeffdings(ys, xs, out error);
      Assert.AreEqual(error, OnlineCalculatorError.None);
      Assert.AreEqual(d, 0.25071, 1E-5);

    }
  }
}
