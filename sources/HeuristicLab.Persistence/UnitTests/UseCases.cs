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
using System.Reflection;
using HeuristicLab.Persistence.Default.Decomposers.Storable;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml.Primitive;
using HeuristicLab.Persistence.Default.Decomposers;

namespace HeuristicLab.Persistence.UnitTest {

  public class NumberTest {
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
  }

  public class NonDefaultConstructorClass {
    [Storable]
    int value;
    public NonDefaultConstructorClass(int value) {
      this.value = value;
    }
  }

  public class IntWrapper {

    [Storable]
    public int Value;

    private IntWrapper() { }

    public IntWrapper(int value) {
      this.Value = value;
    }

    public override bool Equals(object obj) {
      if (obj as IntWrapper == null)
        return false;
      return Value.Equals(((IntWrapper)obj).Value);
    }
    public override int GetHashCode() {
      return Value.GetHashCode();
    }

  }

  public class EventTest {
    public delegate object Filter(object o);
    public event Filter OnChange;
    [Storable]
    private Delegate[] OnChangeListener {
      get { return OnChange.GetInvocationList(); }
      set {
        foreach (Delegate d in value) {
          OnChange += (Filter)d;
        }
      }
    }
  }

  public class PrimitivesTest : NumberTest {
    [Storable]
    private char c = 'e';
    [Storable]
    private long[,] _long_array =
      new long[,] { { 123, 456, }, { 789, 123 } };
    [Storable]
    public List<int> list = new List<int> { 1, 2, 3, 4, 5 };
    [Storable]
    private object o = new object();
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
    public Stack<int> intStack = new Stack<int>();
    [Storable]
    public int[] i = new[] { 3, 4, 5, 6 };
    [Storable(Name = "Test String")]
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

  public enum SimpleEnum { one, two, three }
  public enum ComplexEnum { one = 1, two = 2, three = 3 }
  [FlagsAttribute]
  public enum TrickyEnum { zero = 0, one = 1, two = 2 }

  public class EnumTest {
    [Storable]
    public SimpleEnum simpleEnum = SimpleEnum.one;
    [Storable]
    public ComplexEnum complexEnum = (ComplexEnum)2;
    [Storable]
    public TrickyEnum trickyEnum = (TrickyEnum)15;
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

  public class NonSerializable {
    int x;
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
      StreamReader reader = new StreamReader(tempFile);
      string s = reader.ReadToEnd();
      reader.Close();
      File.Delete(tempFile);
    }

    [TestMethod]
    public void ComplexStorable() {
      Root r = new Root();
      r.intStack.Push(1);
      r.intStack.Push(2);
      r.intStack.Push(3);
      r.selfReferences = new List<Root> { r, r };
      r.c = new Custom { r = r };
      r.dict.Add("one", 1);
      r.dict.Add("two", 2);
      r.dict.Add("three", 3);
      r.myEnum = TestEnum.va1;
      r.i = new[] { 7, 5, 6 };
      r.s = "new value";
      r.intArray = new ArrayList { 3, 2, 1 };
      r.intList = new List<int> { 9, 8, 7 };
      r.multiDimArray = new double[,] { { 5, 4, 3 }, { 1, 4, 6 } };
      r.boolean = false;
      r.dateTime = DateTime.Now;
      r.kvp = new KeyValuePair<string, int>("string key", 321);
      r.uninitialized = null;
      XmlGenerator.Serialize(r, tempFile);
      Root newR = (Root)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(r),
        DebugStringGenerator.Serialize(newR));
      Assert.AreSame(newR, newR.selfReferences[0]);
      Assert.AreNotSame(r, newR);
      Assert.AreEqual(r.myEnum, TestEnum.va1);
      Assert.AreEqual(r.i[0], 7);
      Assert.AreEqual(r.i[1], 5);
      Assert.AreEqual(r.i[2], 6);
      Assert.AreEqual(r.s, "new value");
      Assert.AreEqual(r.intArray[0], 3);
      Assert.AreEqual(r.intArray[1], 2);
      Assert.AreEqual(r.intArray[2], 1);
      Assert.AreEqual(r.intList[0], 9);
      Assert.AreEqual(r.intList[1], 8);
      Assert.AreEqual(r.intList[2], 7);
      Assert.AreEqual(r.multiDimArray[0, 0], 5);
      Assert.AreEqual(r.multiDimArray[0, 1], 4);
      Assert.AreEqual(r.multiDimArray[0, 2], 3);
      Assert.AreEqual(r.multiDimArray[1, 0], 1);
      Assert.AreEqual(r.multiDimArray[1, 1], 4);
      Assert.AreEqual(r.multiDimArray[1, 2], 6);
      Assert.IsFalse(r.boolean);
      Assert.IsTrue((DateTime.Now - r.dateTime).TotalSeconds < 10);
      Assert.AreEqual(r.kvp.Key, "string key");
      Assert.AreEqual(r.kvp.Value, 321);
      Assert.IsNull(r.uninitialized);
      Assert.AreEqual(newR.myEnum, TestEnum.va1);
      Assert.AreEqual(newR.i[0], 7);
      Assert.AreEqual(newR.i[1], 5);
      Assert.AreEqual(newR.i[2], 6);
      Assert.AreEqual(newR.s, "new value");
      Assert.AreEqual(newR.intArray[0], 3);
      Assert.AreEqual(newR.intArray[1], 2);
      Assert.AreEqual(newR.intArray[2], 1);
      Assert.AreEqual(newR.intList[0], 9);
      Assert.AreEqual(newR.intList[1], 8);
      Assert.AreEqual(newR.intList[2], 7);
      Assert.AreEqual(newR.multiDimArray[0, 0], 5);
      Assert.AreEqual(newR.multiDimArray[0, 1], 4);
      Assert.AreEqual(newR.multiDimArray[0, 2], 3);
      Assert.AreEqual(newR.multiDimArray[1, 0], 1);
      Assert.AreEqual(newR.multiDimArray[1, 1], 4);
      Assert.AreEqual(newR.multiDimArray[1, 2], 6);
      Assert.AreEqual(newR.intStack.Pop(), 3);
      Assert.AreEqual(newR.intStack.Pop(), 2);
      Assert.AreEqual(newR.intStack.Pop(), 1);
      Assert.IsFalse(newR.boolean);
      Assert.IsTrue((DateTime.Now - newR.dateTime).TotalSeconds < 10);
      Assert.AreEqual(newR.kvp.Key, "string key");
      Assert.AreEqual(newR.kvp.Value, 321);
      Assert.IsNull(newR.uninitialized);
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
      object o = XmlParser.Deserialize(tempFile);
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
      object o = XmlParser.Deserialize(tempFile);
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
      Manager newM = (Manager)XmlParser.Deserialize(tempFile);
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
      object o = XmlParser.Deserialize(tempFile);
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
      object o = XmlParser.Deserialize(tempFile);
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
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(t),
        DebugStringGenerator.Serialize(o));
    }


    [TestMethod]
    public void SimpleArray() {
      string[] strings = { "ora", "et", "labora" };
      XmlGenerator.Serialize(strings, tempFile);
      object o = XmlParser.Deserialize(tempFile);
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
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(12.3f),
        DebugStringGenerator.Serialize(o));
    }

    private string formatFullMemberName(MemberInfo mi) {
      return new StringBuilder()
        .Append(mi.DeclaringType.Assembly.GetName().Name)
        .Append(": ")
        .Append(mi.DeclaringType.Namespace)
        .Append('.')
        .Append(mi.DeclaringType.Name)
        .Append('.')
        .Append(mi.Name).ToString();
    }

    [TestMethod]
    public void CodingConventions() {
      List<string> lowerCaseMethodNames = new List<string>();
      List<string> lowerCaseProperties = new List<string>();
      List<string> lowerCaseFields = new List<string>();
      foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
        if (!a.GetName().Name.StartsWith("HeuristicLab"))
          continue;
        foreach (Type t in a.GetTypes()) {
          foreach (MemberInfo mi in t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            if (char.IsLower(mi.Name[0])) {
              if (mi.MemberType == MemberTypes.Field)
                lowerCaseFields.Add(formatFullMemberName(mi));
              if (mi.MemberType == MemberTypes.Property)
                lowerCaseProperties.Add(formatFullMemberName(mi));
              if (mi.MemberType == MemberTypes.Method &&
                !mi.Name.StartsWith("get_") &&
                !mi.Name.StartsWith("set_") &&
                !mi.Name.StartsWith("add_") &&
                !mi.Name.StartsWith("remove_"))
                lowerCaseMethodNames.Add(formatFullMemberName(mi));
            }
          }
        }
      }
      //Assert.AreEqual("", lowerCaseFields.Aggregate("", (a, b) => a + "\r\n" + b));
      Assert.AreEqual("", lowerCaseMethodNames.Aggregate("", (a, b) => a + "\r\n" + b));
      Assert.AreEqual("", lowerCaseProperties.Aggregate("", (a, b) => a + "\r\n" + b));
    }

    [TestMethod]
    public void Number2StringDecomposer() {
      NumberTest sdt = new NumberTest();
      XmlGenerator.Serialize(sdt, tempFile,
        new Configuration(new XmlFormat(),
          new List<IFormatter> { new String2XmlFormatter() },
          new List<IDecomposer> { 
            new StorableDecomposer(),
            new Number2StringDecomposer() }));
      object o = XmlParser.Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(sdt),
        DebugStringGenerator.Serialize(o));
    }

    [TestMethod]
    public void Events() {
      EventTest et = new EventTest();
      et.OnChange += (o) => o;
      XmlGenerator.Serialize(et, tempFile);
      EventTest newEt = (EventTest)XmlParser.Deserialize(tempFile);
    }

    [TestMethod]
    public void Enums() {
      EnumTest et = new EnumTest();
      et.simpleEnum = SimpleEnum.two;
      et.complexEnum = ComplexEnum.three;
      et.trickyEnum = TrickyEnum.two | TrickyEnum.one;
      XmlGenerator.Serialize(et, tempFile);
      EnumTest newEt = (EnumTest)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(et.simpleEnum, SimpleEnum.two);
      Assert.AreEqual(et.complexEnum, ComplexEnum.three);
      Assert.AreEqual(et.trickyEnum, (TrickyEnum)3);
    }

    [TestMethod]
    public void TestAliasingWithOverriddenEquals() {
      List<IntWrapper> ints = new List<IntWrapper>();
      ints.Add(new IntWrapper(1));
      ints.Add(new IntWrapper(1));
      Assert.AreEqual(ints[0], ints[1]);
      Assert.AreNotSame(ints[0], ints[1]);
      XmlGenerator.Serialize(ints, tempFile);
      List<IntWrapper> newInts = (List<IntWrapper>)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(newInts[0].Value, 1);
      Assert.AreEqual(newInts[1].Value, 1);
      Assert.AreEqual(newInts[0], newInts[1]);
      Assert.AreNotSame(newInts[0], newInts[1]);
    }

    [TestMethod]
    public void NonDefaultConstructorTest() {
      NonDefaultConstructorClass c = new NonDefaultConstructorClass(1);
      try {
        XmlGenerator.Serialize(c, tempFile);
        Assert.Fail("Exception not thrown");
      } catch (PersistenceException) {
      }
    }

    [TestMethod]
    public void TestSavingException() {      
      List<int> list = new List<int> { 1, 2, 3 };
      XmlGenerator.Serialize(list, tempFile);
      NonSerializable s = new NonSerializable();
      try {
        XmlGenerator.Serialize(s, tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) { }
      List<int> newList = (List<int>)XmlParser.Deserialize(tempFile);
      Assert.AreEqual(list[0], newList[0]);
      Assert.AreEqual(list[1], newList[1]);
    }

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
      ConfigurationService.Instance.Reset();
    }
  }
}
