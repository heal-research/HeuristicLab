using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.BinPacking2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Sorting {
  public static class PackingItemSorter {
    public static Permutation SortByVolumeHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByHeightVolume(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByAreaHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Depth * x.Item.Width)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByHeightArea(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByClusteredAreaHeight(this IList<PackingItem> items, PackingShape bin, double delta) {
      double clusterRange = bin.Width * bin.Depth * delta;
      return new Permutation(PermutationTypes.Absolute,
                items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Width * v.Depth / clusterRange)) })
                    .GroupBy(x => x.ClusterId)
                    .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(y => y.Item.Height).ToList() })
                    .OrderByDescending(x => x.Cluster)
                    .SelectMany(x => x.Items)
                    .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByClusteredHeightArea(this IList<PackingItem> items, PackingShape bin, double delta) {
      double clusterRange2 = bin.Height * delta;
      return new Permutation(PermutationTypes.Absolute,
                items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Height / clusterRange2)) })
                    .GroupBy(x => x.ClusterId)
                    .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(y => y.Item.Depth * y.Item.Width).ToList() })
                    .OrderByDescending(x => x.Cluster)
                    .SelectMany(x => x.Items)
                    .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByMaterialVolumeHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByMaterialHeightVolume(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByMaterialAreaHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByMaterialHeightArea(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width)
                         .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByMaterialClusteredAreaHeight(this IList<PackingItem> items, PackingShape bin, double delta) {
      double clusterRange = bin.Width * bin.Depth * delta;
      return new Permutation(PermutationTypes.Absolute,
                items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Width * v.Depth / clusterRange)) })
                    .GroupBy(x => x.ClusterId)
                    .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(y => y.Item.Height).ToList() })
                    .OrderByDescending(x => x.Cluster)
                    .SelectMany(x => x.Items)
                    .Select(x => x.Index).ToArray());
    }

    public static Permutation SortByMaterialClusteredHeightArea(this IList<PackingItem> items, PackingShape bin,  double delta) {
      double clusterRange2 = bin.Height * delta;
      return new Permutation(PermutationTypes.Absolute,
                items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Height / clusterRange2)) })
                    .GroupBy(x => x.ClusterId)
                    .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(y => y.Item.Depth * y.Item.Width).ToList() })
                    .OrderByDescending(x => x.Cluster)
                    .SelectMany(x => x.Items)
                    .Select(x => x.Index).ToArray());
    }
  }
}
