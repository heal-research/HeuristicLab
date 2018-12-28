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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HEAL.Fossil;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers;
using HeuristicLab.Persistence.Default.DebugString;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Persistence.Fossil.Tests {

  [StorableType("22B5FC22-44FA-40B4-84E3-BB53540E812E")]
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
    public override bool Equals(object obj) {
      NumberTest nt = obj as NumberTest;
      if (nt == null)
        throw new NotSupportedException();
      return
        nt._bool == _bool &&
        nt._byte == _byte &&
        nt._sbyte == _sbyte &&
        nt._short == _short &&
        nt._ushort == _ushort &&
        nt._int == _int &&
        nt._uint == _uint &&
        nt._long == _long &&
        nt._ulong == _ulong;
    }
    public override int GetHashCode() {
      return
        _bool.GetHashCode() ^
        _byte.GetHashCode() ^
        _sbyte.GetHashCode() ^
        _short.GetHashCode() ^
        _short.GetHashCode() ^
        _int.GetHashCode() ^
        _uint.GetHashCode() ^
        _long.GetHashCode() ^
        _ulong.GetHashCode();
    }

    [StorableConstructor]
    protected NumberTest(StorableConstructorFlag _) {
    }
    public NumberTest() {
    }
  }

  [StorableType("2D94AD3B-D411-403F-AC42-60824C78D802")]
  public class IntWrapper {

    [Storable]
    public int Value;

    [StorableConstructor]
    protected IntWrapper(StorableConstructorFlag _) {
    }

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

  [StorableType("45337DD7-26D0-42D0-8CC4-92E184AE0218")]
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
    public override bool Equals(object obj) {
      PrimitivesTest pt = obj as PrimitivesTest;
      if (pt == null)
        throw new NotSupportedException();
      return base.Equals(obj) &&
        c == pt.c &&
        _long_array == pt._long_array &&
        list == pt.list &&
        o == pt.o;
    }
    public override int GetHashCode() {
      return base.GetHashCode() ^
        c.GetHashCode() ^
        _long_array.GetHashCode() ^
        list.GetHashCode() ^
        o.GetHashCode();
    }

    [StorableConstructor]
    protected PrimitivesTest(StorableConstructorFlag _) : base(_) {
    }
    public PrimitivesTest() {
    }
  }

  [StorableType("2F63F603-CE7D-4262-99B4-A797F4D04907")]
  public enum TestEnum { va1, va2, va3, va8 };

  [StorableType("DC944CA9-5F6A-4EF3-AFBD-881FC63797DF")]
  public class RootBase {
    [Storable]
    private string baseString = "   Serial  ";
    [Storable]
    public TestEnum myEnum = TestEnum.va3;
    public override bool Equals(object obj) {
      RootBase rb = obj as RootBase;
      if (rb == null)
        throw new NotSupportedException();
      return baseString == rb.baseString &&
        myEnum == rb.myEnum;
    }
    public override int GetHashCode() {
      return baseString.GetHashCode() ^
        myEnum.GetHashCode();
    }

    [StorableConstructor]
    protected RootBase(StorableConstructorFlag _) {
    }
    public RootBase() {
    }
  }

  [StorableType("C478905A-5029-4F31-9D92-524F41272D46")]
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

    [StorableConstructor]
    protected Root(StorableConstructorFlag _) : base(_) {
    }
    public Root() {
    }
  }

  [StorableType("23DCF22C-EDAB-4C5A-9941-0F2D6030D467")]
  public enum SimpleEnum { one, two, three }
  [StorableType("1FA5C129-129E-485C-A8A7-59FCA10CBB20")]
  public enum ComplexEnum { one = 1, two = 2, three = 3 }
  [FlagsAttribute]
  [StorableType("D4A5D0CD-295C-4AC1-B5DA-D8DA2861E82C")]
  public enum TrickyEnum { zero = 0, one = 1, two = 2 }

  [StorableType("C6EC77AF-C565-4A83-8922-3C6E2370627B")]
  public class EnumTest {
    [Storable]
    public SimpleEnum simpleEnum = SimpleEnum.one;
    [Storable]
    public ComplexEnum complexEnum = (ComplexEnum)2;
    [Storable]
    public TrickyEnum trickyEnum = (TrickyEnum)15;

    [StorableConstructor]
    protected EnumTest(StorableConstructorFlag _) {
    }
    public EnumTest() {
    }
  }

  [StorableType("9E73E52B-9BF1-489D-9349-C490D518B7C4")]
  public class Custom {
    [Storable]
    public int i;
    [Storable]
    public Root r;
    [Storable]
    public string name = "<![CDATA[<![CDATA[Serial]]>]]>";

    [StorableConstructor]
    protected Custom(StorableConstructorFlag _) {
    }

    public Custom() {

    }
  }

  [StorableType("CEE5C689-948F-443A-A645-54868D913364")]
  public class Manager {

    public DateTime lastLoadTime;
    [Storable]
    private DateTime lastLoadTimePersistence {
      get { return lastLoadTime; }
      set { lastLoadTime = DateTime.Now; }
    }
    [Storable]
    public double? dbl;

    [StorableConstructor]
    protected Manager(StorableConstructorFlag _) {
    }
    public Manager() {
    }
  }

  [StorableType("14EB77CD-7061-4B2E-96EB-3E45CC265256")]
  public class C {
    [Storable]
    public C[][] allCs;
    [Storable]
    public KeyValuePair<List<C>, C> kvpList;

    [StorableConstructor]
    protected C(StorableConstructorFlag _) {
    }
    public C() {
    }
  }

  public class NonSerializable {
    int x = 0;
    public override bool Equals(object obj) {
      NonSerializable ns = obj as NonSerializable;
      if (ns == null)
        throw new NotSupportedException();
      return ns.x == x;
    }
    public override int GetHashCode() {
      return x.GetHashCode();
    }
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
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void ComplexStorable() {
      Root r = InitializeComplexStorable();
      var ser = new ProtoBufSerializer();
      ser.Serialize(r, tempFile);
      Root newR = (Root)ser.Deserialize(tempFile);
      CompareComplexStorables(r, newR);
    }

    private static void CompareComplexStorables(Root r, Root newR) {
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

    private static Root InitializeComplexStorable() {
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

      return r;
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void SelfReferences() {
      C c = new C();
      C[][] cs = new C[2][];
      cs[0] = new C[] { c };
      cs[1] = new C[] { c };
      c.allCs = cs;
      c.kvpList = new KeyValuePair<List<C>, C>(new List<C> { c }, c);
      new ProtoBufSerializer().Serialize(cs, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
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
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
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
      new ProtoBufSerializer().Serialize(arrayListArray, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
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
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void CustomSerializationProperty() {
      Manager m = new Manager();
      new ProtoBufSerializer().Serialize(m, tempFile);
      Manager newM = (Manager)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreNotEqual(
        DebugStringGenerator.Serialize(m),
        DebugStringGenerator.Serialize(newM));
      Assert.AreEqual(m.dbl, newM.dbl);
      Assert.AreEqual(m.lastLoadTime, new DateTime());
      Assert.AreNotEqual(newM.lastLoadTime, new DateTime());
      Assert.IsTrue((DateTime.Now - newM.lastLoadTime).TotalSeconds < 10);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void Primitives() {
      PrimitivesTest sdt = new PrimitivesTest();
      new ProtoBufSerializer().Serialize(sdt, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(sdt),
        DebugStringGenerator.Serialize(o));
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void MultiDimensionalArray() {
      string[,] mDimString = new string[,] {
        {"ora", "et", "labora"},
        {"Beten", "und", "Arbeiten"}
      };
      new ProtoBufSerializer().Serialize(mDimString, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(mDimString),
        DebugStringGenerator.Serialize(o));
    }

    [StorableType("59E73F41-B9D4-489B-AA9C-3A72173498CC")]
    public class NestedType {
      [Storable]
      private string value = "value";
      public override bool Equals(object obj) {
        NestedType nt = obj as NestedType;
        if (nt == null)
          throw new NotSupportedException();
        return nt.value == value;
      }
      public override int GetHashCode() {
        return value.GetHashCode();
      }

      [StorableConstructor]
      protected NestedType(StorableConstructorFlag _) {
      }
      public NestedType() {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void NestedTypeTest() {
      NestedType t = new NestedType();
      new ProtoBufSerializer().Serialize(t, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(t),
        DebugStringGenerator.Serialize(o));
      Assert.IsTrue(t.Equals(o));
    }


    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void SimpleArray() {
      string[] strings = { "ora", "et", "labora" };
      new ProtoBufSerializer().Serialize(strings, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(
        DebugStringGenerator.Serialize(strings),
        DebugStringGenerator.Serialize(o));
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void PrimitiveRoot() {
      new ProtoBufSerializer().Serialize(12.3f, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
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

    public void CodingConventions() {
      List<string> lowerCaseMethodNames = new List<string>();
      List<string> lowerCaseProperties = new List<string>();
      List<string> lowerCaseFields = new List<string>();
      foreach (Assembly a in PluginLoader.Assemblies) {
        if (!a.GetName().Name.StartsWith("HeuristicLab"))
          continue;
        foreach (Type t in a.GetTypes()) {
          foreach (MemberInfo mi in t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            if (mi.DeclaringType.Name.StartsWith("<>"))
              continue;
            if (char.IsLower(mi.Name[0])) {
              if (mi.MemberType == MemberTypes.Field)
                lowerCaseFields.Add(formatFullMemberName(mi));
              if (mi.MemberType == MemberTypes.Property)
                lowerCaseProperties.Add(formatFullMemberName(mi));
              if (mi.MemberType == MemberTypes.Method &&
                !mi.Name.StartsWith("get_") &&
                !mi.Name.StartsWith("set_") &&
                !mi.Name.StartsWith("add_") &&
                !mi.Name.StartsWith("remove_") &&
                !mi.Name.StartsWith("op_"))
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
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void Enums() {
      EnumTest et = new EnumTest();
      et.simpleEnum = SimpleEnum.two;
      et.complexEnum = ComplexEnum.three;
      et.trickyEnum = TrickyEnum.two | TrickyEnum.one;
      new ProtoBufSerializer().Serialize(et, tempFile);
      EnumTest newEt = (EnumTest)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(et.simpleEnum, SimpleEnum.two);
      Assert.AreEqual(et.complexEnum, ComplexEnum.three);
      Assert.AreEqual(et.trickyEnum, (TrickyEnum)3);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestAliasingWithOverriddenEquals() {
      List<IntWrapper> ints = new List<IntWrapper>();
      ints.Add(new IntWrapper(1));
      ints.Add(new IntWrapper(1));
      Assert.AreEqual(ints[0], ints[1]);
      Assert.AreNotSame(ints[0], ints[1]);
      new ProtoBufSerializer().Serialize(ints, tempFile);
      List<IntWrapper> newInts = (List<IntWrapper>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(newInts[0].Value, 1);
      Assert.AreEqual(newInts[1].Value, 1);
      Assert.AreEqual(newInts[0], newInts[1]);
      Assert.AreNotSame(newInts[0], newInts[1]);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestSavingException() {
      List<int> list = new List<int> { 1, 2, 3 };
      new ProtoBufSerializer().Serialize(list, tempFile);
      NonSerializable s = new NonSerializable();
      try {
        new ProtoBufSerializer().Serialize(s, tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) { }
      List<int> newList = (List<int>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(list[0], newList[0]);
      Assert.AreEqual(list[1], newList[1]);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestTypeStringConversion() {
      string name = typeof(List<int>[]).AssemblyQualifiedName;
      string shortName =
        "System.Collections.Generic.List`1[[System.Int32, mscorlib]][], mscorlib";
      Assert.AreEqual(name, TypeNameParser.Parse(name).ToString());
      Assert.AreEqual(shortName, TypeNameParser.Parse(name).ToString(false));
      Assert.AreEqual(shortName, typeof(List<int>[]).VersionInvariantName());
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestHexadecimalPublicKeyToken() {
      string name = "TestClass, TestAssembly, Version=1.2.3.4, PublicKey=1234abc";
      string shortName = "TestClass, TestAssembly";
      Assert.AreEqual(name, TypeNameParser.Parse(name).ToString());
      Assert.AreEqual(shortName, TypeNameParser.Parse(name).ToString(false));
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestMultipleFailure() {
      List<NonSerializable> l = new List<NonSerializable>();
      l.Add(new NonSerializable());
      l.Add(new NonSerializable());
      l.Add(new NonSerializable());
      try {
        var s = new ProtoBufSerializer();
        s.Serialize(l);
        Assert.Fail("Exception expected");
      } catch (PersistenceException px) {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void InheritanceTest() {
      New n = new New();
      new ProtoBufSerializer().Serialize(n, tempFile);
      New nn = (New)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(n.Name, nn.Name);
      Assert.AreEqual(((Override)n).Name, ((Override)nn).Name);
    }

    [StorableType("78636BDB-03B9-4BA1-979D-358997AA8063")]
    class Child {
      [Storable]
      public GrandParent grandParent;

      [StorableConstructor]
      protected Child(StorableConstructorFlag _) {
      }
      public Child() {
      }
    }

    [StorableType("B90F2371-DE30-48ED-BDAA-671B175C5698")]
    class Parent {
      [Storable]
      public Child child;

      [StorableConstructor]
      protected Parent(StorableConstructorFlag _) {
      }
      public Parent() {
      }
    }

    [StorableType("C48C28A9-F197-4B75-A21D-F21EF6AC0602")]
    class GrandParent {
      [Storable]
      public Parent parent;

      [StorableConstructor]
      protected GrandParent(StorableConstructorFlag _) {
      }
      public GrandParent() {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void InstantiateParentChainReference() {
      GrandParent gp = new GrandParent();
      gp.parent = new Parent();
      gp.parent.child = new Child();
      gp.parent.child.grandParent = gp;
      Assert.AreSame(gp, gp.parent.child.grandParent);
      new ProtoBufSerializer().Serialize(gp, tempFile);
      GrandParent newGp = (GrandParent)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreSame(newGp, newGp.parent.child.grandParent);
    }

    [StorableType("FB4F08BB-6B65-4FBE-BA72-531DB2194F1F")]
    struct TestStruct {
      int value;
      int PropertyValue { get; set; }
      public TestStruct(int value)
        : this() {
        this.value = value;
        PropertyValue = value;
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void StructTest() {
      TestStruct s = new TestStruct(10);
      new ProtoBufSerializer().Serialize(s, tempFile);
      TestStruct newS = (TestStruct)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(s, newS);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void PointTest() {
      Point p = new Point(12, 34);
      new ProtoBufSerializer().Serialize(p, tempFile);
      Point newP = (Point)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(p, newP);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void NullableValueTypes() {
      double?[] d = new double?[] { null, 1, 2, 3 };
      new ProtoBufSerializer().Serialize(d, tempFile);
      double?[] newD = (double?[])new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(d[0], newD[0]);
      Assert.AreEqual(d[1], newD[1]);
      Assert.AreEqual(d[2], newD[2]);
      Assert.AreEqual(d[3], newD[3]);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void BitmapTest() {
      Icon icon = System.Drawing.SystemIcons.Hand;
      Bitmap bitmap = icon.ToBitmap();
      new ProtoBufSerializer().Serialize(bitmap, tempFile);
      Bitmap newBitmap = (Bitmap)new ProtoBufSerializer().Deserialize(tempFile);

      Assert.AreEqual(bitmap.Size, newBitmap.Size);
      for (int i = 0; i < bitmap.Size.Width; i++)
        for (int j = 0; j < bitmap.Size.Height; j++)
          Assert.AreEqual(bitmap.GetPixel(i, j), newBitmap.GetPixel(i, j));
    }

    [StorableType("5924A5A2-24C7-4588-951E-61212B041B0A")]
    private class PersistenceHooks {
      [Storable]
      public int a;
      [Storable]
      public int b;
      public int sum;
      public bool WasSerialized { get; private set; }
      [StorableHook(HookType.BeforeSerialization)]
      void PreSerializationHook() {
        WasSerialized = true;
      }
      [StorableHook(HookType.AfterDeserialization)]
      void PostDeserializationHook() {
        sum = a + b;
      }

      [StorableConstructor]
      protected PersistenceHooks(StorableConstructorFlag _) {
      }
      public PersistenceHooks() {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void HookTest() {
      PersistenceHooks hookTest = new PersistenceHooks();
      hookTest.a = 2;
      hookTest.b = 5;
      Assert.IsFalse(hookTest.WasSerialized);
      Assert.AreEqual(hookTest.sum, 0);
      new ProtoBufSerializer().Serialize(hookTest, tempFile);
      Assert.IsTrue(hookTest.WasSerialized);
      Assert.AreEqual(hookTest.sum, 0);
      PersistenceHooks newHookTest = (PersistenceHooks)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(newHookTest.a, hookTest.a);
      Assert.AreEqual(newHookTest.b, hookTest.b);
      Assert.AreEqual(newHookTest.sum, newHookTest.a + newHookTest.b);
      Assert.IsFalse(newHookTest.WasSerialized);
    }

    [StorableType("35824217-F1BC-450F-BB40-9B0A4F7C7582")]
    private class CustomConstructor {
      public string Value = "none";
      public CustomConstructor() {
        Value = "default";
      }
      [StorableConstructor]
      private CustomConstructor(StorableConstructorFlag _) {
        Value = "persistence";
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestCustomConstructor() {
      CustomConstructor cc = new CustomConstructor();
      Assert.AreEqual(cc.Value, "default");
      new ProtoBufSerializer().Serialize(cc, tempFile);
      CustomConstructor newCC = (CustomConstructor)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(newCC.Value, "persistence");
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestStreaming() {
      using (MemoryStream stream = new MemoryStream()) {
        Root r = InitializeComplexStorable();
        var ser = new ProtoBufSerializer();
        ser.Serialize(r, stream);
        using (MemoryStream stream2 = new MemoryStream(stream.ToArray())) {
          Root newR = (Root)ser.Deserialize(stream2);
          CompareComplexStorables(r, newR);
        }
      }
    }

    [StorableType("7CD5F148-397E-4539-88E0-EE19907E8BA6")]
    public class HookInheritanceTestBase {
      [Storable]
      public object a;
      public object link;
      [StorableHook(HookType.AfterDeserialization)]
      private void relink() {
        link = a;
      }

      [StorableConstructor]
      protected HookInheritanceTestBase(StorableConstructorFlag _) {
      }
      public HookInheritanceTestBase() {
      }
    }

    [StorableType("79E3EF89-A19A-408B-A18C-BFEB345159F0")]
    public class HookInheritanceTestDerivedClass : HookInheritanceTestBase {
      [Storable]
      public object b;
      [StorableHook(HookType.AfterDeserialization)]
      private void relink() {
        Assert.AreSame(a, link);
        link = b;
      }

      [StorableConstructor]
      protected HookInheritanceTestDerivedClass(StorableConstructorFlag _) : base(_) {
      }
      public HookInheritanceTestDerivedClass() {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestLinkInheritance() {
      HookInheritanceTestDerivedClass c = new HookInheritanceTestDerivedClass();
      c.a = new object();
      new ProtoBufSerializer().Serialize(c, tempFile);
      HookInheritanceTestDerivedClass newC = (HookInheritanceTestDerivedClass)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreSame(c.b, c.link);
    }

    [StorableType(StorableMemberSelection.AllFields, "B32F5C7A-F1C5-4B96-8A61-01E0DB1C526B")]
    public class AllFieldsStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected AllFieldsStorable(StorableConstructorFlag _) { }
      public AllFieldsStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestStorableClassDiscoveryAllFields() {
      AllFieldsStorable afs = new AllFieldsStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      AllFieldsStorable newAfs = (AllFieldsStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(afs.Value1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(0, newAfs.Value3);
      Assert.AreEqual(0, newAfs.Value4);
    }

    [StorableType(StorableMemberSelection.AllProperties, "60EE99CA-B391-4211-9FFB-2677490B33B6")]
    public class AllPropertiesStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected AllPropertiesStorable(StorableConstructorFlag _) { }
      public AllPropertiesStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestStorableClassDiscoveryAllProperties() {
      AllPropertiesStorable afs = new AllPropertiesStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      AllPropertiesStorable newAfs = (AllPropertiesStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(1, newAfs.Value1);
      Assert.AreEqual(2, newAfs.Value2);
      Assert.AreEqual(afs.Value3, newAfs.Value3);
      Assert.AreEqual(afs.Value4, newAfs.Value4);

    }

    [StorableType(StorableMemberSelection.AllFieldsAndAllProperties, "97FAFC16-EC58-44CC-A833-CB951C0DD23B")]
    public class AllFieldsAndAllPropertiesStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected AllFieldsAndAllPropertiesStorable(StorableConstructorFlag _) { }
      public AllFieldsAndAllPropertiesStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestStorableClassDiscoveryAllFieldsAndAllProperties() {
      AllFieldsAndAllPropertiesStorable afs = new AllFieldsAndAllPropertiesStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      AllFieldsAndAllPropertiesStorable newAfs = (AllFieldsAndAllPropertiesStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(afs.Value1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(afs.Value3, newAfs.Value3);
      Assert.AreEqual(afs.Value4, newAfs.Value4);
    }

    [StorableType("74BDE240-59D5-48C9-9A2A-5D44750DAF78")]
    public class MarkedOnlyStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected MarkedOnlyStorable(StorableConstructorFlag _) { }
      public MarkedOnlyStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestStorableClassDiscoveryMarkedOnly() {
      MarkedOnlyStorable afs = new MarkedOnlyStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      MarkedOnlyStorable newAfs = (MarkedOnlyStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(0, newAfs.Value3);
      Assert.AreEqual(0, newAfs.Value4);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestLineEndings() {
      List<string> lineBreaks = new List<string> { "\r\n", "\n", "\r", "\n\r", Environment.NewLine };
      List<string> lines = new List<string>();
      foreach (var br in lineBreaks)
        lines.Add("line1" + br + "line2");
      new ProtoBufSerializer().Serialize(lines, tempFile);
      List<string> newLines = (List<string>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(lines.Count, newLines.Count);
      for (int i = 0; i < lineBreaks.Count; i++) {
        Assert.AreEqual(lines[i], newLines[i]);
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestSpecialNumbers() {
      List<double> specials = new List<double>() { 1.0 / 0, -1.0 / 0, 0.0 / 0 };
      Assert.IsTrue(double.IsPositiveInfinity(specials[0]));
      Assert.IsTrue(double.IsNegativeInfinity(specials[1]));
      Assert.IsTrue(double.IsNaN(specials[2]));
      new ProtoBufSerializer().Serialize(specials, tempFile);
      List<double> newSpecials = (List<double>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.IsTrue(double.IsPositiveInfinity(newSpecials[0]));
      Assert.IsTrue(double.IsNegativeInfinity(newSpecials[1]));
      Assert.IsTrue(double.IsNaN(newSpecials[2]));
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestStringSplit() {
      string s = "1.2;2.3;3.4;;;4.9";
      var l = s.EnumerateSplit(';').ToList();
      Assert.AreEqual("1.2", l[0]);
      Assert.AreEqual("2.3", l[1]);
      Assert.AreEqual("3.4", l[2]);
      Assert.AreEqual("4.9", l[3]);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "medium")]
    public void TestCompactNumberArraySerializer() {
      System.Random r = new System.Random();
      double[] a = new double[CompactNumberArray2StringSerializer.SPLIT_THRESHOLD * 2 + 1];
      for (int i = 0; i < a.Length; i++)
        a[i] = r.Next(10);
      new ProtoBufSerializer().Serialize(a, tempFile);
      double[] newA = (double[])new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(a.Length, newA.Length);
      for (int i = 0; i < a.Rank; i++) {
        Assert.AreEqual(a.GetLength(i), newA.GetLength(i));
        Assert.AreEqual(a.GetLowerBound(i), newA.GetLowerBound(i));
      }
      for (int i = 0; i < a.Length; i++) {
        Assert.AreEqual(a[i], newA[i]);
      }
    }
    [StorableType("A174C85C-3B7C-477D-9E6C-121301470DDE")]
    private class IdentityComparer<T> : IEqualityComparer<T> {

      public bool Equals(T x, T y) {
        return x.Equals(y);
      }

      public int GetHashCode(T obj) {
        return obj.GetHashCode();
      }

      [StorableConstructor]
      protected IdentityComparer(StorableConstructorFlag _) {
      }
      public IdentityComparer() {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestHashSetSerializer() {
      var hashSets = new List<HashSet<int>>() {
        new HashSet<int>(new[] { 1, 2, 3 }),
        new HashSet<int>(new[] { 4, 5, 6 }, new IdentityComparer<int>()),
      };
      new ProtoBufSerializer().Serialize(hashSets, tempFile);
      var newHashSets = (List<HashSet<int>>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.IsTrue(newHashSets[0].Contains(1));
      Assert.IsTrue(newHashSets[0].Contains(2));
      Assert.IsTrue(newHashSets[0].Contains(3));
      Assert.IsTrue(newHashSets[1].Contains(4));
      Assert.IsTrue(newHashSets[1].Contains(5));
      Assert.IsTrue(newHashSets[1].Contains(6));
      Assert.AreEqual(newHashSets[0].Comparer.GetType(), new HashSet<int>().Comparer.GetType());
      Assert.AreEqual(newHashSets[1].Comparer.GetType(), typeof(IdentityComparer<int>));
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestConcreteDictionarySerializer() {
      var dictionaries = new List<Dictionary<int, int>>() {
        new Dictionary<int, int>(),
        new Dictionary<int, int>(new IdentityComparer<int>()),
      };
      dictionaries[0].Add(1, 1);
      dictionaries[0].Add(2, 2);
      dictionaries[0].Add(3, 3);
      dictionaries[1].Add(4, 4);
      dictionaries[1].Add(5, 5);
      dictionaries[1].Add(6, 6);
      new ProtoBufSerializer().Serialize(dictionaries, tempFile);
      var newDictionaries = (List<Dictionary<int, int>>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.IsTrue(newDictionaries[0].ContainsKey(1));
      Assert.IsTrue(newDictionaries[0].ContainsKey(2));
      Assert.IsTrue(newDictionaries[0].ContainsKey(3));
      Assert.IsTrue(newDictionaries[1].ContainsKey(4));
      Assert.IsTrue(newDictionaries[1].ContainsKey(5));
      Assert.IsTrue(newDictionaries[1].ContainsKey(6));
      Assert.IsTrue(newDictionaries[0].ContainsValue(1));
      Assert.IsTrue(newDictionaries[0].ContainsValue(2));
      Assert.IsTrue(newDictionaries[0].ContainsValue(3));
      Assert.IsTrue(newDictionaries[1].ContainsValue(4));
      Assert.IsTrue(newDictionaries[1].ContainsValue(5));
      Assert.IsTrue(newDictionaries[1].ContainsValue(6));
      Assert.AreEqual(new Dictionary<int, int>().Comparer.GetType(), newDictionaries[0].Comparer.GetType());
      Assert.AreEqual(typeof(IdentityComparer<int>), newDictionaries[1].Comparer.GetType());
    }

    [StorableType("A5DAC970-4E03-4B69-A95A-9DAC683D051F")]
    public class ReadOnlyFail {
      [Storable]
      public string ReadOnly {
        get { return "fail"; }
      }

      [StorableConstructor]
      protected ReadOnlyFail(StorableConstructorFlag _) {
      }
      public ReadOnlyFail() {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestReadOnlyFail() {
      try {
        new ProtoBufSerializer().Serialize(new ReadOnlyFail(), tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) {
      } catch {
        Assert.Fail("PersistenceException expected");
      }
    }


    [StorableType("653EBC18-E461-4F5C-8FD6-9F588AAC70D9")]
    public class WriteOnlyFail {
      [Storable]
      public string WriteOnly {
        set { throw new InvalidOperationException("this property should never be set."); }
      }

      [StorableConstructor]
      protected WriteOnlyFail(StorableConstructorFlag _) {
      }
      public WriteOnlyFail() {
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestWriteOnlyFail() {
      try {
        new ProtoBufSerializer().Serialize(new WriteOnlyFail(), tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) {
      } catch {
        Assert.Fail("PersistenceException expected.");
      }
    }

    [StorableType("67BEAF29-9D7C-4C82-BD9F-9957798D6A2D")]
    public class OneWayTest {
      [StorableConstructor]
      protected OneWayTest(StorableConstructorFlag _) {
      }

      public OneWayTest() { this.value = "default"; }
      public string value;
      [Storable(AllowOneWay = true)]
      public string ReadOnly {
        get { return "ReadOnly"; }
      }
      [Storable(AllowOneWay = true)]
      public string WriteOnly {
        set { this.value = value; }
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TupleTest() {
      var t1 = Tuple.Create(1);
      var t2 = Tuple.Create('1', "2");
      var t3 = Tuple.Create(3.0, 3f, 5);
      var t4 = Tuple.Create(Tuple.Create(1, 2, 3), Tuple.Create(4, 5, 6), Tuple.Create(8, 9, 10));
      var tuple = Tuple.Create(t1, t2, t3, t4);
      new ProtoBufSerializer().Serialize(tuple, tempFile);
      var newTuple = (Tuple<Tuple<int>, Tuple<char, string>, Tuple<double, float, int>, Tuple<Tuple<int, int, int>, Tuple<int, int, int>, Tuple<int, int, int>>>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(tuple, newTuple);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void FontTest() {
      List<Font> fonts = new List<Font>() {
        new Font(FontFamily.GenericSansSerif, 12),
        new Font("Times New Roman", 21, FontStyle.Bold, GraphicsUnit.Pixel),
        new Font("Courier New", 10, FontStyle.Underline, GraphicsUnit.Document),
        new Font("Helvetica", 21, FontStyle.Strikeout, GraphicsUnit.Inch, 0, true),
      };
      new ProtoBufSerializer().Serialize(fonts, tempFile);
      var newFonts = (List<Font>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(fonts[0], newFonts[0]);
      Assert.AreEqual(fonts[1], newFonts[1]);
      Assert.AreEqual(fonts[2], newFonts[2]);
      Assert.AreEqual(fonts[3], newFonts[3]);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "medium")]
    public void ConcurrencyTest() {
      int n = 20;
      Task[] tasks = new Task[n];
      for (int i = 0; i < n; i++) {
        tasks[i] = Task.Factory.StartNew((idx) => {
          byte[] data;
          using (var stream = new MemoryStream()) {
            new ProtoBufSerializer().Serialize(new GeneticAlgorithm(), stream);
            data = stream.ToArray();
          }
        }, i);
      }
      Task.WaitAll(tasks);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "medium")]
    public void ConcurrentBitmapTest() {
      Bitmap b = new Bitmap(300, 300);
      System.Random r = new System.Random();
      for (int x = 0; x < b.Height; x++) {
        for (int y = 0; y < b.Width; y++) {
          b.SetPixel(x, y, Color.FromArgb(r.Next()));
        }
      }
      Task[] tasks = new Task[20];
      byte[][] datas = new byte[tasks.Length][];
      for (int i = 0; i < tasks.Length; i++) {
        tasks[i] = Task.Factory.StartNew((idx) => {
          using (var stream = new MemoryStream()) {
            new ProtoBufSerializer().Serialize(b, stream);
            datas[(int)idx] = stream.ToArray();
          }
        }, i);
      }
      Task.WaitAll(tasks);
    }

    [StorableType("6923FC3A-AC33-4CA9-919F-9707C00A663B")]
    public class G<T, T2> {
      [StorableType("16B88964-ECB3-4B41-95BC-EE3BE908CE4A")]
      public class S { }
      [StorableType("23CC1C7C-031E-4CBD-A87A-8F2235803BB4")]
      public class S2<T3, T4> { }
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestInternalClassOfGeneric() {
      var s = new G<int, char>.S();
      var typeName = s.GetType().AssemblyQualifiedName;
      Assert.AreEqual(
        "UseCases.G<Int32,Char>.S",
        TypeNameParser.Parse(typeName).GetTypeNameInCode(false));
      new ProtoBufSerializer().Serialize(s, tempFile);
      var s1 = new ProtoBufSerializer().Deserialize(tempFile);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestInternalClassOfGeneric2() {
      var s = new G<int, float>.S2<int, char>();
      var typeName = s.GetType().AssemblyQualifiedName;
      Assert.AreEqual(
        "UseCases.G<Int32,Single>.S2<Int32,Char>",
        TypeNameParser.Parse(typeName).GetTypeNameInCode(false));
      new ProtoBufSerializer().Serialize(s, tempFile);
      var s1 = new ProtoBufSerializer().Deserialize(tempFile);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestSpecialCharacters() {
      var s = "abc" + "\x15" + "def";
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(s, newS);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestByteArray() {
      var b = new byte[3];
      b[0] = 0;
      b[1] = 200;
      b[2] = byte.MaxValue;
      new ProtoBufSerializer().Serialize(b, tempFile);
      var newB = (byte[])new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(b, newB);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestOptionalNumberEnumerable() {
      var values = new List<double?> { 0, null, double.NaN, double.PositiveInfinity, double.MaxValue, 1 };
      new ProtoBufSerializer().Serialize(values, tempFile);
      var newValues = (List<double?>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(values, newValues);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestOptionalDateTimeEnumerable() {
      var values = new List<DateTime?> { DateTime.MinValue, null, DateTime.Now, DateTime.Now.Add(TimeSpan.FromDays(1)),
        DateTime.ParseExact("10.09.2014 12:21", "dd.MM.yyyy hh:mm", CultureInfo.InvariantCulture), DateTime.MaxValue};
      new ProtoBufSerializer().Serialize(values, tempFile);
      var newValues = (List<DateTime?>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(values, newValues);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestStringEnumerable() {
      var values = new List<string> { "", null, "s", "string", string.Empty, "123", "<![CDATA[nice]]>", "<![CDATA[nasty unterminated" };
      new ProtoBufSerializer().Serialize(values, tempFile);
      var newValues = (List<String>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(values, newValues);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestUnicodeCharArray() {
      var s = Encoding.UTF8.GetChars(new byte[] { 0, 1, 2, 03, 04, 05, 06, 07, 08, 09, 0xa, 0xb });
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = (char[])new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(s, newS);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestUnicode() {
      var s = Encoding.UTF8.GetString(new byte[] { 0, 1, 2, 03, 04, 05, 06, 07, 08, 09, 0xa, 0xb });
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(s, newS);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestQueue() {
      var q = new Queue<int>(new[] { 1, 2, 3, 4, 0 });
      new ProtoBufSerializer().Serialize(q, tempFile);
      var newQ = (Queue<int>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(q, newQ);
    }



    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
      ConfigurationService.Instance.Reset();
    }
  }
}
