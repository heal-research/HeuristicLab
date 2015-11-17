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
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.PermutationEncoding.Tests {
  /// <summary>
  ///This is a test class for UniformLikeCrossoverTest and is intended
  ///to contain all UniformLikeCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class UniformLikeCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void UniformLikeCrossoverApplyTest() {
      // test from the paper
      IRandom random = new TestRandom(new int[] { 0 }, new double[] { 0.2, 0.7, 0.2, 0.2 }); // TODO: Initialize to an appropriate value
      Permutation parent1 = new Permutation(PermutationTypes.Absolute,
        new int[] { 3, 2, 0, 7, 5, 4, 1, 6 });
      Assert.IsTrue(parent1.Validate());
      Permutation parent2 = new Permutation(PermutationTypes.Absolute,
        new int[] { 5, 0, 4, 7, 1, 3, 2, 6 });
      Assert.IsTrue(parent2.Validate());
      Permutation expected = new Permutation(PermutationTypes.Absolute,
        new int[] { 3, 0, 4, 7, 5, 2, 1, 6 });
      Assert.IsTrue(expected.Validate());
      Permutation actual;
      actual = UniformLikeCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, actual));
    }
  }
}
