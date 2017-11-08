using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D;
using System.Collections.Generic;

using HeuristicLab.Problems.BinPacking._3D.Utils.Tests;
using System.Threading;

namespace HeuristicLab.Problems.BinPacking._3D.Tests {
  [TestClass]
  public class ExtremePointAlgorithmTest {

    private PackingShape _packingShape1;
    private PackingShape _packingShape2;
    private IList<PackingItem> _items;
    private ExtremePointAlgorithm _extremPointAlgorithm;

    [TestInitialize]
    public void Initializes() {
      _packingShape1 = new PackingShape(19, 19, 19);
      _packingShape2 = new PackingShape(18, 18, 18);
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
    public void TestMethod1() {
      object[] parameters = new object[] { _packingShape1, _items, new SortingMethod[] { SortingMethod.Given }, new FittingMethod[] { FittingMethod.FirstFit }, new CancellationToken()};
      var result = TestUtils.InvokeMethod(typeof(ExtremePointAlgorithm), _extremPointAlgorithm, "GetBest", parameters) as Tuple<Solution, double, SortingMethod?, FittingMethod?>;
      //InvokeMethod();

      var bins = result.Item1.Bins;
      var items0 = bins[0].Items;
      var positions0 = bins[0].Positions;
      var items1 = bins[1].Items;
      var positions1 = bins[1].Positions;

      var s = 1;
    }
  }
}
