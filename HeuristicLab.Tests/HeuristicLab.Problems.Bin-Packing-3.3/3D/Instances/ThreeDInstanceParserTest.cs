using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using HeuristicLab.Problems.BinPacking3D.Instances;
using System.Collections.Generic;
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking._3D.Instances.Tests {
  [TestClass]
  public class ThreeDInstanceParserTest {
    private PackingShape _packingShape;
    private IList<PackingItem> _items1;
    private IList<PackingItem> _items2;

    [TestInitialize]
    public void Initializes() {
      _packingShape = new PackingShape(22, 23, 21);
      _items1 = new List<PackingItem>();

      _items1.Add(new PackingItem(8, 10, 10, _packingShape, 800, 1, 1)); // 0,  V = 800,  A =  80, h = 10
      _items1.Add(new PackingItem(10, 8, 10, _packingShape, 800, 2, 1)); // 1,  V = 800,  A = 100, h =  8
      _items1.Add(new PackingItem(10, 10, 8, _packingShape, 800, 3, 1)); // 2,  V = 800,  A =  80, h = 10
      _items1.Add(new PackingItem(8, 8, 10,  _packingShape, 640, 4, 1)); // 3,   V = 640,  A =  80, h =  8
      _items1.Add(new PackingItem(10, 8, 8,  _packingShape, 640, 0, 1)); // 4,   V = 640,  A =  80, h =  8
      _items1.Add(new PackingItem(8, 10, 8,  _packingShape, 640, 1, 1)); // 5,   V = 640,  A =  64, h = 10  
      _items1.Add(new PackingItem(8, 8, 8,   _packingShape, 512, 2, 1)); // 6,    V = 512,  A =  64, h =  8

      _items1.Add(new PackingItem(10, 10, 10, _packingShape,1000,3, 1)); // 7, V = 1000, A = 100, h = 10

      _items1.Add(new PackingItem(9, 10, 10, _packingShape, 900, 4, 1)); // 8,  V = 900,  A =  90, h = 10
      _items1.Add(new PackingItem(10, 9, 10, _packingShape, 900, 0, 1)); // 9,  V = 900,  A = 100, h =  9
      _items1.Add(new PackingItem(10, 10, 9, _packingShape, 900, 1, 1)); // 10, V = 900,  A =  90, h = 10
      _items1.Add(new PackingItem(9, 9, 10,  _packingShape, 810, 2, 1)); // 11,  V = 810,  A =  90, h =  9
      _items1.Add(new PackingItem(10, 9, 9,  _packingShape, 810, 3, 1)); // 12,  V = 810,  A =  90, h =  9
      _items1.Add(new PackingItem(9, 10, 9,  _packingShape, 810, 4, 1)); // 13,  V = 810,  A =  81, h = 10
      _items1.Add(new PackingItem(9, 9, 9,   _packingShape, 729, 0, 1)); // 14,   V = 729,  A =  81, h =  9

      _items2 = new List<PackingItem>();
      _items2.Add(new PackingItem(8, 10, 10, _packingShape, 800, 1, 1)); // 0,  V = 800,  A =  80, h = 10
      _items2.Add(new PackingItem(10, 8, 10, _packingShape, 800, 2, 1)); // 1,  V = 800,  A = 100, h =  8
      _items2.Add(new PackingItem(10, 10, 8, _packingShape, 800, 3, 1)); // 2,  V = 800,  A =  80, h = 10
      _items2.Add(new PackingItem(8, 8, 10, _packingShape, 640, 4, 1)); // 3,   V = 640,  A =  80, h =  8
      _items2.Add(new PackingItem(10, 8, 8, _packingShape, 640, 0, 1)); // 4,   V = 640,  A =  80, h =  8
      _items2.Add(new PackingItem(8, 10, 8, _packingShape, 640, 1, 1)); // 5,   V = 640,  A =  64, h = 10  
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestParse() {
      string path = @".\..\HeuristicLab.Tests\HeuristicLab.Problems.Bin-Packing-3.3\TestInstances\Parser\3D-UnitTestParser1.txt";
      ThreeDInstanceParser parser = new ThreeDInstanceParser();

      using (var fileStream = File.Open(path, FileMode.Open)) {
        parser.Parse(fileStream);
      }
      
      Assert.AreEqual(_packingShape.Width, parser.Bin.Width);
      Assert.AreEqual(_packingShape.Height, parser.Bin.Height);
      Assert.AreEqual(_packingShape.Depth, parser.Bin.Depth);

      Assert.AreEqual(_items1.Count, parser.Items.Count);
      for (int i = 0; i < parser.Items.Count; i++) {
        PackingItem expected = _items1[i];
        PackingItem actual = parser.Items[i];
        Assert.AreEqual(expected.Width, actual.Width);
        Assert.AreEqual(expected.Height, actual.Height);
        Assert.AreEqual(expected.Depth, actual.Depth);
        Assert.AreEqual(expected.Layer, actual.Layer);
        Assert.AreEqual(expected.Weight, actual.Weight);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestParseUncompleteFile() {
      string path = @".\..\HeuristicLab.Tests\HeuristicLab.Problems.Bin-Packing-3.3\TestInstances\Parser\3D-UnitTestParser2.txt";
      ThreeDInstanceParser parser = new ThreeDInstanceParser();

      using (var fileStream = File.Open(path, FileMode.Open)) {
        parser.Parse(fileStream);
      }

      Assert.AreEqual(_packingShape.Width, parser.Bin.Width);
      Assert.AreEqual(_packingShape.Height, parser.Bin.Height);
      Assert.AreEqual(_packingShape.Depth, parser.Bin.Depth);

      Assert.AreEqual(_items2.Count, parser.Items.Count);
      for (int i = 0; i < parser.Items.Count; i++) {
        PackingItem expected = _items2[i];
        PackingItem actual = parser.Items[i];
        Assert.AreEqual(expected.Width, actual.Width);
        Assert.AreEqual(expected.Height, actual.Height);
        Assert.AreEqual(expected.Depth, actual.Depth);
        Assert.AreEqual(expected.Layer, actual.Layer);
        Assert.AreEqual(expected.Weight, actual.Weight);
      }
    }
  }
}
