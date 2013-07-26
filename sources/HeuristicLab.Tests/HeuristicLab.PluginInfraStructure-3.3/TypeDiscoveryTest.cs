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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.PluginInfrastructure.Tests {
  /// <summary>
  /// Summary description for TypeDiscoveryTest
  /// </summary>
  [TestClass]
  public class TypeDiscoveryTest {
    public TypeDiscoveryTest() { }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
    }

    [TestMethod]
    public void IsSubTypeOfTest() {
      Assert.IsTrue(typeof(int).IsSubTypeOf(typeof(object)));
      Assert.IsTrue(typeof(IntValue).IsSubTypeOf(typeof(IItem)));
      Assert.IsTrue(typeof(List<int>).IsSubTypeOf(typeof(object)));

      Assert.IsTrue(typeof(List<int>).IsSubTypeOf(typeof(IList)));
      Assert.IsTrue(typeof(List<>).IsSubTypeOf(typeof(IList)));
      Assert.IsFalse(typeof(NamedItemCollection<>).IsSubTypeOf(typeof(ICollection<IItem>)));
      Assert.IsFalse(typeof(NamedItemCollection<>).IsSubTypeOf(typeof(ICollection<NamedItem>)));


      Assert.IsTrue(typeof(List<IItem>).IsSubTypeOf(typeof(IList<IItem>)));
      Assert.IsFalse(typeof(IList<IntValue>).IsSubTypeOf(typeof(IList<IItem>)));
      Assert.IsTrue(typeof(List<IItem>).IsSubTypeOf(typeof(IList<IItem>)));
      Assert.IsFalse(typeof(ItemList<>).IsSubTypeOf(typeof(IList<IItem>)));
      Assert.IsFalse(typeof(ItemList<>).IsSubTypeOf(typeof(List<IItem>)));

      Assert.IsFalse(typeof(List<int>).IsSubTypeOf(typeof(List<>)));
      Assert.IsTrue(typeof(List<>).IsSubTypeOf(typeof(IList<>)));
      Assert.IsTrue(typeof(ItemList<>).IsSubTypeOf(typeof(IList<>)));
      Assert.IsTrue(typeof(NamedItemCollection<>).IsSubTypeOf(typeof(IItemCollection<>)));
      Assert.IsFalse(typeof(List<IntValue>).IsSubTypeOf(typeof(IList<>)));
    }

    [TestMethod]
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
