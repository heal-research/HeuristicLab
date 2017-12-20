using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D.ExtremePointCreation;

namespace HeuristicLab.Problems.BinPacking._3D.ExtremePointCreation.Tests {

  [TestClass]
  public class TestLineProjectionBasedEPCreator {

    BinPacking3D.BinPacking3D _binPacking1;
    BinPacking3D.BinPacking3D _binPacking2;
    BinPacking3D.BinPacking3D _binPacking3;
    BinPacking3D.BinPacking3D _binPacking4;

    BinPacking3D.PackingItem _item1;
    BinPacking3D.PackingItem _item2;
    BinPacking3D.PackingItem _item3;
    BinPacking3D.PackingItem _item4;

    BinPacking3D.PackingPosition _pos1;
    BinPacking3D.PackingPosition _pos2;
    BinPacking3D.PackingPosition _pos3;
    BinPacking3D.PackingPosition _pos4;

    IExtremePointCreator _creator;

    [TestInitialize]
    public void Initialize() {
      _creator = ExtremePointCreatorFactory.CreateExtremePointCreator(BinPacking3D.ExtremePointCreationMethod.LineProjection, false);
      var packingShape = new BinPacking3D.PackingShape(20, 20, 20);
      _item1 = new BinPacking3D.PackingItem(8, 4, 8, packingShape);
      _item2 = new BinPacking3D.PackingItem(4, 10, 4, packingShape);
      _item3 = new BinPacking3D.PackingItem(8, 8, 8, packingShape);
      _item4 = new BinPacking3D.PackingItem(2, 2, 20, packingShape);

      _pos1 = new BinPacking3D.PackingPosition(0, 0, 0, 0);
      _pos2 = new BinPacking3D.PackingPosition(0, 8, 0, 0);
      _pos3 = new BinPacking3D.PackingPosition(0, 12, 0, 0);
      _pos4 = new BinPacking3D.PackingPosition(0, 0, 4, 0);

      _binPacking1 = new BinPacking3D.BinPacking3D(packingShape);

      _binPacking1.PackItem(0, _item1, _pos1);

      _binPacking2 = _binPacking1.Clone() as BinPacking3D.BinPacking3D;
      _binPacking2.PackItem(1, _item2, _pos2);

      _binPacking3 = _binPacking2.Clone() as BinPacking3D.BinPacking3D;
      _binPacking3.PackItem(2, _item3, _pos3);

      _binPacking4 = _binPacking3.Clone() as BinPacking3D.BinPacking3D;
      _binPacking4.PackItem(3, _item4, _pos4);

      Assert.AreEqual(1, _binPacking1.Items.Count);
      Assert.AreEqual(2, _binPacking2.Items.Count);
      Assert.AreEqual(3, _binPacking3.Items.Count);
      Assert.AreEqual(4, _binPacking4.Items.Count);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestUpdateExtremePoints1() {

      _creator.UpdateBinPacking(_binPacking1, _item1, _pos1);
      Assert.AreEqual(3, _binPacking1.ExtremePoints.Count);
      Assert.IsTrue(_binPacking1.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 8, 0, 0)));
      Assert.IsTrue(_binPacking1.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 4, 0)));
      Assert.IsTrue(_binPacking1.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 0, 8)));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestUpdateExtremePoints2() {
      _creator.UpdateBinPacking(_binPacking2, _item2, _pos2);
      Assert.AreEqual(7, _binPacking2.ExtremePoints.Count);
      Assert.IsTrue(_binPacking2.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 12, 0, 0)));
      Assert.IsTrue(_binPacking2.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 8, 0, 4)));
      Assert.IsTrue(_binPacking2.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 0, 8)));
      Assert.IsTrue(_binPacking2.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 4, 0)));
      Assert.IsTrue(_binPacking2.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 4, 4)));
      Assert.IsTrue(_binPacking2.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 10, 0)));
      Assert.IsTrue(_binPacking2.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 8, 10, 0)));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestUpdateExtremePoints3() {
      _creator.UpdateBinPacking(_binPacking3, _item3, _pos3);
      Assert.AreEqual(8, _binPacking3.ExtremePoints.Count);
      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 8, 0, 4)));
      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 0, 8)));
      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 4, 0)));
      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 4, 4)));
      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 12, 8, 0)));

      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 8, 4)));
      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 10, 0)));
      Assert.IsTrue(_binPacking3.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 8, 10, 0)));
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestUpdateExtremePoints4() {
      _creator.UpdateBinPacking(_binPacking4, _item4, _pos4);
      Assert.AreEqual(12, _binPacking4.ExtremePoints.Count);
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 8, 0, 4)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 0, 8)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 2, 0, 8)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 2, 4, 0)));

      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 2, 4, 4)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 6, 0)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 6, 4)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 6, 8)));

      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 12, 8, 0)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 8, 4)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 0, 10, 0)));
      Assert.IsTrue(_binPacking4.ExtremePoints.ContainsKey(new BinPacking3D.PackingPosition(0, 8, 10, 0)));
    }
  }
}
