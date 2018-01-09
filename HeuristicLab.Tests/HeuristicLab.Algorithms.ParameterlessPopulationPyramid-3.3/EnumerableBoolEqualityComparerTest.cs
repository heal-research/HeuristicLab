#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Algorithms.ParameterlessPopulationPyramid;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParameterlessPopulationPyramid.Test {
  [TestClass]
  public class EnumerableBoolEqualityComparerTest {
    private static EnumerableBoolEqualityComparer compare = new EnumerableBoolEqualityComparer();
    MersenneTwister rand = new MersenneTwister();

    private void randFill(IList<bool> data, MersenneTwister rand) {
      for (int i = 0; i < data.Count; i++) {
        data[i] = rand.Next(2) == 1;
      }
    }
    [TestMethod]
    [TestProperty("Time", "short")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void EnumerableBoolEqualsTest() {
      bool[] array = new bool[10];
      // Referencially equal
      randFill(array, rand);
      Assert.IsTrue(compare.Equals(array, array));

      // Clones equal
      var another = (bool[])array.Clone();
      Assert.IsTrue(compare.Equals(array, another));

      // Flipping a bit makes unequal
      int flip = rand.Next(array.Length - 1);
      another[flip] = !another[flip];
      Assert.IsFalse(compare.Equals(array, another));

      // Equality doesn't depend on container
      List<bool> list = new List<bool>(array);
      Assert.IsTrue(compare.Equals(list, array));
      Assert.IsFalse(compare.Equals(list, another));
    }

    [TestMethod]
    [TestProperty("Time", "short")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void EnumerableBoolHashCollisionTest() {
      int collisions = 0;

      bool[] array = new bool[100];
      randFill(array, rand);
      int original = compare.GetHashCode(array);

      bool[] another = (bool[])array.Clone();
      // Perform random walk around string checking for hash collisions
      for (int i = 0; i < array.Length * array.Length; i++) {
        int flip = rand.Next(array.Length - 1);
        another[flip] = !another[flip];
        int hash = compare.GetHashCode(another);
        if (hash == original) {
          // unequal sequences with same hash value
          if (!array.SequenceEqual(another)) collisions++;
        } else {
          // unequal hash should mean unequal sequence
          Assert.IsFalse(array.SequenceEqual(another));
        }
        // Keep the walk from getting too far afield
        if (i % array.Length == 0) {
          another = (bool[])array.Clone();
        }
      }
      // A very lose upper bound, this only fails if you get more than sqrt(N) collisions per attempt 
      Assert.IsTrue(collisions < array.Length);
    }

    [TestMethod]
    [TestProperty("Time", "short")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void EnumerableBoolHashSetTest() {
      HashSet<bool[]> set = new HashSet<bool[]>(compare);
      bool[] array = new bool[40];
      randFill(array, rand);
      set.Add(array);

      // Same object not added
      set.Add(array);
      Assert.AreEqual(1, set.Count);

      // Copy of object not allowed
      var another = (bool[])array.Clone();
      set.Add(another);
      Assert.AreEqual(1, set.Count);

      // Flipping a bit makes unequal
      int flip = rand.Next(array.Length - 1);
      another[flip] = !another[flip];
      set.Add(another);
      Assert.AreEqual(2, set.Count);

      // Add random solutions which are 1 bit different from array to the set
      set.Clear();
      List<bool[]> added = new List<bool[]>();
      for (int i = 0; i < array.Length * 10; i++) {
        // flip a bit at random
        another = (bool[])array.Clone();
        flip = rand.Next(array.Length - 1);
        another[flip] = !another[flip];
        bool unique = set.Add(another);
        bool actuallyUnique = true;
        foreach (var previous in added) {
          if (previous.SequenceEqual(another)) {
            actuallyUnique = false;
          }
        }
        Assert.AreEqual(actuallyUnique, unique);
        if (actuallyUnique) {
          added.Add(another);
        }
      }
    }
  }
}
