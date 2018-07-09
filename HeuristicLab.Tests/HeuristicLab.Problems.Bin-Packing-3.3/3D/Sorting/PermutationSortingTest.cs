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

      _items.Add(new PackingItem(8, 10, 10, _packingShape, 1000, 1, 1)); // 0,  V = 800,  A =  80, h = 10
      _items.Add(new PackingItem(10, 8, 10, _packingShape, 1000, 2, 2)); // 1,  V = 800,  A = 100, h =  8
      _items.Add(new PackingItem(10, 10, 8, _packingShape, 1000, 3, 3)); // 2,  V = 800,  A =  80, h = 10
      _items.Add(new PackingItem(8, 8, 10, _packingShape, 1000, 4, 4)); // 3,   V = 640,  A =  80, h =  8
      _items.Add(new PackingItem(10, 8, 8, _packingShape, 1000, 0, 0)); // 4,   V = 640,  A =  80, h =  8
      _items.Add(new PackingItem(8, 10, 8, _packingShape, 1000, 1, 1)); // 5,   V = 640,  A =  64, h = 10  
      _items.Add(new PackingItem(8, 8, 8, _packingShape, 1000, 2, 2)); // 6,    V = 512,  A =  64, h =  8

      _items.Add(new PackingItem(10, 10, 10, _packingShape, 1000, 3, 3)); // 7, V = 1000, A = 100, h = 10

      _items.Add(new PackingItem(9, 10, 10, _packingShape, 1000, 4, 4)); // 8,  V = 900,  A =  90, h = 10
      _items.Add(new PackingItem(10, 9, 10, _packingShape, 1000, 0, 0)); // 9,  V = 900,  A = 100, h =  9
      _items.Add(new PackingItem(10, 10, 9, _packingShape, 1000, 1, 1)); // 10, V = 900,  A =  90, h = 10
      _items.Add(new PackingItem(9, 9, 10, _packingShape, 1000, 2, 2)); // 11,  V = 810,  A =  90, h =  9
      _items.Add(new PackingItem(10, 9, 9, _packingShape, 1000, 3, 3)); // 12,  V = 810,  A =  90, h =  9
      _items.Add(new PackingItem(9, 10, 9, _packingShape, 1000, 4, 4)); // 13,  V = 810,  A =  81, h = 10
      _items.Add(new PackingItem(9, 9, 9, _packingShape, 1000, 0, 0)); // 14,   V = 729,  A =  81, h =  9
    }

    
    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByVolumeHeight() {
      Permutation actual =  PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.VolumeHeight).SortPackingItems(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7,  8, 10, 9, 13, 11, 12, 0, 2, 1, 14, 5, 3, 4, 6});
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }
    
    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByHeightVolume() {
      Permutation actual = PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.HeightVolume).SortPackingItems(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7, 8, 10, 13, 0, 2, 5, 9, 11, 12, 14, 1, 3, 4, 6 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    
    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByAreaHeight() {
      Permutation actual = PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.AreaHeight).SortPackingItems(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7, 9, 1, 8, 10, 11, 12, 13, 14, 0, 2, 3, 4, 5, 6});
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }
    

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByHeightArea() {
      Permutation actual = PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.HeightArea).SortPackingItems(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 7, 8, 10, 13, 0, 2, 5, 9, 11, 12, 14, 1, 3, 4, 6});
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByClusteredAreaHeight() {
      Permutation actual = ((IPackingItemClusteredSorter)PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.ClusteredAreaHeight)).SortPackingItems(_items, _packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 0, 2, 5, 7, 8, 10, 13, 9, 11, 12, 14, 1, 3, 4, 6 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByClusteredHeightArea() {
      Permutation actual = ((IPackingItemClusteredSorter)PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.ClusteredHeightArea)).SortPackingItems(_items, _packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 1, 7, 9, 8, 10, 11, 12, 13, 14, 0, 2, 3, 4, 5, 6 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortBySequenceGroupVolumeHeight() {
      Permutation actual = PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.VolumeHeight).SortPackingItemsBySequenceGroup(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 9, 14, 4, 10, 0, 5, 11, 1, 6, 7, 12, 2, 8, 13, 3 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialHeightVolume() {
      Permutation actual = PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.HeightVolume).SortPackingItemsBySequenceGroup(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 9, 14, 4, 10, 0, 5, 11, 1, 6, 7, 2, 12, 8, 13, 3 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }


    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialAreaHeight() {
      Permutation actual = PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.AreaHeight).SortPackingItemsBySequenceGroup(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 9, 14, 4, 10, 0, 5, 1, 11, 6, 7, 12, 2, 8, 13, 3 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }


    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialHeightArea() {
      Permutation actual = PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.HeightArea).SortPackingItemsBySequenceGroup(_items, _packingShape);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 9, 14, 4, 10, 0, 5, 11, 1, 6, 7, 2, 12, 8, 13, 3 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialClusteredAreaHeight() {
      Permutation actual = ((IPackingItemClusteredSorter)PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.ClusteredAreaHeight)).SortPackingItemsBySequenceGroup(_items, _packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 9, 14, 4, 0, 5, 10, 11, 1, 6, 2, 7, 12, 8, 13, 3 });
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "short")]
    public void TestSortByMaterialClusteredHeightArea() {
      Permutation actual = ((IPackingItemClusteredSorter)PackingItemSorterFactory.CreatePackingItemSorter(SortingMethod.ClusteredHeightArea)).SortPackingItemsBySequenceGroup(_items, _packingShape, 1.0);
      Permutation expected = new Permutation(PermutationTypes.Absolute, new int[] { 9, 14, 4, 10, 0, 5, 1, 11, 6, 7, 12, 2, 8, 13, 3});
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }    
  }
}
