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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [StorableType("ad8b6097-a26b-440c-bfd4-92e5ecf17894")]
  public abstract class LinearLinkageMultiObjectiveProblem : MultiObjectiveProblem<LinearLinkageEncoding, LinearLinkage> {
    [Storable] protected ReferenceParameter<IntValue> DimensionRefParameter { get; private set; }
    public IValueParameter<IntValue> DimensionParameter => DimensionRefParameter;

    public int Dimension {
      get { return DimensionRefParameter.Value.Value; }
      set { DimensionRefParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected LinearLinkageMultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected LinearLinkageMultiObjectiveProblem(LinearLinkageMultiObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      DimensionRefParameter = cloner.Clone(original.DimensionRefParameter);
      RegisterEventHandlers();
    }

    protected LinearLinkageMultiObjectiveProblem() : this(new LinearLinkageEncoding() { Length = 10 }) { }
    protected LinearLinkageMultiObjectiveProblem(LinearLinkageEncoding encoding) : base(new LinearLinkageEncoding()) {
      EncodingParameter.ReadOnly = true;
      Parameters.Add(DimensionRefParameter = new ReferenceParameter<IntValue>("Dimension", "The dimension of the linear linkage problem.", Encoding.LengthParameter));


      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(LinearLinkage[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);

      var fronts = DominationCalculator.CalculateAllParetoFrontsIndices(individuals, qualities, Maximization);
      var plot = new ParetoFrontScatterPlot<LinearLinkage>(fronts, individuals, qualities, Objectives, BestKnownFront);
      results.AddOrUpdateResult("Pareto Front Scatter Plot", plot);
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualitiesParameter.ActualName;
      }
    }

    private void RegisterEventHandlers() {
      IntValueParameterChangeHandler.Create(DimensionRefParameter, DimensionOnChanged);
    }

    protected virtual void DimensionOnChanged() { }
  }
}
