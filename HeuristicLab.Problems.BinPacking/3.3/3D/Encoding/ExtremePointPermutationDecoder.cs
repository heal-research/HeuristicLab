#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking3D.Packer;

namespace HeuristicLab.Problems.BinPacking3D.Encoding {

  /// <summary>
  /// This class creates a solution
  /// </summary>
  [Item("Extreme-point Permutation Decoder (3d) Base", "Base class for 3d decoders")]
  [StorableClass]
  public class ExtremePointPermutationDecoder : Item, IDecoder<Permutation>
    //where TBinPacker : BinPacker, new ()
    {

    [StorableConstructor]
    protected ExtremePointPermutationDecoder(bool deserializing) : base(deserializing) { }
    protected ExtremePointPermutationDecoder(ExtremePointPermutationDecoder original, Cloner cloner)
      : base(original, cloner) {
    }

    /// <summary>
    /// Creates an extrem point based permutation decoder
    /// </summary>
    /// <param name="binPacker">Contains the packing algorithm for packing the items</param>
    public ExtremePointPermutationDecoder(BinPacker binPacker) : base() {
      _binPacker = binPacker;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtremePointPermutationDecoder(this, cloner);
    }

    BinPacker _binPacker;

    /// <summary>
    /// Creates a solution for displaying it on the HEAL gui
    /// </summary>
    /// <param name="permutation">Permutation of items sorted by a sorting method. The value of each permutation index references to the index of the items list</param>
    /// <param name="binShape">Bin for storing the items</param>
    /// <param name="items">A list of packing items which should be assigned to a bin</param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    public Solution Decode(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      Solution solution = new Solution(binShape, useExtremePoints: true, stackingConstraints: useStackingConstraints);
      foreach (var packedBin in _binPacker.PackItems(permutation, binShape, items, useStackingConstraints)) {
        solution.Bins.Add(packedBin);
      }
      return solution;
    }
  }
}

