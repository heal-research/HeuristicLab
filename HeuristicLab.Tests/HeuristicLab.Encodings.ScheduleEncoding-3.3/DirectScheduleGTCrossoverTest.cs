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

using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;
using HeuristicLab.Encodings.ScheduleEncoding.ScheduleEncoding;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests {
  /// <summary>
  ///This is a test class for DirectScheduleGTCrossoverTest and is intended
  ///to contain all DirectScheduleGTCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class DirectScheduleGTCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Schedule")]
    [TestProperty("Time", "short")]
    public void ApplyTest() {
      IRandom random = new TestRandom(new int[] { 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1 }, new double[] { 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9 });
      Schedule parent1 = TestUtils.CreateTestSchedule1();
      Schedule parent2 = TestUtils.CreateTestSchedule2();
      ItemList<Job> jobData = TestUtils.CreateJobData();
      double mutProp = 0.05;
      Schedule actual;
      actual = DirectScheduleGTCrossover.Apply(random, parent1, parent2, jobData, mutProp);
      Schedule expected = DirectScheduleRandomCreator.Apply(3, 3, new PWREncoding(3, 3, new TestRandom(new int[] { 0, 2, 1, 1, 0, 2, 1, 2, 0 }, null)), TestUtils.CreateJobData());
      Assert.IsTrue(TestUtils.ScheduleEquals(actual, expected));
    }

  }
}
