#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis_34.Tests {
  [TestClass]
  public class OnlineCalculatorPerformanceTest {
    private const int Rows = 5000;
    private const int Columns = 2;
    private const int Repetitions = 10000;

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }


    [TestMethod]
    public void OnlineAccuracyCalculatorPerformanceTest() {
      TestCalculatorPerfomance(OnlineAccuracyCalculator.Calculate);
    }
    [TestMethod]
    public void OnlineCovarianceCalculatorPerformanceTest() {
      TestCalculatorPerfomance(OnlineCovarianceCalculator.Calculate);
    }
    [TestMethod]
    public void OnlineMeanAbsolutePercentageErrorCalculatorPerformanceTest() {
      TestCalculatorPerfomance(OnlineMeanAbsolutePercentageErrorCalculator.Calculate);
    }

    [TestMethod]
    public void OnlineMeanSquaredErrorCalculatorPerformanceTest() {
      TestCalculatorPerfomance(OnlineMeanSquaredErrorCalculator.Calculate);
    }
    [TestMethod]
    public void OnlineNormalizedMeanSquaredErrorCalculatorPerformanceTest() {
      TestCalculatorPerfomance(OnlineNormalizedMeanSquaredErrorCalculator.Calculate);
    }
    [TestMethod]
    public void OnlinePearsonsRSquaredCalculatorPerformanceTest() {
      TestCalculatorPerfomance(OnlinePearsonsRSquaredCalculator.Calculate);
    }

    private delegate double CalcateFunc(IEnumerable<double> estimated, IEnumerable<double> original, out OnlineCalculatorError errorState);
    private void TestCalculatorPerfomance(CalcateFunc calculateFunc) {
      var twister = new MersenneTwister(31415);
      var dataset = CreateRandomDataset(twister, Rows, Columns);
      OnlineCalculatorError errorState = OnlineCalculatorError.None; ;

      Stopwatch watch = new Stopwatch();
      watch.Start();
      for (int i = 0; i < Repetitions; i++) {
        double value = calculateFunc(dataset.GetDoubleValues("y"), dataset.GetDoubleValues("x0"), out errorState);
      }
      Assert.AreEqual(errorState, OnlineCalculatorError.None);
      watch.Stop();

      TestContext.WriteLine("");
      TestContext.WriteLine("Calculated Rows per milisecond: {0}.", Rows * Repetitions * 1.0 / watch.ElapsedMilliseconds);
    }

    public static Dataset CreateRandomDataset(MersenneTwister twister, int rows, int columns) {
      double[,] data = new double[rows, columns];
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
          data[i, j] = twister.NextDouble() * 2.0 - 1.0;
        }
      }
      IEnumerable<string> variableNames = new string[] { "y" }.Concat(Enumerable.Range(0, columns - 1).Select(x => "x" + x.ToString()));
      Dataset ds = new Dataset(variableNames, data);
      return ds;
    }
  }
}
