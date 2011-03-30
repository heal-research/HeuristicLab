#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class SymbolicRegressionTest {
    public SymbolicRegressionTest() { }

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
    [DeploymentItem(@"GA_SymbReg.hl")]
    public void SymbolicRegressionPerformanceTest() {
      ex = null;
      GeneticAlgorithm ga = (GeneticAlgorithm)XmlParser.Deserialize("GA_SymbReg.hl");
      ga.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(ga_ExceptionOccurred);
      ga.Stopped += new EventHandler(ga_Stopped);

      ga.Prepare();
      ga.Start();
      trigger.WaitOne();
      if (ex != null) throw ex;

      TestContext.WriteLine("Runtime: {0}", ga.ExecutionTime.ToString());

      double expectedValidationQuality = 0.72232929212380248;
      double expectedBestQuality = 0.67404328905620081;
      double expectedAverageQuality = 0.43099606868788487;
      double expectedWorstQuality = 0;
      double bestQuality = (ga.Results["CurrentBestQuality"].Value as DoubleValue).Value;
      double averageQuality = (ga.Results["CurrentAverageQuality"].Value as DoubleValue).Value;
      double worstQuality = (ga.Results["CurrentWorstQuality"].Value as DoubleValue).Value;
      double bestValidationSolutionQuality = (ga.Results["Best validation solution quality"].Value as DoubleValue).Value;

      TestContext.WriteLine("");
      TestContext.WriteLine("CurrentBestQuality: {0} (should be {1})", bestQuality, expectedBestQuality);
      TestContext.WriteLine("CurrentAverageQuality: {0} (should be {1})", averageQuality, expectedAverageQuality);
      TestContext.WriteLine("CurrentWorstQuality: {0} (should be {1})", worstQuality, expectedWorstQuality);
      TestContext.WriteLine("Best valdidation solution quality: {0} (should be {1})", bestValidationSolutionQuality, expectedValidationQuality);

      Assert.AreEqual(bestQuality, expectedBestQuality);
      Assert.AreEqual(averageQuality, expectedAverageQuality);
      Assert.AreEqual(worstQuality, expectedWorstQuality);
      Assert.AreEqual(bestValidationSolutionQuality, expectedValidationQuality);
    }

    private void ga_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      ex = e.Value;
    }

    private void ga_Stopped(object sender, EventArgs e) {
      trigger.Set();
    }
  }
}
