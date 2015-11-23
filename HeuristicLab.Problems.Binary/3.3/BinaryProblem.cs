#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Binary {
  [StorableClass]
  public abstract class BinaryProblem : SingleObjectiveBasicProblem<BinaryVectorEncoding> {
    public virtual int Length {
      get { return Encoding.Length; }
      set { Encoding.Length = value; }
    }

    private IFixedValueParameter<IntValue> LengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Length"]; }
    }

    [StorableConstructor]
    protected BinaryProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected BinaryProblem(BinaryProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected BinaryProblem()
      : base() {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the BinaryVector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      RegisterEventHandlers();
    }

    public virtual bool IsBetter(double quality, double bestQuality) {
      return (Maximization && quality > bestQuality || !Maximization && quality < bestQuality);
    }

    public abstract double Evaluate(BinaryVector vector, IRandom random);
    public sealed override double Evaluate(Individual individual, IRandom random) {
      return Evaluate(individual.BinaryVector(), random);
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);
      var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
      var best = Maximization ? orderedIndividuals.Last().Individual : orderedIndividuals.First().Individual;

      if (!results.ContainsKey("Best Solution")) {
        results.Add(new Result("Best Solution", typeof(BinaryVector)));
      }
      results["Best Solution"].Value = (IItem)best.BinaryVector().Clone();
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Encoding.LengthParameter = LengthParameter;
    }


    private void RegisterEventHandlers() {
      LengthParameter.Value.ValueChanged += LengthParameter_ValueChanged;
    }

    protected virtual void LengthParameter_ValueChanged(object sender, EventArgs e) { }
  }
}
