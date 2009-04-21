using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Persistence.Test {

  class DemoClass {

    [Storable(Name = "TestProperty", DefaultValue = 12)]
    public object o;

    [Storable]
    public int x = 2;

    public int y;

  }

  [TestClass]
  public class AttributeTest {

    [TestMethod]
    public void SimpleStorableAttributeTest() {
      DemoClass t = new DemoClass();
      Dictionary<string, DataMemberAccessor> accessors = StorableAttribute.GetStorableAccessors(t);
      Assert.IsTrue(accessors.ContainsKey("o"));
      Assert.IsTrue(accessors.ContainsKey("x"));
      Assert.IsFalse(accessors.ContainsKey("y"));
      object o = new object();
      t.o = o;
      Assert.AreSame(accessors["o"].Get(), o);
      t.x = 12;
      Assert.AreEqual(accessors["x"].Get(), 12);
      t.y = 312890;
      accessors["o"].Set(null);
      Assert.IsNull(t.o);
      accessors["x"].Set(123);
      Assert.AreEqual(t.x, 123);
    }

  }

}
