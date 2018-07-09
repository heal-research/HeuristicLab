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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking3D.Sorting {
  internal class PackingItemClusterdHeightAreaSorter : IPackingItemClusteredSorter {
    public Permutation SortPackingItems(IList<PackingItem> items, PackingShape bin, double delta) {
      return items.SortByClusteredHeightArea(bin, delta);
    }

    public Permutation SortPackingItems(IList<PackingItem> items, PackingShape bin) {
      return SortPackingItems(items, bin, 10.0);
    }

    public Permutation SortPackingItemsBySequenceGroup(IList<PackingItem> items, PackingShape bin, double delta) {
      return items.SortByMaterialClusteredHeightArea(bin, delta);
    }

    public Permutation SortPackingItemsBySequenceGroup(IList<PackingItem> items, PackingShape bin) {
      return SortPackingItemsBySequenceGroup(items, bin, 10.0);
    }
  }
}
