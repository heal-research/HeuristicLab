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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.PermutationEncoding.Tests {
  [TestClass]
  public class PermutationManipulationTest {
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void TestPermutationSwap() {
      var permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Swap(0, 0);
      Assert.IsTrue(permutation.Validate());
      var expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Swap(0, 9);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 9, 1, 2, 3, 4, 5, 6, 7, 8, 0 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Swap(1, 3);
      permutation.Swap(3, 5);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 3, 2, 5, 4, 1, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));
    }

    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void TestPermutationReverse() {
      var permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Reverse(0, 0);
      Assert.IsTrue(permutation.Validate());
      var expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Reverse(9, 1);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Reverse(0, 10);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Reverse(1, 4);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 4, 3, 2, 1, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));
    }

    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void TestPermutationMove() {
      var permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(0, 0, 0);
      Assert.IsTrue(permutation.Validate());
      var expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(0, 0, 1);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 1, 0, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(0, 2, 3);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 3, 4, 5, 0, 1, 2, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(3, 5, 0);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 3, 4, 5, 0, 1, 2, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(3, 5, 2);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 3, 4, 5, 2, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(5, 6, 8);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 7, 8, 9, 5, 6 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(5, 7, 7);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 8, 9, 5, 6, 7 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(2, 6, 5);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 7, 8, 9, 2, 3, 4, 5, 6 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Move(3, 5, 4);
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 6, 3, 4, 5, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      var exception = false;
      try {
        permutation.Move(0, 5, 7);
      } catch { exception = true; }
      Assert.IsTrue(exception);
    }

    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void TestPermutationReplace() {
      var permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Replace(0, new int[0]);
      Assert.IsTrue(permutation.Validate());
      var expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Replace(3, new [] { 3 });
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Replace(5, new [] { 8, 6, 5, 7 });
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 8, 6, 5, 7, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Replace(0, new [] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 });
      Assert.IsTrue(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));

      permutation = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      permutation.Replace(3, new[] { 5, 6, 7 });
      Assert.IsFalse(permutation.Validate());
      expected = new Permutation(PermutationTypes.Absolute, new[] { 0, 1, 2, 5, 6, 7, 6, 7, 8, 9 });
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, permutation));
    }
  }
}
