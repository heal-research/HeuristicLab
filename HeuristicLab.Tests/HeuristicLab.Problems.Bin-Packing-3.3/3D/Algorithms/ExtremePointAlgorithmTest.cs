using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D;
using System.Collections.Generic;

using HeuristicLab.Problems.BinPacking._3D.Utils.Tests;
using System.Threading;
using System.Linq;

namespace HeuristicLab.Problems.BinPacking._3D.Algorithms.Tests {
  [TestClass]
  public class ExtremePointAlgorithmTest {

    private PackingShape _packingShape1;
    private PackingShape _packingShape2;
    private PackingShape _packingShape3;
    private IList<PackingItem> _items;
    private ExtremePointAlgorithm _extremPointAlgorithm;

    [TestInitialize]
    public void Initializes() {
      _packingShape1 = new PackingShape(19, 19, 19);
      _packingShape2 = new PackingShape(18, 18, 18);
      _packingShape3 = new PackingShape(20, 20, 20);
      _items = new List<PackingItem>();

      _items.Add(new PackingItem(8, 10, 10, _packingShape1, 1000, 1)); // 0,  V = 800,  A =  80, h = 10
      _items.Add(new PackingItem(10, 8, 10, _packingShape1, 1000, 2)); // 1,  V = 800,  A = 100, h =  8
      _items.Add(new PackingItem(10, 10, 8, _packingShape1, 1000, 3)); // 2,  V = 800,  A =  80, h = 10
      _items.Add(new PackingItem(8, 8, 10, _packingShape1, 1000, 4)); // 3,   V = 640,  A =  80, h =  8
      _items.Add(new PackingItem(10, 8, 8, _packingShape1, 1000, 0)); // 4,   V = 640,  A =  80, h =  8
      _items.Add(new PackingItem(8, 10, 8, _packingShape1, 1000, 1)); // 5,   V = 640,  A =  64, h = 10  
      _items.Add(new PackingItem(8, 8, 8, _packingShape1, 1000, 2)); // 6,    V = 512,  A =  64, h =  8

      _items.Add(new PackingItem(10, 10, 10, _packingShape1, 1000, 3)); // 7, V = 1000, A = 100, h = 10

      _items.Add(new PackingItem(9, 10, 10, _packingShape1, 1000, 4)); // 8,  V = 900,  A =  90, h = 10
      _items.Add(new PackingItem(10, 9, 10, _packingShape1, 1000, 0)); // 9,  V = 900,  A = 100, h =  9
      _items.Add(new PackingItem(10, 10, 9, _packingShape1, 1000, 1)); // 10, V = 900,  A =  90, h = 10
      _items.Add(new PackingItem(9, 9, 10, _packingShape1, 1000, 2)); // 11,  V = 810,  A =  90, h =  9
      _items.Add(new PackingItem(10, 9, 9, _packingShape1, 1000, 3)); // 12,  V = 810,  A =  90, h =  9
      _items.Add(new PackingItem(9, 10, 9, _packingShape1, 1000, 4)); // 13,  V = 810,  A =  81, h = 10
      _items.Add(new PackingItem(9, 9, 9, _packingShape1, 1000, 0)); // 14,   V = 729,  A =  81, h =  9

      _extremPointAlgorithm = new ExtremePointAlgorithm();
    }



    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithm() {
       
      IList<BinPacking3D.PackingPosition> positionsBin0 = new List<BinPacking3D.PackingPosition>() {
        new BinPacking3D.PackingPosition(0, 0,0,0),
        new BinPacking3D.PackingPosition(0, 8,0,0),
        new BinPacking3D.PackingPosition(0, 0,0,10),
        new BinPacking3D.PackingPosition(0, 10,0,10),
        new BinPacking3D.PackingPosition(0, 8,8,0),
        new BinPacking3D.PackingPosition(0, 0,10,0),
        new BinPacking3D.PackingPosition(0, 0,10,8),
        new BinPacking3D.PackingPosition(0, 8,10,8),
      };

      IList<BinPacking3D.PackingPosition> positionsBin1 = new List<BinPacking3D.PackingPosition>() {
        new BinPacking3D.PackingPosition(1, 0,0,0),
        new BinPacking3D.PackingPosition(1, 9,0,0),
        new BinPacking3D.PackingPosition(1, 0,0,10),
        new BinPacking3D.PackingPosition(1, 10,0,10),
        new BinPacking3D.PackingPosition(1, 9,9,0),
        new BinPacking3D.PackingPosition(1, 0,10,0),
        new BinPacking3D.PackingPosition(1, 0,10,9)
      };

      object[] parameters = new object[] { _packingShape3,
                                           _items,
                                           new SortingMethod[] { SortingMethod.Given },
                                           new FittingMethod[] { FittingMethod.FirstFit },
                                           new ExtremePointCreationMethod[] { ExtremePointCreationMethod.PointProjection},
                                           new CancellationToken() };
      _extremPointAlgorithm.SortingMethodParameter.Value.Value = SortingMethod.Given;
      _extremPointAlgorithm.FittingMethodParameter.Value.Value = FittingMethod.FirstFit;
      _extremPointAlgorithm.ExtremePointCreationMethodParameter.Value.Value = ExtremePointCreationMethod.PointProjection;
      _extremPointAlgorithm.SortByMaterialParameter.Value.Value = false;
      var result = TestUtils.InvokeMethod(typeof(ExtremePointAlgorithm), _extremPointAlgorithm, "GetBest", parameters) as Tuple<Solution, double, SortingMethod?, FittingMethod?, ExtremePointCreationMethod?>;

      int i = 0;
      foreach (var item in result.Item1.Bins[0].Positions.Values) {
        Assert.AreEqual(positionsBin0[i].X, item.X);
        Assert.AreEqual(positionsBin0[i].Y, item.Y);
        Assert.AreEqual(positionsBin0[i].Z, item.Z);
        i++;
      }

      i = 0;
      foreach (var item in result.Item1.Bins[1].Positions.Values) {
        Assert.AreEqual(positionsBin1[i].X, item.X);
        Assert.AreEqual(positionsBin1[i].Y, item.Y);
        Assert.AreEqual(positionsBin1[i].Z, item.Z);
        i++;
      }
    }
  }
}
