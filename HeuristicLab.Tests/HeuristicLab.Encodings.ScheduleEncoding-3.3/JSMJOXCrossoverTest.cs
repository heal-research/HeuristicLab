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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests {
  /// <summary>
  ///This is a test class for JSMJOXCrossoverTest and is intended
  ///to contain all JSMJOXCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class JSMJOXCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Schedule")]
    [TestProperty("Time", "short")]
    public void ApplyTest() {
      IRandom random = new TestRandom(new int[] { 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1 }, null);
      JSMEncoding p1 = TestUtils.CreateTestJSM1();
      JSMEncoding p2 = TestUtils.CreateTestJSM2();
      JSMEncoding expected = new JSMEncoding();
      ItemList<Permutation> jsm = new ItemList<Permutation>();
      for (int i = 0; i < 6; i++) {
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 5, 4, 3, 0, 1, 2 }));
      }
      expected.JobSequenceMatrix = jsm;

      JSMEncoding actual;
      actual = JSMJOXCrossover.Apply(random, p1, p2);

      Assert.IsTrue(TestUtils.JSMEncodingEquals(expected, actual));
    }
  }
}
