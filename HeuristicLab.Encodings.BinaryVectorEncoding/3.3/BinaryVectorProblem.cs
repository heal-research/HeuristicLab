#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [StorableType("2F6FEB34-BD19-47AF-9484-7F48565C0C43")]
  public abstract class BinaryVectorProblem : SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector> {
    [Storable] protected ReferenceParameter<IntValue> DimensionRefParameter { get; private set; }
    [Storable] public IResult<ISingleObjectiveSolutionContext<BinaryVector>> BestSolutionResult { get; private set; }

    private ISingleObjectiveSolutionContext<BinaryVector> BestSolution {
      get => BestSolutionResult.Value;
      set => BestSolutionResult.Value = value;
    }

    public int Dimension {
      get { return DimensionRefParameter.Value.Value; }
      protected set { DimensionRefParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected BinaryVectorProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected BinaryVectorProblem(BinaryVectorProblem original, Cloner cloner)
      : base(original, cloner) {
      DimensionRefParameter = cloner.Clone(original.DimensionRefParameter);
      BestSolutionResult = cloner.Clone(original.BestSolutionResult);
      RegisterEventHandlers();
    }

    protected BinaryVectorProblem() : this(new BinaryVectorEncoding() { Length = 10 }) { }
    protected BinaryVectorProblem(BinaryVectorEncoding encoding) : base(encoding) {
      EncodingParameter.ReadOnly = true;
      EvaluatorParameter.ReadOnly = true;
      Parameters.Add(DimensionRefParameter = new ReferenceParameter<IntValue>("Dimension", "The dimension of the binary vector problem.", Encoding.LengthParameter));
      Results.Add(BestSolutionResult = new Result<ISingleObjectiveSolutionContext<BinaryVector>>("Best Solution"));

      Operators.Add(new HammingSimilarityCalculator());
      // TODO: These should be added in the SingleObjectiveProblem base class (if they were accessible from there)
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(ISingleObjectiveSolutionContext<BinaryVector>[] solutionContexts, ResultCollection results, IRandom random) {
      base.Analyze(solutionContexts, results, random);
      var best = GetBest(solutionContexts);
      if (BestSolution == null || IsBetter(best, BestSolution))
        BestSolution = best.Clone() as SingleObjectiveSolutionContext<BinaryVector>;
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
    }

    protected virtual void DimensionOnChanged() { }
  }
}
