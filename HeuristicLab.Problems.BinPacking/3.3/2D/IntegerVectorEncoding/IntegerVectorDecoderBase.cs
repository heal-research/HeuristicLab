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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.IntegerVectorEncoding;

namespace HeuristicLab.Problems.BinPacking2D {
  [StorableClass]
  public abstract class IntegerVectorDecoderBase : Item, IDecoder<IntegerVector> {

    [StorableConstructor]
    protected IntegerVectorDecoderBase(bool deserializing) : base(deserializing) { }
    protected IntegerVectorDecoderBase(IntegerVectorDecoderBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected IntegerVectorDecoderBase() : base() { }


    public virtual Solution Decode(IntegerVector intVec, PackingShape binShape, IList<PackingItem> items) {
      var sequenceMatrix = IntegerVectorProblem.GenerateSequenceMatrix(intVec);
      Solution result = CreateSolution(binShape);

      //Fill bins according to grouping vector
      IList<int> remainingIDs = new List<int>();
      foreach (var sequence in sequenceMatrix) {
        remainingIDs = remainingIDs.Concat(sequence).ToList();
        result.Bins.Add(CreatePacking(result, ref remainingIDs, items));
      }
      result.UpdateBinPackings();

      //Try to put remaining items in existing bins
      var temp = new List<int>(remainingIDs);
      foreach (int id in temp) {
        foreach (BinPacking2D bp in result.Bins) {
          var position = FindPositionForItem(bp, items[id]);
          if (position != null) {
            bp.PackItem(id, items[id], position);
            remainingIDs.Remove(id);
            break;
          }
        }
      }

      //Put still remaining items in new bins
      while (remainingIDs.Count > 0) {
        result.Bins.Add(CreatePacking(result, ref remainingIDs, items));
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

    protected abstract Solution CreateSolution(PackingShape binShape);
    protected abstract PackingPosition FindPositionForItem(BinPacking2D bp, PackingItem item);
    protected abstract BinPacking2D CreatePacking(Solution partialSolution, ref IList<int> remainingIDs, IList<PackingItem> items);
  }
}
