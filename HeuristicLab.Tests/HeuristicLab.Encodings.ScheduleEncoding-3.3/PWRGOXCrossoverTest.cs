﻿#region License Information
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

using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests {
  /// <summary>
  ///This is a test class for PWRGOXCrossoverTest and is intended
  ///to contain all PWRGOXCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class PWRGOXCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Schedule")]
    public void ApplyTest() {
      IRandom random = new TestRandom(new int[] { 3 }, null);
      PWREncoding parent1 = TestUtils.CreateTestPWR1();
      PWREncoding parent2 = TestUtils.CreateTestPWR2();
      PWREncoding expected = new PWREncoding();
      expected.PermutationWithRepetition = new IntegerVector(new int[] { 1, 0, 1, 0, 2, 0, 1, 2, 2 });
      PWREncoding actual;
      actual = PWRGOXCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(TestUtils.PRWEncodingEquals(expected, actual));
    }
  }
}
