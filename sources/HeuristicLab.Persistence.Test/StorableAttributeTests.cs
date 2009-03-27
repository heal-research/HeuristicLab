using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Test {

  class TestClass {

    [Storable(Name="TestProperty", DefaultValue=12)]
    public object o;

    [Storable]
    public int x = 2;

    public int y;

  }

  class AttributeTest {

    static void Main() {
      TestClass t = new TestClass();
      Dictionary<string, DataMemberAccessor> accessors = StorableAttribute.GetAutostorableAccessors(t);
      foreach ( KeyValuePair<string, DataMemberAccessor> pair in accessors ) {
        Console.WriteLine(pair.Key + ": " + pair.Value);
      }
      t.o = new Object();
      t.x = 12;
      t.y = 312890;
      accessors["o"].Set(null);
      accessors["x"].Set(123);
      try {
        accessors["y"].Set(0);
        throw new InvalidOperationException();
      } catch ( KeyNotFoundException ) { }
      Console.ReadLine();
    }

  }

}
