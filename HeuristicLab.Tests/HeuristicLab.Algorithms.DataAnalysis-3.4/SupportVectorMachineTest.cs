#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class SupportVectorMachineTest {
    public SupportVectorMachineTest() { }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    private EventWaitHandle trigger = new AutoResetEvent(false);
    private Exception ex;

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void SupportVectorMachinePerformanceTest() {
      ex = null;

      var cv = new CrossValidation();
      cv.Algorithm = new SupportVectorRegression();
      var rand = new HeuristicLab.Random.MersenneTwister();
      double[,] data = GenerateData(1000, rand);
      List<string> variables = new List<string>() { "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "y" };
      Dataset ds = new Dataset(variables, data);
      cv.Problem.ProblemDataParameter.ActualValue = new RegressionProblemData(ds, variables.Take(10), variables.Last());
      cv.Folds.Value = 5;
      cv.SamplesStart.Value = 0;
      cv.SamplesEnd.Value = 999;

      cv.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(cv_ExceptionOccurred);
      cv.Stopped += new EventHandler(cv_Stopped);

      cv.Prepare();
      cv.Start();
      trigger.WaitOne();
      if (ex != null) throw ex;

      TestContext.WriteLine("Runtime: {0}", cv.ExecutionTime.ToString());

    }

    // poly-10: y = x1 x2 + x3 x4 + x5 x6 + x1 x7 x9 + x3 x6 x10
    private double[,] GenerateData(int n, IRandom random) {
      double[,] data = new double[n, 11];
      for (int i = 0; i < n; i++) {
        for (int c = 0; c < 10; c++) {
          data[i, c] = random.NextDouble() * 2.0 - 1.0;
        }
        data[i, 10] =
          data[i, 0] * data[i, 1] +
          data[i, 2] * data[i, 3] +
          data[i, 4] * data[i, 5] +
          data[i, 0] * data[i, 6] * data[i, 8] +
          data[i, 2] * data[i, 5] * data[i, 9];
      }
      return data;
    }

    private void cv_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      ex = e.Value;
    }

    private void cv_Stopped(object sender, EventArgs e) {
      trigger.Set();
    }
  }
}
