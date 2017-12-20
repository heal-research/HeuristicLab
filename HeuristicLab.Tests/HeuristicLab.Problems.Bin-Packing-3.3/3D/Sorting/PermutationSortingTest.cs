using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D;
using System.Collections.Generic;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.BinPacking3D.Sorting;

namespace HeuristicLab.Problems.BinPacking._3D.Sorting.Tests {
  [TestClass]
  public class PermutationSortingTest {
    private PackingShape _packingShape;
    private IList<PackingItem> _items;

    [TestInitialize]
    public void Initializes() {
      _packingShape = new PackingShape(20, 20, 20);
      _items = new List<PackingItem>();

      _items.Add(new PackingItem(8, 10, 10, _packingShape, 1000, 1)); // 0,  V = 800,  A =  80, h = 10
      _items.Add(new PackingItem(10, 8, 10, _packingShape, 1000, 2)); // 1,  V = 800,  A = 100, h =  8
      _items.Add(new PackingItem(10, 10, 8, _packingShape, 1000, 3)); // 2,  V = 800,  A =  80, h = 10
      _items.Add(new PackingItem(8, 8, 10, _packingShape, 1000, 4)); // 3,   V = 640,  A =  80, h =  8
      _items.Add(new PackingItem(10, 8, 8, _packingShape, 1000, 0)); // 4,   V = 640,  A =  80, h =  8
      _items.Add(new PackingItem(8, 10, 8, _packingShape, 1000, 1)); // 5,   V = 640,  A =  64, h = 10  
      _items.Add(new PackingItem(8, 8, 8, _packingShape, 1000, 2)); // 6,    V = 512,  A =  64, h =  8

      _items.Add(new PackingItem(10, 10, 10, _packingShape, 1000, 3)); // 7, V = 1000, A = 100, h = 10

      _items.Add(new PackingItem(9, 10, 10, _packingShape, 1000, 4)); // 8,  V = 900,  A =  90, h = 10
      _items.Add(new PackingItem(10, 9, 10, _packingShape, 1000, 0)); // 9,  V = 900,  A = 100, h =  9
      _items.Add(new PackingItem(10, 10, 9, _packingShape, 1000, 1)); // 10, V = 900,  A =  90, h = 10
      _items.Add(new PackingItem(9, 9, 10, _packingShape, 1000, 2)); // 11,  V = 810,  A =  90, h =  9
      _items.Add(new PackingItem(10, 9, 9, _packingShape, 1000, 3)); // 12,  V = 810,  A =  90, h =  9
      _items.Add(new PackingItem(9, 10, 9, _packingShape, 1000, 4)); // 13,  V = 810,  A =  81, h = 10
      _items.Add(new PackingItem(9, 9, 9, _packingShape, 1000, 0)); // 14,   V = 729,  A =  81, h =  9
    }

    
    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByVolumeHeightExtension() {
      Permutation actual = _items.SortByVolumeHeight();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7,  8, 10, 9, 13, 11, 12, 0, 2, 1, 14, 5, 3, 4, 6});
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }
    
    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByHeightVolumeExtension() {
      Permutation actual = _items.SortByHeightVolume();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7, 8, 10, 13, 0, 2, 5, 9, 11, 12, 14, 1, 3, 4, 6 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    
    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByAreaHeightExtension() {
      Permutation actual = _items.SortByAreaHeight();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7, 9, 1, 8, 10, 11, 12, 13, 14, 0, 2, 3, 4, 5, 6});
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }
    

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByHeightAreaExtension() {
      Permutation actual = _items.SortByHeightArea();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7, 8, 10, 13, 0, 2, 5, 9, 11, 12, 14, 1, 3, 4, 6});
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByClusteredAreaHeightExtension() {
      Permutation actual = _items.SortByClusteredAreaHeight(_packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 0, 2, 5, 7, 8, 10, 13, 9, 11, 12, 14, 1, 3, 4, 6 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByClusteredHeightAreaExtension() {
      Permutation actual = _items.SortByClusteredHeightArea(_packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 1, 7, 9, 8, 10, 11, 12, 13, 14, 0, 2, 3, 4, 5, 6 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialVolumeHeightExtension() {
      Permutation actual = _items.SortByMaterialVolumeHeight();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 8, 13, 3, 7, 12, 2, 11, 1, 6, 10, 0, 5, 9, 14, 4 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialHeightVolumeExtension() {
      Permutation actual = _items.SortByMaterialHeightVolume();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 8, 13, 3, 7, 2, 12, 11, 1, 6, 10, 0, 5, 9, 14, 4 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }


    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialAreaHeightExtension() {
      Permutation actual = _items.SortByMaterialAreaHeight();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 8, 13, 3, 7, 12, 2, 1, 11, 6, 10, 0, 5, 9, 14, 4 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }


    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialHeightAreaExtension() {
      Permutation actual = _items.SortByMaterialHeightArea();
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 8, 13, 3, 7, 2, 12, 11, 1, 6, 10, 0, 5, 9, 14, 4 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialClusteredAreaHeightExtension() {
      Permutation actual = _items.SortByMaterialClusteredAreaHeight(_packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 8, 13, 3, 2, 7, 12, 11, 1, 6, 0, 5, 10, 9, 14, 4 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialClusteredHeightAreaExtension() {
      Permutation actual = _items.SortByClusteredHeightArea(_packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 1, 7, 9, 8, 10, 11, 12, 13, 14, 0, 2, 3, 4, 5, 6 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }    
  }
}
