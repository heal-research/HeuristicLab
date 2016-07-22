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

using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.PermutationEncoding.Tests {
  /// <summary>
  ///This is a test class for InversionManipulator and is intended
  ///to contain all InversionManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class InversionManipulatorTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void InversionManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent, expected;
      // The following test is based on an example from Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, pp. 46-47
      random.Reset();
      random.IntNumbers = new int[] { 1, 4 };
      parent = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
      Assert.IsTrue(parent.Validate());

      expected = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 0, 4, 3, 2, 1, 5, 6, 7, 8 });
      Assert.IsTrue(expected.Validate());
      InversionManipulator.Apply(random, parent);
      Assert.IsTrue(parent.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, parent));
    }
  }
}
