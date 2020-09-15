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

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [StorableType("c6081457-a3de-45ce-9f47-e0eb1c851bd2")]
  public abstract class IntegerVectorProblem : SingleObjectiveProblem<IntegerVectorEncoding, IntegerVector> {
    [Storable] protected IResultParameter<IntegerVector> BestResultParameter { get; private set; }
    [Storable] protected ReferenceParameter<IntValue> DimensionRefParameter { get; private set; }
    [Storable] protected ReferenceParameter<IntMatrix> BoundsRefParameter { get; private set; }

    public int Dimension {
      get { return DimensionRefParameter.Value.Value; }
      protected set { DimensionRefParameter.Value.Value = value; }
    }

    public IntMatrix Bounds {
      get { return BoundsRefParameter.Value; }
      protected set { BoundsRefParameter.Value = value; }
    }

    [StorableConstructor]
    protected IntegerVectorProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected IntegerVectorProblem(IntegerVectorProblem original, Cloner cloner)
      : base(original, cloner) {
      BestResultParameter = cloner.Clone(original.BestResultParameter);
      DimensionRefParameter = cloner.Clone(original.DimensionRefParameter);
      BoundsRefParameter = cloner.Clone(original.BoundsRefParameter);
      RegisterEventHandlers();
    }

    protected IntegerVectorProblem() : this(new IntegerVectorEncoding() { Length = 10 }) { }
    protected IntegerVectorProblem(IntegerVectorEncoding encoding) : base(encoding) {
      EncodingParameter.ReadOnly = true;
      EvaluatorParameter.ReadOnly = true;
      Parameters.Add(BestResultParameter = new ResultParameter<IntegerVector>("Best Solution", "The best solution."));
      Parameters.Add(DimensionRefParameter = new ReferenceParameter<IntValue>("Dimension", "The dimension of the integer vector problem.", Encoding.LengthParameter));
      Parameters.Add(BoundsRefParameter = new ReferenceParameter<IntMatrix>("Bounds", "The bounding box and step sizes of the values.", Encoding.BoundsParameter));

      Operators.Add(new HammingSimilarityCalculator());
      // TODO: These should be added in the SingleObjectiveProblem base class (if they were accessible from there)
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(ISingleObjectiveSolutionContext<IntegerVector>[] solutionContext, IRandom random) {
      base.Analyze(solutionContext, random);

      var best = GetBest(solutionContext);

      //TODO reimplement code below using results directly
      //results.AddOrUpdateResult("Best Solution", (IntegerVector)best.EncodedSolution.Clone());
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
      IntMatrixParameterChangeHandler.Create(BoundsRefParameter, BoundsOnChanged);
    }

    protected virtual void DimensionOnChanged() { }

    protected virtual void BoundsOnChanged() { }
  }
}
