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
    public IResultDefinition<IntegerVector> BestResult { get => BestResultParameter; }
    [Storable] protected ReferenceParameter<IntValue> DimensionRefParameter { get; private set; }
    public IValueParameter<IntValue> DimensionParameter => DimensionRefParameter;
    [Storable] protected ReferenceParameter<IntMatrix> BoundsRefParameter { get; private set; }
    public IValueParameter<IntMatrix> BoundsParameter => BoundsRefParameter;

    public int Dimension {
      get { return DimensionRefParameter.Value.Value; }
      set { DimensionRefParameter.Value.Value = value; }
    }

    public IntMatrix Bounds {
      get { return BoundsRefParameter.Value; }
      set { BoundsRefParameter.Value = value; }
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
      Parameters.Add(BestResultParameter = new ResultParameter<IntegerVector>("Best Solution", "The best solution."));
      Parameters.Add(DimensionRefParameter = new ReferenceParameter<IntValue>("Dimension", "The dimension of the integer vector problem.", Encoding.LengthParameter));
      Parameters.Add(BoundsRefParameter = new ReferenceParameter<IntMatrix>("Bounds", "The bounding box and step sizes of the values.", Encoding.BoundsParameter));

      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(IntegerVector[] vectors, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(vectors, qualities, results, random);
      var best = GetBestSolution(vectors, qualities);

      results.AddOrUpdateResult("Best Solution", (IItem)best.Item1.Clone());
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }

    private void RegisterEventHandlers() {
      DimensionRefParameter.Value.ValueChanged += DimensionParameter_Value_ValueChanged;
      BoundsRefParameter.ValueChanged += BoundsParameter_ValueChanged;
    }

    private void DimensionParameter_Value_ValueChanged(object sender, EventArgs e) {
      DimensionOnChanged();
    }

    private void BoundsParameter_ValueChanged(object sender, EventArgs e) {
      BoundsOnChanged();
    }

    protected virtual void DimensionOnChanged() { }

    protected virtual void BoundsOnChanged() { }
  }
}
