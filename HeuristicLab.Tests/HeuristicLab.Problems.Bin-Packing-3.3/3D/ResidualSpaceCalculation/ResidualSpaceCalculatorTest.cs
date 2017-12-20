using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using HeuristicLab.Problems.BinPacking3D.ResidualSpaceCalculation;
using HeuristicLab.Problems.BinPacking._3D.Utils.Tests;
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking._3D.ResidualSpaceCalculation.Tests {
  /// <summary>
  /// Summary description for ResidualSpaceCalculator
  /// </summary>
  [TestClass]
  public class ResidualSpaceCalculatorTest {

    #region Points, residual spaces, packing positions and items

    Vector3D _point1;
    Vector3D _point2;
    Vector3D _point3;
    Vector3D _point4;
    Vector3D _point5;
    Vector3D _point6;
    Vector3D _point7;
    Vector3D _point8;
    Vector3D _point9;
    Vector3D _point10;
    Vector3D _point11;

    ResidualSpace _rs1;
    ResidualSpace _rs2;
    ResidualSpace _rs3;
    ResidualSpace _rs4;
    ResidualSpace _rs5;
    ResidualSpace _rs6;
    ResidualSpace _rs7;

    BinPacking3D.PackingPosition _pos1;
    BinPacking3D.PackingPosition _pos2;
    BinPacking3D.PackingPosition _pos3;
    BinPacking3D.PackingPosition _pos4;
    BinPacking3D.PackingPosition _pos5;
    BinPacking3D.PackingPosition _pos6;
    BinPacking3D.PackingPosition _pos7;
    BinPacking3D.PackingPosition _pos8;
    BinPacking3D.PackingPosition _pos9;
    BinPacking3D.PackingPosition _pos10;
    BinPacking3D.PackingPosition _pos11;

    BinPacking3D.PackingItem _item1;
    BinPacking3D.PackingItem _item2;
    BinPacking3D.PackingItem _item3;
    BinPacking3D.PackingItem _item4;
    BinPacking3D.PackingItem _item5;
    BinPacking3D.PackingItem _item6;
    BinPacking3D.PackingItem _item7;

    BinPacking3D.BinPacking3D _bp1;
    BinPacking3D.BinPacking3D _bp2;
    BinPacking3D.BinPacking3D _bp3;
    #endregion

    IResidualSpaceCalculator _calculator;

    [TestInitialize]
    public void Initialize() {
      _calculator = ResidualSpaceCalculatorFactory.CreateCalculator();
      _point1 = new Vector3D(0, 0, 0);
      _point2 = new Vector3D(0, 10, 10);
      _point3 = new Vector3D(10, 0, 10);
      _point4 = new Vector3D(10, 10, 0);

      _point5 = new Vector3D(10, 20, 20);
      _point6 = new Vector3D(20, 10, 20);
      _point7 = new Vector3D(20, 20, 10);

      _point8 = new Vector3D(0, 20, 20);
      _point9 = new Vector3D(20, 0, 20);
      _point10 = new Vector3D(20, 20, 0);
      _point11 = new Vector3D(20, 20, 20);

      _rs1 = new ResidualSpace(5, 5, 5);
      _rs2 = new ResidualSpace(10, 10, 10);
      _rs3 = new ResidualSpace(20, 20, 20);
      _rs4 = new ResidualSpace(30, 30, 30);
      _rs5 = new ResidualSpace(10, 20, 20);
      _rs6 = new ResidualSpace(20, 10, 20);
      _rs7 = new ResidualSpace(20, 20, 10);

      _pos1 = _point1.ToPackingPosition(0);
      _pos2 = _point2.ToPackingPosition(0);
      _pos3 = _point3.ToPackingPosition(0);
      _pos4 = _point4.ToPackingPosition(0);
      _pos5 = _point5.ToPackingPosition(0);
      _pos6 = _point6.ToPackingPosition(0);
      _pos7 = _point7.ToPackingPosition(0);
      _pos8 = _point8.ToPackingPosition(0);
      _pos9 = _point9.ToPackingPosition(0);
      _pos10 = _point10.ToPackingPosition(0);
      _pos11 = _point11.ToPackingPosition(0);

      _item1 = _rs1.ToPackingItem();
      _item2 = _rs2.ToPackingItem();
      _item3 = _rs3.ToPackingItem();
      _item4 = _rs4.ToPackingItem();
      _item5 = _rs5.ToPackingItem();
      _item6 = _rs6.ToPackingItem();
      _item7 = _rs7.ToPackingItem();

      _bp1 = new BinPacking3D.BinPacking3D(new PackingShape(30, 30, 30));
      _bp1.PackItem(0, _item4, _pos2);
      _bp2 = new BinPacking3D.BinPacking3D(new PackingShape(30, 30, 30));
      _bp2.PackItem(0, _item4, _pos5);
      _bp3 = new BinPacking3D.BinPacking3D(new PackingShape(30, 30, 30));
      _bp3.PackItem(0, _item4, _pos4);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestOverlapsX() {      
      Assert.IsFalse( (bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsX", new object[] { _point1, _rs1, _pos3, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsX", new object[] { _point3, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsX", new object[] { _point3, _rs1, _pos6, _item1 }));

      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsX", new object[] { _point1, _rs3, _pos3, _item3 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsX", new object[] { _point3, _rs3, _pos1, _item3 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsX", new object[] { _point1, _rs3, _pos1, _item2 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsX", new object[] { _point1, _rs2, _pos1, _item3 }));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestOverlapsY() {
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsY", new object[] { _point1, _rs1, _pos2, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsY", new object[] { _point2, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsY", new object[] { _point2, _rs1, _pos5, _item1 }));

      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsY", new object[] { _point1, _rs3, _pos2, _item3 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsY", new object[] { _point2, _rs3, _pos1, _item3 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsY", new object[] { _point1, _rs3, _pos1, _item2 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsY", new object[] { _point1, _rs2, _pos1, _item3 }));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestOverlapsZ() {
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsZ", new object[] { _point1, _rs1, _pos2, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsZ", new object[] { _point2, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsZ", new object[] { _point2, _rs1, _pos5, _item1 }));

      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsZ", new object[] { _point1, _rs3, _pos2, _item3 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsZ", new object[] { _point2, _rs3, _pos1, _item3 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsZ", new object[] { _point1, _rs3, _pos1, _item2 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsZ", new object[] { _point1, _rs2, _pos1, _item3 }));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestOverlapsOnRight() {
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs1, _pos9, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs1, _pos10, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs1, _pos11, _item1 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point9, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point10, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point11, _rs1, _pos1, _item1 }));

      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs4, _pos9, _item4 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs4, _pos10, _item4 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs4, _pos11, _item4 }));
      
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs3, _pos9, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs3, _pos10, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point1, _rs3, _pos11, _item3 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point9, _rs3, _pos1, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point10, _rs3, _pos1, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsOnRight", new object[] { _point11, _rs3, _pos1, _item3 }));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestOverlapsInFront() {
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs1, _pos9, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs1, _pos8, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs1, _pos11, _item1 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point9, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point8, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point11, _rs1, _pos1, _item1 }));

      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs4, _pos9, _item4 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs4, _pos8, _item4 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs4, _pos11, _item4 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs3, _pos9, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs3, _pos8, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point1, _rs3, _pos11, _item3 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point9, _rs3, _pos1, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point8, _rs3, _pos1, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsInFront", new object[] { _point11, _rs3, _pos1, _item3 }));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestOverlapsAbove() {
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs1, _pos8, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs1, _pos10, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs1, _pos11, _item1 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point8, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point10, _rs1, _pos1, _item1 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point11, _rs1, _pos1, _item1 }));

      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs4, _pos8, _item4 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs4, _pos10, _item4 }));
      Assert.IsTrue((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs4, _pos11, _item4 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs3, _pos8, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs3, _pos10, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point1, _rs3, _pos11, _item3 }));

      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point8, _rs3, _pos1, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point10, _rs3, _pos1, _item3 }));
      Assert.IsFalse((bool)TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "OverlapsAbove", new object[] { _point11, _rs3, _pos1, _item3 }));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestLimitResidualSpaceOnRight() {

      ResidualSpace rs1 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs2 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs3 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs4 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs5 = _rs4.Clone() as ResidualSpace;
      rs4.Height = 20;
      rs5.Height = 10;

      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceOnRight", new object[] { _bp1, _point1, rs1 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceOnRight", new object[] { _bp2, _point1, rs2 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceOnRight", new object[] { _bp3, _point1, rs3 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceOnRight", new object[] { _bp2, _point1, rs4 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceOnRight", new object[] { _bp3, _point1, rs5 });
      Assert.AreEqual(30, rs1.Width);
      Assert.AreEqual(10, rs2.Width);
      Assert.AreEqual(10, rs3.Width);
      Assert.AreEqual(30, rs4.Width);
      Assert.AreEqual(30, rs5.Width);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestLimitResidualSpaceInFront() {
      ResidualSpace rs1 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs2 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs3 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs4 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs5 = _rs4.Clone() as ResidualSpace;
      rs4.Height = 20;
      rs5.Height = 10;

      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceInFront", new object[] { _bp1, _point1, rs1 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceInFront", new object[] { _bp2, _point1, rs2 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceInFront", new object[] { _bp3, _point1, rs3 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceInFront", new object[] { _bp2, _point1, rs4 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceInFront", new object[] { _bp1, _point1, rs5 });
      Assert.AreEqual(10, rs1.Depth);
      Assert.AreEqual(20, rs2.Depth);
      Assert.AreEqual(30, rs3.Depth);
      Assert.AreEqual(30, rs4.Depth);
      Assert.AreEqual(30, rs5.Depth);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestLimitResidualSpaceAbove() {
      ResidualSpace rs1 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs2 = _rs4.Clone() as ResidualSpace;
      ResidualSpace rs3 = _rs4.Clone() as ResidualSpace;

      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceAbove", new object[] { _bp1, _point1, rs1 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceAbove", new object[] { _bp2, _point1, rs2 });
      TestUtils.InvokeMethod(_calculator.GetType(), _calculator, "LimitResidualSpaceAbove", new object[] { _bp3, _point1, rs3 });
      Assert.AreEqual(10, rs1.Height);
      Assert.AreEqual(20, rs2.Height);
      Assert.AreEqual(10, rs3.Height);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestCalculateResidualSpaces() {
      var residualSpaces = _calculator.CalculateResidualSpaces(_bp2, _point1).ToList();
      Assert.AreEqual(3, residualSpaces.Count);
      Assert.AreEqual(10, residualSpaces[0].Width);
      Assert.AreEqual(30, residualSpaces[0].Height);
      Assert.AreEqual(30, residualSpaces[0].Depth);
      Assert.AreEqual(30, residualSpaces[1].Width);
      Assert.AreEqual(30, residualSpaces[1].Height);
      Assert.AreEqual(20, residualSpaces[1].Depth);
      Assert.AreEqual(30, residualSpaces[2].Width);
      Assert.AreEqual(20, residualSpaces[2].Height);
      Assert.AreEqual(30, residualSpaces[2].Depth);
    }
  }
}
