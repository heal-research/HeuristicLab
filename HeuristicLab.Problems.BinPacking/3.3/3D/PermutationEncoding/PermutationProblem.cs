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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking3D.Decoder;
using HeuristicLab.Problems.BinPacking3D.Packer;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Bin Packing Problem (3D, permutation encoding) (BPP)", "Represents a three-dimensional bin-packing problem using only bins with identical measures and bins/items with rectangular shapes.")]
  [StorableClass]
  [Creatable(Category = CreatableAttribute.Categories.CombinatorialProblems, Priority = 320)]
  public sealed class PermutationProblem : ProblemBase<PermutationEncoding, Permutation> {
    [StorableConstructor]
    private PermutationProblem(bool deserializing) : base(deserializing) { }

    // cloning
    private PermutationProblem(PermutationProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    public PermutationProblem()
      : base() {
      Decoder = new ExtremePointPermutationDecoder(new BinPackerFirstFit()); // default decoder

      Encoding = new PermutationEncoding(EncodedSolutionName, Items.Count, PermutationTypes.Absolute);
      AddOperators();
      Parameterize();
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PermutationProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }

    private void AddOperators() {
      Operators.Add(new TranslocationMoveEvaluator());
      Operators.Add(new Swap2MoveEvaluator());

      Operators.RemoveAll(x => x is SingleObjectiveMoveGenerator);
      Operators.RemoveAll(x => x is SingleObjectiveMoveMaker);
      Operators.RemoveAll(x => x is SingleObjectiveMoveEvaluator);
      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Encoding.ConfigureOperators(Operators.OfType<IOperator>());

      foreach (var op in Operators.OfType<IOperator<Permutation>>()) {
        op.BinShapeParameter.ActualName = BinShapeParameter.Name;
        op.ItemsParameter.ActualName = ItemsParameter.Name;
        op.SolutionEvaluatorParameter.ActualName = SolutionEvaluatorParameter.Name;
        op.DecoderParameter.ActualName = DecoderParameter.Name;
        op.UseStackingConstraintsParameter.ActualName = UseStackingConstraintsParameter.Name;
      }
    }

    private void RegisterEventHandlers() {
      // update encoding length when number of items is changed
      ItemsParameter.ValueChanged += (sender, args) => Parameterize();
    }

    private void Parameterize() {
      Encoding.Length = Items.Count;
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }
  }
}
