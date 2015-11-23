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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.PermutationEncoding.Tests {
  [TestClass]
  public class PermutationEqualityComparerTest {
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void TestPermutationEqualityComparer() {
      PermutationEqualityComparer comparer = new PermutationEqualityComparer();
      Permutation p = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 3, 2, 0, 1, 4 });
      Permutation q = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 1, 0, 2, 3, 4 });
      Assert.IsTrue(comparer.Equals(p, q));
      Assert.IsTrue(comparer.GetHashCode(p) == comparer.GetHashCode(q));
      Permutation p2 = new Permutation(PermutationTypes.RelativeDirected, new int[] { 2, 3, 4, 1, 0 });
      Permutation q2 = new Permutation(PermutationTypes.RelativeDirected, new int[] { 1, 0, 2, 3, 4 });
      Assert.IsTrue(comparer.Equals(p2, q2));
      Assert.IsTrue(comparer.GetHashCode(p2) == comparer.GetHashCode(q2));

      Assert.IsFalse(comparer.Equals(p, q2));
      Assert.IsFalse(comparer.Equals(p2, q));

      Permutation p3 = new Permutation(PermutationTypes.Absolute, new int[] { 2, 3, 0, 4, 1 });
      Permutation q3 = new Permutation(PermutationTypes.Absolute, new int[] { 2, 3, 0, 4, 1 });
      Assert.IsTrue(comparer.Equals(p3, q3));
      Assert.IsTrue(comparer.GetHashCode(p3) == comparer.GetHashCode(q3));

      Assert.IsFalse(comparer.Equals(p3, q));
      Assert.IsFalse(comparer.Equals(p2, q3));

      Permutation p4 = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 3, 2, 0, 1, 4, 5 });
      Assert.IsFalse(comparer.Equals(p, p4));
      Assert.IsFalse(comparer.Equals(p4, q));
    }
  }
}
