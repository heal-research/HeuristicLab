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
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [StorableType("fb4cfc7c-dc7c-4da6-843f-0dad7d3d7981")]
  public abstract class LinearLinkageProblem : SingleObjectiveProblem<LinearLinkageEncoding, LinearLinkage> {
    [Storable] protected ReferenceParameter<IntValue> DimensionRefParameter { get; private set; }

    public int Dimension {
      get { return DimensionRefParameter.Value.Value; }
      set { DimensionRefParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected LinearLinkageProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected LinearLinkageProblem(LinearLinkageProblem original, Cloner cloner)
      : base(original, cloner) {
      DimensionRefParameter = cloner.Clone(original.DimensionRefParameter);
      RegisterEventHandlers();
    }

    protected LinearLinkageProblem() : this(new LinearLinkageEncoding() { Length = 10 }) { }
    protected LinearLinkageProblem(LinearLinkageEncoding encoding) : base(encoding) {
      EncodingParameter.ReadOnly = true;
      Parameters.Add(DimensionRefParameter = new ReferenceParameter<IntValue>("Dimension", "The dimension of the linear linkage problem.", Encoding.LengthParameter));

      Operators.Add(new HammingSimilarityCalculator());
      // TODO: These should be added in the SingleObjectiveProblem base class (if they were accessible from there)
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(LinearLinkage[] vectors, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(vectors, qualities, results, random);
      var best = GetBestSolution(vectors, qualities);

      results.AddOrUpdateResult("Best Solution", (Item)best.Item1.Clone());
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
