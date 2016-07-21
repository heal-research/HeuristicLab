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

using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("Bin Packing Problem (2D, permutation encoding) (BPP)", "Represents a two-dimensional bin-packing problem using only bins with identical measures and bins/items with rectangular shapes.")]
  [StorableClass]
  [Creatable(Category = CreatableAttribute.Categories.CombinatorialProblems, Priority = 300)]
  public sealed class PermutationProblem : ProblemBase<PermutationEncoding, Permutation> {
    // persistence
    [StorableConstructor]
    private PermutationProblem(bool deserializing) : base(deserializing) { }

    // cloning
    private PermutationProblem(PermutationProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    public PermutationProblem()
      : base() {
      Decoder = new ExtremePointPermutationDecoder(); // default decoder

      Encoding = new PermutationEncoding(EncodedSolutionName, Items.Count, PermutationTypes.Absolute);
      AddOperators();
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PermutationProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }


    private void AddOperators() {
      Operators.Add(new TranslocationMoveEvaluator());
      Operators.Add(new Swap2MoveEvaluator());

      Operators.RemoveAll(x => x is SingleObjectiveMoveGenerator);
      Operators.RemoveAll(x => x is SingleObjectiveMoveMaker);
      Operators.RemoveAll(x => x is SingleObjectiveMoveEvaluator);

      Encoding.ConfigureOperators(Operators.OfType<IOperator>());

      foreach (var op in Operators.OfType<IOperator<Permutation>>()) {
        op.BinShapeParameter.ActualName = BinShapeParameter.Name;
        op.ItemsParameter.ActualName = ItemsParameter.Name;
        op.SolutionEvaluatorParameter.ActualName = SolutionEvaluatorParameter.Name;
        op.DecoderParameter.ActualName = DecoderParameter.Name;
      }
    }

    private void RegisterEventHandlers() {
      // update encoding length when number of items is changed
      ItemsParameter.ValueChanged += (sender, args) => Encoding.Length = Items.Count;
    }
  }
}
