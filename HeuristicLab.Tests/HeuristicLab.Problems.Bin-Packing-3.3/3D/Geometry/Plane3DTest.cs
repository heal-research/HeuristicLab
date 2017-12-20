using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking._3D.Geometry.Test {
  [TestClass]
  public class Plane3DTest {

    Line3D _line1;
    Line3D _line2;
    Line3D _line3;
    Line3D _line4;
    Line3D _line5;
    Line3D _line6;

    Vector3D _point1;
    Vector3D _point2;
    Vector3D _point3;
    Vector3D _point4;
    Vector3D _point5;
    Vector3D _point6;

    Plane3D _plane1;
    Plane3D _plane2;
    Plane3D _plane3;

    [TestInitialize]
    public void Initialize() {
      _line1 = new Line3D(new Vector3D(0, 0, 0), new Vector3D(10, 0, 0));
      _line2 = new Line3D(new Vector3D(0, 0, 0), new Vector3D(0, 10, 0));
      _line3 = new Line3D(new Vector3D(0, 0, 0), new Vector3D(0, 0, 10));

      _line4 = new Line3D(new Vector3D(-10, 2, 2), new Vector3D(10, 0, 0));
      _line5 = new Line3D(new Vector3D(2, -10, 2), new Vector3D(0, 10, 0));
      _line6 = new Line3D(new Vector3D(2, 2, -10), new Vector3D(0, 0, 10));

      _point1 = new Vector3D(3, 0, 0);
      _point2 = new Vector3D(0, 3, 0);
      _point3 = new Vector3D(0, 0, 3);

      _point4 = new Vector3D(5, 5, 5);
      _point5 = new Vector3D(2, 2, 2);
      _point6 = new Vector3D(0, 0, 0);

      _plane1 = new Plane3D(_point6.ToPackingPosition(0), new PackingItem(10, 10, 10, new PackingShape()), Side.Top);
      _plane2 = new Plane3D(_point6.ToPackingPosition(0), new PackingItem(10, 10, 10, new PackingShape()), Side.Right);
      _plane3 = new Plane3D(_point6.ToPackingPosition(0), new PackingItem(10, 10, 10, new PackingShape()), Side.Front);
    }


    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestIsElementOf() {
      Assert.IsTrue(_plane1.IsElementOf(new Vector3D(0, 10, 0)));
      Assert.IsTrue(_plane1.IsElementOf(new Vector3D(5, 10, 0)));
      Assert.IsTrue(_plane1.IsElementOf(new Vector3D(0, 10, 5)));
      Assert.IsTrue(_plane1.IsElementOf(new Vector3D(5, 10, 5)));
      Assert.IsFalse(_plane1.IsElementOf(new Vector3D(0, 9, 0)));
      Assert.IsFalse(_plane1.IsElementOf(new Vector3D(0, 11, 0)));
    }


    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestIntersects() {
      Assert.IsFalse(_plane1.Intersects(_line1));
      Assert.IsTrue(_plane1.Intersects(_line2));
      Assert.IsFalse(_plane1.Intersects(_line3));

      Assert.IsTrue(_plane2.Intersects(_line1));
      Assert.IsFalse(_plane2.Intersects(_line2));
      Assert.IsFalse(_plane2.Intersects(_line3));

      Assert.IsFalse(_plane3.Intersects(_line1));
      Assert.IsFalse(_plane3.Intersects(_line2));
      Assert.IsTrue(_plane3.Intersects(_line3));

      Assert.IsTrue(_plane1.Intersects(new Line3D(new Vector3D(-1, -2, 2), new Vector3D(1, 3, 1))));
      Assert.IsFalse(_plane1.Intersects(new Line3D(new Vector3D(-1, -2, 2), new Vector3D(-1, -3, 1))));
    }


    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestIntersect() {
      Assert.IsNull(_plane1.Intersect(_line1));
      Assert.AreEqual(new Vector3D(0, 10, 0), _plane1.Intersect(_line2));
      Assert.IsNull(_plane1.Intersect(_line3));

      Assert.AreEqual(new Vector3D(10, 0, 0), _plane2.Intersect(_line1));
      Assert.IsNull(_plane2.Intersect(_line2));
      Assert.IsNull(_plane2.Intersect(_line3));

      Assert.IsNull(_plane3.Intersect(_line1));
      Assert.IsNull(_plane3.Intersect(_line2));
      Assert.AreEqual(new Vector3D(0, 0, 10), _plane3.Intersect(_line3));
      Assert.AreEqual(new Vector3D(3, 10, 6), _plane1.Intersect(new Line3D(new Vector3D(-1, -2, 2), new Vector3D(1, 3, 1))));
      Assert.IsNull(_plane1.Intersect(new Line3D(new Vector3D(-1, -2, 2), new Vector3D(-1, -3, 1))));
    }
  }
}
