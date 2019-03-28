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

using System.Collections;
using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.PluginInfrastructure.Tests {
  [TestClass]
  public class TypeExtensionsTest {
    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "short")]
    public void IsSubTypeOfTest() {
      Assert.IsTrue(typeof(int).IsAssignableTo(typeof(object)));
      Assert.IsTrue(typeof(IntValue).IsAssignableTo(typeof(IItem)));
      Assert.IsTrue(typeof(List<int>).IsAssignableTo(typeof(object)));

      Assert.IsTrue(typeof(List<int>).IsAssignableTo(typeof(IList)));
      Assert.IsTrue(typeof(List<>).IsAssignableTo(typeof(IList)));
      Assert.IsFalse(typeof(NamedItemCollection<>).IsAssignableTo(typeof(ICollection<IItem>)));
      Assert.IsFalse(typeof(NamedItemCollection<>).IsAssignableTo(typeof(ICollection<NamedItem>)));

      //new tests      


      Assert.IsTrue(typeof(List<IItem>).IsAssignableTo(typeof(IList<IItem>)));
      Assert.IsFalse(typeof(IList<IntValue>).IsAssignableTo(typeof(IList<IItem>)));
      Assert.IsTrue(typeof(List<IItem>).IsAssignableTo(typeof(IList<IItem>)));
      Assert.IsFalse(typeof(ItemList<>).IsAssignableTo(typeof(IList<IItem>)));
      Assert.IsFalse(typeof(ItemList<>).IsAssignableTo(typeof(List<IItem>)));

      Assert.IsTrue(typeof(List<int>).IsAssignableTo(typeof(List<>)));
      Assert.IsTrue(typeof(List<>).IsAssignableTo(typeof(IList<>)));
      Assert.IsTrue(typeof(ItemList<>).IsAssignableTo(typeof(IList<>)));
      Assert.IsTrue(typeof(NamedItemCollection<>).IsAssignableTo(typeof(IItemCollection<>)));
      Assert.IsTrue(typeof(List<IntValue>).IsAssignableTo(typeof(IList<>)));
    }

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "short")]
    public void BuildTypeTest() {
      Assert.AreEqual(typeof(List<>).BuildType(typeof(List<>)), typeof(List<>));
      Assert.AreEqual(typeof(List<int>).BuildType(typeof(List<>)), typeof(List<int>));
      Assert.AreEqual(typeof(List<>).BuildType(typeof(List<int>)), typeof(List<int>));

      Assert.AreEqual(typeof(ICollection<>).BuildType(typeof(List<>)), typeof(ICollection<>));
      Assert.AreEqual(typeof(ICollection<int>).BuildType(typeof(List<>)), typeof(ICollection<int>));
      Assert.AreEqual(typeof(ICollection<>).BuildType(typeof(List<int>)), typeof(ICollection<int>));

      Assert.AreEqual(typeof(ItemCollection<>).BuildType(typeof(ICollection<int>)), null);
      Assert.AreEqual(typeof(ItemCollection<>).BuildType(typeof(Dictionary<IItem, IItem>)), null);
      Assert.AreEqual(typeof(ItemCollection<>).BuildType(typeof(ICollection<IItem>)), typeof(ItemCollection<IItem>));

      Assert.AreEqual(typeof(FixedValueParameter<>).BuildType(typeof(ItemCollection<DataAnalysisProblemData>)), null);
      Assert.AreEqual(typeof(IFixedValueParameter<>).BuildType(typeof(ItemCollection<DoubleValue>)), typeof(IFixedValueParameter<DoubleValue>));
      Assert.AreEqual(typeof(IFixedValueParameter<>).BuildType(typeof(ItemCollection<IItem>)), typeof(IFixedValueParameter<IItem>));
    }
  }
}
