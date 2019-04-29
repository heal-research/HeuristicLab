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
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [StorableType("b64caac0-a23a-401a-bb7e-ffa3e22b80ea")]
  public abstract class BinaryVectorMultiObjectiveProblem : MultiObjectiveProblem<BinaryVectorEncoding, BinaryVector> {
    public int Length {
      get { return Encoding.Length; }
      set { Encoding.Length = value; }
    }

    private IFixedValueParameter<IntValue> LengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Length"]; }
    }

    [StorableConstructor]
    protected BinaryVectorMultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected BinaryVectorMultiObjectiveProblem(BinaryVectorMultiObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected BinaryVectorMultiObjectiveProblem() : base(new BinaryVectorEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the BinaryVector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;

      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(BinaryVector[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);
      // TODO: Calculate Pareto front and add to results
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Encoding.LengthParameter = LengthParameter;
      Parameterize();
    }

    private void Parameterize() {
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualitiesParameter.ActualName;
      }
    }

    private void RegisterEventHandlers() {
      LengthParameter.Value.ValueChanged += LengthParameter_ValueChanged;
    }

    protected virtual void LengthParameter_ValueChanged(object sender, EventArgs e) { }
  }
}
