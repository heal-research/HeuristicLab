#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.BinPacking2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Sorting {

  /// <summary>
  /// This is a extension class for sorting a permutation.
  /// They are extension methods for the class Permutation.
  /// </summary>
  public static class PermutationPackingItemSorter {
       
    /// <summary>
    /// Sorts a given permutation first by the volume and secoundly by the height.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByVolumeHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permutation first by the heigth and secoundly by the volume.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByHeightVolume(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permutation first by the area and secondly by the height.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByAreaHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Depth * x.Item.Width)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permuation first by the height and secoundly by the area.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByHeightArea(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permutation. The items are being grouped by the cluster id.
    /// The cluster id is calulated as followed: clusterId = Ceiling( (width * depth) / (width * depth * delta))
    /// The permutation is first being sorted by the area and secoundly by the height.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <param name="bin">The bin is needed for building the cluster</param>
    /// <param name="delta">The delta is needed for building the cluster</param>
    /// <returns>A new sorted permutation</returns>
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

    /// <summary>
    /// Sorts a given permutation. The items are being grouped by the cluster id.
    /// The cluster id is calulated as followed: clusterId = Ceiling( (height) / (height * delta))
    /// The permutation is first being sorted by the height and secoundly by the area.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <param name="bin">The bin is needed for building the cluster</param>
    /// <param name="delta">The delta is needed for building the cluster</param>
    /// <returns>A new sorted permutation</returns>
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

    /// <summary>
    /// Sorts a given permutation first by the material, secoundly by the volume and finally by the height.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByMaterialVolumeHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permutation first by the material, secoundly by the heigth and finally by the volume.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByMaterialHeightVolume(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permutation first by the material, secoundly by the area and finally by the height.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByMaterialAreaHeight(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permuation first by the material, secoundly by the height and finally by the area.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByMaterialHeightArea(this IList<PackingItem> items) {
      return new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Material)
                         .ThenByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width)
                         .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permutation. The items are being grouped by the cluster id.
    /// The cluster id is calulated as followed: clusterId = Ceiling( (width * depth) / (width * depth * delta))
    /// The permutation is being clusterd by the area, first sorted by the material, secoundly by the height.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <param name="bin">The bin is needed for building the cluster</param>
    /// <param name="delta">The delta is needed for building the cluster</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByMaterialClusteredAreaHeight(this IList<PackingItem> items, PackingShape bin, double delta) {
      double clusterRange = bin.Width * bin.Depth * delta;
      return new Permutation(PermutationTypes.Absolute,
                items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Width * v.Depth / clusterRange)) })
                    .GroupBy(x => x.ClusterId)
                    .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(z => z.Item.Material).ThenByDescending(y => y.Item.Height).ToList() })
                    .OrderByDescending(x => x.Cluster)
                    .SelectMany(x => x.Items)
                    .Select(x => x.Index).ToArray());
    }

    /// <summary>
    /// Sorts a given permutation. The items are being grouped by the cluster id.
    /// The cluster id is calulated as followed: clusterId = Ceiling( (height) / (height * delta))
    /// The permutation is being clusterd by the height, first sorted by the material, secoundly by the area.
    /// </summary>
    /// <param name="items">Permuation which should be sorted</param>
    /// <param name="bin">The bin is needed for building the cluster</param>
    /// <param name="delta">The delta is needed for building the cluster</param>
    /// <returns>A new sorted permutation</returns>
    public static Permutation SortByMaterialClusteredHeightArea(this IList<PackingItem> items, PackingShape bin,  double delta) {
      double clusterRange2 = bin.Height * delta;
      return new Permutation(PermutationTypes.Absolute,
                items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Height / clusterRange2)) })
                    .GroupBy(x => x.ClusterId)
                    .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(z => z.Item.Material).ThenByDescending(y => y.Item.Depth * y.Item.Width).ToList() })
                    .OrderByDescending(x => x.Cluster)
                    .SelectMany(x => x.Items)
                    .Select(x => x.Index).ToArray());
    }
  }
}
