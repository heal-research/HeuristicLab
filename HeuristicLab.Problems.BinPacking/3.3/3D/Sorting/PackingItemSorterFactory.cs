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

using HeuristicLab.Problems.BinPacking3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Sorting {
  public static class PackingItemSorterFactory {
    public static IPackingItemSorter CreatePackingItemSorter(SortingMethod sortingMethod) {
      IPackingItemSorter sorter = null;
      switch (sortingMethod) {
        case SortingMethod.Given:
          sorter = new PackingItemGivenSorter();
          break;
        case SortingMethod.VolumeHeight:
          sorter = new PackingItemVolumeHeightSorter();
          break;
        case SortingMethod.HeightVolume:
          sorter = new PackingItemHeightVolumeSorter();
          break;
        case SortingMethod.AreaHeight:
          sorter = new PackingItemAreaHeightSorter();
          break;
        case SortingMethod.HeightArea:
          sorter = new PackingItemHeightAreaSorter();
          break;
        case SortingMethod.ClusteredAreaHeight:
          sorter = new PackingItemClusteredAreaHeightSorter();
          break;
        case SortingMethod.ClusteredHeightArea:
          sorter = new PackingItemClusterdHeightAreaSorter();
          break;
        default:
          throw new ArgumentException("Unknown sorting method: " + sortingMethod);
      }
      return sorter;
    }
  }
}
