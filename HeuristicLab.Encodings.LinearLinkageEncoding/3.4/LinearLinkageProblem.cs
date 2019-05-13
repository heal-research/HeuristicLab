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
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [StorableType("fb4cfc7c-dc7c-4da6-843f-0dad7d3d7981")]
  public abstract class LinearLinkageProblem : SingleObjectiveProblem<LinearLinkageEncoding, LinearLinkage> {
    public int Length {
      get { return Encoding.Length; }
      set { Encoding.Length = value; }
    }

    [StorableConstructor]
    protected LinearLinkageProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected LinearLinkageProblem(LinearLinkageProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected LinearLinkageProblem() : this(new LinearLinkageEncoding() { Length = 10 }) { }
    protected LinearLinkageProblem(LinearLinkageEncoding encoding) : base(encoding) {
      EncodingParameter.ReadOnly = true;

      Operators.Add(new HammingSimilarityCalculator());
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
      Encoding.LengthParameter.Value.ValueChanged += LengthParameter_ValueChanged;
    }

    protected virtual void LengthParameter_ValueChanged(object sender, EventArgs e) { }
  }
}
