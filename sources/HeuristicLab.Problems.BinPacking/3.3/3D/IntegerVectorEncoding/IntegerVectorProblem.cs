#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Bin Packing Problem (3D, integer vector encoding) (BPP)", "Represents a two-dimensional bin-packing problem using only bins with identical measures and bins/items with rectangular shapes.")]
  [StorableClass]
  [Creatable(Category = CreatableAttribute.Categories.CombinatorialProblems, Priority = 330)]
  public sealed class IntegerVectorProblem : ProblemBase<IntegerVectorEncoding, IntegerVector> {
    [StorableConstructor]
    private IntegerVectorProblem(bool deserializing) : base(deserializing) { }

    // cloning
    private IntegerVectorProblem(IntegerVectorProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    public IntegerVectorProblem()
      : base() {
      Decoder = new ExtremePointIntegerVectorDecoder(); // default decoder

      // the int vector contains the target bin number for each item
      Encoding = new IntegerVectorEncoding(EncodedSolutionName, Items.Count, min: 0, max: LowerBound + 1); // NOTE: assumes that all items can be packed into LowerBound+1 bins
      AddOperators();
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntegerVectorProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }


    private void AddOperators() {

      // move operators are not yet supported (TODO)
      Operators.RemoveAll(x => x is SingleObjectiveMoveGenerator);
      Operators.RemoveAll(x => x is SingleObjectiveMoveMaker);
      Operators.RemoveAll(x => x is SingleObjectiveMoveEvaluator);

      Encoding.ConfigureOperators(Operators.OfType<IOperator>()); // gkronber: not strictly necessary (only when customer ops are added)
    }

    private void RegisterEventHandlers() {
      // update encoding length when number of items is changed
      ItemsParameter.ValueChanged += (sender, args) => Encoding.Length = Items.Count;
      LowerBoundParameter.Value.ValueChanged += (sender, args) => {
        for (int i = 0; i < Encoding.Bounds.Rows; i++) {
          Encoding.Bounds[i, 1] = LowerBound + 1;
        }
      };
    }

    #region helpers

    public static List<List<int>> GenerateSequenceMatrix(IntegerVector intVec) {
      List<List<int>> result = new List<List<int>>();
      int nrOfBins = intVec.Max() + 1;
      for (int i = 0; i < nrOfBins; i++)
        result.Add(new List<int>());
      for (int i = 0; i < intVec.Length; i++) {
        result[intVec[i]].Add(i);
      }
      return result;
    }
    #endregion
  }
}
