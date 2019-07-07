#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.IntegerVectorEncoding;

namespace HeuristicLab.Problems.BinPacking3D {
  [StorableType("19C0AA6B-8FC5-4EA9-A441-5C4F5AD4C38E")]
  public abstract class IntegerVectorDecoderBase : Item, IDecoder<IntegerVector> {

    [StorableConstructor]
    protected IntegerVectorDecoderBase(StorableConstructorFlag _) : base(_) { }
    protected IntegerVectorDecoderBase(IntegerVectorDecoderBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected IntegerVectorDecoderBase() : base() { }


    public virtual Solution Decode(IntegerVector intVec, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      var sequenceMatrix = IntegerVectorProblem.GenerateSequenceMatrix(intVec);
      Solution result = CreateSolution(binShape, useStackingConstraints);

      //Fill bins according to grouping vector
      IList<int> remainingIDs = new List<int>();
      foreach (var sequence in sequenceMatrix) {
        remainingIDs = remainingIDs.Concat(sequence).ToList();
        result.Bins.Add(CreatePacking(result, ref remainingIDs, items, useStackingConstraints));
      }
      result.UpdateBinPackings();

      //Try to put remaining items in existing bins
      var temp = new List<int>(remainingIDs);
      foreach (int id in temp) {
        foreach (BinPacking3D bp in result.Bins) {
          var position = FindPositionForItem(bp, items[id], useStackingConstraints);
          if (position != null) {
            bp.PackItem(id, items[id], position);
            remainingIDs.Remove(id);
            break;
          }
        }
      }

      //Put still remaining items in new bins
      while (remainingIDs.Count > 0) {
        result.Bins.Add(CreatePacking(result, ref remainingIDs, items, useStackingConstraints));
      }
      result.UpdateBinPackings();

      // gkronber: original implementation by Helm also updates the encoded solution (TODO)
      // var newSolution = new int[intVec.Length];
      // int binIndex = 0;
      // foreach (var bp in result.BinPackings) {
      //   foreach (var entry in bp.ItemPositions)
      //     newSolution[entry.Key] = binIndex;
      //   binIndex++;
      // }
      // solution.GroupingVector = new IntegerVector(newSolution);

      return result;
    }

    protected abstract Solution CreateSolution(PackingShape binShape, bool useStackingConstraints);
    protected abstract PackingPosition FindPositionForItem(BinPacking3D bp, PackingItem item, bool useStackingConstraints);
    protected abstract BinPacking3D CreatePacking(Solution partialSolution, ref IList<int> remainingIDs, IList<PackingItem> items, bool useStackingConstraints);
  }
}
