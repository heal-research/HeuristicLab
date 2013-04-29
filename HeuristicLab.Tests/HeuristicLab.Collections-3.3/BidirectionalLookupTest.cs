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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Collections_33.Tests {
  [TestClass]
  public class BidirectionalLookupTest {
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
    public void TestBidirectionalLookup() {
      var lookup1 = new BidirectionalLookup<int, double>();
      lookup1.Add(4, 2);
      Assert.IsTrue(lookup1.ContainsFirst(4) && lookup1.ContainsSecond(2));
      lookup1.Add(4, 3);
      Assert.IsTrue(lookup1.GetByFirst(4).Count() == 2);
      Assert.IsTrue(lookup1.GetBySecond(2).Contains(4));
      Assert.IsTrue(lookup1.CountFirst == 1);
      Assert.IsTrue(lookup1.CountSecond == 2);
      lookup1.Add(3, 2);
      lookup1.Add(2, 2);
      lookup1.Add(1, 2);
      Assert.IsTrue(lookup1.GetByFirst(1).Single() == 2);
      Assert.IsTrue(lookup1.GetBySecond(2).Count() == 4);
      lookup1.RemovePair(2, 2);
      Assert.IsTrue(!lookup1.ContainsFirst(2));
      Assert.IsTrue(lookup1.GetBySecond(2).Count() == 3);
      lookup1.RemoveByFirst(4);
      Assert.IsTrue(lookup1.CountFirst == 2);
      Assert.IsTrue(lookup1.CountSecond == 1);
      lookup1.RemoveBySecond(2);
      Assert.IsTrue(lookup1.CountFirst == 0);
      Assert.IsTrue(lookup1.CountSecond == 0);
      lookup1.Clear();
      Assert.IsTrue(lookup1.CountFirst == 0);
      Assert.IsTrue(lookup1.CountSecond == 0);

      var dict2 = new BidirectionalLookup<ComplexType, int>(new ComplexTypeEqualityComparer());
      Assert.IsTrue(!dict2.FirstEnumerable.Any());
      dict2.Add(new ComplexType(1), 2);
      Assert.IsTrue(dict2.SecondEnumerable.Any());
      dict2.Add(new ComplexType(2), 1);
      Assert.IsTrue(dict2.ContainsFirst(new ComplexType(2)));
      Assert.IsTrue(dict2.ContainsSecond(2));
      dict2.Add(new ComplexType(2), 3);
      Assert.IsTrue(dict2.GetByFirst(new ComplexType(2)).Count() == 2);
      Assert.IsTrue(dict2.GetByFirst(new ComplexType(2)).Contains(1));
      Assert.IsTrue(dict2.GetByFirst(new ComplexType(2)).Contains(3));
      dict2.Add(new ComplexType(3), 1);
      Assert.IsTrue(dict2.GetBySecond(1).Count() == 2);
      Assert.IsTrue(dict2.GetBySecond(1).Any(x => x.Field == 2));
      Assert.IsTrue(dict2.GetBySecond(1).Any(x => x.Field == 3));
      Assert.IsTrue(dict2.CountFirst == 3);
      Assert.IsTrue(dict2.CountSecond == 3);
      dict2.Add(new ComplexType(2), 2);
      Assert.IsTrue(dict2.CountFirst == 3);
      Assert.IsTrue(dict2.CountSecond == 3);
      dict2.Add(new ComplexType(2), 4);
      Assert.IsTrue(dict2.CountFirst == 3);
      Assert.IsTrue(dict2.CountSecond == 4);
      dict2.RemoveByFirst(new ComplexType(2));
      Assert.IsTrue(dict2.CountFirst == 2);
      Assert.IsTrue(dict2.CountSecond == 2);
      dict2.RemovePair(new ComplexType(1), 1);
      Assert.IsTrue(dict2.CountFirst == 2);
      Assert.IsTrue(dict2.CountSecond == 2);
      dict2.RemovePair(new ComplexType(1), 2);
      Assert.IsTrue(dict2.CountFirst == 1);
      Assert.IsTrue(dict2.CountSecond == 1);
      dict2.Clear();
      Assert.IsTrue(!dict2.SecondEnumerable.Any());
    }
  }
}
