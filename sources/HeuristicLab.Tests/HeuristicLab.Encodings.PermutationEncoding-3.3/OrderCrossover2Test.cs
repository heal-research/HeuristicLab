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

using HeuristicLab.Core;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.PermutationEncoding.Tests {
  /// <summary>
  ///This is a test class for OrderCrossover2Test and is intended
  ///to contain all OrderCrossover2Test Unit Tests
  ///</summary>
  [TestClass()]
  public class OrderCrossover2Test {
    /// <summary>
    ///A test for Cross
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void OrderCrossover2CrossTest() {
      TestRandom random = new TestRandom();
      var privateObject = new PrivateObject(typeof(OrderCrossover2));
      random.Reset();
      bool exceptionFired = false;
      try {
        privateObject.Invoke("Cross", random, new ItemArray<Permutation>(new Permutation[] {
          new Permutation(PermutationTypes.RelativeUndirected, 4), new Permutation(PermutationTypes.RelativeUndirected, 4), new Permutation(PermutationTypes.RelativeUndirected, 4)}));
      }
      catch (System.InvalidOperationException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void OrderCrossover2ApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent1, parent2, expected, actual;
      // The following test is based on an example from Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press. p. 135.
      random.Reset();
      random.IntNumbers = new int[] { 5, 7 };
      parent1 = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(parent1.Validate());
      parent2 = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 2, 5, 6, 0, 9, 1, 3, 8, 4, 7 });
      Assert.IsTrue(parent2.Validate());
      expected = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 2, 0, 9, 1, 3, 5, 6, 7, 8, 4 });
      Assert.IsTrue(expected.Validate());
      actual = OrderCrossover2.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, actual));

      // perform a test when the two permutations are of unequal length
      random.Reset();
      bool exceptionFired = false;
      try {
        OrderCrossover.Apply(random, new Permutation(PermutationTypes.RelativeUndirected, 8), new Permutation(PermutationTypes.RelativeUndirected, 6));
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
