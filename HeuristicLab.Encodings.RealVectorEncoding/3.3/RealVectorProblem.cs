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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [StorableType("7860a955-af16-459a-9cd8-51667e06d38e")]
  public abstract class RealVectorProblem : SingleObjectiveProblem<RealVectorEncoding, RealVector> {
    [Storable] protected ReferenceParameter<IntValue> DimensionRefParameter { get; private set; }
    [Storable] protected ReferenceParameter<DoubleMatrix> BoundsRefParameter { get; private set; }
    [Storable] public IResult<ISingleObjectiveSolutionContext<RealVector>> BestSolutionResult { get; private set; }

    public int Dimension {
      get { return DimensionRefParameter.Value.Value; }
      set { DimensionRefParameter.Value.Value = value; }
    }

    public DoubleMatrix Bounds {
      get { return BoundsRefParameter.Value; }
      set { BoundsRefParameter.Value = value; }
    }

    protected ISingleObjectiveSolutionContext<RealVector> BestSolution {
      get => BestSolutionResult.Value;
      set => BestSolutionResult.Value = value;
    }

    [StorableConstructor]
    protected RealVectorProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected RealVectorProblem(RealVectorProblem original, Cloner cloner)
      : base(original, cloner) {
      DimensionRefParameter = cloner.Clone(original.DimensionRefParameter);
      BoundsRefParameter = cloner.Clone(original.BoundsRefParameter);
      BestSolutionResult = cloner.Clone(original.BestSolutionResult);
      RegisterEventHandlers();
    }

    protected RealVectorProblem() : this(new RealVectorEncoding() { Length = 10 }) { }
    protected RealVectorProblem(RealVectorEncoding encoding) : base(encoding) {
      EncodingParameter.ReadOnly = true;
      EvaluatorParameter.ReadOnly = true;
      Parameters.Add(DimensionRefParameter = new ReferenceParameter<IntValue>("Dimension", "The dimension of the real vector problem.", Encoding.LengthParameter));
      Parameters.Add(BoundsRefParameter = new ReferenceParameter<DoubleMatrix>("Bounds", "The bounding box of the values.", Encoding.BoundsParameter));
      Results.Add(BestSolutionResult = new Result<ISingleObjectiveSolutionContext<RealVector>>("Best Solution", "The best solution found so far."));

      Operators.Add(new HammingSimilarityCalculator());

      // TODO: These should be added in the SingleObjectiveProblem base class (if they were accessible from there)
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(ISingleObjectiveSolutionContext<RealVector>[] solutionContexts, IRandom random) {
      base.Analyze(solutionContexts, random);
      var best = GetBest(solutionContexts);
      if (BestSolution == null || IsBetter(best, BestSolution))
        BestSolution = best.Clone() as SingleObjectiveSolutionContext<RealVector>;
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
      DoubleMatrixParameterChangeHandler.Create(BoundsRefParameter, BoundsOnChanged);
    }

    protected virtual void DimensionOnChanged() { }

    protected virtual void BoundsOnChanged() { }
  }
}
