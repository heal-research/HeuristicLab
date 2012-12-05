#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Collections_33.Tests {
  [TestClass]
  public class BidirectionalDictionaryTest {
    private class ComplexType {
      public readonly int Field;
      public ComplexType(int field) {
        Field = field;
      }
    }

    private class ComplexTypeEqualityComparer : EqualityComparer<ComplexType> {
      public override bool Equals(ComplexType x, ComplexType y) {
        return x.Field == y.Field;
      }
      public override int GetHashCode(ComplexType obj) {
        return obj.Field.GetHashCode();
      }
    }

    [TestMethod]
    public void TestBidirectionalDictionary() {
      var dict1 = new BidirectionalDictionary<int, double>();
      dict1.Add(4, 2.0);
      Assert.IsTrue(dict1.ContainsFirst(4) && dict1.ContainsSecond(2));
      bool exceptionOnDuplicate = false;
      try {
        dict1.Add(4, 3.0);
      } catch (ArgumentException) { exceptionOnDuplicate = true; }
      Assert.IsTrue(exceptionOnDuplicate);
      Assert.IsTrue(dict1.GetByFirst(4) == 2);
      Assert.IsTrue(dict1.GetBySecond(2) == 4);
      Assert.IsTrue(dict1.Count == 1);
      dict1.Clear();
      Assert.IsTrue(dict1.Count == 0);

      var dict2 = new BidirectionalDictionary<ComplexType, int>(new ComplexTypeEqualityComparer());
      Assert.IsTrue(!dict2.FirstEnumerable.Any());
      dict2.Add(new ComplexType(1), 2);
      Assert.IsTrue(dict2.SecondEnumerable.Any());
      dict2.Add(new ComplexType(2), 1);
      Assert.IsTrue(dict2.ContainsFirst(new ComplexType(2)));
      Assert.IsTrue(dict2.ContainsSecond(2));
      exceptionOnDuplicate = false;
      try {
        dict2.Add(new ComplexType(2), 3);
      } catch (ArgumentException) { exceptionOnDuplicate = true; }
      Assert.IsTrue(exceptionOnDuplicate);
      exceptionOnDuplicate = false;
      try {
        dict2.Add(new ComplexType(3), 1);
      } catch (ArgumentException) { exceptionOnDuplicate = true; }
      Assert.IsTrue(exceptionOnDuplicate);
      Assert.IsTrue(dict2.Count == 2);
      Assert.IsTrue(dict2.GetBySecond(1).Field == 2);
      Assert.IsTrue(dict2.GetByFirst(new ComplexType(1)) == 2);
      dict2.Clear();
      Assert.IsTrue(!dict2.SecondEnumerable.Any());
    }
  }
}
