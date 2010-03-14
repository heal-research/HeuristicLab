using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Reflection;

namespace HeuristicLab.Persistence_33.Tests {

  [StorableClass] 
  class DemoClass {

    [Storable(Name = "TestProperty", DefaultValue = 12)]
    public object o;

    [Storable]
    public int x = 2;

    public int y;

  }

  [StorableClass]
  class Base {
    public string baseName;
    [Storable]
    public virtual string Name {
      get { return "Base"; }
      set { baseName = value; }
    }
  }

  [StorableClass]
  class Override : Base {
    [Storable]
    public override string Name {
      get { return "Override"; }
      set { base.Name = value; }
    }
  }

  [StorableClass]
  class Intermediate : Override {
  }

  [StorableClass]
  class New : Intermediate {
    public string newName;
    [Storable]
    public new string Name {
      get { return "New"; }
      set { newName = value; }
    }
  }

  [TestClass]
  public class AttributeTest {

    [TestMethod]
    public void SimpleStorableAttributeTest() {
      DemoClass t = new DemoClass();
      IEnumerable<DataMemberAccessor> accessorList = StorableAttribute.GetStorableAccessors(t);
      Dictionary<string, DataMemberAccessor> accessors = new Dictionary<string, DataMemberAccessor>();
      foreach (var a in accessorList)
        accessors.Add(a.Name, a);
      Assert.IsTrue(accessors.ContainsKey("TestProperty"));
      Assert.IsTrue(accessors.ContainsKey("x"));
      Assert.IsFalse(accessors.ContainsKey("y"));
      object o = new object();
      t.o = o;
      Assert.AreSame(accessors["TestProperty"].Get(), o);
      t.x = 12;
      Assert.AreEqual(accessors["x"].Get(), 12);
      t.y = 312890;
      accessors["TestProperty"].Set(null);
      Assert.IsNull(t.o);
      accessors["x"].Set(123);
      Assert.AreEqual(t.x, 123);
    }

    [TestMethod]
    public void TestInheritance() {
      New n = new New();
      var accessors = StorableAttribute.GetStorableAccessors(n);
      var accessDict = new Dictionary<string, DataMemberAccessor>();
      foreach (var accessor in accessors) // assert uniqueness
        accessDict.Add(accessor.Name, accessor);
      Assert.IsTrue(accessDict.ContainsKey(typeof(New).FullName + ".Name"));
      Assert.IsTrue(accessDict.ContainsKey(typeof(Override).FullName + ".Name"));
      Assert.AreEqual("New", accessDict[typeof(New).FullName + ".Name"].Get());
      Assert.AreEqual("Override", accessDict[typeof(Override).FullName + ".Name"].Get());
    }

  }

}
