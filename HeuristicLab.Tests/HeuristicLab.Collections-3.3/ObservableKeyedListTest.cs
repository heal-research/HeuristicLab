#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Collections.Tests {
  [TestClass]
  public class ObservableKeyedListTest {
    private class IntKeyedList : ObservableKeyedList<int, int> {
      protected override int GetKeyForItem(int item) {
        return item;
      }

      public new Dictionary<int, int> Dictionary {
        get { return base.Dictionary; }
      }
    }

    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "short")]
    public void KeyedListAddDuplicateItemTest() {
      var list = new IntKeyedList();
      var numbers = new List<int>() { 1, 2, 3 };
      list.AddRange(numbers);

      Assert.IsTrue(list.SequenceEqual(numbers));
      Assert.AreEqual(list.Count, 3);

      try {
        list.Add(1);
      }
      catch (ArgumentException e) {
        Assert.AreEqual(e.Message, "An element with the same key already exists.");
        Assert.IsTrue(list.SequenceEqual(numbers));
        Assert.IsTrue(numbers.All(list.ContainsKey));
      }

      try {
        list.AddRange(numbers);
      }
      catch (ArgumentException e) {
        Assert.AreEqual(e.Message, "An element with the same key already exists.");
        Assert.IsTrue(list.SequenceEqual(numbers));
        Assert.IsTrue(numbers.All(list.ContainsKey));
      }
    }
  }
}
