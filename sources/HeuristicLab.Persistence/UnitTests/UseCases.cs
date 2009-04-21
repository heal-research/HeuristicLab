using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Persistence.Core;
using System.Collections;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Default.DebugString;
using System.IO;

namespace HeuristicLab.Persistence.UnitTest {

  public class PrimitivesTest {
    [Storable]
    private bool _bool = true;
    [Storable]
    private byte _byte = 0xFF;
    [Storable]
    private sbyte _sbyte = 0xF;
    [Storable]
    private short _short = -123;
    [Storable]
    private ushort _ushort = 123;
    [Storable]
    private int _int = -123;
    [Storable]
    private uint _uint = 123;
    [Storable]
    private long _long = 123456;
    [Storable]
    private ulong _ulong = 123456;
    [Storable]
    private long[,] _long_array =
      new long[,] { { 123, 456, }, { 789, 123 } };
    [Storable]
    public List<int> list = new List<int> { 1, 2, 3, 4, 5 };
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
    public string name = "<![CDATA[<![CDATA[Serial]]>]]>";
  }

  public class Manager {

    public DateTime lastLoadTime;
    [Storable]
    private DateTime lastLoadTimePersistence {
      get { return lastLoadTime; }
      set { lastLoadTime = DateTime.Now; }
    }
    [Storable]
    public double? dbl;
  }

  public class C {
    [Storable]
    public C[][] allCs;
    [Storable]
    public KeyValuePair<List<C>, C> kvpList;
  }


  [TestClass]
  public class UseCases {

    private string tempFile;

    [TestInitialize()]
    public void CreateTempFile() {
      tempFile = Path.GetTempFileName();
    }

    [TestCleanup()]
    public void ClearTempFile() {
      File.Delete(tempFile);
    }

    [TestMethod]
    public void ComplexStorable() {
      Root r = new Root();
      r.selfReferences = new List<Root> { r, r };
      r.c = new Custom { r = r };
      r.dict.Add("one", 1);
      r.dict.Add("two", 2);
      r.dict.Add("three", 3);
      r.myEnum = TestEnum.va1;
      XmlGenerator.Serialize(r, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(r),
        DebugStringGenerator.Serialize(o));
      Root newR = (Root)o;
      Assert.AreSame(newR, newR.selfReferences[0]);
      Assert.AreNotSame(r, newR);
    }

    [TestMethod]
    public void SelfReferences() {
      C c = new C();
      C[][] cs = new C[2][];
      cs[0] = new C[] { c };
      cs[1] = new C[] { c };
      c.allCs = cs;
      c.kvpList = new KeyValuePair<List<C>, C>(new List<C> { c }, c);
      XmlGenerator.Serialize(cs, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(cs),
        DebugStringGenerator.Serialize(o));
      Assert.AreSame(c, c.allCs[0][0]);
      Assert.AreSame(c, c.allCs[1][0]);
      Assert.AreSame(c, c.kvpList.Key[0]);
      Assert.AreSame(c, c.kvpList.Value);
      C[][] newCs = (C[][])o;
      C newC = newCs[0][0];
      Assert.AreSame(newC, newC.allCs[0][0]);
      Assert.AreSame(newC, newC.allCs[1][0]);
      Assert.AreSame(newC, newC.kvpList.Key[0]);
      Assert.AreSame(newC, newC.kvpList.Value);
    }

    [TestMethod]
    public void ArrayCreation() {
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
      XmlGenerator.Serialize(arrayListArray, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(arrayListArray),
        DebugStringGenerator.Serialize(o));
      ArrayList[] newArray = (ArrayList[])o;
      Assert.AreSame(arrayListArray, arrayListArray[0][0]);
      Assert.AreSame(arrayListArray, arrayListArray[2][1]);
      Assert.AreSame(newArray, newArray[0][0]);
      Assert.AreSame(newArray, newArray[2][1]);
    }

    [TestMethod]
    public void CustomSerializationProperty() {
      Manager m = new Manager();
      XmlGenerator.Serialize(m, tempFile);
      Manager newM = (Manager)XmlParser.DeSerialize(tempFile);
      Assert.AreNotEqual(
        DebugStringGenerator.Serialize(m),
        DebugStringGenerator.Serialize(newM));
      Assert.AreEqual(m.dbl, newM.dbl);
      Assert.AreEqual(m.lastLoadTime, new DateTime());
      Assert.AreNotEqual(newM.lastLoadTime, new DateTime());
      Assert.IsTrue((DateTime.Now - newM.lastLoadTime).TotalSeconds < 10);
    }

    [TestMethod]
    public void Primitives() {
      PrimitivesTest sdt = new PrimitivesTest();
      XmlGenerator.Serialize(sdt, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(sdt),
        DebugStringGenerator.Serialize(o));
    }

    [TestMethod]
    public void MultiDimensionalArray() {
      string[,] mDimString = new string[,] {
        {"ora", "et", "labora"},
        {"Beten", "und", "Arbeiten"}
      };
      XmlGenerator.Serialize(mDimString, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(mDimString),
        DebugStringGenerator.Serialize(o));
    }

    public class NestedType {
      [Storable]
      private string value = "value";
    }

    [TestMethod]
    public void NestedTypeTest() {
      NestedType t = new NestedType();
      XmlGenerator.Serialize(t, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(t),
        DebugStringGenerator.Serialize(o));
    }


    [TestMethod]
    public void SimpleArray() {
      string[] strings = { "ora", "et", "labora" };
      XmlGenerator.Serialize(strings, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(strings),
        DebugStringGenerator.Serialize(o));
    }

    [TestMethod]
    public void BinaryFormatTest() {
      Root r = new Root();
      Assert.Fail("Not Implemented");
      //BinaryGenerator.Serialize(r, "test.bin");
    }


    [TestMethod]
    public void PrimitiveRoot() {
      XmlGenerator.Serialize(12.3f, tempFile);
      object o = XmlParser.DeSerialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(12.3f),
        DebugStringGenerator.Serialize(o));
    }


    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
      ConfigurationService.Instance.Reset();
    }
  }
}
