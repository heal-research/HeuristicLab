using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab_33.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.PluginInfraStructure.Tests {
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
