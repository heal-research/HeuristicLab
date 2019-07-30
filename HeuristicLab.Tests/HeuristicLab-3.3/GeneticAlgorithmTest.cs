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
using HEAL.Attic;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Common;
using HeuristicLab.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GeneticAlgorithmTest {
    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();
    public GeneticAlgorithmTest() { }

    private TestContext testContextInstance;
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    private Exception ex;

    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "long")]
    public void GeneticAlgorithmPerformanceTest() {
      ex = null;
      GeneticAlgorithm ga = (GeneticAlgorithm)serializer.Deserialize(@"Test Resources\GA_TSP.hl");
      ga.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(ga_ExceptionOccurred);
      ga.SetSeedRandomly.Value = false;
      ga.Seed.Value = 0;

      ga.Prepare();
      ga.Start();
      if (ex != null) throw ex;

      TestContext.WriteLine("Runtime: {0}", ga.ExecutionTime.ToString());

      double expectedBestQuality = 12332.0;
      double expectedAverageQuality = 13123.2;
      double expectedWorstQuality = 14538.0;
      double bestQuality = (ga.Results["CurrentBestQuality"].Value as DoubleValue).Value;
      double averageQuality = (ga.Results["CurrentAverageQuality"].Value as DoubleValue).Value;
      double worstQuality = (ga.Results["CurrentWorstQuality"].Value as DoubleValue).Value;

      TestContext.WriteLine("");
      TestContext.WriteLine("CurrentBestQuality: {0} (should be {1})", bestQuality, expectedBestQuality);
      TestContext.WriteLine("CurrentAverageQuality: {0} (should be {1})", averageQuality, expectedAverageQuality);
      TestContext.WriteLine("CurrentWorstQuality: {0} (should be {1})", worstQuality, expectedWorstQuality);

      Assert.AreEqual(bestQuality, expectedBestQuality);
      Assert.AreEqual(averageQuality, expectedAverageQuality);
      Assert.AreEqual(worstQuality, expectedWorstQuality);
    }

    private void ga_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      ex = e.Value;
    }
  }
}
