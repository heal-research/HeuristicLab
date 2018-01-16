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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  public interface IBinPacker {
    /// <summary>
    /// Packs all items of the bin packer and returns a collection of BinPacking3D objects
    /// </summary>
    /// <param name="sortedItems">Permutation of items sorted by a sorting method. The value of each permutation index references to the index of the items list</param>
    /// <param name="binShape">Bin for storing the items</param>
    /// <param name="items">A list of packing items which should be assigned to a bin</param>
    /// <param name="useStackingConstraints">Flag for using stacking constraints</param>
    /// <returns>Returns a collection of bin packing 3d objects. Each object represents a bin and the packed items</returns>
    IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints);

  }
}
