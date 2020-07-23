#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [StorableType("aceff7a2-0666-4055-b698-6ea3628713b6")]
  public abstract class PermutationProblem : SingleObjectiveProblem<PermutationEncoding, Permutation> {
    [Storable] protected ReferenceParameter<IntValue> DimensionRefParameter { get; private set; }
    [Storable] protected ReferenceParameter<EnumValue<PermutationTypes>> PermutationTypeRefParameter { get; private set; }

    public int Dimension {
      get { return DimensionRefParameter.Value.Value; }
      set { DimensionRefParameter.Value.Value = value; }
    }

    public PermutationTypes Type {
      get { return PermutationTypeRefParameter.Value.Value; }
      set { PermutationTypeRefParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected PermutationProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected PermutationProblem(PermutationProblem original, Cloner cloner)
      : base(original, cloner) {
      DimensionRefParameter = cloner.Clone(original.DimensionRefParameter);
      PermutationTypeRefParameter = cloner.Clone(original.PermutationTypeRefParameter);
      RegisterEventHandlers();
    }

    protected PermutationProblem() : this(new PermutationEncoding() { Length = 10, Type = PermutationTypes.Absolute }) { }
    protected PermutationProblem(PermutationEncoding encoding) : base(encoding) {
      EncodingParameter.ReadOnly = true;
      EvaluatorParameter.ReadOnly = true;
      Parameters.Add(DimensionRefParameter = new ReferenceParameter<IntValue>("Dimension", "The dimension of the permutation problem.", Encoding.LengthParameter));
      Parameters.Add(PermutationTypeRefParameter = new ReferenceParameter<EnumValue<PermutationTypes>>("Type", "The type of the permutation.", Encoding.PermutationTypeParameter));
      
      Operators.Add(new HammingSimilarityCalculator());
      // TODO: These should be added in the SingleObjectiveProblem base class (if they were accessible from there)
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(Permutation[] permutations, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(permutations, qualities, results, random);
      var best = GetBestSolution(permutations, qualities);
      results.AddOrUpdateResult("Best Solution", (IItem)best.Item1.Clone());
    }

    protected override sealed void OnEvaluatorChanged() {
      throw new InvalidOperationException("Evaluator may not change!");
    }

    protected override sealed void OnEncodingChanged() {
      throw new InvalidOperationException("Encoding may not change!");
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      Parameterize();
    }

    private void Parameterize() {
      // TODO: this is done in base class as well (but operators are added at this level of the hierarchy)
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }

    private void RegisterEventHandlers() {
      IntValueParameterChangeHandler.Create(DimensionRefParameter, DimensionOnChanged);
      EnumValueParameterChangeHandler<PermutationTypes>.Create(PermutationTypeRefParameter, TypeOnChanged);
    }

    protected virtual void DimensionOnChanged() { }

    protected virtual void TypeOnChanged() { }
  }
}
