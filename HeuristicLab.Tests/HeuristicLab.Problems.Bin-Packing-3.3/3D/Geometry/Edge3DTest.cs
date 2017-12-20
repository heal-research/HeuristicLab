using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D.Geometry;

namespace HeuristicLab.Problems.BinPacking._3D.Geometry.Test {
  [TestClass]
  public class Edge3DTest {

    Edge3D _edge1;
    Edge3D _edge2;
    Edge3D _edge3;
    Edge3D _edge4;
    Edge3D _edge5;
    Edge3D _edge6;
    Edge3D _edge7;
    Edge3D _edge8;
    Edge3D _edge9;

    Vector3D _point1;
    Vector3D _point2;
    Vector3D _point3;
    Vector3D _point4;
    Vector3D _point5;
    Vector3D _point6;

    [TestInitialize]
    public void Initialize() {
      _edge1 = new Edge3D(new Vector3D(0, 0, 0), new Vector3D(10, 0, 0));
      _edge2 = new Edge3D(new Vector3D(0, 0, 0), new Vector3D(0, 10, 0));
      _edge3 = new Edge3D(new Vector3D(0, 0, 0), new Vector3D(0, 0, 10));

      _edge4 = new Edge3D(new Vector3D(-10, 2, 2), new Vector3D(10, 2, 2));
      _edge5 = new Edge3D(new Vector3D(2, -10, 2), new Vector3D(2, 10, 2));
      _edge6 = new Edge3D(new Vector3D(2, 2, -10), new Vector3D(2, 2, 10));

      _edge7 = new Edge3D(new Vector3D(-10, 5, 5), new Vector3D(10, 5, 5));
      _edge8 = new Edge3D(new Vector3D(5, -10, 5), new Vector3D(5, 10, 5));
      _edge9 = new Edge3D(new Vector3D(5, 5, -10), new Vector3D(5, 5, 10));

      _point1 = new Vector3D(3, 0, 0);
      _point2 = new Vector3D(0, 3, 0);
      _point3 = new Vector3D(0, 0, 3);

      _point4 = new Vector3D(5, 5, 5);
      _point5 = new Vector3D(2, 2, 2);
      _point6 = new Vector3D(0, 0, 0);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestConstructors() {
      var e1 = new Edge3D() {
        Start = new Vector3D(10, 0, 0),
        End = new Vector3D(10, 5, 0)
      };
      var e2 = new Edge3D(new Vector3D(10, 0, 0), new Vector3D(10, 5, 0));
      var e3 = new Edge3D(new BinPacking3D.PackingPosition(0, 10, 0, 0), new BinPacking3D.PackingPosition(0, 10, 5, 0));

      Assert.AreEqual(e1.Start, e2.Start);
      Assert.AreEqual(e1.End, e2.End);
      Assert.AreEqual(e1.Start, e3.Start);
      Assert.AreEqual(e1.End, e3.End);
      Assert.AreEqual(e3.Start, e2.Start);
      Assert.AreEqual(e3.End, e2.End);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestLiesOn() {
      Assert.IsTrue(_edge1.LiesOn(_point1));
      Assert.IsFalse(_edge1.LiesOn(_point4));
      Assert.IsTrue(_edge3.LiesOn(_point6));
      Assert.IsFalse(_edge2.LiesOn(_point5));
      Assert.IsTrue(_edge4.LiesOn(_point5));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestIntersects() {
      Assert.IsNull(_edge1.Intersects(_edge4));
      Assert.IsNull(_edge4.Intersects(_edge1));
      Assert.IsNull(_edge7.Intersects(_edge5));
      Assert.IsNull(_edge8.Intersects(_edge6));

      Assert.IsNotNull(_edge1.Intersects(_edge2));
      Assert.AreEqual(_point6, _edge1.Intersects(_edge2));
      Assert.IsNotNull(_edge5.Intersects(_edge5));
      Assert.AreEqual(_point5, _edge5.Intersects(_edge6));
      Assert.IsNotNull(_edge8.Intersects(_edge7));
      Assert.AreEqual(_point4, _edge8.Intersects(_edge7));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestStaticIntersects() {
      Assert.IsNull(Edge3D.Intersects(_edge1, _edge4));
      Assert.IsNull(Edge3D.Intersects(_edge4, _edge1));
      Assert.IsNull(Edge3D.Intersects(_edge7, _edge5));
      Assert.IsNull(Edge3D.Intersects(_edge8, _edge6));
      
      Assert.IsNotNull(Edge3D.Intersects(new Edge3D(new Vector3D(0, 0, 0), new Vector3D(10, 0, 0)), new Edge3D(new Vector3D(0, 0, 0), new Vector3D(0, 10, 0))));
      Assert.IsNotNull(Edge3D.Intersects(new Edge3D(new Vector3D(-10, 0, 0), new Vector3D(0, 0, 0)), new Edge3D(new Vector3D(0, -10, 0), new Vector3D(0, 0, 0))));

      Assert.IsNotNull(Edge3D.Intersects(_edge1, _edge2));
      Assert.AreEqual(_point6, Edge3D.Intersects(_edge1, _edge2));
      Assert.IsNotNull(Edge3D.Intersects(_edge5, _edge5));
      Assert.AreEqual(_point5, Edge3D.Intersects(_edge5, _edge6));
      Assert.IsNotNull(Edge3D.Intersects(_edge8, _edge7));
      Assert.AreEqual(_point4, Edge3D.Intersects(_edge8, _edge7));
    }    
  }
}

