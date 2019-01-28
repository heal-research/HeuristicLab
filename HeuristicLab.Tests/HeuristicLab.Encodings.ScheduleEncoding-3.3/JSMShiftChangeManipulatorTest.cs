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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests {
  /// <summary>
  ///This is a test class for JSMShiftChangeManipulatorTest and is intended
  ///to contain all JSMShiftChangeManipulatorTest Unit Tests
  ///</summary>
  [TestClass()]
  public class JSMShiftChangeManipulatorTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Schedule")]
    [TestProperty("Time", "short")]
    public void ApplyTest() {
      IRandom random = new TestRandom(new int[] { 2, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 }, null);
      JSMEncoding individual = TestUtils.CreateTestJSM1();
      JSMShiftChangeManipulator.Apply(random, individual);
      JSMEncoding expected = new JSMEncoding();
      ItemList<Permutation> jsm = new ItemList<Permutation>();
      for (int i = 0; i < 3; i++) {
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 0, 1, 3, 2, 4, 5 }));
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 0, 1, 3, 4, 2, 5 }));
      }
      expected.JobSequenceMatrix = jsm;

      Assert.IsTrue(TestUtils.JSMEncodingEquals(expected, individual));
    }
  }
}
