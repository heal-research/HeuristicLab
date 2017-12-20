using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking._3D.Geometry.Test {
  [TestClass]
  public class Vector3DTest {

    Vector3D _point1;
    Vector3D _point2;
    Vector3D _point3;
    Vector3D _point4;
    Vector3D _point5;
    Vector3D _point6;

    PackingItem _item1;
    PackingItem _item2;
    PackingItem _item3;

    [TestInitialize]
    public void Initialize() {
      _point1 = new Vector3D(3, 0, 0);
      _point2 = new Vector3D(0, 3, 0);
      _point3 = new Vector3D(0, 0, 3);

      _point4 = new Vector3D(5, 5, 5);
      _point5 = new Vector3D(2, 2, 2);
      _point6 = new Vector3D(0, 0, 0);

      _item1 = new PackingItem(10, 10, 10, new PackingShape());
      _item2 = new PackingItem(20, 20, 20, new PackingShape());
      _item3 = new PackingItem(30, 30, 30, new PackingShape());
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestAlongX() {
      Assert.AreEqual(new Vector3D(13, 0, 0) ,Vector3D.AlongX(_point1.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(23, 0, 0), Vector3D.AlongX(_point1.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(33, 0, 0), Vector3D.AlongX(_point1.ToPackingPosition(0), _item3));


      Assert.AreEqual(new Vector3D(10, 3, 0), Vector3D.AlongX(_point2.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(20, 3, 0), Vector3D.AlongX(_point2.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(30, 3, 0), Vector3D.AlongX(_point2.ToPackingPosition(0), _item3));

      Assert.AreEqual(new Vector3D(10, 0, 3), Vector3D.AlongX(_point3.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(20, 0, 3), Vector3D.AlongX(_point3.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(30, 0, 3), Vector3D.AlongX(_point3.ToPackingPosition(0), _item3));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestAlongY() {
      Assert.AreEqual(new Vector3D(3, 10, 0), Vector3D.AlongY(_point1.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(3, 20, 0), Vector3D.AlongY(_point1.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(3, 30, 0), Vector3D.AlongY(_point1.ToPackingPosition(0), _item3));


      Assert.AreEqual(new Vector3D(0, 13, 0), Vector3D.AlongY(_point2.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(0, 23, 0), Vector3D.AlongY(_point2.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(0, 33, 0), Vector3D.AlongY(_point2.ToPackingPosition(0), _item3));

      Assert.AreEqual(new Vector3D(0, 10, 3), Vector3D.AlongY(_point3.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(0, 20, 3), Vector3D.AlongY(_point3.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(0, 30, 3), Vector3D.AlongY(_point3.ToPackingPosition(0), _item3));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestAlongZ() {
      Assert.AreEqual(new Vector3D(3, 0, 10), Vector3D.AlongZ(_point1.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(3, 0, 20), Vector3D.AlongZ(_point1.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(3, 0, 30), Vector3D.AlongZ(_point1.ToPackingPosition(0), _item3));


      Assert.AreEqual(new Vector3D(0, 3, 10), Vector3D.AlongZ(_point2.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(0, 3, 20), Vector3D.AlongZ(_point2.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(0, 3, 30), Vector3D.AlongZ(_point2.ToPackingPosition(0), _item3));

      Assert.AreEqual(new Vector3D(0, 0, 13), Vector3D.AlongZ(_point3.ToPackingPosition(0), _item1));
      Assert.AreEqual(new Vector3D(0, 0, 23), Vector3D.AlongZ(_point3.ToPackingPosition(0), _item2));
      Assert.AreEqual(new Vector3D(0, 0, 33), Vector3D.AlongZ(_point3.ToPackingPosition(0), _item3));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestCross() {
      Assert.AreEqual(new Vector3D(0, 0, 9), _point1.Cross(_point2));
      Assert.AreEqual(new Vector3D(0, 0, -9), _point2.Cross(_point1));
      Assert.AreEqual(new Vector3D(9, 0, 0), _point2.Cross(_point3));
      Assert.AreEqual(new Vector3D(-9, 0, 0), _point3.Cross(_point2));
      Assert.AreEqual(new Vector3D(0, -9, 0), _point1.Cross(_point3));
      Assert.AreEqual(new Vector3D(0, 9, 0), _point3.Cross(_point1));


      Assert.AreEqual(new Vector3D(0, -15, 15), _point1.Cross(_point4));
      Assert.AreEqual(new Vector3D(15, 0, -15), _point2.Cross(_point4));
      Assert.AreEqual(new Vector3D(-15, 15, 0), _point3.Cross(_point4));

      Assert.AreEqual(new Vector3D(0, 15, -15), _point4.Cross(_point1));
      Assert.AreEqual(new Vector3D(-15, 0, 15), _point4.Cross(_point2));
      Assert.AreEqual(new Vector3D(15, -15, 0), _point4.Cross(_point3));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestIsInside() {
      Assert.IsFalse(_point1.IsInside(_point4.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsFalse(_point2.IsInside(_point4.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsFalse(_point3.IsInside(_point4.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));

      Assert.IsTrue(_point1.IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsTrue(_point2.IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsTrue(_point3.IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsTrue(_point6.IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));

      Assert.IsTrue(_point4.IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsTrue(_point5.IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsTrue((new Vector3D(9, 9, 9)).IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsFalse((new Vector3D(11, 9, 9)).IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsFalse((new Vector3D(9, 11, 9)).IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
      Assert.IsFalse((new Vector3D(9, 9, 11)).IsInside(_point6.ToPackingPosition(0), ResidualSpace.Create(10, 10, 10)));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestOperators() {
      // static int operator *(Vector3D a, Vector3D b) 
      Assert.AreEqual(0, (_point1 * _point2));
      Assert.AreEqual(0, (_point2 * _point1));
      Assert.AreEqual(0, (_point4 * _point6));
      Assert.AreEqual(0, (_point6 * _point4));
      Assert.AreEqual(15, (_point1 * _point4));
      Assert.AreEqual(15, (_point4 * _point1));
      Assert.AreEqual(30, (_point5 * _point4));
      Assert.AreEqual(30, (_point4 * _point5));

      // static Vector3D operator *(int a, Vector3D b), static Vector3D operator *(Vector3D a, int b)
      Assert.AreEqual(new Vector3D(3, 0, 0), 1 * _point1);
      Assert.AreEqual(new Vector3D(6, 0, 0), 2 * _point1);
      Assert.AreEqual(new Vector3D(3, 0, 0), _point1 * 1);
      Assert.AreEqual(new Vector3D(6, 0, 0), _point1 * 2);
      Assert.AreEqual(2 * _point1, _point1 * 2);

      Assert.AreEqual(new Vector3D(9, 0, 0), _point1 * 3);
      Assert.AreEqual(new Vector3D(0, 9, 0), _point2 * 3);
      Assert.AreEqual(new Vector3D(0, 0, 9), _point3 * 3);

      Assert.AreEqual(new Vector3D(20, 20, 20), 4 * _point4);
      Assert.AreEqual(new Vector3D(20, 20, 20), _point4 * 4);      
      Assert.AreEqual(4 * _point4, _point4 * 4);

      // static Vector3D operator *(double a, Vector3D b), static Vector3D operator *(Vector3D a, double b)
      Assert.AreEqual(new Vector3D(3, 0, 0), 1.1 * _point1);
      Assert.AreEqual(new Vector3D(6, 0, 0), 2.2 * _point1);
      Assert.AreEqual(new Vector3D(3, 0, 0), _point1 * 1.1);
      Assert.AreEqual(new Vector3D(6, 0, 0), _point1 * 2.2);
      Assert.AreEqual(2.2 * _point1, _point1 * 2.2);
      
      Assert.AreEqual(new Vector3D(9, 0, 0), _point1 * 3.3);
      Assert.AreEqual(new Vector3D(0, 9, 0), _point2 * 3.3);
      Assert.AreEqual(new Vector3D(0, 0, 9), _point3 * 3.3);

      Assert.AreEqual(new Vector3D(22, 22, 22), 4.4 * _point4);
      Assert.AreEqual(new Vector3D(22, 22, 22), _point4 * 4.4);
      Assert.AreEqual(4.4 * _point4, _point4 * 4.4);

      // static Vector3D operator +(Vector3D a, Vector3D b)
      Assert.AreEqual(new Vector3D(8, 5, 5), _point1 + _point4);
      Assert.AreEqual(new Vector3D(5, 8, 5), _point2 + _point4);
      Assert.AreEqual(new Vector3D(5, 5, 8), _point3 + _point4);
      Assert.AreEqual(new Vector3D(7, 7, 7), _point4 + _point5);
      Assert.AreEqual(_point4, _point4 + _point6);

      // static Vector3D operator -(Vector3D a, Vector3D b)
      Assert.AreEqual(new Vector3D(-2, -5, -5), _point1 - _point4);
      Assert.AreEqual(new Vector3D(-5, -2, -5), _point2 - _point4);
      Assert.AreEqual(new Vector3D(-5, -5, -2), _point3 - _point4);
      Assert.AreEqual(new Vector3D(3, 3, 3), _point4 - _point5);
      Assert.AreEqual(_point4, _point4 - _point6);

    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestEquals() {
      Assert.IsFalse(_point1.Equals(_point2));
      Assert.IsFalse(_point1.Equals(new Vector3D(3, 0, 1)));
      Assert.IsFalse(_point1.Equals(new Vector3D(3, 1, 0)));
      Assert.IsFalse(_point1.Equals(new Vector3D(2, 0, 0)));

      Assert.IsTrue(_point1.Equals(_point1));
      Assert.IsTrue(_point5.Equals(new Vector3D(2, 2, 2)));
    }
    
  }
}
