using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.ViewOnly;
using HeuristicLab.Tracing;
using log4net;
using log4net.Config;

namespace HeuristicLab.Persistence.Test {

  public class StringDecomposerTest {
    [Storable] private bool _bool = true;
    [Storable] private byte _byte = 0xFF;
    [Storable] private sbyte _sbyte = 0xF;
    [Storable] private short _short = -123;
    [Storable] private ushort _ushort = 123;
    [Storable] private int _int = -123;
    [Storable] private uint _uint = 123;
    [Storable] private long _long = 123456;
    [Storable] private ulong _ulong = 123456;
    [Storable] private long[,] _long_array =
      new long[,] { { 123, 456, },  {789, 123 }} ;
    [Storable] public List<int> list = new List<int>{ 1, 2, 3, 4, 5};
  }

  public enum TestEnum { va1, va2, va3, va8 } ;

  public class RootBase {
    [Storable]
    private string baseString = "   Serial  ";
    [Storable]
    public TestEnum myEnum = TestEnum.va3;
  }

  public class Root : RootBase {
    [Storable]
    public int[] i = new[] { 3, 4, 5, 6 };
    [Storable]
    public string s;
    [Storable]
    public ArrayList intArray = new ArrayList(new[] { 1, 2, 3 });
    [Storable]
    public List<int> intList = new List<int>(new[] { 321, 312, 321 });
    [Storable]
    public Custom c;
    [Storable]
    public List<Root> selfReferences;
    [Storable]
    public double[,] multiDimArray = new double[,] { { 1, 2, 3 }, { 3, 4, 5 } };
    [Storable]
    public bool boolean = true;
    [Storable]
    public DateTime dateTime;
    [Storable]
    public KeyValuePair<string, int> kvp = new KeyValuePair<string, int>("Serial", 123);
    [Storable]
    public Dictionary<string, int> dict = new Dictionary<string, int>();
    [Storable(DefaultValue = "default")]
    public string uninitialized;
  }

  public class Custom {
    [Storable]
    public int i;
    [Storable]
    public Root r;
    [Storable]
    public string name = "Serial";
  }

  public class CloneableRoot {
    public int[] i = new[] { 3, 4, 5, 6 };
    public string s;
    public ArrayList intArray = new ArrayList(new[] { 1, 2, 3 });
    public List<int> intList = new List<int>(new[] { 321, 312, 321 });
    public CloneableCustom c;
    public List<CloneableRoot> selfReferences;
    public double[,] multiDimArray = new double[,] { { 1, 2, 3 }, { 3, 4, 5 } };
    public bool boolean = true;
    public DateTime dateTime;
    public KeyValuePair<string, int> kvp = new KeyValuePair<string, int>("test key", 123);
    public Dictionary<string, int> dict = new Dictionary<string, int>();
    public object Clone(Dictionary<object, object> twins) {
      if (twins.ContainsKey(this))
        return twins[this];
      CloneableRoot cr = new CloneableRoot();
      twins.Add(this, cr);
      cr.i = i;
      cr.s = s;
      cr.intArray = new ArrayList(intArray);
      cr.intList = new List<int>(intList);
      cr.c = (CloneableCustom)c.Clone(twins);
      cr.selfReferences = new List<CloneableRoot>();
      for (int j = 0; j < selfReferences.Count; j++) {
        cr.selfReferences.Add(this);
      }
      cr.multiDimArray = (double[,])multiDimArray.Clone();
      cr.dateTime = new DateTime(dateTime.Ticks);
      cr.kvp = new KeyValuePair<string, int>(kvp.Key, kvp.Value);
      cr.dict = new Dictionary<string, int>(dict);
      return cr;
    }
  }

  public class CloneableCustom {
    public int i;
    public CloneableRoot r;
    public string name = "Serial";
    public object Clone(Dictionary<object, object> twins) {
      if (twins.ContainsKey(this))
        return twins[this];
      CloneableCustom cc = new CloneableCustom();
      twins.Add(this, cc);
      cc.i = i;
      cc.r = (CloneableRoot)r.Clone(twins);
      cc.name = name;
      return cc;
    }
  }

  public class Manager {

    private DateTime lastLoadTime;
    [Storable]
    private DateTime lastLoadTimePersistence {
      get { return lastLoadTime; }
      // ReSharper disable ValueParameterNotUsed
      set { lastLoadTime = DateTime.Now; }
      // ReSharper restore ValueParameterNotUsed
    }
    [Storable]
    private double? dbl;
  }

  public class StorableObject {

    [Storable]
    Dictionary<int, string> dict;

    public void Init() {
      dict = new Dictionary<int, string>();
      for (int i = 0; i < 1000000; i++) {
        dict.Add(i, i.ToString());
      }
    }
  }

  public class CloneableObject : ICloneable {

    Dictionary<int, string> dict;

    public void Init() {
      dict = new Dictionary<int, string>();
      for (int i = 0; i < 1000000; i++) {
        dict.Add(i, i.ToString());
      }
    }
    public object Clone() {
      CloneableObject clone = new CloneableObject {
        dict = new Dictionary<int, string>(dict)
      };
      return clone;
    }
  }

  public class C {
    [Storable]
    public C[][] allCs;
    public KeyValuePair<List<C>, C> kvpList;
  }

  public class NewSerializationTest {

    public static void Test1() {      
      Root r = new Root();
      r.selfReferences = new List<Root> {r, r};
      r.c = new Custom {r = r};
      r.dict.Add("one", 1);
      r.dict.Add("two", 2);
      r.dict.Add("three", 3);
      r.myEnum = TestEnum.va1;
      XmlGenerator.Serialize(r, "test.zip");      
      object o = XmlParser.DeSerialize("test.zip");
      Console.Out.WriteLine(Util.AutoFormat(o, true));
      Console.WriteLine(ViewOnlyGenerator.Serialize(r));
      Console.WriteLine(ViewOnlyGenerator.Serialize(o));
    }

    public static void Test3() {
      C c = new C();
      C[][] cs = new C[2][];
      cs[0] = new C[] { c };
      cs[1] = new C[] { c };
      c.allCs = cs;
      c.kvpList = new KeyValuePair<List<C>, C>(new List<C> { c }, c);
      XmlGenerator.Serialize(cs, "test3.zip");
      object o = XmlParser.DeSerialize("test3.zip");
      Console.Out.WriteLine(Util.AutoFormat(o, true));
      Console.WriteLine(ViewOnlyGenerator.Serialize(cs));
      Console.WriteLine(ViewOnlyGenerator.Serialize(o));
    }

    public static void Test4() {
      ArrayList[] arrayListArray = new ArrayList[4];
      arrayListArray[0] = new ArrayList();
      arrayListArray[0].Add(arrayListArray);
      arrayListArray[0].Add(arrayListArray);
      arrayListArray[1] = new ArrayList();
      arrayListArray[1].Add(arrayListArray);
      arrayListArray[2] = new ArrayList();
      arrayListArray[2].Add(arrayListArray);
      arrayListArray[2].Add(arrayListArray);
      Array a = Array.CreateInstance(
                              typeof(object),
                              new[] { 1, 2 }, new[] { 3, 4 });
      arrayListArray[2].Add(a);
      XmlGenerator.Serialize(arrayListArray, "test4.zip");
      object o = XmlParser.DeSerialize("test4.zip");
      Console.Out.WriteLine(Util.AutoFormat(o, true));
      Console.WriteLine(ViewOnlyGenerator.Serialize(arrayListArray));
      Console.WriteLine(ViewOnlyGenerator.Serialize(o));
    }

    public static void Test2() {
      Manager m = new Manager();      
      XmlGenerator.Serialize(m, "test2.zip");
      object o = XmlParser.DeSerialize("test2.zip");
      Console.Out.WriteLine(Util.AutoFormat(o, true));      
    }    

    

    public static void Test5() {
      StringDecomposerTest sdt = new StringDecomposerTest();      
      XmlGenerator.Serialize(sdt, "test5.zip");
      object o = XmlParser.DeSerialize("test5.zip");
      Console.WriteLine(ViewOnlyGenerator.Serialize(sdt));
      Console.WriteLine(ViewOnlyGenerator.Serialize(o));
    }

    public static void Test6() {
      string[,] mDimString = new string[,] {
        {"ora", "et", "labora"},
        {"Beten", "und", "Arbeiten"}
      };
      XmlGenerator.Serialize(mDimString, "test6.zip");
      object o = XmlParser.DeSerialize("test6.zip");
      Console.WriteLine(ViewOnlyGenerator.Serialize(mDimString));
      Console.WriteLine(ViewOnlyGenerator.Serialize(o));
    }

    public class NestedType {
      [Storable]
      private string value = "value";
    }

    public static void Test7() {
      NestedType t = new NestedType();
      XmlGenerator.Serialize(t, "test7.zip");
      object o = XmlParser.DeSerialize("test7.zip");
      Console.WriteLine(ViewOnlyGenerator.Serialize(t));
      Console.WriteLine(ViewOnlyGenerator.Serialize(o));
    }


    public static void Main() {
      BasicConfigurator.Configure();
      Test1();      
      Test2();
      Test3();
      Test4();
      Test5();
      Test6();
      Test7();
      //SpeedTest();
      //SpeedTest2();
      Console.In.ReadLine();
    }
  }
}
